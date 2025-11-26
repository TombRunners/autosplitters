using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.UI.Components.AutoSplit;
using Util;
using Timer = LiveSplit.UI.Components.Timer;

namespace TR456;

/// <summary>
///     Implementation of <see cref="AutoSplitComponent" />.
/// </summary>
/// <remarks>
///     <see cref="AutoSplitComponent" /> is derived from <see cref="LogicComponent" />,
///     which derives from <see cref="IComponent" /> and <see cref="IDisposable" />.
/// </remarks>
public class Component : AutoSplitComponent
{
    private readonly Autosplitter _splitter;
    private readonly LiveSplitState _state;

    private bool? _aslComponentPresent;

    private List<string> _layoutAndTimingMethods =
    [
    ];

    private TimingMethod? _lsCurrentTimingMethod;

    /// <summary>Allows creation of an event when an important LiveSplit layout or setting change occurs.</summary>
    private delegate void ImportantLayoutOrSettingChangedDelegate(bool aslComponentIsPresent, bool timerWithGameTimeIsPresent);

    /// <summary>Allows subscribers to know when an important LiveSplit layout or setting change occurs.</summary>
    private ImportantLayoutOrSettingChangedDelegate _onImportantLayoutOrSettingChanged;

    private bool? _timerWithGameTimePresent;

