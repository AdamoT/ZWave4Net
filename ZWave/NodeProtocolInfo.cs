using System;
using System.Collections.Generic;

namespace ZWave
{
    public class NodeProtocolInfo
    {
        #region Properties

        public byte Capability { get; private set; }
        public byte Reserved { get; private set; }
        public BasicType BasicType { get; private set; }
        public GenericType GenericType { get; private set; }
        public byte SpecificType { get; private set; }
        public Security Security { get; private set; }

        public bool Routing => (Capability & 0x40) != 0;
        public bool IsListening => (Capability & 0x80) != 0;
        public byte Version => (byte)((Capability & 0x07) + 1);
        public int MaxBaudrate => ((Capability & 0x38) == 0x10) ? 40000 : 9600;

        public IReadOnlyList<byte> Bytes { get; private set; }

        #endregion Properties

        #region Public Methods

        public static NodeProtocolInfo Parse(byte[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length < 6)
                throw new ArgumentException("At least 6 bytes required", nameof(data));

            return new NodeProtocolInfo()
            {
                Bytes = data,
                Capability = data[0],
                Security = (Security)data[1],
                Reserved = data[2],
                BasicType = (BasicType)data[3],
                GenericType = (GenericType)data[4],
                SpecificType = data[5],
            };
        }

        public override string ToString()
        {
            return $"GenericType = {GenericType}, BasicType = {BasicType}, Listening = {IsListening}, Version = {Version}, Security = [{Security}], Routing = {Routing}, MaxBaudrate = {MaxBaudrate}";
        }

        #endregion Public Methods
    }
}
