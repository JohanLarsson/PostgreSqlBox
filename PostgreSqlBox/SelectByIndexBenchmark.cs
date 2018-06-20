namespace PostgreSqlBox
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    internal static class SelectByIndexBenchmark
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
                var first = db.Foos.First(x => x.Text == "1");
                sw.Stop();
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N1} ms.)");

                sw.Restart();
                first = db.Foos.First(x => x.Text == "2");
                sw.Stop();
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N1} ms.)");

                sw.Restart();
                first = db.Foos.First(x => x.Text == "3");
                sw.Stop();
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N1} ms.)");

                sw.Restart();
                first = db.Foos.First(x => x.Text == "4");
                sw.Stop();
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N1} ms.)");
            }
        }

        internal static void Ado()
        {
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                var sw = Stopwatch.StartNew();
                db.Open();
                var match = db.Select(1);
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N1} ms.)");

                match = db.Select(2);
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N1} ms.)");

                match = db.Select(3);
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N1} ms.)");

                match = db.Select(4);
                Console.WriteLine($"Selecting one item took {sw.ElapsedMilliseconds:N1} ms.)");
            }
        }

        private static Foo Select(this NpgsqlConnection connection, int id)
        {
            using (var command = new NpgsqlCommand("SELECT \"Id\", \"Text\" FROM \"Foos\" WHERE \"Id\" = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.Read())
                    {
                        return new Foo
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Text = reader.GetString(reader.GetOrdinal("Text"))
                        };
                    }
                }
            }

            throw new InvalidOperationException();
        }
    }
}