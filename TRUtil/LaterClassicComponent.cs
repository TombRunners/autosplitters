using System;                             // IDisposable
using System.Windows.Forms;               // Control, TableLayoutPanel
using System.Xml;                         // XmlDocument, XmlElement XmlNode
using LiveSplit.Model;                    // LiveSplitState
using LiveSplit.UI;                       // LayoutMode
using LiveSplit.UI.Components;            // IComponent, LogicComponent, SettingsHelper
using LiveSplit.UI.Components.AutoSplit;  // AutoSplitComponent, IAutoSplitter

namespace TRUtil
{
    /// <summary>
    ///     Implementation of <see cref="AutoSplitComponent"/>.
    /// </summary>
    /// <remarks>
    ///     <see cref="AutoSplitComponent"/> is derived from <see cref="LogicComponent"/>,
    ///     which derives from <see cref="IComponent"/> and <see cref="IDisposable"/>.
    /// </remarks>
    public abstract class LaterClassicComponent : BaseComponent
    {
        private readonly LaterClassicAutosplitter _splitter;
        private readonly LiveSplitState _state;

        protected LaterClassicComponent(BaseAutosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
        {
            _splitter = (LaterClassicAutosplitter)autosplitter;
            _state = state;
            _state.OnSplit += (s, e) => _splitter?.OnSplit(BaseGameData.Level.Current);
            _state.OnStart += (s, e) => _splitter?.OnStart();
            _state.OnUndoSplit += (s, e) => _splitter?.OnUndoSplit();
        }

        /// <inheritdoc/>
        /// <param name="mode"><see cref="LayoutMode"/> passed by LiveSplit</param>
        /// <remarks>
        ///     Returned object must contain at least an empty <see cref="TableLayoutPanel"/>, otherwise the Layout Settings menu doesn't show up!
        /// </remarks>
        public override Control GetSettingsControl(LayoutMode mode) => _splitter.Settings;

        public override XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settingsNode = document.CreateElement("Settings");
            settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.FullGame), _splitter.Settings.FullGame));
            settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.Deathrun), _splitter.Settings.Deathrun));
            settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.Glitchless), _splitter.Settings.Glitchless));
            return settingsNode;
        }

        public override void SetSettings(XmlNode settings)
        {
            _splitter.Settings.FullGame = settings["FullGame"]?.InnerText == "True";
            _splitter.Settings.Deathrun = settings["Deathrun"]?.InnerText == "True";
            _splitter.Settings.Glitchless = settings["Glitchless"]?.InnerText == "True";

            if (_splitter.Settings.FullGame)
            {
                _splitter.Settings.FullGameModeButton.Checked = true;
            }
            else if (_splitter.Settings.Deathrun)
            {
                _splitter.Settings.DeathrunModeButton.Checked = true;
            }
            else
            {
                _splitter.Settings.ILModeButton.Checked = true;
            }

            if (_splitter.Settings.Glitchless)
                _splitter.Settings.GlitchlessCheckbox.Checked = true;
        }

        public override void Dispose()
        {
            _state.OnSplit -= (s, e) => _splitter?.OnSplit(BaseGameData.Level.Current);
            _state.OnStart -= (s, e) => _splitter?.OnStart();
            _state.OnUndoSplit -= (s, e) => _splitter?.OnUndoSplit();
            _splitter?.Dispose();
        }

        public override string ComponentName => "Later Classic Tomb Raider Component";

        /// <summary>
        ///     Adds <see cref="LaterClassicGameData"/> and <see cref="LaterClassicAutosplitter"/> management to <see cref="AutoSplitComponent.Update"/>.
        /// </summary>
        /// <param name="invalidator"><see cref="IInvalidator"/> passed by LiveSplit</param>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <param name="width">width passed by LiveSplit</param>
        /// <param name="height">height passed by LiveSplit</param>
        /// <param name="mode"><see cref="LayoutMode"/> passed by LiveSplit</param>
        /// <remarks>
        ///     This override allows <see cref="LaterClassicAutosplitter"/> to use <see cref="ClassicGameData"/> in its logic.
        /// </remarks>
        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (_splitter.Data.Update())
                base.Update(invalidator, state, width, height, mode);
        }
    }
}
