using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses.Application
{
    public class SwitchBinary : CommandClassBase
    {
        #region Types

        public enum CommandV1
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        #endregion Types

        #region ICommandClass

        public override CommandClass Class => CommandClass.SwitchBinary;

        #endregion ICommandClass

        #region ICommandClassInternal

        public override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SwitchBinaryReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SwitchBinaryReport>(report));
        }

        #endregion ICommandClassInternal

        #region Events

        public event EventHandler<ReportEventArgs<SwitchBinaryReport>> Changed;

        #endregion Events

        #region Protected Methods

        protected virtual void OnChanged(ReportEventArgs<SwitchBinaryReport> e)
        {
            var handler = Changed;
            if (handler != null) handler(this, e);
        }

        #endregion Protected Methods

        #region Public Methods

        public async Task<SwitchBinaryReport> Get(CancellationToken cancellationToken = default)
        {
            var response = await Send(new Command(Class, CommandV1.Get), CommandV1.Report, cancellationToken)
                .ConfigureAwait(false);

            return new SwitchBinaryReport(Node, response);
        }

        public Task Set(bool value, CancellationToken cancellationToken = default)
        {
            return Send(new Command(Class, CommandV1.Set, value ? (byte) 0xFF : (byte) 0x00), cancellationToken);
        }

        #endregion Public Methods
    }
}
