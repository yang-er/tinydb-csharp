using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyDb.Indexing
{
    public class BTree
    {
        public static IndexNode Root { get; private set; }

        /// <summary>
        /// 构造B+树
        /// </summary>
        /// <param name="indexList">索引列表</param>
        /// <returns>根节点</returns>
        public static IndexNode Build(List<DataEntry> indexList)
        {
            Root = null;

            foreach (var de in indexList)
            {
                IndexLeafNode node = Find(Root, de.Key);
                Insert(node, de);
            }

            return Root;
        }

        /// <summary>
        /// 向B+树插入元素
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="de">数据内容</param>
        private static void Insert(IndexLeafNode node, DataEntry de)
        {
            if (node == null)
            {
                // 如果待插入节点为空，表明树为空，则新建叶子节点作为根节点
                var data = new List<DataEntry> { de };

                //没有父节点，没孩子节点，是叶节点，数据，不是右叶子节点
                Root = new IndexLeafNode(true, null, null, data, null);
                return;
            }

            bool isExist = false;
            foreach (DataEntry e in node.Data)
            {
                // 在叶节点中查找是否有相同索引，如果有则添加到该索引的值集合中
                if (e.Key == de.Key)
                {
                    e.Index.AddRange(de.Index);
                    isExist = true;
                    break;
                }
            }

            if (!isExist)
            {
                // 叶节点中无该索引
                node.Data.Add(de); // 添加关键字至关键字集合
                node.Data.Sort();// 将关键字排序

                if (node.Data.Last().Equals(de) && node.RightNode == null)
                {
                    // 如果插入的关键字大于所有已插入的关键字，则将父亲的最大关键字设置为插入的关键字值
                    var temp = node.Parent;
                    while (temp != null)
                    {
                        temp.Childs.Last().Key = de.Key;
                        temp = temp.Parent;
                    }
                }

                if (node.Data.Count > IndexNode.PNUMBER)
                {
                    // 关键字数目大于指定阶数
                    Split(node); // 分裂
                }
            }
        }

        /// <summary>
        /// 分裂
        /// </summary>
        /// <param name="node">要分裂的节点</param>
        private static void Split(IndexNode node)
        {
            long leftkey;

            if (node.IsLeaf)
            {
                // 叶子节点
                var tn = (IndexLeafNode)node; // 强制类型转换为叶节点
                int n = tn.Data.Count / 2; // 分裂位置
                leftkey = tn.Data[n].Key; //分裂处的关键字

                var newLeaf = new IndexLeafNode(true, null, null, new List<DataEntry>(), tn.RightNode);

                while (n-- > 0)
                {
                    newLeaf.Data.Add(tn.Data[IndexNode.PNUMBER / 2 + 1]);
                    tn.Data.RemoveAt(IndexNode.PNUMBER / 2 + 1);
                }

                tn.RightNode = newLeaf;
                newLeaf.Data.Sort();

                long rightKey = newLeaf.Data.Last().Key;
                if (node.Parent is null)
                {
                    // 是根节点
                    var newParent = new IndexNode(false, null, new List<NodeEntry>());
                    var ln = new NodeEntry(leftkey, node);
                    var rn = new NodeEntry(rightKey, newLeaf);
                    newParent.Childs.Add(ln);
                    newParent.Childs.Add(rn);
                    node.Parent = newParent;
                    newLeaf.Parent = newParent;
                    Root = newParent;
                }
                else
                {
                    // 不是根结点
                    var parent = node.Parent; // 获取父亲结点

                    foreach (var ne in parent.Childs)
                    {
                        // 遍历父亲结点
                        if (ne.Key == rightKey)
                        {
                            ne.Key = leftkey;
                            break;
                        }
                    }

                    parent.Childs.Add(new NodeEntry(rightKey, newLeaf));
                    newLeaf.Parent = parent;
                    parent.Childs.Sort();

                    if (parent.Childs.Count > IndexNode.PNUMBER)
                    {
                        // 如果分裂后的父亲结点孩子数大于指定阶数则继续分裂
                        Split(parent);
                    }
                }
            }
            else
            {
                // 内结点
                int n = node.Childs.Count / 2; // 分裂位置
                leftkey = node.Childs[n].Key; // 分裂处的关键字
                var newNode = new IndexNode(false, node.Parent, new List<NodeEntry>()); // 创建新结点

                while (n-- > 0)
                {
                    // 将分裂的结点添加到新街点
                    newNode.Childs.Add(node.Childs[IndexNode.PNUMBER / 2 + 1]);
                    node.Childs.RemoveAt(IndexNode.PNUMBER / 2 + 1);
                    newNode.Childs.Last().Node.Parent = newNode;
                    // 更新新结点中的孩子结点的父节点
                }

                long rightKey = newNode.Childs.Last().Key;

                if (node.Parent == null)
                {
                    // 是根节点
                    IndexNode newParent = new IndexNode(false, null, new List<NodeEntry>());
                    NodeEntry ln = new NodeEntry(leftkey, node);
                    NodeEntry rn = new NodeEntry(rightKey, newNode);
                    newParent.Childs.Add(ln);
                    newParent.Childs.Add(rn);
                    node.Parent = newParent;
                    newNode.Parent = newParent;
                    Root = newParent;
                }
                else
                {
                    // 不是根节点
                    // 获取父亲结点
                    IndexNode parent = node.Parent;

                    foreach (var ne in parent.Childs)
                    {
                        // 遍历父亲结点
                        if (ne.Key == rightKey)
                        {
                            ne.Key = leftkey;
                            break;
                        }
                    }

                    parent.Childs.Add(new NodeEntry(rightKey, newNode));
                    newNode.Parent = parent;
                    parent.Childs.Sort();

                    if (parent.Childs.Count > IndexNode.PNUMBER)
                    {
                        // 如果分裂后的父亲结点孩子数大于指定阶数则继续分裂
                        Split(parent);
                    }
                }
            }

        }

        /// <summary>
        /// 查找所给关键字插入位置,如果所给关键字为空，则返回叶子节点的第一个结点（即最小关键字所在结点）
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IndexLeafNode Find(IndexNode node, long key)
        {
            if (node == null)
            {
                // 如果没有根节点，返回空
                return null;
            }

            if (node.IsLeaf)
            {
                // 如果是叶结点则表明此叶结点就是待插入位置
                return (IndexLeafNode)node;
            }

            foreach (var e in node.Childs)
            {
                // 如果是内节点遍历孩子结点
                if (key <= e.Key)
                {
                    // 如果待插入关键字<=某孩子结点关键字，把此孩子作为根结点递归查找
                    return Find(e.Node, key);
                }
            }

            return Find(node.Childs.Last().Node, key);
            // 如果此结点中没有比待插入关键字小的结点，则把孩子节点中关键字最大的结点作为根节点递归查找
        }

        /// <summary>
        /// 显示B+树
        /// </summary>
        /// <param name="node">目前节点</param>
        public static void Display(IndexNode node)
        {
            IndexLeafNode dis = Find(node, long.MinValue);
            while (dis != null)
            {
                Console.WriteLine(dis.Data);
                dis = dis.RightNode;
            }
        }
    }
}
