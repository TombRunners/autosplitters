namespace LiveSplit.UI.Components
{
    public interface INamedTargetCounter : ICounter
    {
        /// <summary>Name of the counter.</summary>
        string Name { get; }

        /// <summary>The value to compare against.</summary>
        int Target { get; }

        bool TargetReached { get; }

        /// <summary>Sets the target to <paramref name="value"/>.</summary>
        /// <param name="value">Value to set the target to</param>
        void SetTarget(int value);

        /// <summary>Sets the name to <paramref name="name"/>.</summary>
        /// <param name="name">String to set the name</param>
        void SetName(string name);
    }
}
