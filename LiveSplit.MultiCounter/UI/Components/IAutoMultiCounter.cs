using LiveSplit.Model;
using System.Collections.Generic;

namespace LiveSplit.UI.Components
{
    public interface IAutoMultiCounter
    {
        /// <summary>Returns if the multi-counter should advance to the next set of counters.</summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns></returns>
        bool ShouldAdvance(LiveSplitState state);
        
        /// <summary>Returns the indices of counter components to be decremented, if any./// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns>A <see cref="HashSet{int}"/> of counters to decrement, otherwise an empty <see cref="HashSet{int}"/></returns>
        HashSet<int> ShouldDecrement(LiveSplitState state);

        /// <summary>Returns the indices of counter components to be incremented, if any./// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns>A <see cref="HashSet{int}"/> of counters to increment, otherwise an empty <see cref="HashSet{int}"/></returns>
        HashSet<int> ShouldIncrement(LiveSplitState state);

        /// <summary>
        ///     Returns the indices of <see cref="MultiCounterComponent.CounterComponents"/> and desired values, if any.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns>A <see cref="Dictionary{string, int}"/> of names and values, otherwise an empty <see cref="Dictionary{string, int}"/></returns>
        Dictionary<int, int> ShouldSet(LiveSplitState state);

        /// <summary>
        ///     Returns the indices of <see cref="MultiCounterComponent.CounterComponents"/> to be reset to their starting value, if any.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><A <see cref="HashSet{int}"/> of counters to be reset, otherwise an empty <see cref="HashSet{int}"/>/returns>
        HashSet<int> ShouldReset(LiveSplitState state);
    }
}
