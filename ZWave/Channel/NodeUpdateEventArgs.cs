using System;

namespace ZWave.Channel
{
    public class NodeUpdateEventArgs : EventArgs
    {
        public readonly byte NodeID;

        public NodeUpdateEventArgs(byte nodeID)
        {
            if ((NodeID = nodeID) == 0)
                throw new ArgumentOutOfRangeException(nameof(NodeID), nodeID, "NodeID can not be 0");
        }
    }
}
