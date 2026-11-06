#:project ../src/DvbMetadataDecoder/DvbMetadataDecoder.csproj
#:package Microsoft.Extensions.DependencyInjection@10.0.*

using System.IO.Pipes;
using Kreisverkehr.DvbMetadataDecoder;
using Microsoft.Extensions.DependencyInjection;

IServiceProvider sp = new ServiceCollection()
    .AddDvbMetadataDecoder()
    .BuildServiceProvider()
;

await using var scope = sp.CreateAsyncScope();

var decoder = scope.ServiceProvider.GetRequiredService<IDvbMetadataDecoder>();
var sectionReader = scope.ServiceProvider.GetRequiredService<IServiceInformationSectionFactory>().Subscribe();

ReadOnlyMemory<byte> data = await File.ReadAllBytesAsync("Samples/testdata.ts");
// data = data[..(10000 * 188)];
var feederTask = decoder.FeedAsync(data).AsTask().ContinueWith(async _ =>
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
        await scope.DisposeAsync();
    });

while (await sectionReader.WaitToReadAsync() && sectionReader.TryRead(out var sectionData))
{
    // Console.WriteLine($"{section.GetType()}: {section.TableId:X2}");

    (ushort pid, var section) = sectionData;

    if(section is IProgramAssociationSection patSection)
    {
        Console.WriteLine($"NetworkIDs: {string.Join(", ", patSection.NetworkPids)}");
        Console.WriteLine($"Associations: {string.Join(", ", patSection.PidAssociations)}");
    }

    if(section is INetworkInformationSection nitSection)
    {
        Console.WriteLine($"NIT found! TableId = {nitSection.TableId:X2}, NetworkId = {nitSection.NetworkId:X4}");
    }

    if(section is IProgramMapSection pmtSection)
    {
        Console.WriteLine($"ProgramMap found! TableId = {pmtSection.TableId:X2}, ProgramNumber = {pmtSection.ProgramNumber:X4}, PCR PID = {pmtSection.PcrPid:X4}");
    }

    if(section is ITransportStreamDescriptionSection tsdSection)
    {
        Console.WriteLine($"TS Description found! TableId = {tsdSection.TableId:X2}");
    }

    if(section is IConditionalAccessSection caSection)
    {
        Console.WriteLine($"CA Section found! TableId = {caSection.TableId:X2}");
    }
}

await feederTask;