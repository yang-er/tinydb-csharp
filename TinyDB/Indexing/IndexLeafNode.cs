using System.Collections.Generic;

namespace TinyDb.Indexing
{
    public class IndexLeafNode : IndexNode
    {
        public List<DataEntry> Data { get; set; }

        public IndexLeafNode RightNode { get; set; }

        public IndexLeafNode() { }

        public IndexLeafNode(bool isLeaf, IndexNode parent,
            List<NodeEntry> childs, List<DataEntry> data,
            IndexLeafNode rightNode) : base(isLeaf, parent, childs)
        {
            Data = data;
            RightNode = rightNode;
        }
    }
}
