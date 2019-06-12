using TinyDb.Structure;

namespace TinyDb.Indexing
{
    public interface IIndexNode<T> where T : IDbEntry
    {
        bool IsLeaf { get; }

        IIndexNode<T> Parent { get; set; }

        int Key { get; set; }
    }
}
