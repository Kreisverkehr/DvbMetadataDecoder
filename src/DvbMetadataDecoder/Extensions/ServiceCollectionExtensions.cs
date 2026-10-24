using Kreisverkehr.DvbMetadataDecoder;
using Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation;
using Kreisverkehr.DvbMetadataDecoder.Implementations.TransportStream;

namespace Microsoft.Extensions.DependencyInjection;

public static class SsdpServiceCollectionExtensions
{
    public static IServiceCollection AddDvbMetadataDecoder(this IServiceCollection services) => services
        .AddScoped<IDvbMetadataDecoder, Kreisverkehr.DvbMetadataDecoder.DvbMetadataDecoder>()
        .AddScoped<ITransportStreamPackageFactory, PackageFactory>()
        .AddScoped<IServiceInformationTableFactory, TableFactory>()
        ;
}