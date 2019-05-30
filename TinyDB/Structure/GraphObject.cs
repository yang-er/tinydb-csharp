using System.Collections.Generic;
using System.Text;

namespace TinyDb.Structure
{
    /// <summary>
    /// 图数据库对象
    /// </summary>
    public class GraphObject : Dictionary<string, object>
    {
        public const string AnonymousObjectName = "AnonymousObject";

        public string Name { get; set; }

        public GraphObject(string name = null)
        {
            Name = name ?? AnonymousObjectName;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GraphObject go)) return false;
            if (go.Name != Name) return false;
            if (Keys.Count != go.Keys.Count) return false;

            var iter2 = go.GetEnumerator();
            foreach (var (key, val) in this)
            {
                if (key != iter2.Current.Key || val != iter2.Current.Value)
                    return false;
                iter2.MoveNext();
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hashcode = 0;
            foreach (var (key, val) in this)
                hashcode ^= unchecked(key.GetHashCode() + val.GetHashCode());
            return hashcode;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var hashcode = GetHashCode();
            sb.AppendLine($"# {Name} @{hashcode:x}");
            foreach (var item in this)
                sb.AppendLine($"\t{item.Key}:{item.Value}");
            sb.AppendLine($"~ {Name} @{hashcode:x}");
            return base.ToString();
        }
    }
}
