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
            public bool IsEquals(SingleSegment other) => this.Left == other.Left && this.Right == other.Right && this.LeftEqual == other.LeftEqual && this.RightEqual == other.RightEqual;
            

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

        public bool Equal(Segment segment)
        {
            Segments.Sort();
            segment.Segments.Sort();
            if (Segments.Count != segment.Segments.Count) return false;
            else
            {
                for(int i = 0;i<Segments.Count;i++)
                {
                    if (!Segments[i].IsEquals(segment.Segments[i])) return false;
                }
                return true;
            }
        }
      

        public static Segment Less(int n) => new Segment(int.MinValue, n, true, false);
        public static Segment LessEqual(int n) => new Segment(int.MinValue, n, true, true);
        public static Segment Greater(int n) => new Segment(n, int.MaxValue, false, true);
        public static Segment GreaterEqual(int n) => new Segment(n, int.MaxValue, true, true);
        public static Segment SinglePoint(int n) => new Segment(n, n, true, true);

        public Segment Reverse()
        {
            List<Segment> reverselist = new List<Segment>();
            foreach(SingleSegment item in Segments)
            {
                Segment a = new Segment();
                SingleSegment b1 = new SingleSegment();
                SingleSegment b2 = new SingleSegment();
                b1.Left = int.MinValue;
                b1.LeftEqual = true;
                b1.Right = item.Left;
                b1.RightEqual = !item.LeftEqual;
                b2.Right = int.MaxValue;
                b2.RightEqual = true;
                b2.Left = item.Right;
                b2.LeftEqual = !item.RightEqual;
                a.Segments.Add(b1);
                a.Segments.Add(b2);
                reverselist.Add(a);
            }
            //Segment result = new Segment();
            //SingleSegment max = new SingleSegment();
            /*max.Left = int.MinValue;
            max.LeftEqual = true;
            max.Right = int.MaxValue;
            max.RightEqual = true;
            result.Segments.Add(max);*/
            Segment result = reverselist[0];
            foreach(Segment item in reverselist)
            {
                result = result.Intersect(item);
            }
            return result;
        }

        public Segment Intersect(Segment segment)
        {
            Segment result = new Segment();
            foreach(var item in Segments)
            {
                foreach(var item2 in segment.Segments)
                {
                    var a = Segment.SmallIntersect(item, item2);
                    if(a.HasValue)
                    result.Segments.Add(a.Value);
                }
            }
            result.Segments.Sort();
            List<SingleSegment> list = result.Segments;
            for (int i = 0;i<list.Count-1; )
            {
                if (list[i].Intersects(list[i + 1]))
                {
                    SingleSegment a = list[i];
                    SingleSegment b = list[i + 1];
                    list.RemoveAt(i);
                    list.RemoveAt(i);
                    list.Add(Segment.SmallJoin(a, b));
                    list.Sort();
                }
                else
                {
                    i++;
                }
            }
            return result;
        }
        public static SingleSegment SmallJoin(SingleSegment a, SingleSegment b)
        {
            int Left;
            int Right;
            bool Leftequal;
            bool Rightequal;
            if (a.Left < b.Left)
            {
                Left = a.Left;
                Leftequal = a.LeftEqual;
            }
            else if (a.Left > b.Left)
            {
                Left = b.Left;
                Leftequal = b.LeftEqual;
            }
            else
            {
                Left = a.Left;
                Leftequal = a.LeftEqual || b.LeftEqual;
            }

            if (a.Right > b.Right)
            {
                Right = a.Right;
                Rightequal = a.RightEqual;
            }
            else if (a.Right < b.Right)
            {
                Right = b.Right;
                Rightequal = b.RightEqual;
            }
            else
            {
                Right = a.Right;
                Rightequal = a.RightEqual || b.RightEqual;
            }
            SingleSegment result = new SingleSegment();
            result.Left = Left;
            result.LeftEqual = Leftequal;
            result.Right = Right;
            result.RightEqual = Rightequal;
            return result;
        }
        public static SingleSegment? SmallIntersect(SingleSegment a,SingleSegment b)
        {
            if (!a.Intersects(b)) return null;
            else
            {
                int Left ;
                int Right ;
                bool Leftequal;
                bool Rightequal;
                if(a.Left > b.Left)
                {
                    Left = a.Left;
                    Leftequal = a.LeftEqual;
                }
                else if(a.Left < b.Left)
                {
                    Left = b.Left;
                    Leftequal = b.LeftEqual;
                }
                else
                {
                    Left = a.Left;
                    Leftequal = a.LeftEqual && b.LeftEqual;
                }

                if (a.Right < b.Right)
                {
                    Right = a.Right;
                    Rightequal = a.RightEqual;
                }
                else if (a.Right > b.Right)
                {
                    Right = b.Right;
                    Rightequal = b.RightEqual;
                }
                else
                {
                    Right = a.Right;
                    Rightequal = a.RightEqual && b.RightEqual;
                }
                SingleSegment result = new SingleSegment();
                result.Left = Left;
                result.LeftEqual = Leftequal;
                result.Right = Right;
                result.RightEqual = Rightequal;
                return result;
            }
        }
        public Segment Join(Segment segment)
        {
            Segment result = new Segment();
            foreach (SingleSegment item in Segments)
            {
                result.Segments.Add(item);
            }
            foreach (SingleSegment item in segment.Segments)
            {
                result.Segments.Add(item);
            }
            result.Segments.Sort();
            List<SingleSegment> list = result.Segments;
            for (int i = 0; i < list.Count-1;)
            {
                if (list[i].Intersects(list[i + 1]))
                {
                    SingleSegment a = list[i];
                    SingleSegment b = list[i + 1];
                    list.RemoveAt(i);
                    list.RemoveAt(i);
                    list.Add(Segment.SmallJoin(a, b));
                    list.Sort();
                }
                else
                {
                    i++;
                }
            }
            return result;
        }
    }
}
