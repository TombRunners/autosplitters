using System.Collections.Generic;

namespace TRUtil
{
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
