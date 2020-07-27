using Paged.Collections.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static System.FormattableString;

namespace Paged.Collections
{
    /// <summary>Represents a page as a subset of a collection.</summary>
    /// <typeparam name="T">
    /// The type of the items in at the page.
    /// </typeparam>
    [DebuggerDisplay("{DebuggerDisplay}"), DebuggerTypeProxy(typeof(CollectionDebugView))]
    public class Page<T> : Subset<T>
    {
        /// <summary>Creates a new instance of <see cref="Page{T}"/> when the total count is unknown.</summary>
        /// <param name="items">
        /// The paged items.
        /// </param>
        /// <param name="options">
        /// The page options (page number and size).
        /// </param>
        internal Page(IList<T> items, SubsetOptions options)
            : base(items, options)
        {
            Index = GuardPageIndex(options);
        }

        /// <summary>Creates a new instance of <see cref="Page{T}"/>.</summary>
        /// <param name="items">
        /// The paged items.
        /// </param>
        /// <param name="options">
        /// The page options (page number and size).
        /// </param>
        /// <param name="totalCount">
        /// The total count of all items.
        /// </param>
        internal Page(IList<T> items, SubsetOptions options, int totalCount)
        : base(items, options, totalCount)
        {
            Index = GuardPageIndex(options);
        }

        /// <exception cref="ArgumentException">
        /// If the subset does not describe a page.
        /// </exception>
        private int GuardPageIndex(SubsetOptions options)
        {
            if (!options.IsPaged)
            {
                throw new ArgumentException(Messages.ArgumentException_NotPaged, (nameof(options)));
            }
            return options.PageIndex;
        }

        /// <summary>Gets the index (zero-based) of the page relative to all available pages.</summary>
        public int Index { get; }

        /// <summary>Gets the page number (one-based) of the page relative to all available pages.</summary>
        public int PageNumber => Index + 1;

        /// <summary>Gets the page count of the available pages (if known).</summary>
        public int? PageCount => TotalCount.HasValue ? 1 + (TotalCount - 1) / SubsetSize : null;

        /// <summary>Represents the <see cref="Page{T}"/> as a DEBUG <see cref="string"/>.</summary>
        internal override string DebuggerDisplay
        {
            get
            {
                var sb = new StringBuilder(base.DebuggerDisplay)
                    .Append(Invariant($", Page: {PageNumber:#,##0}"));

                if (PageCount.HasValue)
                {
                    sb.Append(Invariant($"/{PageCount:#,##0}"));
                }
                return sb.ToString();
            }
        }
    }
}
