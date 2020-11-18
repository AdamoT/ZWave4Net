using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Battery : CommandClassBase_OLD
    {
        public Battery(IZwaveNode node) : base(node, CommandClass.Battery)
        {
        }

        public event EventHandler<ReportEventArgs<BatteryReport>> Changed;

        public Task<BatteryReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<BatteryReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new BatteryReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new BatteryReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<BatteryReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<BatteryReport> e)
        {
            var handler = Changed;
            if (handler != null) handler(this, e);
        }

        private enum command
        {
            Get = 0x02,
            Report = 0x03
        }
    }
}
