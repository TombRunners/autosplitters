using System;                             // IDisposable
using System.Windows.Forms;               // Control
using System.Xml;                         // XmlDocument, XmlElement XmlNode
using LiveSplit.Model;                    // LiveSplitState
using LiveSplit.UI;                       // LayoutMode, SettingsHelper
using LiveSplit.UI.Components;            // IComponent, LogicComponent
using LiveSplit.UI.Components.AutoSplit;  // AutoSplitComponent

namespace TR1
{
    /// <summary>
    ///     Implementation of <see cref="AutoSplitComponent"/> for the component.
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
        /// <param name="autoSplitter">The <see cref="AutoSplitter"/> logic to use</param>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        public Component(Autosplitter autoSplitter, LiveSplitState state) : base(autoSplitter, state)
        {
            _splitter = autoSplitter;
        }

        /// <summary>
        ///     Shows a dialog where the user can configure the component.
        /// </summary>
        /// <param name="mode">LayoutMode passed by LiveSplit</param>
        /// <remarks>
        ///     Returned object must contain at least an empty TableLayoutPanel, otherwise the Layout Settings menu doesn't show up!
        /// </remarks>
        /// <returns>In what way the dialog was closed.</returns>
        public override Control GetSettingsControl(LayoutMode mode)
        {
            return _splitter.Settings;
        }

        /// <summary>
        ///     Writes your component's settings into the <c>.lss</c> file.
        /// </summary>
        /// <param name="document"><see cref="XmlDocument"/> passed by LiveSplit</param>
        /// <remarks>
        ///     Even if you don't have any settings, you can't return with null.
        ///     If you do, LiveSplit spams the Event Viewer with the <c>Object reference not set to an instance of an object.</c> error message.
        /// </remarks>
        /// <example>
        ///     https://github.com/LiveSplit/LiveSplit.ScriptableAutoSplit/blob/7e5a6cbe91569e7688fdb37446d32326b4b14b1c/ComponentSettings.cs#L70
        ///     https://github.com/CapitaineToinon/LiveSplit.DarkSoulsIGT/blob/master/LiveSplit.DarkSoulsIGT/UI/DSSettings.cs#L25
        /// </example>
        /// <returns>
        ///     The document to be written out.
        /// </returns>
        public override XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settingsNode = document.CreateElement("Settings");
            settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.FullGame), _splitter.Settings.FullGame));
            return settingsNode;
        }

        /// <summary>
        ///     Loads the settings of this component from XML.
        /// </summary>
        /// <param name="settings"><see cref="XmlNode"/> passed by LiveSplit</param>
        /// <remarks>
        ///     This might happen more than once (e.g. when the settings dialog is cancelled, to restore previous settings).
        ///     The XML file is the <c>[game - category].lss</c> file in your LiveSplit folder.
        ///     See the remarks on <see cref="GetSettings"/> for how to use it.
        /// </remarks>
        public override void SetSettings(XmlNode settings)
        {
            _splitter.Settings.FullGame = settings["FullGame"]?.InnerText == "True";
            if (_splitter.Settings.FullGame)
            {
                _splitter.Settings._fullGameModeButton.Checked = true;
                _splitter.Settings._ilModeButton.Checked = false;
            }
            else
            {
                _splitter.Settings._fullGameModeButton.Checked = false;
                _splitter.Settings._ilModeButton.Checked = true;
            }
        }

        /// <summary>
        ///     Implements <see cref="IDisposable"/> for the component.
        /// </summary>
        public override void Dispose()
        {
            _splitter.GameMemory = null;
            _splitter = null;
        }

        /// <summary>
        ///     Returns the component's name.
        /// </summary>
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
        ///     This override allows <see cref="AutoSplitter"/> to use <see cref="GameMemory"/> in its logic.
        /// </remarks>
        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (_splitter.GameMemory.Update())
            {
                if (state.CurrentPhase == TimerPhase.NotRunning && _splitter.Settings.FullGame)
                    _splitter.ResetValues();
                
                base.Update(invalidator, state, width, height, mode);
            }
        }
    }
}
