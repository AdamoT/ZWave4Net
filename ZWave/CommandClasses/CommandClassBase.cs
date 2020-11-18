using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.CommandClasses.TransportEncapsulation;

namespace ZWave.CommandClasses
{
    public abstract class CommandClassBase : ICommandClassInternal
    {
        #region Properties

        protected ZWaveChannel Channel { get; private set; }

        #endregion Properties

        #region ICommandClassInternal

        public void Initialize(IZwaveNode node, byte endPoint = 0)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Channel = Node.Controller.Channel ?? throw new InvalidOperationException("Missing channel");

            EndPoint = endPoint;
        }

        #endregion ICommandClassInternal

        #region ICommandClass

        public IZwaveNode Node { get; private set; }
        public abstract CommandClass Class { get; }
        public virtual byte Version => 1;
        public byte EndPoint { get; private set; }

        public virtual void HandleEvent(Command command)
        {
        }

        #endregion ICommandClass

        #region Protected Methods

        protected async Task<byte[]> Send(Command command, Enum responseCommand, CancellationToken cancellationToken)
        {
            if (EndPoint == 0)
            {
                return await Channel.Send(Node, command, responseCommand, cancellationToken);
            }

            var response = await MultiChannel.SendEncapsulatedMessage(Node, Class, EndPoint, command, responseCommand, cancellationToken)
                .ConfigureAwait(false);

            return MultiChannel.ExtractEndpointResponse(Class, EndPoint, response, responseCommand);
        }

        protected async Task Send(Command command, CancellationToken cancellationToken)
        {
            if (EndPoint == 0)
            {
                await Channel.Send(Node, command, cancellationToken);
            }
            else
            {
                var controllerId = await Node.Controller.GetNodeID(cancellationToken);
                var encapsolatedCommand = MultiChannel.EncapsulateCommandForEndpoint(controllerId, EndPoint, command);
                await Channel.Send(Node, encapsolatedCommand, cancellationToken);
            }
        }

        #endregion Protected Methods
    }
}
