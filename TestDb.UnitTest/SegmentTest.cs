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

        [TestMethod]
        public void Test1()
        {
            var s1 = new Segment(
                new Segment.SingleSegment(1,5,true,true),
                new Segment.SingleSegment(10,14,true,true)
                );
            var s2 = new Segment(
                new Segment.SingleSegment(0, 2, true, true),
                new Segment.SingleSegment(6, 7, true, true)           
                );
            var s3 = s1.Intersect(s2);

            var s3t = new Segment(
               new Segment.SingleSegment(1, 2, true, true)
               );
            Assert.IsTrue(s3.Equal(s3t));
            var s4 = s1.Join(s2);
            var s4t = new Segment(
               new Segment.SingleSegment(0, 5, true, true),
               new Segment.SingleSegment(6, 7, true, true),
               new Segment.SingleSegment(10, 14, true, true)
               );
            Assert.IsTrue(s4.Equal(s4t));
            var s5 = s1.Reverse();
            var s5t = new Segment(
               new Segment.SingleSegment(int.MinValue, 1, true, false),
               new Segment.SingleSegment(5, 10, false, false),
               new Segment.SingleSegment(14, int.MaxValue, false, true)
               );
            Assert.IsTrue(s5.Equal(s5t));
        }
    }
}
