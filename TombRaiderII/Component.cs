<<<<<<< HEAD
﻿using LiveSplit.Model; // LiveSplitState
using TRUtil;          // ClassicAutosplitter, ClassicComponent
=======
﻿using System;                             // IDisposable
using System.Windows.Forms;               // Control, TableLayoutPanel
using System.Xml;                         // XmlDocument, XmlElement, XmlNode
using LiveSplit.Model;                    // LiveSplitState
using LiveSplit.UI;                       // LayoutMode, SettingsHelper
using LiveSplit.UI.Components;            // IComponent, LogicComponent
using LiveSplit.UI.Components.AutoSplit;  // AutoSplitComponent, IAutoSplitter
>>>>>>> cc251a4 (Cleaned typographical errors without bumping version.)

namespace TR2
{
    /// <summary>Implementation of <see cref="ClassicComponent"/>.</summary>
    internal sealed class Component : ClassicComponent
    {
        /// <inheritdoc/>
        public Component(ClassicAutosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
        {
        }
        
        public override string ComponentName => "Tomb Raider II and Golden Mask";
    }
}
