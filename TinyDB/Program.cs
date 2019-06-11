using System;
using System.Linq;

namespace TinyDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var people = new DbSet<Person>("person");
            var list = people
                .Select(p => new { p.Id, p.Name })
                .Where(p => p.Id > 200)
                .ToList();
            foreach (var item in list) Console.WriteLine(item.Name);
        }
    }
}
