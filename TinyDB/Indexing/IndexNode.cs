using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyDb.Structure;

namespace TinyDb.Indexing
{
    /// <summary>
    /// B+树索引节点
    /// </summary>
    public class IndexNode<T> where T : IDbEntry
    {
        /// <summary>
        /// 单层节点内容表大小
        /// </summary>
        internal const int PNUMBER = 6;

        /// <summary>
        /// 主键值
        /// </summary>
        public int Key { get; set; }

        /// <summary>
        /// 是否为叶节点
        /// </summary>
        public bool IsLeaf { get; }

        /// <summary>
        /// 非叶节点的子节点
        /// </summary>
        public List<IndexNode<T>> Childs { get; }

        /// <summary>
        /// 父亲节点
        /// </summary>
        public IndexNode<T> Parent { get; set; }

        /// <summary>
        /// 叶节点保存的数据对象
        /// </summary>
        public Guid? Data { get; }

        readonly WeakReference<List<T>> dataRef;

        /// <summary>
        /// 获取所有的数据。
        /// </summary>
        /// <returns>数据</returns>
        public List<T> GetData()
        {
            System.Diagnostics.Debug.Assert(IsLeaf);

            if (!dataRef.TryGetTarget(out var data))
            {
                if (File.Exists($"{Data}.data"))
                {
                    var lines = File.ReadAllLines($"{Data}.data");
                    dataRef.SetTarget(data = lines.Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s.ParseJson<T>()).ToList());
                }
                else
                {
                    Console.WriteLine("Warning: may data loss.");
                }

                if (data is null) data = new List<T>();
            }

            return data;
        }

        /// <summary>
        /// 强制写入数据内容。
        /// </summary>
        public void FlushData()
        {
            if (dataRef.TryGetTarget(out var data))
            {
                var lns = data.Select(s => s.ToJson());
                File.WriteAllLines($"{Data}.data", lns);
            }
        }

        /// <summary>
        /// 下一个节点的内容
        /// </summary>
        public IndexNode<T> RightNode { get; set; }

        /// <summary>
        /// 构造一个索引节点。
        /// </summary>
        /// <param name="isLeaf">是否为叶节点</param>
        /// <param name="parent">父节点</param>
        /// <param name="childs">子节点</param>
        public IndexNode(IndexNode<T> parent, List<IndexNode<T>> childs)
        {
            IsLeaf = false;
            Parent = parent;
            Childs = childs ?? new List<IndexNode<T>>();
        }

        /// <summary>
        /// 构造一个索引叶节点。
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="data">数据内容</param>
        /// <param name="nextNode">下一个节点</param>
        public IndexNode(IndexNode<T> parent, List<T> data, IndexNode<T> next)
        {
            IsLeaf = true;
            dataRef = new WeakReference<List<T>>(data);
            Parent = parent;
            Data = Guid.NewGuid();
            RightNode = next;
        }

        /// <summary>
        /// 恢复索引节点。
        /// </summary>
        public IndexNode(bool isLeaf, int key, Guid? data)
        {
            IsLeaf = isLeaf;
            Key = key;
            Data = data;

            if (isLeaf)
            {
                dataRef = new WeakReference<List<T>>(null);
            }
            else
            {
                Childs = new List<IndexNode<T>>();
            }
        }
    }
}
