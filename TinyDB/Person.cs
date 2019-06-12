using System;
using System.Linq.Expressions;
using System.Reflection;
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

        public int Version => 1;

        public override string ToString()
        {
            return $"Person(V{Version}) [Id={Id},Name={Name},Age={Age},Sex={Sex}]";
        }
    }
}
