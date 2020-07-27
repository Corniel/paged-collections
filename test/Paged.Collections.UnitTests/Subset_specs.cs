using NUnit.Framework;
using Paged.Collections;
using System.Linq;
using System.Threading.Tasks;
using static Paged.Collections.UnitTests.Arrange;

namespace Subset_specs
{
    public class All
    {
        [Test]
        public void Gets_all_elements()
        {
            var elements = new[] { 0, 1, 2, 3, 4 };
            var subset = Subset.All(elements);

            Assert.AreEqual(elements, subset);
            Assert.AreEqual(0, subset.Index);
            Assert.AreEqual(5, subset.SubsetSize);
            Assert.AreEqual(1, subset.PageCount);
            Assert.AreEqual(5, subset.TotalCount);
        }
    }

    public class ForPageNumber
    {
        [TestCase(1, 3, "0,1,2", true)]
        [TestCase(2, 3, "3,4,5", true)]
        [TestCase(3, 3, "6,7", false)]
        [TestCase(2, 4, "4,5,6,7", false)]
        [TestCase(2, 8, "", false)]
        [TestCase(9, 9, "", false)]
        public void Created_from_Collection(int pageNumber, int pageSize, string exp, bool hasNext)
        {
            var options = SubsetOptions.ForPageNumber(pageNumber, pageSize);
            var subset = Subset.FromCollection(Items, options);

            Assert.AreEqual(NewArray(exp), subset.ToArray());
            Assert.AreEqual(hasNext, subset.HasNext);
        }

        [TestCase(1, 3, "0,1,2", true)]
        [TestCase(2, 3, "3,4,5", true)]
        [TestCase(3, 3, "6,7", false)]
        [TestCase(2, 4, "4,5,6,7", false)]
        [TestCase(2, 8, "", false)]
        [TestCase(9, 9, "", false)]
        public void Created_from_Enumerable(int pageNumber, int pageSize, string exp, bool hasNext)
        {
            var options = SubsetOptions.ForPageNumber(pageNumber, pageSize);
            var subset = Subset.FromEnumerable(Items, options);

            Assert.AreEqual(NewArray(exp), subset.ToArray());
            Assert.AreEqual(hasNext, subset.HasNext);
        }

        [TestCase(1, 3, "0,1,2", true)]
        [TestCase(2, 3, "3,4,5", true)]
        [TestCase(3, 3, "6,7", false)]
        [TestCase(2, 4, "4,5,6,7", false)]
        [TestCase(2, 8, "", false)]
        [TestCase(9, 9, "", false)]
        public async Task Created_from_async_Enumerable(int pageNumber, int pageSize, string exp, bool hasNext)
        {
            var options = SubsetOptions.ForPageNumber(pageNumber, pageSize);

            var subset = await Subset.FromEnumerableAsync(SelectItems(options), () => Count(), options);

            Assert.AreEqual(NewArray(exp), subset.ToArray());
            Assert.AreEqual(hasNext, subset.HasNext);
        }

        [TestCase(1, 3, "0,1,2", true)]
        [TestCase(2, 3, "3,4,5", true)]
        [TestCase(3, 3, "6,7", false)]
        [TestCase(2, 4, "4,5,6,7", false)]
        [TestCase(2, 8, "", false)]
        [TestCase(9, 9, "", false)]
        public void Created_from_Queryable(int pageNumber, int pageSize, string exp, bool hasNext)
        {
            var options = SubsetOptions.ForPageNumber(pageNumber, pageSize);
            var subset = Subset.FromQueryable(QueryableItems, options);

            Assert.AreEqual(NewArray(exp), subset.ToArray());
            Assert.AreEqual(hasNext, subset.HasNext);
        }
    }

    public class PageCount
    {
        [TestCase(0, 3, 1)]
        [TestCase(1, 3, 1)]
        [TestCase(2, 3, 1)]
        [TestCase(3, 3, 1)]
        [TestCase(4, 3, 2)]
        public void For_elements_with_total_and_page_size(int total, int pageSize, int pageCount)
        {
            var page = Subset.FromCollection(new string('*', total).ToArray(), SubsetOptions.ForIndex(0, pageSize)) as Page<char>;
            Assert.AreEqual(pageCount, page.PageCount);
        }

        [TestCase]
        public void For_enumerable_not_on_last_page_is_not_set()
        {
            var page = Subset.FromEnumerable(new string('*', 8), SubsetOptions.ForIndex(0, 3)) as Page<char>;
            Assert.IsNull(page.PageCount);
        }
    }
}
