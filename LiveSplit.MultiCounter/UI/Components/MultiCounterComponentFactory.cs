using LiveSplit.Model;          // LiveSplitState
using UpdateManager;            // IUpdateable
using System;                   // Version
using System.Reflection;        // Assembly

namespace LiveSplit.UI.Components
{
    /// <summary>
    ///     Implementation of <see cref="IComponentFactory"/> for the component.
    /// </summary>
    /// <remarks>
    ///     This class is necessary for LiveSplit to create and apply metadata to the component.
    ///     It must implement all of <see cref="IComponentFactory"/> which is derived from <see cref="IUpdateable"/>
    ///     IComponentFactory: https://github.com/LiveSplit/LiveSplit/blob/master/LiveSplit/LiveSplit.Core/UI/Components/IComponentFactory.cs
    ///     IUpdateable: https://github.com/LiveSplit/LiveSplit/blob/master/LiveSplit/UpdateManager/IUpdateable.cs
    /// </remarks>
    public class MultiCounterComponentFactory : IComponentFactory
    {
        #region IComponentFactory Implementations

        /// <inheritdoc/>
        /// <remarks>
        ///     Using <see cref="ComponentCategory.Timer"/> makes the component appear in the <c>Timer</c> section of the <c>+</c> menu in LiveSplit's Layout Editor.
        /// </remarks>
        public ComponentCategory Category => ComponentCategory.List;

        /// <inheritdoc/>
        /// <remarks>
        ///     This is the text you see in the menu which you see after pressing + in the Layout Editor.
        /// </remarks>
        public string ComponentName => "Multi-Counter";

        /// <inheritdoc/>
        /// <remarks>
        ///     This is the text that appears when you hover over the component in the <c>+</c> menu in LiveSplit's Layout Editor.
        /// </remarks>
        public string Description => "Displays a list of counters with optional target values.";

        /// <inheritdoc/>
        /// <param name="state">State passed by LiveSplit</param>
        /// <remarks>
        ///     This loads the component's code into LiveSplit.
        /// </remarks>
        public IComponent Create(LiveSplitState state) => new MultiCounterComponent(state);

        #endregion

        #region IUpdateable Implementations

        /// <summary>
        ///     Returns the component's version.
        /// </summary>
        /// <remarks>
        ///     This can be set through Visual Studio's GUI.
        ///     Right-click the component's project, select Properties -> Application -> Assembly Information...
        ///     Set Assembly Version and File Version to the desired value.
        /// </remarks>
        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        ///     Name used for component updates.
        /// </summary>
        /// <remarks>
        ///     This can be the same as <see cref="IComponentFactory.ComponentName"/>.
        /// </remarks>
        public string UpdateName => ComponentName;

        /// <summary>
        ///     Returns the base URL of the component's repository.
        /// </summary>
        /// <remarks>
        ///     It must be a raw link.
        /// </remarks>
        public string UpdateURL => "https://raw.githubusercontent.com/TombRunners/autosplitters/master/LiveSplit.MultiCounter/UI";

        /// <summary>
        ///     XML file which is checked to see if the component needs updated.
        /// </summary>
        /// <remarks>
        ///     Value should be: UpdateURL + <c>[relative path to the XML file]</c>
        /// </remarks>
        public string XMLURL => UpdateURL + "Component/update.LiveSplit.MultiCounter.xml";

        #endregion
    }
}
