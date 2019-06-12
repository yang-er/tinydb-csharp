using TinyDb.Structure;

namespace TinyDb
{
    public class Contest : IDbEntry
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual int Version => 1;
    }

    public class Problem : IDbEntry
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ContestId { get; set; }

        public virtual int Version => 1;
    }
}
