namespace ZWave.CommandClasses
{
    public class WakeUpReport : NodeReport
    {
        public readonly bool Awake;

        internal WakeUpReport(IZwaveNode node) : base(node)
        {
            Awake = true;
        }

        public override string ToString()
        {
            return $"Awake:{Awake}";
        }
    }
}
