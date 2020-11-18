using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses.Application
{
    /// <summary>
    ///     Thermostat SetPoint Command Class
    /// </summary>
    /// <see
    ///     cref="https://www.silabs.com/documents/login/miscellaneous/SDS13781-Z-Wave-Application-Command-Class-Specification.pdf" />
    public class ThermostatSetpoint : CommandClassBase
    {
        #region ICommandClass

        public override CommandClass Class => CommandClass.ThermostatSetpoint;

        #endregion ICommandClass

        #region ICommandClassInternal

        public override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new ThermostatSetpointReport(Node, command.Payload);
            Changed?.Invoke(this, report);
        }

        #endregion ICommandClassInternal

        #region Events

        public event EventHandler<ThermostatSetpointReport> Changed;

        #endregion Evnets

        #region Private Methods

        public static byte[] GetBytes(float value, byte decimals, byte scale)
        {
            var size = 0;
            var bytes = default(byte[]);
            var number = (long) Math.Round(value * Math.Pow(10, decimals));

            if (number >= sbyte.MinValue && number <= sbyte.MaxValue)
            {
                size = sizeof(sbyte);
                bytes = PayloadConverter.GetBytes((sbyte) number);
            }
            else if (number >= short.MinValue && number <= short.MaxValue)
            {
                size = sizeof(short);
                bytes = PayloadConverter.GetBytes((short) number);
            }
            else if (number >= int.MinValue && number <= int.MaxValue)
            {
                size = sizeof(int);
                bytes = PayloadConverter.GetBytes((int) number);
            }
            else
            {
                for (var i = decimals; i >= 0; i--)
                {
                    number = (long) Math.Round(value * Math.Pow(10, i));
                    if (number >= int.MinValue && number <= int.MaxValue)
                    {
                        decimals = i;
                        size = sizeof(int);
                        bytes = PayloadConverter.GetBytes((int) number);
                        break;
                    }
                }
            }

            if (bytes == null)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value can not be converted in a valid format.");

            // create payload, patch first byte below
            var payload = new byte[] {0x00}.Concat(bytes).ToArray();

            // bits 7,6,5: precision, bits 4,3: scale, bits 2,1,0 : size
            payload[0] |= (byte) ((decimals & 0x0f) << 5);
            payload[0] |= (byte) ((scale & 0x03) << 3);
            payload[0] |= (byte) (size & 0x0f);

            return payload;
        }

        #endregion Private Methods

        #region Types

        private enum CommandV1 : byte
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        #endregion Types

        #region Public Methods

        public async Task<ThermostatSetpointReport> Get(ThermostatSetpointType type, CancellationToken cancellationToken = default)
        {
            var response = await Send(new Command(Class, CommandV1.Get, Convert.ToByte(type)), CommandV1.Report, cancellationToken)
                .ConfigureAwait(false);

            return new ThermostatSetpointReport(Node, response);
        }

        /// <summary>
        ///     Sets Thermostat SetPoint
        /// </summary>
        /// <param name="type">Setpoint type</param>
        /// <param name="value">Value to be set</param>
        /// <param name="precision">Value decimal places</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Set(ThermostatSetpointType type, float value, byte precision = 1, TemperatureUnit unit = TemperatureUnit.Celcius, CancellationToken cancellationToken = default)
        {
            var encoded = GetBytes(value, precision, (byte) unit);

            var payload = new[] {Convert.ToByte(type)}.Concat(encoded).ToArray();
            await Send(new Command(Class, CommandV1.Set, payload), cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets all supported Setpoints
        /// </summary>
        /// <param name="result">Result container</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Awaitable Task</returns>
        public virtual async Task GetSupportedSetPoints(ICollection<ThermostatSetpointType> result, CancellationToken cancellationToken = default)
        {
            var allSetPointTypes = Enum.GetValues(typeof(ThermostatSetpointType)).Cast<ThermostatSetpointType>();

            foreach (var setPointType in allSetPointTypes)
            {
                if (setPointType == ThermostatSetpointType.Unused
                    || setPointType == ThermostatSetpointType.Unused3
                    || setPointType == ThermostatSetpointType.Unused4
                    || setPointType == ThermostatSetpointType.Unused5
                    || setPointType == ThermostatSetpointType.Unused6)
                    //Don't query unsupported values
                    continue;

                var report = await Get(setPointType, cancellationToken)
                    .ConfigureAwait(false);

                if (report.SetpointType == ThermostatSetpointType.Unused)
                    continue;

                result.Add(report.SetpointType);
            }
        }

        #endregion Public Methods
    }
}
