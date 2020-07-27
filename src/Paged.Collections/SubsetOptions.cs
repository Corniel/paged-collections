using System;
using System.Diagnostics;
using static System.FormattableString;

namespace Paged.Collections
{
    /// <summary>Represents options to get the subset on a collection.</summary>
    public readonly struct SubsetOptions : IEquatable<SubsetOptions>
    {
        /// <summary>Gets the default page options (no top, with a subset size of 50).</summary>
        public static readonly SubsetOptions Default;
        
        /// <summary>The default page size is 50.</summary>
        public static readonly int DefaultSubsetSize = 50;

        /// <summary>Creates a new instance of <see cref="SubsetOptions"/>.</summary>
        private SubsetOptions(int skip, int subsetSize, bool withPageCount)
        {
            Skip = skip;
            _offset = subsetSize - DefaultSubsetSize;
            WithTotalCount = withPageCount;
        }

        /// <summary>Gets the Skip.</summary>
        public int Skip { get; }

        /// <summary>Gets the subset size.</summary>
        public int SubsetSize => _offset + DefaultSubsetSize;

        /// <summary>The offset of the subset size.</summary>
        /// <remarks>
        /// By doing this, this struct will be initialized with the default
        /// subset size value.
        /// </remarks>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int _offset;

        /// <summary>Returns true if the total count should be determined, otherwise false.</summary>
        public bool WithTotalCount { get; }

        /// <summary>Returns if the subset is a page or not.</summary>
        public bool IsPaged => Skip % SubsetSize == 0;

        /// <summary>Gets the index (when a paged subset).</summary>
        public int PageIndex => Skip / SubsetSize;

        /// <summary>Gets the page number (the index plus one).</summary>
        public int? PageNumber => PageIndex + 1;

        /// <summary>Gets the options for the first page.</summary>
        public SubsetOptions First() => new SubsetOptions(0, SubsetSize, WithTotalCount);
        
        /// <summary>Gets the options for the page after the current.</summary>
        public SubsetOptions Next() => new SubsetOptions(Skip + SubsetSize, SubsetSize, WithTotalCount);

        /// <inheritdoc/>
        public bool Equals(SubsetOptions other)
        {
            return
                Skip == other.Skip &&
                SubsetSize == other.SubsetSize &&
                WithTotalCount == other.WithTotalCount;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is SubsetOptions other && Equals(other);

        /// <summary>Return true if the left argument equals the right argument, otherwise false.</summary>
        public static bool operator ==(SubsetOptions left, SubsetOptions right) => left.Equals(right);

        /// <summary>Return false if the left argument equals the right argument, otherwise true.</summary>
        public static bool operator !=(SubsetOptions left, SubsetOptions right) => !(left == right);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                Skip ^
                (SubsetSize << 15) ^
                (WithTotalCount.GetHashCode() << 30);
        }

        /// <summary>Represents the <see cref="SubsetOptions"/> as <see cref="string"/>.</summary>
        public override string ToString() =>
            Invariant($"Skip: {Skip:#,##0}, Size: {SubsetSize:#,##0}{(WithTotalCount ? ", WithTotalCount" : "")}");

        /// <summary>Creates <see cref="SubsetOptions"/> based on the top.</summary>
        public static SubsetOptions ForTop(int skip, int top = 50, bool withPageCount = false)
        {
            Guard.NotNegative(skip, nameof(skip));
            Guard.Positive(top, nameof(top));

            return new SubsetOptions(skip, top, withPageCount);
        }

        /// <summary>Creates <see cref="SubsetOptions"/> based on the page index.</summary>
        public static SubsetOptions ForIndex(int index, int pageSize = 50, bool withPageCount = false)
        {
            Guard.NotNegative(index, nameof(index));
            Guard.Positive(pageSize, nameof(pageSize));

            var skip = index * pageSize;
            return new SubsetOptions(skip, pageSize, withPageCount);
        }

        /// <summary>Creates <see cref="SubsetOptions"/> based on the page number.</summary>
        public static SubsetOptions ForPageNumber(int pageNumber, int pageSize = 50, bool withPageCount = false)
        {
            Guard.Positive(pageNumber, nameof(pageNumber));
            Guard.Positive(pageSize, nameof(pageSize));

            var skip = (pageNumber - 1) * pageSize;
            return new SubsetOptions(skip, pageSize, withPageCount);
        }
    }
}
