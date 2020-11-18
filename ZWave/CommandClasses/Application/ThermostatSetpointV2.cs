using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses.Application
{
    /// <summary>
    ///     Thermostat SetPoint Command Class V2
    /// </summary>
    /// <see
    ///     cref="https://www.silabs.com/documents/login/miscellaneous/SDS13781-Z-Wave-Application-Command-Class-Specification.pdf" />
    public class ThermostatSetpointV2 : ThermostatSetpoint
    {
        #region ICommandClass

        public override byte Version => 2;

        #endregion ICommandClass

        #region Types

        private enum CommandV2
        {
            SupportedGet = 0x04,
            SupportedReport = 0x05
        }

        #endregion Types

        #region Public Methods

        /// <summary>
        ///     Gets all supported Setpoints
        /// </summary>
        /// <param name="isAinterpretation">True if should follow A interpretation. See docs.</param>
        /// <param name="result">Result container</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Awaitable Task</returns>
        public virtual async Task GetSupportedSetPoints(ICollection<ThermostatSetpointType> result, bool isAinterpretation = true, CancellationToken cancellationToken = default)
        {
            var report = await GetSupportedSetPointsReport(isAinterpretation, cancellationToken)
                .ConfigureAwait(false);

            for (var i = 0; i < report.SupportedTypes.Count; ++i)
                result.Add(report.SupportedTypes[i]);
        }

        public async Task<ThermostatSetPointSupportedReport> GetSupportedSetPointsReport(bool isAinterpretation = true, CancellationToken cancellationToken = default)
        {
            var response = await Send(new Command(Class, CommandV2.SupportedGet), CommandV2.SupportedReport, cancellationToken)
                .ConfigureAwait(false);

            return new ThermostatSetPointSupportedReport(Node, isAinterpretation, response);
        }

        #endregion Public Methods
    }
}
