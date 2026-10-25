using Kreisverkehr.DvbMetadataDecoder;
using Kreisverkehr.DvbMetadataDecoder.Extensions;

namespace DvbMetadataDecoder.Implementations.ServiceInformation.Sections;

internal class ProgramAssociationSection : Section, IProgramAssociationSection
{
    internal readonly List<ushort> _networkPids = [];
    internal readonly List<(ushort ProgramNumber, ushort ProgramMapPid)> _pidAssociations = [];

    public ProgramAssociationSection(ReadOnlySpan<byte> data) : base(data)
    {
        TransportStreamId = (ushort)((data[3] << 8) | data[4]);

        var bodyData = GetDataPart(data);

        foreach (var chunk in bodyData.Chunk(4))
        {
            ushort progNum = (ushort)((chunk[0] << 8) | chunk[1]);
            ushort pid = (ushort)(((chunk[2] & 0b00011111) << 8) | chunk[3]);

            if (progNum == 0)
                _networkPids.Add(pid);
            else
                _pidAssociations.Add((progNum, pid));
        }
    }

    public IReadOnlyCollection<ushort> NetworkPids => _networkPids.AsReadOnly();

    public IReadOnlyCollection<(ushort ProgramNumber, ushort ProgramMapPid)> PidAssociations => _pidAssociations.AsReadOnly();

    public ushort TransportStreamId { get; private set; }
}
