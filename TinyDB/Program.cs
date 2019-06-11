using System;
using System.Linq;

namespace TinyDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var people = new DbSet<Person>("person");
            foreach (var item in people) Console.WriteLine(item.Id +":"+ item.Name);
            var list = people
                .Where(p => (p.Id >= 111) && p.Id < 250)
                .Select(p => new { p.Id, p.Name })
                .ToList();
            foreach (var item in list) Console.WriteLine(item.Name);
        }
    }
}
