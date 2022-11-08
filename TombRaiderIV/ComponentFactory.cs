using LiveSplit.Model;          // LiveSplitState
using LiveSplit.UI.Components;  // IComponentFactory, IComponent, InfoTextComponent, ComponentCategory
using System;                   // Version
using System.Reflection;        // Assembly
using TR4;                      // For [assembly:...]
using UpdateManager;            // IUpdateable

[assembly: ComponentFactory(typeof(ComponentFactory))]

namespace TR4;

/// <summary>
///     Implementation of <see cref="IComponentFactory"/> for the component.
/// </summary>
/// <remarks>
///     This class is necessary for LiveSplit to create and apply metadata to the component.
///     It must implement all of <see cref="IComponentFactory"/> which is derived from <see cref="IUpdateable"/>
///     IComponentFactory: https://github.com/LiveSplit/LiveSplit/blob/master/LiveSplit/LiveSplit.Core/UI/Components/IComponentFactory.cs
///     IUpdateable: https://github.com/LiveSplit/LiveSplit/blob/master/LiveSplit/UpdateManager/IUpdateable.cs
/// </remarks>
internal sealed class ComponentFactory : IComponentFactory
{
    #region IComponentFactory Implementations

    /// <inheritdoc/>
    /// <remarks>
    ///     Using <see cref="ComponentCategory.Timer"/> makes the component appear in the <c>Timer</c> section of the <c>+</c> menu in LiveSplit's Layout Editor.
    /// </remarks>
    public ComponentCategory Category => ComponentCategory.Timer;

    /// <inheritdoc/>
    /// <remarks>
    ///     This is the text you see in the menu which you see after pressing + in the Layout Editor.
    /// </remarks>
    public string ComponentName => "Tomb Raider IV and The Times Exclusive";

    /// <inheritdoc/>
    /// <remarks>
    ///     This is the text that appears when you hover over the component in the <c>+</c> menu in LiveSplit's Layout Editor.
    /// </remarks>
    public string Description => "Autosplitter for Tomb Raider IV and The Times Exclusive";

    /// <inheritdoc/>
    /// <param name="state">State passed by LiveSplit</param>
    /// <remarks>
    ///     This loads the component's code into LiveSplit.
    /// </remarks>
    public IComponent Create(LiveSplitState state) => new Component(new Autosplitter(), state);

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
    public string UpdateURL => "https://raw.githubusercontent.com/TombRunners/autosplitters/master/";

    /// <summary>
    ///     XML file which is checked to see if the component needs updated.
    /// </summary>
    /// <remarks>
    ///     Value should be: UpdateURL + <c>[relative path to the XML file]</c>
    /// </remarks>
    public string XMLURL => UpdateURL + "TombRaiderIV/Components/update.xml";

    #endregion
}