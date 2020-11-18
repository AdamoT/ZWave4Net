﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SensorAlarm : CommandClassBase_OLD
    {
        public SensorAlarm(IZwaveNode node) : base(node, CommandClass.SensorAlarm)
        {
        }

        public event EventHandler<ReportEventArgs<SensorAlarmReport>> Changed;

        public Task<SensorAlarmReport> Get(AlarmType type)
        {
            return Get(type, CancellationToken.None);
        }

        public async Task<SensorAlarmReport> Get(AlarmType type, CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, Convert.ToByte(type)), command.Report, cancellationToken);
            return new SensorAlarmReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SensorAlarmReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SensorAlarmReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<SensorAlarmReport> e)
        {
            var handler = Changed;
            if (handler != null) handler(this, e);
        }

        private enum command
        {
            Get = 0x01,
            Report = 0x02,
            SupportedGet = 0x03,
            SupportedReport = 0x04
        }
    }
}
