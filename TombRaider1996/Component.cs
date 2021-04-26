using System;                             // IDisposable
using System.Windows.Forms;               // Control, TableLayoutPanel
using System.Xml;                         // XmlDocument, XmlElement XmlNode
using LiveSplit.Model;                    // LiveSplitState
using LiveSplit.UI;                       // LayoutMode, SettingsHelper
using LiveSplit.UI.Components;            // IComponent, LogicComponent
using LiveSplit.UI.Components.AutoSplit;  // AutoSplitComponent, IAutoSplitter

namespace TR1
{
    /// <summary>
    ///     Implementation of <see cref="AutoSplitComponent"/>.
    /// </summary>
    /// <remarks>
    ///     <see cref="AutoSplitComponent"/> is derived from <see cref="LogicComponent"/>,
    ///     which derives from <see cref="IComponent"/> and <see cref="IDisposable"/>.
    /// </remarks>
    internal class Component : AutoSplitComponent
    {
        private Autosplitter _splitter;

        /// <summary>
        ///     Initializes the component.
        /// </summary>
        /// <param name="autoSplitter"><see cref="IAutoSplitter"/> implementation</param>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        public Component(Autosplitter autoSplitter, LiveSplitState state) : base(autoSplitter, state)
        {
            _splitter = autoSplitter;
        }

        /// <inheritdoc/>
        /// <param name="mode"><see cref="LayoutMode"/> passed by LiveSplit</param>
        /// <remarks>
        ///     Returned object must contain at least an empty <see cref="TableLayoutPanel"/>, otherwise the Layout Settings menu doesn't show up!
        /// </remarks>
        public override Control GetSettingsControl(LayoutMode mode) => _splitter.Settings;
        
        /// <inheritdoc/>
        /// <param name="document"><see cref="XmlDocument"/> passed by LiveSplit</param>
        /// <remarks>
        ///     Even if you don't have any settings, you can't return with null.
        ///     If you do, LiveSplit spams the Event Viewer with <c>Object reference not set to an instance of an object.</c> error messages.
        /// </remarks>
        /// <example>
        ///     https://github.com/LiveSplit/LiveSplit.ScriptableAutoSplit/blob/7e5a6cbe91569e7688fdb37446d32326b4b14b1c/ComponentSettings.cs#L70
        ///     https://github.com/CapitaineToinon/LiveSplit.DarkSoulsIGT/blob/master/LiveSplit.DarkSoulsIGT/UI/DSSettings.cs#L25
        /// </example>
        public override XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settingsNode = document.CreateElement("Settings");
            settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.FullGame), _splitter.Settings.FullGame));
            settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.Deathrun), _splitter.Settings.Deathrun));
            return settingsNode;
        }

        /// <inheritdoc/>
        /// <param name="settings"><see cref="XmlNode"/> passed by LiveSplit</param>
        /// <remarks>
        ///     This might happen more than once (e.g. when the settings dialog is cancelled, to restore previous settings).
        ///     The XML file is the <c>[game - category].lss</c> file in your LiveSplit folder.
        /// </remarks>
        /// <example><inheritdoc cref="GetSettings"/></example>
        public override void SetSettings(XmlNode settings)
        {
            _splitter.Settings.FullGame = settings["FullGame"]?.InnerText == "True";
            _splitter.Settings.Deathrun = settings["Deathrun"]?.InnerText == "True";

            if (_splitter.Settings.FullGame)
            {
                _splitter.Settings.FullGameModeButton.Checked = true;
                _splitter.Settings.ILModeButton.Checked = false;
                _splitter.Settings.DeathrunModeButton.Checked = false;
            }
            else
            {
                _splitter.Settings.FullGameModeButton.Checked = false;
                _splitter.Settings.ILModeButton.Checked = !_splitter.Settings.Deathrun;
                _splitter.Settings.DeathrunModeButton.Checked = _splitter.Settings.Deathrun;
            }
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            _splitter.GameMemory = null;
            _splitter = null;
        }

        /// <inheritdoc/>
        /// <remarks>
        ///     This is the text that is displayed in the white area of LiveSplit's Layout Editor.
        /// </remarks>
        public override string ComponentName => "Tomb Raider (1996)";

        /// <summary>
        ///     Adds <see cref="GameMemory"/> and <see cref="Autosplitter"/> management to <see cref="AutoSplitComponent.Update"/>.
        /// </summary>
        /// <param name="invalidator"><see cref="IInvalidator"/> passed by LiveSplit</param>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <param name="width">width passed by LiveSplit</param>
        /// <param name="height">height passed by LiveSplit</param>
        /// <param name="mode"><see cref="LayoutMode"/> passed by LiveSplit</param>
        /// <remarks>
        ///     This override allows <see cref="Autosplitter"/> to use <see cref="GameMemory"/> in its logic.
        /// </remarks>
        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (!_splitter.GameMemory.Update()) 
                return;

            if (state.CurrentPhase == TimerPhase.NotRunning && _splitter.Settings.FullGame)
                _splitter.ResetValues();

            base.Update(invalidator, state, width, height, mode);
        }
    }
}
