using System;
using System.Linq;

namespace TinyDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var people = new DbSet<Person>("person");
            foreach (var item in people) Console.WriteLine(item.ToString());
            var query = (from p in people
                         where p.Id >= 111 && p.Id < 250 && p.Name.StartsWith("l")
                         select new { p.Id, p.Name }).ToList();
            var list = people
                .Where(p => (p.Id >= 111) && p.Id < 250 && p.Name.StartsWith("l"))
                .Select(p => new { p.Id, p.Name })
                .ToList();
            foreach (var item in list) Console.WriteLine(item.Name);
        }
    }
}
