using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paged.Collections
{
    /// <summary>Factory class for <see cref="Subset{T}"/>.</summary>
    public static class Subset
    {
        /// <summary>Creates a page based on a <see cref="IReadOnlyCollection{T}"/> that contains all elements.</summary>
        /// <param name="collection">
        /// The collection source.
        /// </param>
        public static Page<T> All<T>(ICollection<T> collection)
        {
            Guard.NotNull(collection, nameof(collection));
            var list = collection as IList<T> ?? collection.ToList();
            return new Page<T>(list, SubsetOptions.ForTop(0, list.Count, true), list.Count);
        }

        /// <summary>Creates a subset based on a <see cref="ICollection{T}"/>.</summary>
        /// <param name="collection">
        /// The collection source.
        /// </param>
        /// <param name="options">
        /// The options to create the subset.
        /// </param>
        public static Subset<T> FromCollection<T>(ICollection<T> collection, SubsetOptions options)
        {
            Guard.NotNull(collection, nameof(collection));

            var buffer = collection
                .Skip(options.Skip)
                .Take(options.SubsetSize)
                .ToArray();

            return New(buffer, options, collection.Count);
        }

        /// <summary>Creates a subset based on a <see cref="IEnumerable{T}"/>.</summary>
        /// <param name="enumerable">
        /// The enumerable source.
        /// </param>
        /// <param name="options">
        /// The options to create the subset.
        /// </param>
        public static Subset<T> FromEnumerable<T>(IEnumerable<T> enumerable, SubsetOptions options)
        {
            Guard.NotNull(enumerable, nameof(enumerable));

            var enumerator = enumerable.GetEnumerator();
            var total = enumerator.Skip(options);

            // no items.
            if (total < options.Skip)
            {
                return New(new T[0], options, total);
            }

            var buffer = new List<T>(options.SubsetSize);

            while (buffer.Count < options.SubsetSize && enumerator.MoveNext())
            {
                buffer.Add(enumerator.Current);
                total++;
            }

            var hasNext = buffer.Count == options.SubsetSize && enumerator.MoveNext();

            // If any items, and on the last page, we always know the total.
            if (buffer.Any() && !hasNext)
            {
                return New(buffer, options, total);
            }

            // Count the remaining items.
            if (options.WithTotalCount)
            {
                do
                {
                    total++;
                }
                while (enumerator.MoveNext());

                return New(buffer, options, total);
            }
            else
            {
                return New(buffer, options);
            }
        }


        /// <summary>Creates a subset based on asynchronous a <see cref="IEnumerable{T}"/>.</summary>
        /// <param name="enumerable">
        /// The select that should retrieve the subset.
        /// </param>
        /// <param name="count">
        /// The count that calculates the total if required.
        /// </param>
        /// <param name="options">
        /// The options to create the subset.
        /// </param>
        /// <remarks>
        /// The select function should potentially return one 1 item more than
        /// requested by the subset options, to be able to determine if there
        /// is a next page or not.
        /// </remarks>
        public static async Task<Subset<T>> FromEnumerableAsync<T>(
            Task<IEnumerable<T>> enumerable,
            Func<Task<int?>> count,
            SubsetOptions options)
        {
            _ = Guard.NotNull(enumerable, nameof(enumerable));

            var buffer = new List<T>(options.SubsetSize);
            var total = options.Skip;

            var enumerator = (await enumerable.ConfigureAwait(false)).GetEnumerator();

            while (buffer.Count < options.SubsetSize && enumerator.MoveNext())
            {
                buffer.Add(enumerator.Current);
                total++;
            }

            // Has next.
            var hasNext = buffer.Count == options.SubsetSize && enumerator.MoveNext();

            // If any items, and on the last page, we always know the total.
            if (buffer.Any() && !hasNext)
            {
                return New(buffer, options, total);
            }
            else
            {
                return options.WithTotalCount
                    ? New(buffer, options, await count().ConfigureAwait(false))
                    : New(buffer, options);
            }
        }

        /// <summary>Creates a subset based on a <see cref="IQueryable{T}"/>.</summary>
        /// <param name="queryable">
        /// The query-able source.
        /// </param>
        /// <param name="options">
        /// The options to create the subset.
        /// </param>
        public static Subset<T> FromQueryable<T>(IQueryable<T> queryable, SubsetOptions options)
        {
            Guard.NotNull(queryable, nameof(queryable));

            var buffer = new List<T>(options.SubsetSize);
            var total = options.Skip;

            var enumerator = queryable
                .Skip(options.Skip)
                // We want to detect if there is a follow-up.
                .Take(options.SubsetSize + 1)
                .GetEnumerator();

            while (buffer.Count < options.SubsetSize && enumerator.MoveNext())
            {
                buffer.Add(enumerator.Current);
                total++;
            }

            // Has next.
            var hasNext = buffer.Count == options.SubsetSize && enumerator.MoveNext();

            // If any items, and on the last page, we always know the total.
            if (buffer.Any() && !hasNext)
            {
                return New(buffer, options, total);
            }
            else
            {
                return options.WithTotalCount
                    ? New(buffer, options, queryable.Count())
                    : New(buffer, options);
            }
        }

        /// <summary>Creates a new <see cref="Page{T}"/> or <see cref="Subset{T}"/>
        /// based on if the <see cref="SubsetOptions.IsPaged"/> or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="options"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static Subset<T> New<T>(IList<T> collection, SubsetOptions options, int? total = null)
        {
            Guard.NotNull(collection, nameof(collection));

            if (options.IsPaged)
            {
                return total.HasValue
                    ? new Page<T>(collection, options, total.Value)
                    : new Page<T>(collection, options);
            }
            else
            {
                return total.HasValue
                    ? new Subset<T>(collection, options, total.Value)
                    : new Subset<T>(collection, options);
            }
        }

        /// <summary>Skip the number of items specified in the <see cref="SubsetOptions"/>.</summary>
        /// <returns>
        /// The number of items that could actually be skipped which can be
        /// lower than the provided skip value if there are not enough items.
        /// </returns>
        private static int Skip(this IEnumerator enumerator, SubsetOptions options)
        {
            var total = 0;

            // first skip the previous pages.
            while (total < options.Skip && enumerator.MoveNext())
            {
                total++;
            }
            return total;
        }
    }
}
