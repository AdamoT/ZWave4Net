﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SensorBinary : EndpointSupportedCommandClassBase
    {
        public SensorBinary(IZwaveNode node)
            : base(node, CommandClass.SensorBinary)
        {
        }

        internal SensorBinary(IZwaveNode node, byte endpointId)
            : base(node, CommandClass.SensorBinary, endpointId)
        {
        }

        public event EventHandler<ReportEventArgs<SensorBinaryReport>> Changed;

        public Task<SensorBinaryReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<SensorBinaryReport> Get(CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.Get), command.Report, cancellationToken);
            return new SensorBinaryReport(Node, response);
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new SensorBinaryReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<SensorBinaryReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<SensorBinaryReport> e)
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
