using System;

namespace ZWave.Channel
{
    public class NodeEventArgs : EventArgs
    {
        public readonly Command Command;
        public readonly byte NodeID;

        public NodeEventArgs(byte nodeID, Command command)
        {
            if ((NodeID = nodeID) == 0)
                throw new ArgumentOutOfRangeException(nameof(NodeID), nodeID, "NodeID can not be 0");
            if ((Command = command) == null)
                throw new ArgumentNullException(nameof(command));
        }
    }
}
