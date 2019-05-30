using System.Collections.Generic;

namespace TinyDb.Indexing
{
    public class IndexNode
    {
        public const int PNUMBER = 6;

        public bool IsLeaf { get; set; }

        public List<NodeEntry> Childs { get; set; }

        public IndexNode Parent { get; set; }

        public IndexNode() { }

        public IndexNode(bool isLeaf, IndexNode parent, List<NodeEntry> childs2)
        {
            IsLeaf = isLeaf;
            Parent = parent;
            Childs = childs2;
        }
    }
}
