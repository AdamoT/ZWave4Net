namespace ZWave.CommandClasses
{
    public class ReportEventArgs<T> where T : NodeReport
    {
        public readonly T Report;

        public ReportEventArgs(T report)
        {
            Report = report;
        }
    }
}
