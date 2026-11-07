using System.Dynamic;
using Kreisverkehr.DvbMetadataDecoder.Lookups;

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

public interface INetworkNameDescriptor : IServiceInformationDescriptor
{
    string Text { get; }
}

public interface IServiceListDescriptor : IServiceInformationDescriptor
{
    IReadOnlyCollection<IServiceListEntry> Services { get; }
    public interface IServiceListEntry
    {
        ushort ServiceId { get; }
        byte ServiceType { get; }
        DvbServiceTypeInfo? Info { get; }
    }
}

public interface IShortEventDescriptor : IServiceInformationDescriptor
{
    string Iso639Code { get; }
    string Name { get; }
    string Text { get; }
}

public interface IExtendedEventDescriptor : IServiceInformationDescriptor
{
    ushort DescriptorNo { get; }
    ushort LastDescriptorNo { get; }
    string Iso639Code { get; }
    IReadOnlyCollection<IExtendedEventItem> Items { get; }
    string Text { get; }
    public interface IExtendedEventItem
    {
        string Description { get; }
        string Text { get; }
    }
}

public interface IComponentDescriptor : IServiceInformationDescriptor
{
    byte StreamContentExt { get; }
    byte StreamContent { get; }
    byte ComponentType { get; }
    byte ComponentTag { get; }
    string Iso639Code { get; }
    string Description { get; }
}

public interface IMultilingualNetworkNameDescriptor : IServiceInformationDescriptor
{
    IReadOnlyCollection<IMultilingualNetworkName> NetworkNames { get; }
    public interface IMultilingualNetworkName
    {
        string Iso639Code { get; }
        string Text { get; }
    }
}

public interface IMultilingualBouquetNameDescriptor : IServiceInformationDescriptor
{
    IReadOnlyCollection<IMultilingualBouquetName> BouquetNames { get; }
    public interface IMultilingualBouquetName
    {
        string Iso639Code { get; }
        string Text { get; }
    }
}

public interface IMultilingualServiceNameDescriptor : IServiceInformationDescriptor
{
    IReadOnlyCollection<IMultilingualServiceName> ServiceNames { get; }
    public interface IMultilingualServiceName
    {
        string Iso639Code { get; }
        string ServiceProviderName { get; }
        string ServiceName { get; }
    }
}

public interface IMultilingualComponentNameDescriptor : IServiceInformationDescriptor
{
    IReadOnlyCollection<IMultilingualComponentName> ComponentNames { get; }
    public interface IMultilingualComponentName
    {
        string Iso639Code { get; }
        string Text { get; }
    }
}