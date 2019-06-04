namespace TinyDb.Structure
{
    /// <summary>
    /// 一条数据记录所需要的接口
    /// </summary>
    public interface IDbEntry
    {
        /// <summary>
        /// 只读的主键值
        /// </summary>
        int PrimaryKey { get; }

        /// <summary>
        /// 数据记录的版本
        /// </summary>
        int Version { get; }
    }
}
