using Kreisverkehr.DvbMetadataDecoder.Extensions;
using Kreisverkehr.DvbMetadataDecoder.Lookups;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class ServiceListDescriptor : Descriptor, IServiceListDescriptor
{
    private readonly List<ServiceListEntry> _entries = [];
    public ServiceListDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        foreach(var entryData in data[2..].Chunk(3))
        {
            _entries.Add(new(entryData));
        }
    }

    IReadOnlyCollection<IServiceListDescriptor.IServiceListEntry> IServiceListDescriptor.Services => _entries.AsReadOnly();

    internal class ServiceListEntry : IServiceListDescriptor.IServiceListEntry
    {
        internal ServiceListEntry(ReadOnlySpan<byte> data)
        {
            ServiceId = (ushort)((data[0] << 8) | data[1]);
            ServiceType = data[2];
        }

        public ushort ServiceId {get;private set;}

        public byte ServiceType {get;private set;}

        public DvbServiceTypeInfo? Info => StaticValues.ServiceTypes.TryGetValue(ServiceType, out DvbServiceTypeInfo? value) ? value : null;
    }
}