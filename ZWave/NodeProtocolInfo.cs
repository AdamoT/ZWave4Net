using System;
using System.Collections.Generic;
using ZWave.Channel;

namespace ZWave
{
    public class NodeProtocolInfo
    {
        #region Properties

        public NodeCapabilities Capability { get; private set; }
        public NodeSecurityFlags Security { get; private set; }
        public byte Reserved { get; private set; }
        public BasicType BasicType { get; private set; }
        public GenericType GenericType { get; private set; }
        public byte SpecificType { get; private set; }

        public byte ProtocolVersion => (byte) ((byte) Capability & 0b111);
        public int MaxBaudrate => ((byte) Capability & 0b111000) == 0x10 ? 40000 : 9600;

        public IReadOnlyList<CommandClass> CommandClasses => _CommandClasses;
        private readonly List<CommandClass> _CommandClasses = new List<CommandClass>();

        public IReadOnlyList<byte> Bytes { get; private set; }

        #endregion Properties

        #region Public Methods

        public static NodeProtocolInfo Parse(byte[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length < 6)
                throw new ArgumentException("At least 6 bytes required", nameof(data));

            var result = new NodeProtocolInfo
            {
                Bytes = data,
                Capability = (NodeCapabilities) data[0],
                Security = (NodeSecurityFlags) data[1],
                Reserved = data[2],
                BasicType = (BasicType) data[3],
                GenericType = (GenericType) data[4],
                SpecificType = data[5]
            };

            for (var i = 6; i < data.Length; ++i)
                result._CommandClasses.Add((CommandClass) data[i]);

            return result;
        }

        public override string ToString()
        {
            return $"Capability = [{Capability}], Security = [{Security}], BasicType = {BasicType}, GenericType = {GenericType}, SpecificType = {SpecificType}, Listening = {Capability.HasFlag(NodeCapabilities.Listening)}, ProtocolVersion = {ProtocolVersion}, Routing = {Capability.HasFlag(NodeCapabilities.Routing)}, MaxBaudrate = {MaxBaudrate}, CommandClasses = [{string.Join(", ", CommandClasses)}]";
        }

        #endregion Public Methods
    }
}
