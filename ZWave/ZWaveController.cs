using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ZWave.Channel;

namespace ZWave
{
    public sealed class ZWaveController : IDisposable
    {
        #region IDisposable

        public void Dispose()
        {
            Close();
        }

        #endregion IDisposable

        #region Properties

        public ZWaveChannel Channel { get; }

        #endregion Properties

        #region Fields

        private Task<IReadOnlyList<Node>> _GetNodesTask = null;
        private string _ControllerVersion = null;
        private uint? _ControllerHomeID;
        private byte? _ControllerNodeID;

        #endregion Fields

        #region Events

        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler ChannelClosed;

        #endregion Events

        #region Constructors

        private ZWaveController(ZWaveChannel channel)
        {
            Channel = channel;
        }

        public ZWaveController(ISerialPort port)
            : this(new ZWaveChannel(port))
        {
        }

#if NET || WINDOWS_UWP || NETCOREAPP2_0 || NETSTANDARD2_0
        public ZWaveController(string portName)
            : this(new ZWaveChannel(portName))
        {
        }
#endif

#if WINDOWS_UWP
        public ZWaveController(ushort vendorId, ushort productId)
             : this(new ZWaveChannel(vendorId, productId))
        {
        }
#endif

        #endregion Constructors

        private void OnError(ErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        private void OnChannelClosed(EventArgs e)
        {
            ChannelClosed?.Invoke(this, e);
        }

        public void Open()
        {
            Channel.NodeEventReceived += Channel_NodeEventReceived;
            Channel.NodeUpdateReceived += Channel_NodeUpdateReceived;
            Channel.Error += Channel_Error;
            Channel.Closed += Channel_Closed;
            Channel.Open();
        }

        private void Channel_Error(object sender, ErrorEventArgs e)
        {
            OnError(e);
        }

        private void Channel_Closed(object sender, EventArgs e)
        {
            OnChannelClosed(e);
        }

        private async void Channel_NodeEventReceived(object sender, NodeEventArgs e)
        {
            try
            {
                var nodes = await GetNodes()
                    .ConfigureAwait(false);

                var target = nodes[e.NodeID];
                if (target != null)
                {
                    target.HandleEvent(e.Command);
                }
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        private async void Channel_NodeUpdateReceived(object sender, NodeUpdateEventArgs e)
        {
            try
            {
                var nodes = await GetNodes()
                    .ConfigureAwait(false);

                var target = nodes[e.NodeID];
                if (target != null)
                {
                    target.HandleUpdate();
                }
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        public void Close()
        {
            if (Channel != null)
            {
                Channel.Error -= Channel_Error;
                Channel.NodeEventReceived -= Channel_NodeEventReceived;
                Channel.NodeUpdateReceived -= Channel_NodeUpdateReceived;
                Channel.Close();
            }
        }

        public Task<string> GetVersion()
        {
            return GetVersion(CancellationToken.None);
        }

        public async Task<string> GetVersion(CancellationToken cancellationToken)
        {
            if (_ControllerVersion == null)
            {
                var response = await Channel.Send(Function.GetVersion, cancellationToken)
                    .ConfigureAwait(false);

                var data = response.TakeWhile(element => element != 0).ToArray();
                _ControllerVersion = Encoding.UTF8.GetString(data, 0, data.Length);
            }
            return _ControllerVersion;
        }

        public Task<uint> GetHomeID()
        {
            return GetHomeID(CancellationToken.None);
        }

        public async Task<uint> GetHomeID(CancellationToken cancellationToken)
        {
            if (_ControllerHomeID == null)
            {
                var response = await Channel.Send(Function.MemoryGetId, cancellationToken)
                    .ConfigureAwait(false);

                _ControllerHomeID = PayloadConverter.ToUInt32(response);
            }
            return _ControllerHomeID.Value;
        }

        public Task<byte> GetNodeID()
        {
            return GetNodeID(CancellationToken.None);
        }

        public async Task<byte> GetNodeID(CancellationToken cancellationToken)
        {
            if (_ControllerNodeID == null)
            {
                var response = await Channel.Send(Function.MemoryGetId, cancellationToken)
                    .ConfigureAwait(false);
                _ControllerNodeID = response[4];
            }
            return _ControllerNodeID.Value;
        }

        public Task<IReadOnlyList<Node>> DiscoverNodes()
        {
            return DiscoverNodes(CancellationToken.None);
        }

        public Task<IReadOnlyList<Node>> DiscoverNodes(CancellationToken cancellationToken)
        {
            return _GetNodesTask = Task.Run(async () =>
            {
                var response = await Channel.Send(Function.DiscoveryNodes, cancellationToken)
                    .ConfigureAwait(false);
                var values = response.Skip(3).Take(29).ToArray();

                var nodes = new List<Node>();
                var bits = new BitArray(values);
                for (byte i = 0; i < bits.Length; i++)
                {
                    if (bits[i])
                    {
                        var node = new Node((byte)(i + 1), this);
                        nodes.Add(node);
                    }
                }
                return nodes as IReadOnlyList<Node>;
            });
        }

        public Task<IReadOnlyList<Node>> GetNodes()
        {
            return GetNodes(CancellationToken.None);
        }

        public Task<IReadOnlyList<Node>> GetNodes(CancellationToken cancellationToken)
        {
            return (_GetNodesTask ?? DiscoverNodes(cancellationToken));
        }
    }
}
