namespace PostgreSqlBox
{
    using System;
    using System.Diagnostics;

    class Program
    {
        static void Main()
        {
            const int n = 100_000;
            using (var db = new Database())
            {
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < n; i++)
                {
                    db.Foos.Add(new Foo { Text = "abc" });
                }

                db.SaveChanges();
                sw.Stop();
                Console.WriteLine($"Inserting {n} items took {sw.ElapsedMilliseconds} ms ({sw.ElapsedMilliseconds / n} ms/insert)");
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
