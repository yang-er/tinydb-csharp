using System;
using System.Linq.Expressions;
using System.Reflection;

namespace TinyDb.Structure
{
    /// <summary>
    /// 一条数据记录所需要的接口
    /// </summary>
    public interface IDbEntry
    {
        int Id { get; }

        /// <summary>
        /// 数据记录的版本
        /// </summary>
        int Version { get; }
    }
}
