using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class MultiChannelAssociation : CommandClassBase_OLD
    {
        private const byte MultiChannelAssociationSetMarker = 0;
        private const byte MultiChannelAssociationRemoveMarker = 0;

        public MultiChannelAssociation(IZwaveNode node)
            : base(node, CommandClass.MultiChannelAssociation)
        {
        }

        public Task Add(byte groupId, byte[] destinationNodeIds, Endpoint[] destinationEndpoints)
        {
            return Add(groupId, destinationNodeIds, destinationEndpoints, CancellationToken.None);
        }

        public Task Add(byte groupId, byte[] destinationNodeIds, Endpoint[] destinationEndpoints, CancellationToken cancellationToken)
        {
            return Channel.Send(Node, new Command(Class, command.Set, GetEndpointsPayload(groupId, destinationNodeIds, destinationEndpoints, MultiChannelAssociationSetMarker)), cancellationToken);
        }

        public Task<MultiChannelAssociationReport> Get(byte groupdId)
        {
            return Get(groupdId, CancellationToken.None);
        }

        public async Task<MultiChannelAssociationReport> Get(byte groupId, CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, groupId), command.Report, cancellationToken);
            return new MultiChannelAssociationReport(Node, response);
        }

        public Task Remove(byte groupId, byte[] destinationNodeIds, Endpoint[] destinationEndpoints)
        {
            return Remove(groupId, destinationNodeIds, destinationEndpoints, CancellationToken.None);
        }

        public Task Remove(byte groupId, byte[] destinationNodeIds, Endpoint[] destinationEndpoints, CancellationToken cancellationToken)
        {
            return Channel.Send(Node, new Command(Class, command.Remove, GetEndpointsPayload(groupId, destinationNodeIds, destinationEndpoints, MultiChannelAssociationRemoveMarker)), cancellationToken);
        }

        public Task<AssociationGroupsReport> GetGroups()
        {
            return GetGroups(CancellationToken.None);
        }

        public async Task<AssociationGroupsReport> GetGroups(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.GroupingsGet), command.GroupingsReport, cancellationToken);
            return new AssociationGroupsReport(Node, response);
        }

        private static byte[] GetEndpointsPayload(byte groupId, byte[] destinationNodeIds, Endpoint[] destinationEndpoints, byte marker)
        {
            var endpointsPayload = destinationEndpoints.SelectMany(e => new[] {e.NodeId, e.EndpointId});
            return new[] {groupId}.Concat(destinationNodeIds).Concat(new[] {marker}).Concat(endpointsPayload).ToArray();
        }

        public class Endpoint
        {
            public Endpoint(byte nodeId, byte endpointId)
            {
                NodeId = nodeId;
                EndpointId = endpointId;
            }

            public byte NodeId { get; }

            public byte EndpointId { get; }
        }

        private enum command
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03,
            Remove = 0x04,
            GroupingsGet = 0x05,
            GroupingsReport = 0x06
        }
    }
}
