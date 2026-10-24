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
var tableReader = scope.ServiceProvider.GetRequiredService<IServiceInformationTableFactory>().Subscribe();

ReadOnlyMemory<byte> data = await File.ReadAllBytesAsync("Samples/testdata.ts");
data = data[..(1000 * 188)];
var feedTask = decoder.FeedAsync(data);

while(await tableReader.WaitToReadAsync() && tableReader.TryRead(out var table))
{
    Console.WriteLine($"{table.TableId:X2}");
}

await feedTask;
