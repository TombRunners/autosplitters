namespace TRUtil
{
    public struct LaterClassicProgressItem
    {
        public uint Section { get; set; }
        public uint Value { get; set; }

        public LaterClassicProgressItem(uint section, uint value)
        {
            Section = section;
            Value = value;
        }
    }
}
