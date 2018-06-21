namespace PostgreSqlBox
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    internal static class SelectAllBenchmark
    {
        internal static void Run()
        {
            using (var db = new Database())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE \"Foos\" RESTART IDENTITY");
                db.AddRange(
                    new Foo { Text = "1" },
                    new Foo { Text = "2" },
                    new Foo { Text = "3" },
                    new Foo { Text = "4" });
                db.SaveChanges();
            }

            Console.WriteLine("EF");
            Ef();
            Console.WriteLine("EF");
            Ef();
            Console.WriteLine("ADO");
            Ado();
            Console.WriteLine("ADO");
            Ado();
        }

        internal static void Ef()
        {
            using (var db = new Database())
            {
                var sw = Stopwatch.StartNew();
                var foos = db.Foos.ToList();
                sw.Stop();
                Console.WriteLine($"Selecting {foos.Count} items took {sw.ElapsedMilliseconds:N1} ms.");

                sw.Restart();
                foos = db.Foos.ToList();
                sw.Stop();
                Console.WriteLine($"Selecting {foos.Count} items took {sw.ElapsedMilliseconds:N1} ms.");
            }
        }

        internal static void Ado()
        {
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                var sw = Stopwatch.StartNew();
                db.Open();
                var foos = db.Select().ToList();
                sw.Stop();
                Console.WriteLine($"Selecting {foos.Count} items took {sw.ElapsedMilliseconds:N1} ms.");

                sw.Restart();
                foos = db.Select().ToList();
                sw.Stop();
                Console.WriteLine($"Selecting {foos.Count} items took {sw.ElapsedMilliseconds:N1} ms.");
            }
        }

        private static IEnumerable<Foo> Select(this NpgsqlConnection connection)
        {
            using (var command = new NpgsqlCommand("SELECT \"Id\", \"Text\" FROM \"Foos\"", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    var id = reader.GetOrdinal("Id");
                    var text = reader.GetOrdinal("Text");
                    while (reader.Read())
                    {
                        yield return new Foo
                        {
                            Id = reader.GetInt32(id),
                            Text = reader.GetString(text)
                        };
                    }
                }
            }
        }
    }
}