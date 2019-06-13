namespace TinyDb.Structure
{
    /// <summary>
    /// 一条数据记录所需要的接口
    /// </summary>
    public interface IDbEntry
    {
        int Id { get; set; }

        int Version { get; }

        object Clone();
    }
}
