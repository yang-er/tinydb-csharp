using System;

namespace TinyDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var people = new DbSet<Person>("person");
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 155, Name = "ych", Age = 18, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 152, Name = "gs", Age = 19, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 154, Name = "ylz", Age = 20, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 153, Name = "eric", Age = 200, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 151, Name = "luke", Age = 200, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 150, Name = "luke", Age = 200, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 111, Name = "luke", Age = 200, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 255, Name = "ych", Age = 18, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 252, Name = "gs", Age = 19, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 254, Name = "ylz", Age = 20, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 253, Name = "eric", Age = 200, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 251, Name = "luke", Age = 200, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 250, Name = "luke", Age = 200, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
            people.Insert(new Person { Id = 221, Name = "luke", Age = 200, Sex = Sex.Male });
            foreach (var item in people) Console.WriteLine(item.Name);
        }
    }
}
