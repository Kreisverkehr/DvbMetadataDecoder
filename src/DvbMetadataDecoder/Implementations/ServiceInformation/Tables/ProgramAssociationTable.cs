using System.Threading.Channels;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Tables;

internal class ProgramAssociationTable : IProgramAssociationTable
{
    private readonly IServiceInformationSectionFactory _sectionFactory;
    private readonly ChannelReader<(ushort Pid, IServiceInformationSection Section)> _subscription;
    private readonly Task _subscriptionWorkerTask;

    public ProgramAssociationTable(IServiceInformationSectionFactory sectionFactory)
    {
        _sectionFactory = sectionFactory;
        _subscription = _sectionFactory.Subscribe();
        _subscriptionWorkerTask = ProcessSections();
    }

    private async Task ProcessSections()
    {
        while (await _subscription.WaitToReadAsync() && _subscription.TryRead(out var sectionData))
        {
            (var pid, var section) = sectionData;
            if(pid == 0x00 && section.VersionNumber != VersionNumber && section is IProgramAssociationSection patSection)
            {
                VersionNumber = patSection.VersionNumber;
                NetworkPids = patSection.NetworkPids;
                PidAssociations = patSection.PidAssociations;
            }
        }
    }

    public byte VersionNumber { get; private set; }

    public IReadOnlyCollection<ushort> NetworkPids { get; private set; } = [];

    public IReadOnlyCollection<(ushort ProgramNumber, ushort ProgramMapPid)> PidAssociations { get; private set; } = [];

    public IEnumerable<ushort> ProgramMapPids => PidAssociations.Select(a => a.ProgramMapPid);
}