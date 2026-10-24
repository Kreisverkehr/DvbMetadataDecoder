using System.Dynamic;

namespace Kreisverkehr.DvbMetadataDecoder;

public interface ITransportStreamPackage
{
    bool TransportErrorIndicator { get; }
    bool PayloadUnitStartIndicator { get; }
    bool TransportPriority { get; }
    ushort PacketIdentifier { get; }
    bool IsScrambled { get; }
    bool IsScrambledWithEvenKey { get; }
    bool IsScrambledWithOddKey { get; }
    byte ContinuityCounter { get; }
    ITransportStreamPackageAdaptationField? AdaptationField { get; }
    ReadOnlyMemory<byte>? Payload { get; }
}

public interface ITransportStreamPackageAdaptationField
{
    bool DiscontinuityIndicator { get; }
    bool RandomAccessIndicator { get; }
    bool ElementaryStreamPriorityIndicator { get; }
    ulong? ProgramClockReference { get; }
    ulong? OriginalProgramClockReference { get; }
    sbyte? SpliceCountdown { get; }
    ReadOnlyMemory<byte>? TransportPrivateData { get; }
    ITransportStreamPackageAdaptationFieldExtension? Extension { get; }
}

public interface ITransportStreamPackageAdaptationFieldExtension
{
    bool? LegalTimeWindowValid { get; }
    ushort? LegalTimeWindowOffset { get; }
    uint? PiecewiseRate { get; }
    byte? SpliceType { get; }
    ulong? DtsNextAccessUnit { get; }
}