namespace PostgreSqlBox
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    class Program
    {
        static void Main()
        {
            const int n = 100_000;
            using (var db = new Database())
            {
                var sw = Stopwatch.StartNew();
                db.Foos.AddRange(Enumerable.Range(0, n).Select(x => new Foo { Text = x.ToString() }));
                db.SaveChanges();
                sw.Stop();
                Console.WriteLine($"Inserting {n:N0} items took {sw.ElapsedMilliseconds:N0} ms.");

                sw.Restart();
                var list = db.Foos.ToList();
                sw.Stop();
                Console.WriteLine($"Selecting {list.Count:N0} items took {sw.ElapsedMilliseconds:N0} ms.");

                sw.Restart();
                var first = db.Foos.First(x => x.Text == "10000");
                sw.Stop();
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N0} ms.)");

                sw.Restart();
                first = db.Foos.First(x => x.Text == "20000");
                sw.Stop();
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N0} ms.)");

                sw.Restart();
                first = db.Foos.First(x => x.Text == "30000");
                sw.Stop();
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N0} ms.)");

                sw.Restart();
                first = db.Foos.First(x => x.Text == "40000");
                sw.Stop();
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N0} ms.)");

                db.Database.ExecuteSqlCommand("TRUNCATE \"Foos\" RESTART IDENTITY");
                db.SaveChanges();
            }
        }
    }
}
