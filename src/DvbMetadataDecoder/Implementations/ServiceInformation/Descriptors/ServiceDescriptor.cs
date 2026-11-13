using System.Diagnostics;
using System.Text;
using Kreisverkehr.DvbMetadataDecoder.Extensions;
using Kreisverkehr.DvbMetadataDecoder.Lookups;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

[DebuggerDisplay("{ServiceProviderName} - {ServiceName}")]
internal class ServiceDescriptor : Descriptor, IServiceDescriptor
{
    public ServiceDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        ServiceType = data[2];

        int providerNameLen = data[3];
        if(providerNameLen > 0)
            ServiceProviderName = Encoding.DvbText.GetString(data.Slice(4, providerNameLen));
        int serviceNameLen = data[providerNameLen + 4];
        if(serviceNameLen > 0)
            ServiceName = Encoding.DvbText.GetString(data.Slice(providerNameLen + 5, serviceNameLen));
    }

    public byte ServiceType { get; private set; }

    public string ServiceProviderName { get; private set; } = string.Empty;

    public string ServiceName { get; private set; } = string.Empty;

    public DvbServiceTypeInfo? ServiceInfo => StaticValues.ServiceTypes.TryGetValue(ServiceType, out DvbServiceTypeInfo? value) ? value : null;
}