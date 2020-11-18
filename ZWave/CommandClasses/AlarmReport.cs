using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class AlarmReport : NodeReport
    {
        internal AlarmReport(IZwaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. Report: {GetType().Name}, Payload: {BitConverter.ToString(payload)}");

            Type = (AlarmType) payload[0];
            Level = payload[1];
            if (payload.Length > 2) Unknown = payload[2];
            if (payload.Length > 5) Detail = (AlarmDetailType) payload[5];
        }

        public AlarmType Type { get; }
        public byte Level { get; }
        public AlarmDetailType Detail { get; }
        public byte Unknown { get; }

        public override string ToString()
        {
            return $"Type:{Type}, Level:{Level}, Detail:{Detail}, Unknown:{Unknown}";
        }
    }
}
