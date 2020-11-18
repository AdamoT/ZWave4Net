using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class ManufacturerSpecific : CommandClassBase_OLD
    {
        public ManufacturerSpecific(IZwaveNode node) : base(node, CommandClass.ManufacturerSpecific)
        {
        }

        public Task<ManufacturerSpecificReport> Get()
        {
            return Get(CancellationToken.None);
        }

        public async Task<ManufacturerSpecificReport> Get(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report, cancellationToken);
            return new ManufacturerSpecificReport(Node, response);
        }

        private enum command
        {
            Get = 0x04,
            Report = 0x05
        }
    }
}
