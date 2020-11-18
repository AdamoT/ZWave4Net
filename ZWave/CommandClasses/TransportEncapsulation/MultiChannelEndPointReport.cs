using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses.TransportEncapsulation
{
    public class MultiChannelEndPointReport : NodeReport
    {
        public MultiChannelEndPointReport(IZwaveNode node, byte[] payload)
            : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            IsDynamicNumberOfEndpoints = (payload[0] & 0x80) > 0;
            AllEndpointsAreIdentical = (payload[0] & 0x40) > 0;
            NumberOfIndividualEndPoints = payload[1];

            // For version 4 only.
            //
            NumberOfAggregatedEndPoints = payload.Length > 2 ? payload[2] : (byte) 0;
        }

        public bool IsDynamicNumberOfEndpoints { get; }
        public bool AllEndpointsAreIdentical { get; }
        public byte NumberOfIndividualEndPoints { get; }
        public byte NumberOfAggregatedEndPoints { get; }
    }
}
