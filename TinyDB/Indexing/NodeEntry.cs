using System;

namespace TinyDb.Indexing
{
    public class NodeEntry : IComparable<NodeEntry>
    {
        public long Key { get; set; }

        public IndexNode Node { get; set; }

        public NodeEntry(long leftkey, IndexNode node)
        {
            Key = leftkey;
            Node = node;
        }

        public NodeEntry()
        {

        }

        public int CompareTo(NodeEntry o)
        {
            return Key - o.Key > 0 ? 1 : -1;
        }

        public override string ToString()
        {
            return $"NodeEntry [key={Key}, node={Node}]";
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + (int)(Key ^ (Key >> 32));
            result = prime * result + ((Node == null) ? 0 : Node.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (this == obj) return true;
            if (GetType() != obj.GetType()) return false;

            NodeEntry other = (NodeEntry)obj;
            if (Key != other.Key) return false;
            if (Node is null) return other.Node == null;
            else return Node.Equals(other.Node);
        }
    }
}
