using System;
using System.Linq;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses.Application
{
    public class ThermostatSetpointReport : NodeReport
    {
        public readonly byte Scale;
        public readonly ThermostatSetpointType SetpointType;
        public readonly TemperatureUnit Unit;
        public readonly float Value;

        internal ThermostatSetpointReport(IZwaveNode node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            SetpointType = (ThermostatSetpointType) (payload[0] & 0b1111);
            Value = PayloadConverter.ToFloat(payload.Skip(1).ToArray(), out Scale);
            Unit = (TemperatureUnit) Scale;
        }

        public override string ToString()
        {
            return $"Type:{SetpointType}, Value: {Value} {Unit}";
        }
    }
}
