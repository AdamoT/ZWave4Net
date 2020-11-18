using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses.Application
{
    public class SwitchBinaryV2 : SwitchBinary
    {
        #region ICommandClass

        public override byte Version => 2;

        #endregion ICommandClass

        #region ICommandClassInternal

        public override void HandleEvent(Command command)
        {
            var report = new SwitchBinaryReportV2(Node, command.Payload);
            OnChanged(new ReportEventArgs<SwitchBinaryReport>(report));
            ChangedV2?.Invoke(this, report);
        }

        #endregion ICommandClassInternal

        #region Events

        public event EventHandler<SwitchBinaryReportV2> ChangedV2;

        #endregion Events

        #region Public Methods

        public Task Set(bool targetValue, byte duration, CancellationToken cancellationToken = default)
        {
            return Send(new Command(Class, CommandV1.Set, targetValue ? (byte) 0xFF : (byte) 0x00, duration), cancellationToken);
        }

        #endregion Public Methods
    }
}
