using System.Collections.Generic;

namespace TRUtil
{
    /// <summary>A collection of <see cref="LaterClassicProgressItem"/>s.</summary>
    public class LaterClassicProgressEntry : HashSet<LaterClassicProgressItem>
    {
        public void Add(uint section, uint value)
        {
            Add(new LaterClassicProgressItem(section, value));
        }
    }
}
