using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyDb.Structure;

namespace TinyDb.Indexing
{
    public class IndexLeaf<T> : IIndexNode<T> where T : IDbEntry
    {
        readonly WeakReference<List<T>> dataRef;

        public bool IsLeaf => true;

        public Guid DiskGuid { get; }

        public IReadOnlyList<T> Data => GetDataReference();

        public IndexLeaf<T> RightNode { get; set; }

        public IIndexNode<T> Parent { get; set; }

        public int Key { get; set; }

        private void LoadFromDisk()
        {
            if (File.Exists($"{DiskGuid}.data"))
            {
                var lines = File.ReadAllLines($"{DiskGuid}.data");
                dataRef.SetTarget(lines.Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.ParseJson<T>()).ToList());
            }
            else
            {
                Console.WriteLine("Warning: may data loss.");
                dataRef.SetTarget(new List<T>());
            }
        }

        public override string ToString()
        {
            return $"Leaf, {Key}";
        }

        internal void SaveToDisk()
        {
            if (dataRef.TryGetTarget(out var data))
            {
                var lns = data.Select(s => s.ToJson());
                File.WriteAllLines($"{DiskGuid}.data", lns);
            }
        }

        private List<T> GetDataReference()
        {
            if (!dataRef.TryGetTarget(out var _)) LoadFromDisk();
            System.Diagnostics.Debug.Assert(dataRef.TryGetTarget(out var data));
            return data;
        }

        public void Add(T entity)
        {
            var list = GetDataReference();
            list.Add(entity);
            list.Sort((a, b) => a.Id.CompareTo(b.Id));

            if (entity.Id < Key)
            {
                IIndexNode<T> node = this;
                while (node != null)
                {
                    if (node.Key < entity.Id)
                        break;
                    node.Key = entity.Id;
                    node = node.Parent;
                }
            }

            SaveToDisk();
        }

        internal List<T> Split()
        {
            var current = GetDataReference();
            var outs = current.Skip(4).ToList();
            current.RemoveRange(4, current.Count - 4);
            SaveToDisk();
            return outs;
        }

        public IndexLeaf(int key, Guid disk)
        {
            Key = key;
            DiskGuid = disk;
            dataRef = new WeakReference<List<T>>(null);
        }

        public IndexLeaf(List<T> items)
        {
            Key = items.First().Id;
            DiskGuid = Guid.NewGuid();
            dataRef = new WeakReference<List<T>>(items);
            SaveToDisk();
        }
    }
}
