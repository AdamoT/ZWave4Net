using System.Collections;
using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class SensorMultilevelSupportedSensorReport : NodeReport
    {
        public SensorMultilevelSupportedSensorReport(IZwaveNode node, byte[] payload)
            : base(node)
        {
            var supportedTypes = new LinkedList<SensorType>();
            var bits = new BitArray(payload);
            for (byte i = 0; i < bits.Length; i++)
                if (bits[i])
                    supportedTypes.AddLast((SensorType) (i + 1));

            SupportedSensorTypes = supportedTypes;
        }

        public IReadOnlyCollection<SensorType> SupportedSensorTypes { get; }
    }
}
