using System.Collections.Generic;

namespace ZWave.CommandClasses.Application
{
    public class ThermostatSetPointSupportedReport : NodeReport
    {
        private readonly List<ThermostatSetpointType> _SupportedTypes = new List<ThermostatSetpointType>();

        /// <summary>
        ///     Constructs supported report
        /// </summary>
        /// <param name="node">Node for which report should be constructed</param>
        /// <param name="isAinterpretation">True if should follow A interpretation. See docs.</param>
        /// <param name="payload">Message Payload</param>
        public ThermostatSetPointSupportedReport(IZwaveNode node, bool isAInterpretation, byte[] payload) : base(node)
        {
            for (var maskIndex = 0; maskIndex < 2; ++maskIndex)
            {
                if (payload.Length < maskIndex)
                    break;

                var mask = payload[maskIndex];
                if (maskIndex == 0)
                {
                    if (PayloadConverter.IsBitSet(mask, 1))
                        _SupportedTypes.Add(ThermostatSetpointType.Cooling);
                    if (PayloadConverter.IsBitSet(mask, 2))
                        _SupportedTypes.Add(ThermostatSetpointType.Cooling);


                    if (isAInterpretation)
                    {
                        if (PayloadConverter.IsBitSet(mask, 3))
                            _SupportedTypes.Add(ThermostatSetpointType.Furnace);
                        if (PayloadConverter.IsBitSet(mask, 4))
                            _SupportedTypes.Add(ThermostatSetpointType.DryAir);
                        if (PayloadConverter.IsBitSet(mask, 5))
                            _SupportedTypes.Add(ThermostatSetpointType.MoistAir);
                        if (PayloadConverter.IsBitSet(mask, 6))
                            _SupportedTypes.Add(ThermostatSetpointType.AutoChangeover);
                        if (PayloadConverter.IsBitSet(mask, 7))
                            _SupportedTypes.Add(ThermostatSetpointType.EnergySaveHeating);
                    }
                    else
                    {
                        if (PayloadConverter.IsBitSet(mask, 7))
                            _SupportedTypes.Add(ThermostatSetpointType.Furnace);
                    }
                }
                else if (maskIndex == 1)
                {
                    if (isAInterpretation)
                    {
                        if (PayloadConverter.IsBitSet(mask, 0))
                            _SupportedTypes.Add(ThermostatSetpointType.EnergySaveCooling);
                        if (PayloadConverter.IsBitSet(mask, 1))
                            _SupportedTypes.Add(ThermostatSetpointType.AwayHeating);
                        if (PayloadConverter.IsBitSet(mask, 2))
                            _SupportedTypes.Add(ThermostatSetpointType.AwayCooling);
                        if (PayloadConverter.IsBitSet(mask, 3))
                            _SupportedTypes.Add(ThermostatSetpointType.FullPower);
                    }
                    else
                    {
                        if (PayloadConverter.IsBitSet(mask, 0))
                            _SupportedTypes.Add(ThermostatSetpointType.DryAir);
                        if (PayloadConverter.IsBitSet(mask, 1))
                            _SupportedTypes.Add(ThermostatSetpointType.MoistAir);
                        if (PayloadConverter.IsBitSet(mask, 2))
                            _SupportedTypes.Add(ThermostatSetpointType.AutoChangeover);
                        if (PayloadConverter.IsBitSet(mask, 3))
                            _SupportedTypes.Add(ThermostatSetpointType.EnergySaveHeating);
                        if (PayloadConverter.IsBitSet(mask, 4))
                            _SupportedTypes.Add(ThermostatSetpointType.EnergySaveCooling);
                        if (PayloadConverter.IsBitSet(mask, 5))
                            _SupportedTypes.Add(ThermostatSetpointType.AwayHeating);
                        if (PayloadConverter.IsBitSet(mask, 6))
                            _SupportedTypes.Add(ThermostatSetpointType.AwayCooling);
                        if (PayloadConverter.IsBitSet(mask, 7))
                            _SupportedTypes.Add(ThermostatSetpointType.FullPower);
                    }
                }
            }
        }

        public IReadOnlyList<ThermostatSetpointType> SupportedTypes => _SupportedTypes;
    }
}
