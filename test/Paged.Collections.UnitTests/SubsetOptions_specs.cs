using NUnit.Framework;
using Paged.Collections;

namespace SubsetOptions_specs
{
    public class ForTop
    {
        [Test]
        public void Creates_options_based_on_skip_and_top()
        {
            var settings = SubsetOptions.ForTop(12, 10, true);

            Assert.AreEqual(12, settings.Skip);
            Assert.AreEqual(10, settings.SubsetSize);
            Assert.AreEqual(true, settings.WithTotalCount);
        }
    }

    public class ForIndex
    {
        [Test]
        public void Creates_options_based_on_zero_based_index_and_page_size()
        {
            var settings = SubsetOptions.ForIndex(3, 10, true);

            Assert.AreEqual(30, settings.Skip);
            Assert.AreEqual(10, settings.SubsetSize);
            Assert.AreEqual(true, settings.WithTotalCount);
        }
    }

    public class ForPageNumber
    {
        [Test]
        public void Creates_options_based_on_one_based_page_number_and_page_size()
        {
            var settings = SubsetOptions.ForPageNumber(4, 10, true);

            Assert.AreEqual(30, settings.Skip);
            Assert.AreEqual(10, settings.SubsetSize);
            Assert.AreEqual(true, settings.WithTotalCount);
        }
    }

    public class First
    {
        [Test]
        public void Gets_options_without_skip()
        {
            var options = SubsetOptions.ForIndex(17, 200, true);
            var first = options.First();
            var expected = SubsetOptions.ForIndex(0, 200, true);

            Assert.AreEqual(expected, first);
        }
    }

    public class Next
    {
        [Test]
        public void Gets_the_options_with_skip_increased_by_subset_size()
        {
            var options = SubsetOptions.ForIndex(17, 200, true);
            var next = options.Next();
            var expected = SubsetOptions.ForIndex(18, 200, true);

            Assert.AreEqual(expected, next);
        }
    }

    public class DebuggerExperience
    {
        [Test]
        public void Shows_the_Skip_and_Size_properties()
        {
            var options = SubsetOptions.ForIndex(4, 20);
            Assert.AreEqual("Skip: 80, Size: 20", options.ToString());
        }

        [Test]
        public void Shows_WithTotalCount_when_enabled()
        {
            var options = SubsetOptions.ForIndex(2, 2000, true);
            Assert.AreEqual("Skip: 4,000, Size: 2,000, WithTotalCount", options.ToString());
        }
    }
}
