namespace LiveSplit.UI.Components
{
    /// <summary>A counter that protects against overflows and allows for quick comparison against a target value.</summary>
    /// <remarks>This class lifts code from <see cref="Counter"/>'s methods while removing some private variables.</remarks>
    public class NamedTargetCounter : INamedTargetCounter
    {
        /// <summary>Current value of the counter.</summary>
        public int Count { get; private set; }

        /// <inheritdoc cref="INamedTargetCounter.Name"/>
        public string Name { get; private set; }

        /// <summary>Amount by which to increment/decrement.</summary>
        public int IncrementValue { get; private set; }

        /// <summary>Value when initialized or reset.</summary>
        public int Start { get; private set; }

        /// <inheritdoc cref="ITargetCounter.Target"/>
        public int Target { get; private set; }

        /// <summary>Decides if and how the counter's value should be compared to its target.</summary>
        public TargetCondition TargetCondition { get; private set; }

        /// <summary><see langword="true"/> if the target has been met.</summary>
        public bool TargetReached
        {
            get
            {
                switch (TargetCondition)
                {
                    case TargetCondition.NE:
                        return Count != Target;
                    case TargetCondition.EQ:
                    case TargetCondition.LE:
                    case TargetCondition.GE:
                        return Count == Target;
                    case TargetCondition.LT:
                        return Count <= Target;
                    case TargetCondition.GT:
                        return Count >= Target;
                    case TargetCondition.None:    
                    default:
                        return false;
                }
            }
        }

        /// <summary>Initializes the <see cref="NamedTargetCounter"/>.</summary>
        /// <param name="name">Counter name</param>
        /// <param name="start">Initial value</param>
        /// <param name="increment">Amount by which to increment/decrement</param>
        /// <param name="target">Target value used by <see cref="TargetReached"/></param>
        public NamedTargetCounter(string name, int start = 0, int increment = 1, int target = 0, TargetCondition targetCondition = TargetCondition.None)
        {
            Name = name;
            Start = start;
            IncrementValue = increment;
            Target = target;
            TargetCondition = targetCondition;

            Count = start;
        }

        /// <summary>Initializes the <see cref="NamedTargetCounter"/>.</summary>
        /// <param name="settings">Values to apply to the <see cref="NamedTargetCounter"/></param>
        public NamedTargetCounter(NamedTargetCounterSettings settings)
        {
            Name = settings.Name;
            Start = settings.Start;
            IncrementValue = settings.IncrementValue;
            Target = settings.Target;
            TargetCondition = settings.TargetCondition;

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

        /// <summary>Sets the count to <paramref name="value"/>.</summary>
        /// <param name="value">Value to set the counter to</param>
        public void SetCount(int value) => Count = value;

        /// <inheritdoc cref="INamedTargetCounter.SetName(string)"/>
        public void SetName(string name) => Name = name;

        /// <summary>Sets the increment to <paramref name="value"/>.</summary>
        /// <param name="value">Value to set the increment to</param>
        public void SetIncrement(int value) => IncrementValue = value;

        /// <summary></summary>
        /// <param name="value"></param>
        public void SetStart(int value) => Start = value;
        
        /// <inheritdoc cref="INamedTargetCounter.SetTarget(int)"/>
        public void SetTarget(int value) => Target = value;

        /// <summary>Sets the condition to decide if and how to evaluate if the target was reached.</summary>
        /// <param name="targetCondition">Comparison type</param>
        public void SetTargetCondition(TargetCondition targetCondition) => TargetCondition = targetCondition;

        /// <returns>A string summarizing count and target in the form of "X / Y" or only "X" if no target is set.</returns>
        public override string ToString() => TargetCondition is TargetCondition.None ? $"{Count}" : $"{Count} / {Target}";
    }
}
