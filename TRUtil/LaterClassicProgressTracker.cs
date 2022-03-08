using System.Collections.Generic;

namespace TRUtil
{
    /// <summary>A wrapper around <see cref="Stack{LaterClassicProgressEntry}"/>.</summary>
    public class LaterClassicProgressTracker : Stack<LaterClassicProgressEntry>
    {
        public LaterClassicProgressTracker()
        {
        }

        public LaterClassicProgressTracker(int capacity) : base(capacity)
        {
        }
    }
}
