using System;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components.AutoSplit;

namespace TR1
{
    internal class Component : AutoSplitComponent
    {
        private Autosplitter _splitter;

        public Component(Autosplitter autoSplitter, LiveSplitState state) : base(autoSplitter, state)
        {
            _splitter = autoSplitter;
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return _splitter.Settings;
        }

        /// <summary>
        ///     Writes your component's settings into the <c>.lss</c> file.
        /// </summary>
        /// <param name="document">Document passed by LiveSplit</param>
        /// <remarks>
        ///     Examples of usage:
        ///     https://github.com/LiveSplit/LiveSplit.ScriptableAutoSplit/blob/7e5a6cbe91569e7688fdb37446d32326b4b14b1c/ComponentSettings.cs#L70
        ///     https://github.com/CapitaineToinon/LiveSplit.DarkSoulsIGT/blob/master/LiveSplit.DarkSoulsIGT/UI/DSSettings.cs#L25
        /// </remarks>
        /// <returns>
        ///     The document to be written out.
        /// </returns>
        public override XmlNode GetSettings(XmlDocument document)
        {
            // Even if you don't have any settings, you can't return with null.
            // If you do, LiveSplit spams the Event Viewer with the "Object reference not set to an instance of an object." error message.
            XmlElement settingsNode = document.CreateElement("Settings");
            settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.FullGame), _splitter.Settings.FullGame));
            return settingsNode;
        }

        /// <summary>
        ///     Loads the settings of this component from XML.
        /// </summary>
        /// <param name="settings">Node passed by LiveSplit</param>
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
    }
}
