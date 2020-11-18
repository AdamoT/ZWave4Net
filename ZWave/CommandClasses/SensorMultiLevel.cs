using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SensorMultiLevel : EndpointSupportedCommandClassBase
    {
        private const int GetSupportedSensorsMinimalProtocolVersion = 5;

        public SensorMultiLevel(IZwaveNode node)
            : base(node, CommandClass.SensorMultiLevel)
        {
        }

        internal SensorMultiLevel(IZwaveNode node, byte endpointId)
            : base(node, CommandClass.SensorMultiLevel, endpointId)
        {
        }

        public event EventHandler<ReportEventArgs<SensorMultiLevelReport>> Changed;

        public Task<SensorMultilevelSupportedSensorReport> GetSupportedSensors()
        {
            return GetSupportedSensors(CancellationToken.None);
        }

        public async Task<SensorMultilevelSupportedSensorReport> GetSupportedSensors(CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            return new SensorMultilevelSupportedSensorReport(Node, response);
        }

        public Task<SensorMultiLevelReport> Get(SensorType type)
        {
            return Get(type, CancellationToken.None);
        }

        public async Task<SensorMultiLevelReport> Get(SensorType type, CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.Get, (byte) type), command.Report, cancellationToken);
            return new SensorMultiLevelReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SensorMultiLevelReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SensorMultiLevelReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<SensorMultiLevelReport> e)
        {
            var handler = Changed;
            if (handler != null) handler(this, e);
        }

        private enum command
        {
            SupportedGet = 0x01,
            SupportedReport = 0x02,
            Get = 0x04,
            Report = 0x05
        }
    }
}
