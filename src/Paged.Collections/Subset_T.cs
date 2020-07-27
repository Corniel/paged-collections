using Paged.Collections.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static System.FormattableString;

namespace Paged.Collections
{
    /// <summary>Represents a subset of a collection.</summary>
    /// <typeparam name="T">
    /// The type of the items in at the page.
    /// </typeparam>
    [DebuggerDisplay("{DebuggerDisplay}"), DebuggerTypeProxy(typeof(CollectionDebugView))]
    public class Subset<T> : IReadOnlyList<T>
    {
        private Subset(IList<T> items) => subset = items.ToArray();

        /// <summary>Creates a new instance of <see cref="Subset{T}"/> when the total count is unknown.</summary>
        /// <param name="items">
        /// The paged items.
        /// </param>
        /// <param name="options">
        /// The page options (page number and size).
        /// </param>
        internal Subset(IList<T> items, SubsetOptions options) : this(items)
        {
            if (options.WithTotalCount)
            {
                throw new ArgumentNullException(nameof(options), Messages.ArgumentException_TotalCountNotSpecified);
            }
            Skip = options.Skip;
            SubsetSize = options.SubsetSize;
        }

        /// <summary>Creates a new instance of <see cref="Subset{T}"/>.</summary>
        /// <param name="items">
        /// The subset.
        /// </param>
        /// <param name="options">
        /// The subset options.
        /// </param>
        /// <param name="totalCount">
        /// The total count of all items.
        /// </param>
        internal Subset(IList<T> items, SubsetOptions options, int totalCount) : this(items)
        {
            Skip = options.Skip;
            TotalCount = totalCount;
            SubsetSize = options.SubsetSize;
        }

        /// <inheritdoc />
        public T this[int index] => subset[index];

        /// <inheritdoc />
        public int Count => subset.Length;

        /// <summary>Gets the top (zero-based) of the subset relative to all available items.</summary>
        public int Skip { get; }

        /// <summary>Gets the subset size (should be equal to count, with the exception of some last subsets.</summary>
        public int SubsetSize { get; }

        /// <summary>Gets the total count of the available collection (if known).</summary>
        public int? TotalCount { get; }

        /// <summary>Return true if this subset is the last subset of the available collection.</summary>
        public bool HasNext => Count != 0 && (!TotalCount.HasValue || TotalCount > Skip + SubsetSize);

        /// <summary>Represents the <see cref="Subset{T}"/> as a DEBUG <see cref="string"/>.</summary>
        internal virtual string DebuggerDisplay
        {
            get
            {
                var sb = new StringBuilder()
                    .Append(Invariant($"Skip: {Skip:#,##0}"))
                    .Append(Invariant($", Size: {SubsetSize:#,##0}"));

                // Only show the Count if it differs from the size.
                if (SubsetSize != Count)
                {
                    sb.Append(Invariant($" (Count: {Count:#,##0})"));
                }

                if (TotalCount.HasValue)
                {
                    sb.Append(Invariant($", Total: {TotalCount:#,##0}"));
                }
                return sb.ToString();
            }
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)subset).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly T[] subset;
    }
}
