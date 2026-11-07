using System.Text;

namespace Kreisverkehr.DvbMetadataDecoder.Text;

public sealed class DvbEncoding : Encoding
{
    public static DvbEncoding Instance { get; } = new();

    static DvbEncoding()
    {
        RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    private DvbEncoding()
    {
    }

    public override string EncodingName => "DVB Text Encoding";

    public override int CodePage => 0;

    public override int GetByteCount(char[] chars, int index, int count) => GetByteCount(chars.AsSpan(index, count));

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) => GetBytes(chars.AsSpan(charIndex, charCount), bytes.AsSpan(byteIndex));

    public override int GetCharCount(byte[] bytes, int index, int count) => Decode(bytes.AsSpan(index, count)).Length;

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) => GetChars(bytes.AsSpan(byteIndex, byteCount), chars.AsSpan(charIndex));

    public override int GetMaxByteCount(int charCount) => checked(UTF8.GetMaxByteCount(charCount) + 1);

    public override int GetMaxCharCount(int byteCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(byteCount);
        return byteCount;
    }

    public override int GetByteCount(ReadOnlySpan<char> chars) => checked(UTF8.GetByteCount(chars) + 1);

    public override int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        var required = GetByteCount(chars);

        if (bytes.Length < required)
            throw new ArgumentException("Destination buffer is too small.");

        bytes[0] = 0x15;

        return UTF8.GetBytes(chars, bytes[1..]) + 1;
    }

    public override int GetCharCount(ReadOnlySpan<byte> bytes) => Decode(bytes).Length;

    public override int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars)
    {
        var decoded = Decode(bytes);

        if (chars.Length < decoded.Length)
            throw new ArgumentException("Destination buffer is too small.");

        decoded.AsSpan().CopyTo(chars);

        return decoded.Length;
    }

    public override string GetString(byte[] bytes, int index, int count) => Decode(bytes.AsSpan(index, count));

    // DVB text fields are self-contained. Their selector must not
    // be interpreted independently for every streaming chunk.
    public override Encoder GetEncoder()
    {
        throw new NotSupportedException("DVB encoding requires complete text fields.");
    }

    public override Decoder GetDecoder()
    {
        throw new NotSupportedException("DVB decoding requires complete text fields.");
    }

    private static string Decode(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
            return string.Empty;

        try
        {
            // first byte 0x15 marks utf8 text
            if (data[0] == 0x15)
                return Trim(UTF8.GetString(data[1..]));

            // first byte 0x10 marks a dynamically selected coding table identified by the following two bytes
            // luckily they correspond to the codepage numbers nicely and we only need to add an offset and
            // exclude some reserved values
            if (data[0] == 0x10
                && data.Length >= 3
                && data[1] == 0x00
                && data[2] is >= 0x01 and <= 0x10
                && data[2] != 0x0C)
            {
                var codePage = 28590 + data[2];

                return Trim(GetEncoding(codePage).GetString(data[3..]));
            }

            // first byte directly specifies the coding table and with an offset we can calculate the codepage numer
            if (data[0] is >= 0x01 and <= 0x0B
                && data[0] != 0x08)
            {
                var isoNumber = data[0] switch
                {
                    <= 0x07 => data[0] + 4,
                    0x09 => 13,
                    0x0A => 14,
                    0x0B => 15,
                    _ => throw new ArgumentException("Unsupported DVB selector.")
                };

                return Trim(GetEncoding(28590 + isoNumber).GetString(data[1..]));
            }

            // // first byte 0x11 marks UCS-2 / Big Endian text
            if (data[0] == 0x11)
                return Trim(BigEndianUnicode.GetString(data[1..]));

            // first byte is 0x1F. This specifies that the second byte is an encoding_type_id which broadcasters can 
            // register. What that means? Only the broadcaster knows. Hopefully the Latin1 encoding will get something 
            // usefull...
            if (data[0] == 0x1F)
                return Trim(Latin1.GetString(data[1..]));

            // No recognized selector: Using ISO-6937 as fallback.
            return Trim(GetEncoding(20269).GetString(data));
        }
        catch (ArgumentException)
        {
            // Unknown or unsupported encoding: fall back to Latin-1.
            return Trim(Latin1.GetString(data));
        }
    }

    private static string Trim(string value)
    {
        return value.TrimEnd('\0', ' ');
    }
}