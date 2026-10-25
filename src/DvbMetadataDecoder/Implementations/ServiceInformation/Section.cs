using System;
using DvbMetadataDecoder.Implementations.ServiceInformation.Sections;
using Kreisverkehr.DvbMetadataDecoder;

namespace DvbMetadataDecoder.Implementations.ServiceInformation;

internal abstract class Section : IServiceInformationSection
{
    private readonly byte _tableId;
    protected readonly ushort _sectionLength;
    private readonly byte _versionNumber;
    private readonly bool _currentNextIndicator;
    private readonly byte _sectionNumber;
    private readonly byte _lastSectionNumber;

    internal Section(ReadOnlySpan<byte> data)
    {
        _tableId = data[0];
        _sectionLength = (ushort)(((data[1] & 0b00001111) << 8) | data[2]);
        _versionNumber = (byte)((data[5] & 0b00111110) >> 1);
        _currentNextIndicator = (data[5] & 0b00000001) != 0;
        _sectionNumber = data[6];
        _lastSectionNumber = data[7];
    }

    protected ReadOnlySpan<byte> GetDataPart(ReadOnlySpan<byte> data) => data[8..(_sectionLength - 1)];

    public byte TableId => _tableId;
    public byte VersionNumber => _versionNumber;
    public bool IsCurrent => _currentNextIndicator;
    public byte SectionNumber => _sectionNumber;
    public byte LastSectionNumber => _lastSectionNumber;
}

internal class UnknownSection : Section, IUnknownSection
{
    public UnknownSection(ReadOnlySpan<byte> data) : base(data)
    {
        SectionData = data.ToArray();
    }

    public ReadOnlyMemory<byte> SectionData { get; private set; }
}