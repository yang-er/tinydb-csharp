using System;
using System.Linq;

namespace TinyDb
{
    class Program
    {
        static void TestForPerson()
        {
            var people = new DbSet<Person>("person");
            /*
            people.Insert(new Person(55171115, "软件学院", Sex.Male, 2017, "于晨晖", "Mark"));
            people.Insert(new Person(55171102, "软件学院", Sex.Male, 2017, "杨良正", "Ian"));
            people.Insert(new Person(55171112, "软件学院", Sex.Male, 2017, "高赛", "Sail"));
            people.Insert(new Person(55171101, "软件学院", Sex.Male, 2017, "陈宇哲", ""));
            people.Insert(new Person(55171103, "软件学院", Sex.Male, 2017, "安迪", "Andy"));
            people.Insert(new Person(55171104, "软件学院", Sex.Male, 2017, "陆静涵", "Luke"));
            people.Insert(new Person(55171105, "软件学院", Sex.Female, 2017, "王惠宁", ""));
            people.Insert(new Person(55171106, "软件学院", Sex.Male, 2017, "杜维康", "Austin"));
            people.Insert(new Person(55171107, "软件学院", Sex.Female, 2017, "陈希", ""));
            people.Insert(new Person(55171108, "软件学院", Sex.Male, 2017, "倪泽栋", "Zachary"));
            people.Insert(new Person(55171109, "软件学院", Sex.Female, 2017, "史凯阅", "Kaylor"));
            people.Insert(new Person(55171110, "软件学院", Sex.Female, 2017, "李玲玲", ""));
            people.Insert(new Person(55171111, "软件学院", Sex.Male, 2017, "贺翔", "Alex"));
            people.Insert(new Person(55171113, "软件学院", Sex.Male, 2017, "马晨凯", ""));
            people.Insert(new Person(55171114, "软件学院", Sex.Male, 2017, "张迩瀚", "Eric"));
            people.Insert(new Person(55171116, "软件学院", Sex.Male, 2017, "刘慧祥", ""));
            people.Insert(new Person(55171117, "软件学院", Sex.Female, 2017, "薛江莹", ""));
            people.Insert(new Person(55171118, "软件学院", Sex.Male, 2017, "谢旭晨", "Sheldon"));
            people.Insert(new Person(55171119, "软件学院", Sex.Male, 2017, "黄旭", ""));
            people.Insert(new Person(55171120, "软件学院", Sex.Male, 2017, "李广鹏", ""));
            people.Insert(new Person(55171121, "软件学院", Sex.Male, 2017, "冯文显", ""));
            people.Insert(new Person(55171122, "软件学院", Sex.Male, 2017, "刘畅", ""));
            people.Insert(new Person(55171123, "软件学院", Sex.Male, 2017, "刘世喆", ""));
            people.Insert(new Person(55171124, "软件学院", Sex.Male, 2017, "赵国璋", ""));
            people.Insert(new Person(55171125, "软件学院", Sex.Female, 2017, "赵岩", ""));
            people.Insert(new Person(55171126, "软件学院", Sex.Male, 2017, "张浛锋", ""));
            people.Insert(new Person(55171127, "软件学院", Sex.Male, 2017, "张桐", ""));
            people.Insert(new Person(55171128, "软件学院", Sex.Female, 2017, "洪乐", ""));
            people.Insert(new Person(55171129, "软件学院", Sex.Male, 2017, "黄星", ""));
            people.Insert(new Person(55171130, "软件学院", Sex.Male, 2017, "傅宇航", ""));
            people.Insert(new Person(55171131, "软件学院", Sex.Male, 2017, "修可栋", ""));
            //
            people.Insert(new Person(12180903, "计算机学院", Sex.Female, 2018, "刘晓夏", "NaN"));
            people.Insert(new Person(21180108, "计算机学院", Sex.Female, 2018, "王宇", "NaN"));
            people.Insert(new Person(21180126, "计算机学院", Sex.Male, 2018, "李子轩", "NaN"));
            people.Insert(new Person(21180226, "计算机学院", Sex.Male, 2018, "吴致远", "NaN"));
            people.Insert(new Person(21180307, "计算机学院", Sex.Female, 2018, "张蕾", "NaN"));
            people.Insert(new Person(21180319, "计算机学院", Sex.Male, 2018, "任可", "NaN"));
            people.Insert(new Person(21180530, "计算机学院", Sex.Male, 2018, "刘晋汐", "NaN"));
            people.Insert(new Person(21180705, "计算机学院", Sex.Female, 2018, "宋欣怡", "NaN"));
            people.Insert(new Person(21180706, "计算机学院", Sex.Female, 2018, "董锦", "NaN"));
            people.Insert(new Person(21180722, "计算机学院", Sex.Male, 2018, "严祎霖", "NaN"));
            people.Insert(new Person(21180724, "计算机学院", Sex.Male, 2018, "石长隆", "NaN"));
            people.Insert(new Person(21180820, "计算机学院", Sex.Male, 2018, "陈梓铭", "NaN"));
            people.Insert(new Person(21180901, "计算机学院", Sex.Female, 2018, "廖祝鑫", "NaN"));
            people.Insert(new Person(21180926, "计算机学院", Sex.Male, 2018, "丁甲", "NaN"));
            people.Insert(new Person(21181021, "计算机学院", Sex.Male, 2018, "葛郅琦", "NaN"));
            people.Insert(new Person(21181118, "计算机学院", Sex.Male, 2018, "方哲剑", "NaN"));
            people.Insert(new Person(21181205, "计算机学院", Sex.Female, 2018, "胡竹语", "NaN"));
            people.Insert(new Person(21181219, "计算机学院", Sex.Male, 2018, "胡昕", "NaN"));
            people.Insert(new Person(21181222, "计算机学院", Sex.Male, 2018, "卢金达", "NaN"));
            people.Insert(new Person(21181224, "计算机学院", Sex.Male, 2018, "张小龙", "NaN"));
            people.Insert(new Person(21181225, "计算机学院", Sex.Male, 2018, "孙延浩", "NaN"));
            people.Insert(new Person(21181318, "计算机学院", Sex.Male, 2018, "叶航廷", "NaN"));
            people.Insert(new Person(21181408, "计算机学院", Sex.Female, 2018, "冉艳月", "NaN"));
            people.Insert(new Person(21181422, "计算机学院", Sex.Male, 2018, "傅贤", "NaN"));
            people.Insert(new Person(21181520, "计算机学院", Sex.Male, 2018, "高贤骏", "NaN"));
            people.Insert(new Person(21181522, "计算机学院", Sex.Male, 2018, "杜小龙", "NaN"));
            people.Insert(new Person(12180906, "计算机学院", Sex.Male, 2018, "丁正浩", "NaN"));
            //*/

            //foreach (var item in people) Console.WriteLine(item.ToString());
            var query = (from p in people
                         where p.Name.Length <= 2
                         where p.Id > 21180000 && p.Id < 55160000
                         select new { p.Id, p.Name }).ToList();
            foreach (var item in query) Console.WriteLine(item.Name);

            var upd = people
                .Where(p => p.Id > 55170000)
                .Where(p => p.EnglishName == "QAQ")
                .Update(p => p.EnglishName = "FAF");
            Console.WriteLine("AffectedRows: " + upd);

            foreach (var item in people) Console.WriteLine(item);
        }

