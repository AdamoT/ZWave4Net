using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.CommandClasses;
using ZWave.Utils;
using Version = ZWave.CommandClasses.Version;

namespace ZWave
{
    public class ZWaveNode : IZwaveNodeInternal
    {
        #region Constructors

        internal ZWaveNode(byte nodeID, ZWaveController contoller)
        {
            NodeID = nodeID;
            Controller = contoller;

            _ProtocolInfoProperty = new AsyncLazyProperty<NodeProtocolInfo>(async (existingValue, cancellationToken) =>
            {
                var response = await Controller.Channel.Send(Function.GetNodeProtocolInfo, cancellationToken, NodeID)
                    .ConfigureAwait(false);

                return NodeProtocolInfo.Parse(response);
            });

            _SupportedCommandClassesProperty = new AsyncLazyProperty<Dictionary<CommandClass, ICommandClassInternal>>(DiscoverCommandClasses);
        }

        #endregion Constructors

        #region Private Methods

        private async Task<Dictionary<CommandClass, ICommandClassInternal>> DiscoverCommandClasses(Dictionary<CommandClass, ICommandClassInternal> commandClasses, CancellationToken cancellationToken = default)
        {
            commandClasses = _SupportedCommandClasses;

            if (NodeID == await Controller.GetNodeID()
                    .ConfigureAwait(false))
                //Controller doesn't support any application level command classes
                return commandClasses;

            var protocolInfo = await GetProtocolInfo()
                .ConfigureAwait(false);

            if (protocolInfo.CommandClasses.Count > 0)
                //Command classes are reported in NIF
                if (!protocolInfo.CommandClasses.Contains(CommandClass.Version))
                {
                    //Version command class is not supported - possibly because of security? - currently not supported

                    //Assume all command classes reported in NIF are in version 1
                    for (var i = 0; i < protocolInfo.CommandClasses.Count; ++i)
                    {
                        var supportedCommandClass = protocolInfo.CommandClasses[i];
                        var commandClassInstance = CommandClassFactory.CreateCommandClass(this, supportedCommandClass, 1);
                        _SupportedCommandClasses.Add(supportedCommandClass, commandClassInstance);
                    }

                    return _SupportedCommandClasses;
                }
            //else: Version command class is supported - continue with command class interview

            //Create and store a temporary version class instance
            Version versionCommandClass = null;
            if (commandClasses.TryGetValue(CommandClass.Version, out var commandClass))
                versionCommandClass = commandClass as Version;
            else commandClasses.Add(CommandClass.Version, versionCommandClass = CommandClassFactory.CreateCommandClass(this, CommandClass.Version, 1) as Version);

            var commandClassesToQuery = protocolInfo.CommandClasses.Count > 0
                ? protocolInfo.CommandClasses
                //: new[] { CommandClass.Version, CommandClass.MultiChannel };
                : Enum.GetValues(typeof(CommandClass)).Cast<CommandClass>();

            foreach (var commandClassType in commandClassesToQuery)
            {
                if (commandClassType == CommandClass.NoOperation)
                    //Already handled or shuld not be handled
                    continue;

                var versionReport = await versionCommandClass.GetCommandClass(commandClassType, cancellationToken)
                    .ConfigureAwait(false);

                if (versionReport.Version == 0)
                    //Command class is not supported
                    continue;


                var commandClassInstanceType = CommandClassFactory.GetCommandClassInstanceType(commandClassType, versionReport.Version);
                if (commandClassInstanceType != null &&
                    (!commandClasses.TryGetValue(commandClassType, out var existingInstance) || commandClassInstanceType != existingInstance.GetType()))
                {
                    //Create new instance of the command class
                    var commandClassInstance = CommandClassFactory.CreateCommandClass(this, commandClassType, versionReport.Version);
                    commandClasses[commandClassType] = commandClassInstance;
                }
            }

            return commandClasses;
        }

        #endregion Private Methods

        #region INode

        public ZWaveController Controller { get; }
        public byte NodeID { get; }

        public Task<NodeProtocolInfo> GetProtocolInfo(CancellationToken cancellationToken = default)
        {
            return _ProtocolInfoProperty.GetValue(cancellationToken);
        }

        public async Task<IEnumerable<ICommandClass>> GetSupportedCommandClasses(CancellationToken cancellationToken = default)
        {
            if (_SupportedCommandClasses.Count == 0 || _SupportedCommandClasses.First().Key == CommandClass.Version)
                //Get command classes
                _SupportedCommandClasses = await _SupportedCommandClassesProperty.GetValue(cancellationToken)
                    .ConfigureAwait(false);
            return _SupportedCommandClasses.Values;
        }

        public event EventHandler<NodeEventArgs> UnknownCommandReceived;
        public event EventHandler<EventArgs> UpdateReceived;
        public event EventHandler<EventArgs> MessageReceived;

        #endregion INode

        #region IZWaveNodeInternal

        public void HandleEvent(Command command)
        {
            MessageReceived?.Invoke(this, EventArgs.Empty);

            if (_SupportedCommandClasses != null && _SupportedCommandClasses.TryGetValue((CommandClass) command.ClassID, out var commandClass))
            {
                commandClass.HandleEvent(command);
            }
            else
            {
                if (UnknownCommandReceived != null)
                    UnknownCommandReceived.Invoke(this, new NodeEventArgs(NodeID, command));
            }
        }

        public void HandleUpdate()
        {
            MessageReceived?.Invoke(this, EventArgs.Empty);
            if (UpdateReceived != null)
                UpdateReceived.Invoke(this, EventArgs.Empty);
        }

        #endregion IZWaveNodeInternal

        #region Fields

        private readonly AsyncLazyProperty<NodeProtocolInfo> _ProtocolInfoProperty = null;
        private readonly AsyncLazyProperty<Dictionary<CommandClass, ICommandClassInternal>> _SupportedCommandClassesProperty = null;
        private Dictionary<CommandClass, ICommandClassInternal> _SupportedCommandClasses = new Dictionary<CommandClass, ICommandClassInternal>();

        #endregion Fields
    }
}
