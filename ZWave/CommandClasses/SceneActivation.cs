using System;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SceneActivation : CommandClassBase_OLD
    {
        public SceneActivation(IZwaveNode node) : base(node, CommandClass.SceneActivation)
        {
        }

        public event EventHandler<ReportEventArgs<SceneActivationReport>> Changed;

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            if (command.CommandID == Convert.ToByte(SceneActivation.command.Set))
            {
                var report = new SceneActivationReport(Node, command.Payload);
                OnChanged(new ReportEventArgs<SceneActivationReport>(report));
            }
        }

        protected virtual void OnChanged(ReportEventArgs<SceneActivationReport> e)
        {
            Changed?.Invoke(this, e);
        }

        private enum command : byte
        {
            Set = 0x01
        }
    }
}
