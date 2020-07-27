using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paged.Collections.UnitTests
{
    internal static class Arrange
    {
        internal static int[] Items => Enumerable.Range(0, 8).ToArray();
        internal static IQueryable<int> QueryableItems => Items.AsQueryable();
        internal static Task<int?> Count() => Task.FromResult((int?)Items.Count());
        internal static int[] NewArray(string exp) => exp
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(str => int.Parse(str))
            .ToArray();

        internal static Task<IEnumerable<int>> SelectItems(SubsetOptions options)
        {
            return Task.FromResult(Items.Skip(options.Skip).Take(options.SubsetSize + 1));
        }
    }
}
