using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyDb.Structure;

namespace TinyDb.Indexing
{
    public class BpTree<T> where T : IDbEntry
    {
        protected IIndexNode<T> root;
        const int MaxChildrenCount = 6;

        public string TableName { get; }

        /// <summary>
        /// 寻找第一个键值小于给定key的叶节点。
        /// </summary>
        protected IndexLeaf<T> FindNode(IIndexNode<T> _node, int key)
        {
            // 当目前节点为叶节点或空时，返回这个节点
            if (_node?.IsLeaf ?? true) return (IndexLeaf<T>)_node;

            var node = (IndexInner<T>)_node;
            var lessOrEqual = node.Child
                .TakeWhile(p => p.Key <= key)
                .LastOrDefault();
            if (lessOrEqual != null)
                return FindNode(lessOrEqual, key);
            return FindNode(node.Child.FirstOrDefault(), key);
        }

        /// <summary>
        /// 分裂一个子节点个数大于 MaxChildrenCount 数量的节点。
        /// </summary>
        protected void SplitNode(IIndexNode<T> node)
        {
            if (node is IndexLeaf<T> leaf)
            {
                var newLeaf = new IndexLeaf<T>(leaf.Split());
                newLeaf.RightNode = leaf.RightNode;
                leaf.RightNode = newLeaf;

                if (node.Parent is IndexInner<T> parent)
                {
                    parent.Add(newLeaf);
                    newLeaf.Parent = parent;
                    if (parent.Child.Count > MaxChildrenCount)
                        SplitNode(parent);
                }
                else
                {
                    var inns = new List<IIndexNode<T>> { leaf, newLeaf };
                    var newParent = new IndexInner<T>(inns);
                    leaf.Parent = newLeaf.Parent = newParent;
                    root = newParent;
                }
            }
            else if (node is IndexInner<T> inner)
            {
                var newNode = new IndexInner<T>(inner.Split());
                newNode.Child.ForEach(t => t.Parent = newNode);

                if (node.Parent is IndexInner<T> parent)
                {
                    parent.Add(newNode);
                    newNode.Parent = parent;
                    if (parent.Child.Count > MaxChildrenCount)
                        SplitNode(parent);
                }
                else
                {
                    var chlds = new List<IIndexNode<T>> { inner, newNode };
                    var newParent = new IndexInner<T>(chlds);
                    newNode.Parent = inner.Parent = newParent;
                    root = newParent;
                }
            }
        }

        /// <summary>
        /// 插入一个实体。
        /// </summary>
        /// <param name="entity">所插入实体</param>
        public void Insert(T entity)
        {
            var node = FindNode(root, entity.Id);

            if (node is null)
            {
                root = new IndexLeaf<T>(new List<T> { entity });
            }
            else
            {
                if (node.Data.Any(t => t.Id == entity.Id))
                    throw new ArgumentException("Key should be different.");
                node.Add(entity);
                if (node.Data.Count > MaxChildrenCount)
                    SplitNode(node);
            }

            File.WriteAllText($"{TableName}.index", (root is null ? null : Convert(root)).ToJson());
        }

        internal BpTree(string tableName)
        {
            TableName = tableName;
            if (File.Exists($"{tableName}.index"))
            {
                var vt = File.ReadAllText($"{tableName}.index")
                    .ParseJson<VirtualTree>();
                root = ConvertBack(vt);
            }
        }

        private class VirtualTree
        {
            public bool IsLeaf { get; set; }
            public int Key { get; set; }
            public List<VirtualTree> Childs { get; set; }
            public Guid? Data { get; set; }

            public override string ToString()
            {
                return $"{(IsLeaf ? "Leaf" : "Inner")}, Key = {Key}, ChildCnt = {Childs?.Count() ?? 0}";
            }
        }

        private VirtualTree Convert(IIndexNode<T> node)
        {
            if (node is IndexLeaf<T> leaf)
            {
                return new VirtualTree
                {
                    Childs = null,
                    Data = leaf.DiskGuid,
                    Key = leaf.Key,
                    IsLeaf = true,
                };
            }
            else if (node is IndexInner<T> inner)
            {
                return new VirtualTree
                {
                    Data = null,
                    IsLeaf = false,
                    Key = inner.Key,
                    Childs = inner.Child.Select(t => Convert(t)).ToList()
                };
            }
            else
            {
                return null;
            }
        }

        private IIndexNode<T> ConvertBack(VirtualTree src)
        {
            IndexLeaf<T> lastAccess = null;
            Func<VirtualTree, IIndexNode<T>> solve = null;

            solve = tree =>
            {
                if (tree.IsLeaf)
                {
                    var inode = new IndexLeaf<T>(tree.Key, tree.Data.Value);
                    if (lastAccess != null) lastAccess.RightNode = inode;
                    lastAccess = inode;
                    return inode;
                }
                else
                {
                    var lst = new List<IIndexNode<T>>(tree.Childs.Select(t => solve(t)));
                    var inode = new IndexInner<T>(lst);
                    lst.ForEach(t => t.Parent = inode);
                    return inode;
                }
            };

            return solve(src);
        }
    }
}
