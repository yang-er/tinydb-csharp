using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TinyDb.Querying;

namespace TinyDb
{
    /// <summary>
    /// B+树的索引拓展方法类
    /// </summary>
    public static class BPlusTreeExtensions
    {
        /// <summary>
        /// 将JSON字符串转化为 <see cref="T" /> 的对象。
        /// </summary>
        /// <typeparam name="T">目标转换类型。</typeparam>
        /// <param name="jsonString">源JSON字符串。</param>
        /// <returns>反序列化后的值。</returns>
        /// <exception cref="JsonException" />
        public static T ParseJson<T>(this string jsonString)
        {
            var json = new JsonSerializer();
            if (jsonString == "") throw new JsonReaderException();
            return json.Deserialize<T>(new JsonTextReader(new StringReader(jsonString)));
        }

        /// <summary>
        /// 将对象序列化为JSON文本。
        /// </summary>
        /// <param name="value">要被序列化的对象。</param>
        /// <returns>序列化后的JSON文本。</returns>
        public static string ToJson(this object value)
        {
            var json = new JsonSerializer();
            var sb = new StringBuilder();
            json.Serialize(new JsonTextWriter(new StringWriter(sb)), value);
            return sb.ToString();
        }

        public static int Update<T>(this IQueryable<T> query, params Action<T>[] exps)
        {
            if (exps.Length == 0) return 0;
            int affectedRows = ((QueryProvider)query.Provider).ExecuteUpdate(query.Expression, exps);
            if (affectedRows == -1) throw new InvalidOperationException();
            return affectedRows;
        }
    }
}
