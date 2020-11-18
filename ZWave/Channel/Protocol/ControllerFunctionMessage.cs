using System;

namespace ZWave.Channel.Protocol
{
    internal class ControllerFunctionMessage : Message
    {
        public readonly byte[] Payload;

        public ControllerFunctionMessage(MessageType type, Function function, byte[] payload)
            : base(FrameHeader.SOF, type, function)
        {
            Payload = payload;
        }

        public override string ToString()
        {
            if (Payload != null) return string.Concat(base.ToString(), " ", $"Payload:{BitConverter.ToString(Payload)}");
            return base.ToString();
        }
    }
}
