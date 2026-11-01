using Kreisverkehr.DvbMetadataDecoder.Extensions;
using static Kreisverkehr.DvbMetadataDecoder.IIso639LanguageDescriptor;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class Iso639LanguageDescriptor : Descriptor, IIso639LanguageDescriptor
{
    private readonly List<LanguageEntry> _languages = [];

    public Iso639LanguageDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        foreach(var i in data[2..].Chunk(4))
        {
            _languages.Add(new LanguageEntry(i));
        }
    }

    public IReadOnlyList<ILanguageEntry> Languages => _languages.AsReadOnly();

    private class LanguageEntry : ILanguageEntry
    {
        internal LanguageEntry(ReadOnlySpan<byte> data)
        {
            LanguageCode = System.Text.Encoding.ASCII.GetString(data[..3]);
            AudioType = data[3];
        }

        public string LanguageCode { get; private set; }
        public byte AudioType { get; private set; }
    }
}