namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Tables;

internal sealed class ServiceDescriptionTable : IServiceDescriptionTable
{
    public ServiceDescriptionTable(
        byte tableId,
        ushort transportStreamId,
        ushort originalNetworkId,
        byte versionNumber,
        IEnumerable<IServiceDescriptionSection.IServiceDescription> services)
    {
        TableId = tableId;
        TransportStreamId = transportStreamId;
        OriginalNetworkId = originalNetworkId;
        VersionNumber = versionNumber;
        Services = Array.AsReadOnly(services.ToArray());
    }

    public byte TableId { get; }

    public ushort TransportStreamId { get; }

    public ushort OriginalNetworkId { get; }

    public byte VersionNumber { get; }

    public IReadOnlyCollection<IServiceDescriptionSection.IServiceDescription> Services { get; }
}