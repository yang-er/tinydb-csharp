using System;
using System.Collections.Generic;
using System.Linq;
using TinyDb.Indexing;

namespace TinyDb
{
    public partial class DbSet<T>
    {
        IndexNode<T> root;
        /// <summary>
        /// 寻找一个节点。
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="node">节点</param>
        /// <param name="key">查询键值</param>
        /// <returns>所找到的节点</returns>
        private IndexNode<T> FindNode(IndexNode<T> node, int key)
        {
            if (node is null) return null;

            // 如果是叶结点则表明此叶结点就是待插入位置
            if (node.IsLeaf) return node;

            // 如果是内节点遍历孩子结点
            foreach (var child in node.Childs)
                if (key <= child.Key)
                    return FindNode(child, key);

            // 如果没有比待插入关键字小的结点，则把最大的结点作为根节点递归查找
            return FindNode(node.Childs.LastOrDefault(), key);
        }

        /// <summary>
        /// 向B+树插入元素
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="de">数据内容</param>
        private void Insert(IndexNode<T> node, T de)
        {
            if (node == null)
            {
                // 如果待插入节点为空，表明树为空，则新建叶子节点作为根节点
                var data = new List<T> { de };

                //没有父节点，没孩子节点，是叶节点，数据，不是右叶子节点
                root = new IndexNode<T>(null, data, null);
                return;
            }

            System.Diagnostics.Debug.Assert(node.IsLeaf);

            // 叶节点中无该索引
            node.GetData().Add(de); // 添加关键字至关键字集合
            node.GetData().Sort((t, t2) => t.Id.CompareTo(t2.Id)); // 将关键字排序
            node.FlushData();

            if (node.GetData().Last().Equals(de) && node.RightNode == null)
            {
                // 如果插入的关键字大于所有已插入的关键字，则将父亲的最大关键字设置为插入的关键字值
                var temp = node.Parent;
                while (temp != null)
                {
                    temp.Childs.Last().Key = de.Id;
                    temp = temp.Parent;
                }
            }

            if (node.GetData().Count > IndexNode<T>.PNUMBER)
            {
                // 关键字数目大于指定阶数
                SplitNode(node); // 分裂
            }
        }

        /// <summary>
        /// 分裂节点
        /// </summary>
        /// <param name="node">节点</param>
        private void SplitNode(IndexNode<T> node)
        {
            if (node.IsLeaf)
            {
                int n = node.GetData().Count / 2; // 分裂位置
                int leftkey = node.GetData()[n].Id; //分裂处的关键字
                var newLeaf = new IndexNode<T>(null, new List<T>(), node.RightNode);

                while (n-- > 0)
                {
                    newLeaf.GetData().Add(node.GetData()[IndexNode<T>.PNUMBER / 2 + 1]);
                    node.GetData().RemoveAt(IndexNode<T>.PNUMBER / 2 + 1);
                }

                node.RightNode = newLeaf;
                newLeaf.GetData().Sort((t, t2) => t.Id.CompareTo(t2.Id));
                int rightKey = newLeaf.GetData().Last().Id;

                node.FlushData();
                newLeaf.FlushData();

                if (node.Parent is null)
                {
                    // 是根节点
                    node.Key = leftkey;
                    newLeaf.Key = rightKey;
                    var newParent = new IndexNode<T>(null, new List<IndexNode<T>> { node, newLeaf });
                    node.Parent = newLeaf.Parent = newParent;
                    root = newParent;
                }
                else
                {
                    // 不是根结点
                    var parent = node.Parent;

                    foreach (var ne in parent.Childs)
                    {
                        // 遍历父亲结点
                        if (ne.Key == rightKey)
                        {
                            ne.Key = leftkey;
                            break;
                        }
                    }

                    newLeaf.Key = rightKey;
                    parent.Childs.Add(newLeaf);
                    newLeaf.Parent = parent;
                    parent.Childs.Sort((t, t1) => t.Key.CompareTo(t1.Key));

                    if (parent.Childs.Count > IndexNode<T>.PNUMBER)
                    {
                        // 如果分裂后的父亲结点孩子数大于指定阶数则继续分裂
                        SplitNode(parent);
                    }
                }
            }
            else
            {
                // 内结点
                int n = node.Childs.Count / 2; // 分裂位置
                int leftkey = node.Childs[n].Key; // 分裂处的关键字
                var newNode = new IndexNode<T>(node.Parent, childs: null); // 创建新结点

                while (n-- > 0)
                {
                    // 将分裂的结点添加到新节点
                    newNode.Childs.Add(node.Childs[IndexNode<T>.PNUMBER / 2 + 1]);
                    node.Childs.RemoveAt(IndexNode<T>.PNUMBER / 2 + 1);
                    newNode.Childs.Last().Parent = newNode;
                    // 更新新结点中的孩子结点的父节点
                }

                int rightKey = newNode.Childs.Last().Key;

                if (node.Parent == null)
                {
                    // 是根节点
                    node.Key = leftkey;
                    newNode.Key = rightKey;
                    var newParent = new IndexNode<T>(null, new List<IndexNode<T>> { node, newNode });
                    node.Parent = newNode.Parent = newParent;
                    root = newParent;
                }
                else
                {
                    // 不是根节点，获取父亲结点
                    var parent = node.Parent;

                    foreach (var ne in parent.Childs)
                    {
                        // 遍历父亲结点
                        if (ne.Key == rightKey)
                        {
                            ne.Key = leftkey;
                            break;
                        }
                    }

                    newNode.Key = rightKey;
                    parent.Childs.Add(newNode);
                    newNode.Parent = parent;
                    parent.Childs.Sort();

                    if (parent.Childs.Count > IndexNode<T>.PNUMBER)
                    {
                        // 如果分裂后的父亲结点孩子数大于指定阶数则继续分裂
                        SplitNode(parent);
                    }
                }
            }
        }

        /// <summary>
        /// 当作可枚举内容。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="node">节点</param>
        /// <returns>所有内容</returns>
        public IEnumerable<T> AsEnumerable(IndexNode<T> node)
        {
            var leaf = FindNode(node, int.MinValue);

            while (leaf != null)
            {
                foreach (var item in leaf.GetData())
                    yield return item;
                leaf = leaf.RightNode;
            }
        }

        private VirtualTree Convert(IndexNode<T> src)
        {
            if (src is null) return null;
            return new VirtualTree
            {
                IsLeaf = src.IsLeaf,
                Key = src.Key,
                Childs = src.Childs?.Select(s => Convert(s)).ToList(),
                Data = src.Data,
            };
        }

        private class VirtualTree
        {
            public bool IsLeaf { get; set; }
            public int Key { get; set; }
            public List<VirtualTree> Childs { get; set; }
            public Guid? Data { get; set; }
        }

        private IndexNode<T> ConvertBack(VirtualTree src)
        {
            var inode = new IndexNode<T>(src.IsLeaf, src.Key, src.Data);

            if (!src.IsLeaf)
            {
                inode.Childs.AddRange(src.Childs.Select(t =>
                {
                    var n = ConvertBack(t);
                    n.Parent = inode;
                    return n;
                }));
            }
            else
            {
                if (root != null)
                    root.RightNode = inode;
                root = inode;
            }

            return inode;
        }
    }
}
