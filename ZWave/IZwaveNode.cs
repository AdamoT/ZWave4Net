using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.CommandClasses;

namespace ZWave
{
    public interface IZwaveNode
    {
        #region Properties

        byte NodeID { get; }
        ZWaveController Controller { get; } //TODO: this should be an interface

        #endregion Properties

        #region Events

        /// <summary>
        ///     Will be fired when unknown command received.
        /// </summary>
        event EventHandler<NodeEventArgs> UnknownCommandReceived;

        /// <summary>
        ///     Will be fired when node update command received.
        /// </summary>
        event EventHandler<EventArgs> UpdateReceived;

        /// <summary>
        ///     Will be fired when any command received.
        /// </summary>
        event EventHandler<EventArgs> MessageReceived;

        #endregion Events

        #region Methods

        Task<NodeProtocolInfo> GetProtocolInfo(CancellationToken cancellationToken = default);
        Task<IEnumerable<ICommandClass>> GetSupportedCommandClasses(CancellationToken cancellationToken = default);

        #endregion Methods
    }
}
