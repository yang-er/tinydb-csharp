using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyDb.Querying;

namespace TestDb.UnitTest
{
    [TestClass]
    public class SegmentTest
    {
        [DataTestMethod]
        [DataRow(1, 2, 3, true, true, false)]
        [DataRow(1, 3, 2, true, true, true)]
        [DataRow(1, 2, 2, true, true, true)]
        [DataRow(1, 2, 2, true, false, false)]
        public void SingleSegmentContains(int a, int b, int c, bool le, bool re, bool res)
        {
            Assert.AreEqual(res, new Segment.SingleSegment(a, b, le, re).Contains(c));
        }

        [DataTestMethod]
        [DataRow(1, 3, 2, 4, true, true, true, true, true)]
        [DataRow(1, 2, 2, 4, true, true, true, true, true)]
        [DataRow(1, 2, 2, 4, true, false, true, true, false)]
        [DataRow(1, 1, 2, 4, true, true, true, true, false)]
        [DataRow(1, 2, 3, 4, true, false, true, true, false)]
        public void SingleSegmentIntersects(int a, int b, int c, int d, bool le1, bool re1, bool le2, bool re2, bool res)
        {
            var s1 = new Segment.SingleSegment(a, b, le1, re1);
            var s2 = new Segment.SingleSegment(c, d, le2, re2);
            Assert.AreEqual(res, s1.Intersects(s2));
            Assert.AreEqual(res, s2.Intersects(s1));
        }
    }
}
