using System;

namespace ZWave.CommandClasses
{
    public class NodeReport
    {
        public readonly IZwaveNode Node;

        public NodeReport(IZwaveNode node)
        {
            if ((Node = node) == null)
                throw new ArgumentNullException(nameof(node));
        }
    }
}
