using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class BatteryReport : NodeReport
    {
        public readonly bool IsLow;
        public readonly byte Value;

        internal BatteryReport(IZwaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            IsLow = payload[0] == 0xFF;
            Value = IsLow ? (byte) 0x00 : payload[0];
        }

        public override string ToString()
        {
            return IsLow ? "Low" : $"Value:{Value}%";
        }
    }
}
