using System.Text;
using Kreisverkehr.DvbMetadataDecoder.Extensions;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class MultilingualServiceNameDescriptor : Descriptor, IMultilingualServiceNameDescriptor
{
    private readonly List<MultilingualServiceName> _names = [];

    public MultilingualServiceNameDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        int entryStart = 2;
        while(entryStart < data.Length - 2)
        {
            int entryLength = data[entryStart + 3] + data[data[entryStart + 3] + 4];
            _names.Add(new(data.Slice(entryStart, entryLength + 5)));
            entryStart += entryLength + 5;
        }
    }

    public IReadOnlyCollection<IMultilingualServiceNameDescriptor.IMultilingualServiceName> ServiceNames => _names.AsReadOnly();

    private class MultilingualServiceName : IMultilingualServiceNameDescriptor.IMultilingualServiceName
    {
        internal MultilingualServiceName(ReadOnlySpan<byte> data)
        {
            Iso639Code = Encoding.ASCII.GetString(data[..3]);
            int spLen = data[3];
            ServiceProviderName = Encoding.DvbText.GetString(data.Slice(4, spLen));
            int sLen = data[spLen + 4];
            ServiceName = Encoding.DvbText.GetString(data.Slice(spLen + 5, sLen));
        }

        public string Iso639Code { get; private set; }

        public string ServiceProviderName { get; private set; }

        public string ServiceName { get; private set; }
    }
}