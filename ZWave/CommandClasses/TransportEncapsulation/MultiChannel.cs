using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses.TransportEncapsulation
{
    /// <summary>
    ///     MultiChannel command class
    /// </summary>
    /// <see
    ///     cref="https://www.silabs.com/documents/login/miscellaneous/SDS13783-Z-Wave-Transport-Encapsulation-Command-Class-Specification.pdf" />
    public class MultiChannel : CommandClassBase
    {
        #region Types

        public enum CommandV3
        {
            EndPointGet = 0x07,
            EndPointReport = 0x08,
            CapabilityGet = 0x09,
            CapabilityReport = 0x0a,
            Encap = 0x0d
        }

        #endregion Types

        #region Fields

        private readonly Dictionary<byte, Dictionary<CommandClass, ICommandClassInternal>> _SupportedEndPointCommandClasses = new Dictionary<byte, Dictionary<CommandClass, ICommandClassInternal>>();

        #endregion Fields

        #region ICommandClassInternal

        public override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            if (command.Payload.Length < 4)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(command.Payload)}");

            var endPointId = command.Payload[0];
            var commandClass = (CommandClass) command.Payload[2];
            var commandType = command.Payload[3];

            lock (_SupportedEndPointCommandClasses)
            {
                if (_SupportedEndPointCommandClasses.TryGetValue(endPointId, out var commandClasses)
                    && commandClasses.TryGetValue(commandClass, out var commandClassInstance))
                    commandClassInstance.HandleEvent(new Command(commandClass, commandType, command.Payload.Skip(4).ToArray()));
            }
        }

        #endregion ICommandClassInternal

        #region ICommandClass

        public override CommandClass Class => CommandClass.MultiChannel;
        public override byte Version => 3;

        #endregion ICommandClass

        #region Public Methods

        public async Task<MultiChannelEndPointReport> GetEndPoints(CancellationToken cancellationToken = default)
        {
            var response = await Channel.Send(Node, new Command(Class, CommandV3.EndPointGet), CommandV3.EndPointReport, cancellationToken)
                .ConfigureAwait(false);

            return new MultiChannelEndPointReport(Node, response);
        }

        public async Task<MultiChannelCapabilityReport> GetEndPointCapabilities(byte endPointId, CancellationToken cancellationToken = default)
        {
            if (endPointId == 0)
                throw new ArgumentException("Endpoint id must be grater then 0.", nameof(endPointId));

            var response = await Channel.Send(Node, new Command(Class, CommandV3.CapabilityGet, endPointId), CommandV3.CapabilityReport, cancellationToken)
                .ConfigureAwait(false);

            return new MultiChannelCapabilityReport(Node, response);
        }

        public ICommandClass GetEndPointCommandClass(byte endPoint, CommandClass commandClass, byte version)
        {
            if (endPoint == 0)
                throw new ArgumentException("Endpoint id must be grater then 0.", nameof(endPoint));

            lock (_SupportedEndPointCommandClasses)
            {
                if (!_SupportedEndPointCommandClasses.TryGetValue(endPoint, out var commandClasses))
                    _SupportedEndPointCommandClasses.Add(endPoint, commandClasses = new Dictionary<CommandClass, ICommandClassInternal>());

                if (commandClasses.TryGetValue(commandClass, out var commandClassInstance))
                    return commandClassInstance;

                commandClassInstance = CommandClassFactory.CreateCommandClass(Node, commandClass, version, endPoint);
                commandClasses.Add(commandClass, commandClassInstance);

                return commandClassInstance;
            }
        }

        #endregion Public Methods

        #region Static Methods

        internal static async Task<byte[]> SendEncapsulatedMessage(IZwaveNode node, CommandClass commandClass, byte endPoint, Command command, Enum responseCommand, CancellationToken cancellationToken = default)
        {
            var controllerId = await node.Controller.GetNodeID(cancellationToken);
            var encapsulatedCommand = EncapsulateCommandForEndpoint(controllerId, endPoint, command);
            var response = await node.Controller.Channel.Send(node, encapsulatedCommand, CommandV3.Encap, EncapsulateCommandEndpointValidator(endPoint, responseCommand), cancellationToken);
            return ExtractEndpointResponse(commandClass, endPoint, response, responseCommand);
        }

        internal static Command EncapsulateCommandForEndpoint(byte controllerId, byte endPoint, Command command)
        {
            // Encapsulation have additional 4 params.
            const int encapsolationEdditionalParams = 4;
            var payload = new byte[command.Payload.Length + encapsolationEdditionalParams];
            payload[0] = controllerId;
            payload[1] = endPoint;
            payload[2] = command.ClassID;
            payload[3] = command.CommandID;
            for (var i = 0; i < command.Payload.Length; i++) payload[i + encapsolationEdditionalParams] = command.Payload[i];

            return new Command(CommandClass.MultiChannel, CommandV3.Encap, payload);
        }

        internal static byte[] ExtractEndpointResponse(CommandClass commandClass, byte endPoint, byte[] response, Enum expectedResponseCommand)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            if (response.Length < 4)
                throw new ReponseFormatException($"The response was not in the expected format. {commandClass}: Payload: {BitConverter.ToString(response)}");

            // Check sub report
            //
            if (response[0] != endPoint)
                throw new ReponseFormatException($"Got response for endpoint id {response[0]}, while this command class serves endpoint {endPoint}.");

            if (response[2] != Convert.ToByte(commandClass) || response[3] != Convert.ToByte(expectedResponseCommand)) throw new ReponseFormatException($"Got unexpected response for encapsolate message for command class {commandClass}. The response was for class {response[2]}, and was of type {response[3]}.");

            return response.Skip(4).ToArray();
        }

        internal static Func<byte[], bool> EncapsulateCommandEndpointValidator(byte endPoint, Enum responseCommand)
        {
            return payload => payload.Length >= 4 && payload[0] == endPoint && payload[3] == Convert.ToByte(responseCommand);
        }

        #endregion Static Methods
    }
}
