using System;
using System.Collections.Generic;
using System.Text;

namespace TinyDb.Querying
{
    public class Segment
    {
        public struct SingleSegment : IComparable<SingleSegment>
        {
            public int Left;
            public int Right;
            public bool LeftEqual;
            public bool RightEqual;

            public SingleSegment(int left, int right, bool le, bool ge)
            {
                Left = left;
                Right = right;
                LeftEqual = le;
                RightEqual = ge;
            }

            public int CompareTo(SingleSegment other)
            {
                return Left.CompareTo(other.Left);
            }

            public bool Contains(int n)
            {
                if (n < Left || n > Right)
                    return false;
                else if (Left == n && !LeftEqual)
                    return false;
                else if (Right == n && !RightEqual)
                    return false;
                return true;
            }

            public bool Intersects(SingleSegment other)
            {
                if (Right < other.Left) return false;
                if (Left > other.Right) return false;
                if (Right == other.Left && (!RightEqual || !other.LeftEqual)) return false;
                if (Left == other.Right && (!LeftEqual || !other.RightEqual)) return false;
                return true;
            }
        }

        public List<SingleSegment> Segments { get; }

        public Segment(params SingleSegment[] segments)
        {
            Segments = new List<SingleSegment>(segments);
        }

        public Segment(int left, int right, bool le, bool ge)
        {
            Segments = new List<SingleSegment>
            {
                new SingleSegment
                {
                    Left = left,
                    Right = right,
                    LeftEqual = le,
                    RightEqual = ge,
                }
            };
        }

        public Segment Less(int n) => new Segment(int.MinValue, n, true, false);
        public Segment LessEqual(int n) => new Segment(int.MinValue, n, true, true);
        public Segment Greater(int n) => new Segment(n, int.MaxValue, false, true);
        public Segment GreaterEqual(int n) => new Segment(n, int.MaxValue, true, true);
        public Segment SinglePoint(int n) => new Segment(n, n, true, true);

        public Segment Reverse()
        {
            return null;
        }

        public Segment Intersect(Segment segment)
        {
            return null;
        }

        public Segment Join(Segment segment)
        {
            return null;
        }
    }
}