    /// <inheritdoc />
    public Component(Autosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
    {
        _splitter = autosplitter;
        _onImportantLayoutOrSettingChanged += _splitter.Settings.SetLayoutWarningLabelVisibilities;

        state.Settings.RefreshRate = 60;

        _state = state;
        _state.OnSplit += StateOnSplit;
        _state.OnStart += StateOnStart;
        _state.OnUndoSplit += StateOnUndoSplit;
    }

    public override string ComponentName => "Tomb Raider IV-VI Remastered";

    private void StateOnStart(object _0, EventArgs _1) => _splitter?.OnStart(_state);

    private void StateOnSplit(object _0, EventArgs _1) => _splitter?.OnSplit(GameData.CurrentActiveGame);

    private void StateOnUndoSplit(object _0, EventArgs _1) => _splitter?.OnUndoSplit();

    /// <inheritdoc />
    /// <param name="mode"><see cref="LayoutMode" /> passed by LiveSplit</param>
    /// <remarks>
    ///     The returned object must contain at least an empty <see cref="TableLayoutPanel" />,
    ///     otherwise the Layout Settings menu doesn't show up!
    /// </remarks>
    public override Control GetSettingsControl(LayoutMode mode) => _splitter.Settings;

    /// <inheritdoc />
    /// <param name="document"><see cref="XmlDocument" /> passed by LiveSplit</param>
    /// <remarks>
    ///     Even if you don't have any settings, you can't return with null.
    ///     If you do, LiveSplit spams the Event Viewer with error messages:
    ///     <c>Object reference not set to an instance of an object.</c>
    ///     <see href="https://github.com/LiveSplit/LiveSplit.ScriptableAutoSplit/blob/7e5a6cbe91569e7688fdb37446d32326b4b14b1c/ComponentSettings.cs#L70" />
    ///     <see href="https://github.com/CapitaineToinon/LiveSplit.DarkSoulsIGT/blob/master/LiveSplit.DarkSoulsIGT/UI/DSSettings.cs#L25" />
    /// </remarks>
    public override XmlNode GetSettings(XmlDocument document)
    {
        XmlElement settingsNode = document.CreateElement("Settings");
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.EnableAutoReset), _splitter.Settings.EnableAutoReset));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.RunType), _splitter.Settings.RunType));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.PickupSplitSetting), _splitter.Settings.PickupSplitSetting));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.SplitSecurityBreach), _splitter.Settings.SplitSecurityBreach));

        AppendTransitionSettings(document, settingsNode, nameof(_splitter.Settings.Tr4LevelTransitions), _splitter.Settings.Tr4LevelTransitions, t => t.ToXmlElement(document));
        AppendTransitionSettings(document, settingsNode, nameof(_splitter.Settings.Tr6LevelTransitions), _splitter.Settings.Tr6LevelTransitions, t => t.ToXmlElement(document));

        return settingsNode;
    }

    private static void AppendTransitionSettings<T>(XmlDocument doc, XmlNode settingsNode, string elementName, IEnumerable<T> transitions, Func<T, XmlNode> toXmlElement)
    {
        XmlElement transitionsNode = doc.CreateElement(elementName);
        foreach (T transition in transitions)
            transitionsNode.AppendChild(toXmlElement(transition));

        settingsNode.AppendChild(transitionsNode);
    }

    /// <inheritdoc />
    /// <param name="settings"><see cref="XmlNode" /> passed by LiveSplit</param>
    /// <remarks>
    ///     This might happen more than once (e.g., when the settings dialog is canceled, to restore previous settings).
    ///     The XML file is the <c>[game - category].lss</c> file in your LiveSplit folder.
    ///     <see href="https://github.com/LiveSplit/LiveSplit.ScriptableAutoSplit/blob/7e5a6cbe91569e7688fdb37446d32326b4b14b1c/ComponentSettings.cs#L70" />
    ///     <see href="https://github.com/CapitaineToinon/LiveSplit.DarkSoulsIGT/blob/master/LiveSplit.DarkSoulsIGT/UI/DSSettings.cs#L25" />
    /// </remarks>
    public override void SetSettings(XmlNode settings)
    {
        // Read serialized values, or keep defaults if they are not yet serialized.
        _splitter.Settings.EnableAutoReset = SettingsHelper.ParseBool(settings[nameof(_splitter.Settings.EnableAutoReset)], _splitter.Settings.EnableAutoReset);
        _splitter.Settings.RunType = SettingsHelper.ParseEnum(settings[nameof(_splitter.Settings.RunType)], _splitter.Settings.RunType);
        _splitter.Settings.PickupSplitSetting = SettingsHelper.ParseEnum(settings[nameof(_splitter.Settings.PickupSplitSetting)], _splitter.Settings.PickupSplitSetting);
        _splitter.Settings.SplitSecurityBreach = SettingsHelper.ParseBool(settings[nameof(_splitter.Settings.SplitSecurityBreach)]);

        // Assign values to Settings.
        if (_splitter.Settings.FullGame) // Grouped RadioButtons
            _splitter.Settings.FullGameButton.Checked = true;
        else if (_splitter.Settings.Deathrun)
            _splitter.Settings.DeathrunButton.Checked = true;
        else
            _splitter.Settings.IlOrAreaButton.Checked = true;

        switch (_splitter.Settings.PickupSplitSetting) // Grouped RadioButtons
        {
            case PickupSplitSetting.None:
                _splitter.Settings.SplitNoPickupsButton.Checked = true;
                break;

            case PickupSplitSetting.All:
                _splitter.Settings.SplitAllPickupsButton.Checked = true;
                break;

            case PickupSplitSetting.SecretsOnly:
                _splitter.Settings.SplitSecretsOnlyButton.Checked = true;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        _splitter.Settings.EnableAutoResetCheckbox.Checked = _splitter.Settings.EnableAutoReset;         // CheckBox
        _splitter.Settings.SplitSecurityBreachCheckbox.Checked = _splitter.Settings.SplitSecurityBreach; // CheckBox

        // Read and set level transition settings.
        ProcessTr4LevelTransitionSettings(settings, nameof(_splitter.Settings.Tr4LevelTransitions), _splitter.Settings.Tr4LevelTransitions);
        ProcessTr6LevelTransitionSettings(settings, nameof(_splitter.Settings.Tr6LevelTransitions), _splitter.Settings.Tr6LevelTransitions);
    }

    private static void ProcessTransitionSettings<T>(
        XmlNode settings, string transitionsNodeName, List<T> settingsList, string settingsPrefix,
        Func<XmlNode, T> fromXml, Action<T, T> updateSettings, Func<T, ulong> getId
    )
    {
        XmlElement transitionsNode = settings[transitionsNodeName];
        if (transitionsNode == null)
            return;

        int transitionsCount = settingsList.Count;
        int xmlTransitionsCount = transitionsNode.ChildNodes.Count;
        if (xmlTransitionsCount != transitionsCount)
        {
            Log.Error(
                $"Refusing to apply {settingsPrefix} level transition settings due to a mismatched count. " +
                $"{xmlTransitionsCount} found in XML, expected {transitionsCount}. Reverting to default/existing."
            );
            return;
        }

        // Create a copy of the existing settings for potential reversion.
        var defaultOrExistingTransitions = settingsList.ToArray();
        var encounteredSettingIds = new HashSet<ulong>(transitionsCount);

        foreach (XmlNode transitionNode in transitionsNode.ChildNodes)
        {
            T settingFromXml;
            try
            {
                settingFromXml = fromXml(transitionNode);
            }
            catch (Exception ex)
            {
                Log.Error($"{settingsPrefix} level transition deserialization failed: {ex.Message}\n\n{ex.StackTrace}");
                RevertSettings(settingsList, defaultOrExistingTransitions);
                break;
            }

            var settingsNeedReversion = false;
            var existingSettings = settingsList.Where(t => getId(t) == getId(settingFromXml)).ToList();
            if (existingSettings.Count != 1)
            {
                settingsNeedReversion = true;
                Log.Error(
                    $"Found unexpected amount of matches ({existingSettings.Count}) for {settingsPrefix} level transition " +
                    $"from XML with ID {getId(settingFromXml)}. Reverting to default/existing."
                );
            }

            if (!encounteredSettingIds.Add(getId(settingFromXml)))
            {
                settingsNeedReversion = true;
                Log.Error(
                    $"Encountered {settingsPrefix} level transition setting more than once " +
                    $"from XML with ID {getId(settingFromXml)}. Reverting to default/existing."
                );
            }

            if (settingsNeedReversion)
            {
                RevertSettings(settingsList, defaultOrExistingTransitions);
                break;
            }

            // Update the existing setting with properties from the XML-parsed setting.
            updateSettings(existingSettings[0], settingFromXml);
        }
    }

    private static void RevertSettings<T>(List<T> settings, T[] defaultOrExistingSettings)
    {
        settings.Clear();
        settings.AddRange(defaultOrExistingSettings);
    }

    // The TR4-specific wrapper simply calls the generic method with the appropriate delegates.
    private static void ProcessTr4LevelTransitionSettings(XmlNode settings, string nodeName, List<Tr4LevelTransitionSetting> settingsList)
        => ProcessTransitionSettings(
            settings, nodeName, settingsList, "TR4R",
            Tr4LevelTransitionSetting.FromXmlElement,
            static (existing, xml) =>
            {
                existing.UpdateActive(xml.Active);
                existing.SelectedDirectionality = xml.SelectedDirectionality;
            },
            static t => t.Id
        );

    // The TR6-specific wrapper also calls the generic method with its own delegates.
    private static void ProcessTr6LevelTransitionSettings(XmlNode settings, string nodeName, List<Tr6LevelTransitionSetting> settingsList)
        => ProcessTransitionSettings(
            settings, nodeName, settingsList, "TR6R",
            Tr6LevelTransitionSetting.FromXmlElement, static (existing, xml) =>
            {
                existing.UpdateActive(xml.Active);
            },
            static t => t.Id
        );

    /// <summary>
    ///     Adds <see cref="GameData" /> and <see cref="Autosplitter" /> management to <see cref="AutoSplitComponent.Update" />.
    /// </summary>
    /// <param name="invalidator"><see cref="IInvalidator" /> passed by LiveSplit</param>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <param name="width">Width passed by LiveSplit</param>
    /// <param name="height">Height passed by LiveSplit</param>
    /// <param name="mode"><see cref="LayoutMode" /> passed by LiveSplit</param>
    /// <remarks>
    ///     This override allows <see cref="Autosplitter" /> to use <see cref="GameData" /> in its logic.
    /// </remarks>
    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        bool valuesNotInitialized = _aslComponentPresent is null;

        TimingMethod currentTimingMethod = state.CurrentTimingMethod;
        var layoutAndTimingMethods = state
            .Layout
            .LayoutComponents
            .Select(static comp =>
                {
                    return comp.Component switch
                    {
                        Timer timer                 => timer.Settings.TimingMethod,
                        DetailedTimer detailedTimer => detailedTimer.Settings.TimingMethod,
                        _                           => comp.Component.ComponentName,
                    };
                }
            )
            .ToList();
        bool layoutOrTimingMethodChanged = currentTimingMethod != _lsCurrentTimingMethod ||
                                           !layoutAndTimingMethods.SequenceEqual(_layoutAndTimingMethods);

        bool importantLayoutOrSettingChanged = valuesNotInitialized || layoutOrTimingMethodChanged;
        if (importantLayoutOrSettingChanged)
        {
            _layoutAndTimingMethods = layoutAndTimingMethods;
            _lsCurrentTimingMethod = currentTimingMethod;
            HandleLayoutOrSettingUpdates(state);

            if (!ComponentSettings.GameVersionInitialized)
            {
                GameData.GameProcess = null;
                GameData.CurrentGameVersion = VersionDetector.None;
            }
        }

        if (GameData.Update())
            base.Update(invalidator, state, width, height, mode);
    }

    private void HandleLayoutOrSettingUpdates(LiveSplitState state)
    {
        bool lsTimingMethodIsGameTime = _lsCurrentTimingMethod == TimingMethod.GameTime;
        var timerWithGameTimeInLayout = false;
        foreach (ILayoutComponent timerComponent in state.Layout.LayoutComponents.Where(static comp => comp.Component is Timer or DetailedTimer))
        {
            timerWithGameTimeInLayout = timerComponent.Component switch
            {
                DetailedTimer detailedTimer => TimerUsesGameTime(detailedTimer.Settings.TimingMethod, lsTimingMethodIsGameTime),
                Timer timer                 => TimerUsesGameTime(timer.Settings.TimingMethod, lsTimingMethodIsGameTime),
                _                           => false,
            };

            if (timerWithGameTimeInLayout)
                break;
        }

        bool aslInLayout = state.Layout.LayoutComponents.Any(static comp => comp.Component is ASLComponent);
        if (_aslComponentPresent == aslInLayout && timerWithGameTimeInLayout == _timerWithGameTimePresent)
            return;

        _aslComponentPresent = aslInLayout;
        _timerWithGameTimePresent = timerWithGameTimeInLayout;
        _onImportantLayoutOrSettingChanged.Invoke(aslInLayout, timerWithGameTimeInLayout);
    }

    private static bool TimerUsesGameTime(string method, bool globalMethodIsGameTime)
    {
        const string current = "Current Timing Method";
        const string gameTime = "Game Time";
        bool timerWithGameTimeInLayout = method == gameTime || (globalMethodIsGameTime && method == current);
        return timerWithGameTimeInLayout;
    }

    public override void Dispose()
    {
        _state.OnSplit -= StateOnSplit;
        _state.OnStart -= StateOnStart;
        _state.OnUndoSplit -= StateOnUndoSplit;
        _onImportantLayoutOrSettingChanged -= _splitter.Settings.SetLayoutWarningLabelVisibilities;
        _splitter?.Dispose();
    }
}