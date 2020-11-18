using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Version : CommandClassBase
    {
        #region ICommandClass

        public override CommandClass Class => CommandClass.Version;

        #endregion ICommandClass

        public async Task<VersionReport> Get(CancellationToken cancellationToken = default)
        {
            var response = await Channel.Send(Node, new Command(Class, CommandsV1.Get), CommandsV1.Report, cancellationToken)
                .ConfigureAwait(false);

            return new VersionReport(Node, response);
        }

        public async Task<VersionCommandClassReport> GetCommandClass(CommandClass @class, CancellationToken cancellationToken = default)
        {
            var response = await Channel.Send(
                    Node,
                    new Command(Class, CommandsV1.CommandClassGet, Convert.ToByte(@class)),
                    CommandsV1.CommandClassReport,
                    VersionCommandClassReport.GetResponseValidatorForCommandClass(Node, @class),
                    cancellationToken)
                .ConfigureAwait(false);

            return new VersionCommandClassReport(Node, response);
        }

        #region Types

        protected enum CommandsV1 : byte
        {
            Get = 0x11,
            Report = 0x12,
            CommandClassGet = 0x13,
            CommandClassReport = 0x14
        }

        #endregion Types
    }
}
