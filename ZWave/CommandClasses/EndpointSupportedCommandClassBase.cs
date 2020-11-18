using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;
using ZWave.CommandClasses.TransportEncapsulation;

namespace ZWave.CommandClasses
{
    public abstract class EndpointSupportedCommandClassBase : CommandClassBase_OLD
    {
        protected EndpointSupportedCommandClassBase(IZwaveNode node, CommandClass commandClass) :
            this(node, commandClass, 0)
        {
        }

        /// <summary>
        ///     Use this constructor if this command class can be accessed only with endpoint id.
        /// </summary>
        /// <param name="node">The node</param>
        /// <param name="endpointId">The endpoint id. 0 means there is no endpoint.</param>
        protected EndpointSupportedCommandClassBase(IZwaveNode node, CommandClass commandClass, byte endpointId)
            : base(node, commandClass)
        {
            EndPoint = endpointId;
        }

        public byte EndPoint { get; }

        protected async Task<byte[]> Send(Command command, Enum responseCommand, CancellationToken cancellationToken)
        {
            if (EndPoint == 0)
            {
                // Send in regular manner.
                //
                return await Channel.Send(Node, command, responseCommand, cancellationToken);
            }

            var encapsolatedCommand = await EncapsulatCommandForEndpoint(command, cancellationToken);
            var response = await Channel.Send(Node, encapsolatedCommand, MultiChannel.CommandV3.Encap, EncapsulatCommandEndpointValidator(responseCommand), cancellationToken);
            return ExtractEndpointResponse(response, responseCommand);
        }

        protected async Task Send(Command command, CancellationToken cancellationToken)
        {
            if (EndPoint == 0)
            {
                // Send in regular manner.
                //
                await Channel.Send(Node, command, cancellationToken);
            }
            else
            {
                var encapsolatedCommand = await EncapsulatCommandForEndpoint(command, cancellationToken);
                await Channel.Send(Node, encapsolatedCommand, cancellationToken);
            }
        }

        internal void HandleEndpointReport(byte[] wrappedPayload, byte reportType)
        {
            HandleEvent(new Command(Class, reportType, wrappedPayload));
        }

        private async Task<Command> EncapsulatCommandForEndpoint(Command command, CancellationToken cancellationToken)
        {
            var controllerId = await Node.Controller.GetNodeID(cancellationToken);

            // Encapsulation have additional 4 params.
            const int encapsolationEdditionalParams = 4;
            var payload = new byte[command.Payload.Length + encapsolationEdditionalParams];
            payload[0] = controllerId;
            payload[1] = EndPoint;
            payload[2] = command.ClassID;
            payload[3] = command.CommandID;
            for (var i = 0; i < command.Payload.Length; i++) payload[i + encapsolationEdditionalParams] = command.Payload[i];

            return new Command(CommandClass.MultiChannel, MultiChannel.CommandV3.Encap, payload);
        }

        private byte[] ExtractEndpointResponse(byte[] response, Enum expectedResponseCommand)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            if (response.Length < 4)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(response)}");

            // Check sub report
            //
            if (response[0] != EndPoint)
                throw new ReponseFormatException($"Got response for endpoint id {response[0]}, while this command class serves endpoint {EndPoint}.");

            if (response[2] != Convert.ToByte(Class) || response[3] != Convert.ToByte(expectedResponseCommand)) throw new ReponseFormatException($"Got unexpected response for encapsolate message for command class {GetType().Name}. The response was for class {response[2]}, and was of type {response[3]}.");

            return response.Skip(4).ToArray();
        }

        private Func<byte[], bool> EncapsulatCommandEndpointValidator(Enum responseCommand)
        {
            return payload => payload.Length >= 4 && payload[0] == EndPoint && payload[3] == Convert.ToByte(responseCommand);
        }
    }
}
