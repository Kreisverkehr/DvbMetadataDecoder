namespace Kreisverkehr.DvbMetadataDecoder;

public interface IServiceInformationDescriptor
{
    public ushort Tag { get; }
}

public interface IUnknownDescriptor : IServiceInformationDescriptor
{
    public ReadOnlyMemory<byte> Data { get; }
}

public interface IVideoStreamDescriptor : IServiceInformationDescriptor
{
    bool MultipleFramerate { get; }
    byte FrameRateCode { get; }
    bool Mpeg1Only { get; }
    bool ConstrainedParameter { get; }
    bool StillPicture { get; }
    byte? ProfileAndLevelIndication { get; }
    byte? ChromaFormat { get; }
    bool? FramerateExtension { get; }
}

public interface IAudioStreamDescriptor : IServiceInformationDescriptor
{
    bool FreeFormat { get; }
    bool Id { get; }
    byte Layer { get; }
    bool VariableRateAudio { get; }
}

public interface IHierarchyDescriptor : IServiceInformationDescriptor
{
    bool ViewScalability { get; }
    bool TemporalScalability { get; }
    bool SpatialScalability { get; }
    bool QualityScalability { get; }
    byte HierarchyType { get; }
    string HierarchyTypeDescription { get; }
    byte HierarchyLayerIndex { get; }
    bool TrefPresent { get; }
    byte HierarchyEmbeddedLayerIndex { get; }
    byte HierarchyChannel { get; }
}

public interface IRegistrationDescriptor : IServiceInformationDescriptor
{
    uint FormatIdentifier { get; }
    ReadOnlyMemory<byte> AdditionalIdentificationInfo { get; }
}

public interface IDataStreamAlignmentDescriptor : IServiceInformationDescriptor
{
    byte AlignmentType { get; }
}

public interface ITargetBackgroundGridDescriptor : IServiceInformationDescriptor
{
    ushort HorizontalSize { get; }
    ushort VerticalSize { get; }
    byte AspectRatioInformation { get; }
}

public interface IVideoWindowDescriptor : IServiceInformationDescriptor
{
    ushort HorizontalOffset { get; }
    ushort VerticalOffset { get; }
    byte WindowPriority { get; }
}

public interface IConditionalAccessDescriptor : IServiceInformationDescriptor
{
    ushort CaSystemId { get; }
    ushort CaPid { get; }
    ReadOnlyMemory<byte> PrivateData { get; }
}

public interface IIso639LanguageDescriptor : IServiceInformationDescriptor
{
    IReadOnlyList<ILanguageEntry> Languages { get; }

    public interface ILanguageEntry
    {
        string LanguageCode { get; }
        byte AudioType { get; }
    }
}

public interface ISystemClockDescriptor : IServiceInformationDescriptor
{
    bool ExternalClockReference { get; }
    byte ClockAccuracyInteger { get; }
    byte ClockAccuracyExponent { get; }
}

public interface IMultiplexBufferUtilizationDescriptor : IServiceInformationDescriptor
{
    bool BoundValid { get; }
    ushort LtwOffsetLowerBound { get; }
    ushort LtwOffsetUpperBound { get; }
}

public interface ICopyrightDescriptor : IServiceInformationDescriptor
{
    uint CopyrightIdentifier { get; }
    ReadOnlyMemory<byte> AdditionalCopyrightInfo { get; }
}

public interface IMaximumBitrateDescriptor : IServiceInformationDescriptor
{
    uint MaximumBitrate { get; }
}

public interface IPrivateDataIndicatorDescriptor : IServiceInformationDescriptor
{
    uint PrivateDataIndicator { get; }
}

public interface ISmoothingBufferDescriptor : IServiceInformationDescriptor
{
    uint SbLeakRate { get; }
    uint SbSize { get; }
}

public interface IStdDescriptor : IServiceInformationDescriptor
{
    bool LeakValid { get; }
}

public interface IIbpDescriptor : IServiceInformationDescriptor
{
    bool GopClosed { get; }
    bool GopIdentical { get; }
    uint MaxGopLength { get; }
}

public interface IMpeg4Descriptor : IServiceInformationDescriptor
{
    byte ProfileAndLevel { get; }
}

public interface IMpeg4VideoDescriptor : IMpeg4Descriptor
{
    byte VisualProfileAndLevel { get; }
}

public interface IMpeg4AudioDescriptor : IMpeg4Descriptor
{
    byte AudioProfileAndLevel { get; }
}