using System.Collections.Generic;

namespace TinyDb
{
    public class DataEntry
    {
        public long Key { get; set; }

        public ISet<int> Index { get; set; }

        public DataEntry() { }

        public DataEntry(long key, ISet<int> index)
        {
            Key = key;
            Index = index;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + (int)(Key ^ (Key >> 32));
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (base.Equals(obj)) return true;
            if (GetType() != obj.GetType()) return false;
            DataEntry other = (DataEntry)obj;
            return Key == other.Key;
        }

        public override string ToString()
        {
            return $"DataEntry [key={Key}, index={Index}]";
        }

        public int CompareTo(DataEntry o)
        {
            return Key.CompareTo(o.Key);
        }
    }
}
