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

        public string School { get; set; }

        public string Name { get; set; }

        public string EnglishName { get; set; }

        public int Grade { get; set; }

        public Sex Sex { get; set; }

        public int Version => 1;

        public Person() { }

        public Person(int id, string sch, Sex sex, int grade, string cn, string en)
        {
            Id = id;
            School = sch;
            Sex = sex;
            Grade = grade;
            Name = cn;
            EnglishName = en;
        }

        public override string ToString()
        {
            return $"Person (V{Version}) [Id={Id},Name={Name},Grade={Grade},School={School},Sex={Sex},EnglishName={EnglishName}]";
        }

        public object Clone()
        {
            return new Person
            {
                Id = Id,
                School = School,
                Sex = Sex,
                Grade = Grade,
                Name = Name,
                EnglishName = EnglishName
            };
        }
    }
}
