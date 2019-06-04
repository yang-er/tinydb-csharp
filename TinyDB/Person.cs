using TinyDb.Structure;

namespace TinyDb
{
    public enum Sex
    {
        Male, Female
    }

    public class Person : IDbEntry
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public Sex Sex { get; set; }

        public int PrimaryKey => Id;

        public int Version => 1;
    }
}
