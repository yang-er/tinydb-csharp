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
                .Where(p => !(p.Id > 300) || p.Id < 500)
                .Select(p => new { p.Id, p.Name })
                .ToList();
            foreach (var item in list) Console.WriteLine(item.Name);
        }
    }
}
