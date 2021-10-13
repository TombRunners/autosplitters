namespace LiveSplit.UI.Components
{
    public struct SimpleCounterSettings
    {
        public string Name { get; }
        public int Start { get; }
        public int Increment { get; }
        public int? Target { get; }

        public SimpleCounterSettings(string name, int start = 0, int increment = 1, int? target = null)
        {
            Name = name;
            Start = start;
            Increment = increment;
            Target = target;
        }
    }
}
