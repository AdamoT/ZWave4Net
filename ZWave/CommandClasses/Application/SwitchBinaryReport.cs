using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses.Application
{
    public class SwitchBinaryReport : NodeReport
    {
        internal SwitchBinaryReport(IZwaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            CurrentValue = payload[0] == 0xFF;
        }

        public bool CurrentValue { get; }

        public override string ToString()
        {
            return $"CurrentValue: {CurrentValue}";
        }
    }
}
