namespace LiveSplit.UI.Components
{
    /// <summary>Parameters for <see cref="TargetCounter"/> construction.</summary>
    public struct TargetCounterSettings
    {
        /// <inheritdoc cref="TargetCounter.Name"/>
        public string Name { get; }
        /// <inheritdoc cref="TargetCounter.Start"/>
        public int Start { get; }
        /// <inheritdoc cref="TargetCounter.IncrementValue"/>
        public int IncrementValue { get; }
        /// <inheritdoc cref="TargetCounter.Target"/>
        public int? Target { get; }
        /// <inheritdoc cref="TargetCounter.InvertTargetCondition"/>
        public bool InvertTargetCondition { get; }

        public TargetCounterSettings(string name, int start = 0, int incrementValue = 1, int? target = null, bool invertTargetCondition = false)
        {
            Name = name;
            Start = start;
            IncrementValue = incrementValue;
            Target = target;
            InvertTargetCondition = invertTargetCondition;
        }
    }
}
