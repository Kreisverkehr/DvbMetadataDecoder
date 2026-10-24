namespace Kreisverkehr.DvbMetadataDecoder.Extensions;

public static class ReadOnlySpanExtensions
{
    public static ReadOnlySpanChunks<T> Chunk<T>(this ReadOnlySpan<T> source, int chunkSize)
    {
        return new ReadOnlySpanChunks<T>(source, chunkSize);
    }
}

public readonly ref struct ReadOnlySpanChunks<T>
{
    private readonly ReadOnlySpan<T> _source;
    private readonly int _chunkSize;

    public ReadOnlySpanChunks(ReadOnlySpan<T> source, int chunkSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkSize);

        _source = source;
        _chunkSize = chunkSize;
    }

    public Enumerator GetEnumerator()
        => new(_source, _chunkSize);

    public ref struct Enumerator
    {
        private readonly ReadOnlySpan<T> _source;
        private readonly int _chunkSize;
        private int _offset;

        public ReadOnlySpan<T> Current { get; private set; }

        internal Enumerator(ReadOnlySpan<T> source, int chunkSize)
        {
            _source = source;
            _chunkSize = chunkSize;
            _offset = 0;
            Current = default;
        }

        public bool MoveNext()
        {
            if (_offset >= _source.Length)
            {
                Current = default;
                return false;
            }

            int length = Math.Min(
                _chunkSize,
                _source.Length - _offset);

            Current = _source.Slice(_offset, length);
            _offset += length;

            return true;
        }
    }
}