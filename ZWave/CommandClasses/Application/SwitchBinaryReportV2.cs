using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses.Application
{
    public class SwitchBinaryReportV2 : SwitchBinaryReport
    {
        internal SwitchBinaryReportV2(IZwaveNode node, byte[] payload) : base(node, payload)
        {
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            TargetValue = payload[1] == 0xFF;
            Duration = payload[2];
        }

        public bool TargetValue { get; }
        public byte Duration { get; }

        public override string ToString()
        {
            return $"CurrentValue: {CurrentValue} TargetValue: {TargetValue} Duration {Duration}";
        }
    }
}
