using System;

namespace ZWave.Channel.Protocol
{
    internal class UnknownMessage : Message
    {
        public readonly byte[] Payload;

        public UnknownMessage(FrameHeader header, MessageType type, Function function, byte[] payload)
            : base(header, type, function)
        {
            Payload = payload;
        }

        public override string ToString()
        {
            return string.Concat(base.ToString(), " ", $"Payload:{BitConverter.ToString(Payload)}");
        }
    }
}
