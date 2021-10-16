namespace LiveSplit.UI.Components
{
    /// <summary>Parameters for <see cref="NamedTargetCounter"/> construction.</summary>
    public struct NamedTargetCounterSettings : ICounterSettings
    {
        /// <inheritdoc cref="NamedTargetCounter.Name"/>
        public string Name { get; }
        /// <inheritdoc cref="NamedTargetCounter.Start"/>
        public int Start { get; }
        /// <inheritdoc cref="NamedTargetCounter.IncrementValue"/>
        public int IncrementValue { get; }
        /// <inheritdoc cref="NamedTargetCounter.Target"/>
        public int Target { get; }
        /// <inheritdoc cref="NamedTargetCounter.TargetCondition"/>
        public TargetCondition TargetCondition { get; }

        public NamedTargetCounterSettings(string name, int start = 0, int incrementValue = 1, int target = 0, TargetCondition targetCondition = TargetCondition.None)
        {
            Name = name;
            Start = start;
            IncrementValue = incrementValue;
            Target = target;
            TargetCondition = targetCondition;
        }
    }
}
