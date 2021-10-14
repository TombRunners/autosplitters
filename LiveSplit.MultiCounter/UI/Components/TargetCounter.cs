namespace LiveSplit.UI.Components
{
    /// <summary>A counter that protects against overflows and allows for quick comparison against a target value.</summary>
    /// <remarks>This class lifts code from <see cref="Counter"/>'s methods while removing explicit "Set" methods.</remarks>
    public class TargetCounter
    {
        /// <summary>Current value of the counter.</summary>
        public int Count { get; set; }

        /// <summary>Name of the counter.</summary>
        public string Name { get; set; }

        /// <summary>Amount by which to increment/decrement.</summary>
        public int IncrementValue { get; set; }

        /// <summary>Value when initialized or reset.</summary>
        public int Start { get; set; } 

        /// <summary>The value to compare against.</summary>
        public int? Target { get; set; }

        /// <summary>Decides if value should be above or below target.</summary>
        /// <remarks>The default, <see langword="false"/>, checks if value >= target.</remarks>
        public bool InvertTargetCondition { get; set; }
        
        /// <summary><see langword="true"/> if the target has been met according to the target condition.</summary>
        public bool TargetReached => Target != null && InvertTargetCondition ? Count <= Target : Count >= Target;

        /// <summary>Initializes the <see cref="TargetCounter"/>.</summary>
        /// <param name="name">Counter name</param>
        /// <param name="start">Initial value</param>
        /// <param name="increment">Amount by which to increment/decrement</param>
        /// <param name="target">Target value used by <see cref="TargetReached"/></param>
        public TargetCounter(string name, int start = 0, int increment = 1, int? target = null)
        {
            Name = name;
            Start = start;
            IncrementValue = increment;
            Target = target;

            Count = start;
        }

        /// <summary>Initializes the <see cref="TargetCounter"/>.</summary>
        /// <param name="settings">Values to apply to the <see cref="TargetCounter"/></param>
        public TargetCounter(TargetCounterSettings settings)
        {
            Name = settings.Name;
            Start = settings.Start;
            IncrementValue = settings.IncrementValue;
            Target = settings.Target;
            InvertTargetCondition = settings.InvertTargetCondition;

            Count = Start;
        }

        /// <summary>Increments the counter.</summary>
        /// <returns><see langword="true"/> if successfully incremented, <see langword="false"/> otherwise.</returns>
        public bool Increment()
        {
            if (Count == int.MaxValue)
                return false;

            try
            {
                Count = checked(Count + IncrementValue);
            }
            catch (System.OverflowException)
            {
                Count = int.MaxValue;
                return false;
            }

            return true;
        }

        /// <summary>Decrements the counter.</summary>
        /// <returns><see langword="true"/> if successfully decremented, <see langword="false"/> otherwise.</returns>
        public bool Decrement()
        {
            if (Count == int.MinValue)
                return false;

            try
            {
                Count = checked(Count - IncrementValue);
            }
            catch (System.OverflowException)
            {
                Count = int.MinValue;
                return false;
            }

            return true;
        }

        /// <summary>Resets the counter to its initial value.</summary>
        public void Reset() => Count = Start;

        /// <returns>A string summarizing count and target in the form of "X / Y" or only "X" if no target is set.</returns>
        public override string ToString() => Target is null ? $"{Count}" : $"{Count} / {Target}";
    }
}
