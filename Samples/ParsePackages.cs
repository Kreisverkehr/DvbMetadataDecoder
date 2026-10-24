#:project ../src/DvbMetadataDecoder/DvbMetadataDecoder.csproj
#:package Microsoft.Extensions.DependencyInjection@10.0.*

using Kreisverkehr.DvbMetadataDecoder;
using Microsoft.Extensions.DependencyInjection;

IServiceProvider sp = new ServiceCollection()
    .AddDvbMetadataDecoder()
    .BuildServiceProvider()
;

await using var scope = sp.CreateAsyncScope();

var factory = scope.ServiceProvider.GetRequiredService<ITransportStreamPackageFactory>();

ReadOnlyMemory<byte> data = await File.ReadAllBytesAsync("Samples/testdata.ts");
data = data[..(1000 * 188)];
var feedTask = factory.FeedAsync(data);

var packageCount = data.Length / 188;
for (var index = 0; index < packageCount; index++)
{
    var package = await factory.ReadAsync();
    Console.WriteLine(
        $"Packet {index + 1:00000000}: PID=0x{package.PacketIdentifier:X4} ({package.PacketIdentifier}), " +
        $"TEI={package.TransportErrorIndicator}, " +
        $"PUSI={package.PayloadUnitStartIndicator}, " +
        $"Priority={package.TransportPriority}, " +
        $"Scrambled={package.IsScrambled}, " +
        $"Key={(package.IsScrambledWithEvenKey ? "even" : package.IsScrambledWithOddKey ? "odd" : "none")}, " +
        $"ContinuityCounter={package.ContinuityCounter}, " +
        $"Payload: {package.Payload?.Length} bytes"
    );

    if (package.AdaptationField is not null)
    {
        Console.WriteLine(
            $"AdaptationField: " +
            $"DiscontinuityIndicator: {package.AdaptationField.DiscontinuityIndicator}, " +
            $"RandomAccessIndicator: {package.AdaptationField.RandomAccessIndicator}, " +
            $"ElementaryStreamPriorityIndicator: {package.AdaptationField.ElementaryStreamPriorityIndicator}, "
        );
        Console.WriteLine("                 " +
            $"PCR: {package.AdaptationField.ProgramClockReference}, " +
            $"OPCR: {package.AdaptationField.OriginalProgramClockReference}, " +
            $"SpliceCountDown: {package.AdaptationField.SpliceCountdown}, " +
            $"PrivateData: {package.AdaptationField.TransportPrivateData?.Length ?? 0} bytes, "
        );
        if (package.AdaptationField.Extension is not null)
        {
            Console.WriteLine("      Extension: " +
                $"LTW Valid: {package.AdaptationField.Extension.LegalTimeWindowValid}, " +
                $"LTW offset: {package.AdaptationField.Extension.LegalTimeWindowOffset}, " +
                $"Piecewise rate: {package.AdaptationField.Extension.PiecewiseRate}, " +
                $"Splice type: {package.AdaptationField.Extension.SpliceType:X1}, " +
                $"DTS next access unit: {package.AdaptationField.Extension.DtsNextAccessUnit}, "
            );
        }
    }
}

await feedTask;
