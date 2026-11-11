#:project ../src/DvbMetadataDecoder/DvbMetadataDecoder.csproj
#:package Microsoft.Extensions.DependencyInjection@10.0.*

using Kreisverkehr.DvbMetadataDecoder;
using Microsoft.Extensions.DependencyInjection;

IServiceProvider sp = new ServiceCollection()
    .AddDvbMetadataDecoder()
    .BuildServiceProvider()
;

await using var scope = sp.CreateAsyncScope();

var decoder = scope.ServiceProvider.GetRequiredService<IDvbMetadataDecoder>();
var metadataRepository = scope.ServiceProvider.GetRequiredService<IDvbMetadataRepository>();

ReadOnlyMemory<byte> data = await File.ReadAllBytesAsync(@"Samples/completemultiplex.ts");
var feedTask = decoder.FeedAsync(data);



await feedTask;