        static void TestForJoin()
        {
            var contests = new DbSet<Contest>("contest");
            var problems = new DbSet<Problem>("problem");

            /*
            contests.Insert(new Contest { Id = 1, Name = "第一次实验课" });
            contests.Insert(new Contest { Id = 2, Name = "第二次实验课" });
            contests.Insert(new Contest { Id = 6, Name = "第六次实验课" });
            contests.Insert(new Contest { Id = 7, Name = "第七次实验课" });
            contests.Insert(new Contest { Id = 8, Name = "第八次实验课" });
            contests.Insert(new Contest { Id = 3, Name = "第三次实验课" });
            contests.Insert(new Contest { Id = 4, Name = "第四次实验课" });
            contests.Insert(new Contest { Id = 5, Name = "第五次实验课" });

            problems.Insert(new Problem { Id = 4, Name = "C", ContestId = 3 });
            problems.Insert(new Problem { Id = 5, Name = "C", ContestId = 8 });
            problems.Insert(new Problem { Id = 1, Name = "A", ContestId = 1 });
            problems.Insert(new Problem { Id = 2, Name = "C", ContestId = 1 });
            problems.Insert(new Problem { Id = 3, Name = "C", ContestId = 5 });
            //*/

            var tst2 = from p in problems where p.Id > 2
                       join c in contests on p.ContestId equals c.Id
                       select new { p.Id, p.Name, Contest = c.Name };
            foreach (var item in tst2.ToList())
            {
                Console.WriteLine($"{item.Id}, {item.Name}, {item.Contest}");
            }
        }

        static void Main(string[] args)
        {
            TestForPerson();
        }
    }
}
