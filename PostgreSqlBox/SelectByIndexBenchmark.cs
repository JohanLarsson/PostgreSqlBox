﻿namespace PostgreSqlBox
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
            Console.WriteLine(nameof(SelectByIndexBenchmark));
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

            Ef();
            Ef();
            Ado();
            Ado();
        }

        internal static void Ef()
        {
            Console.WriteLine("EF");
            using (var db = new Database())
            {
                var sw = Stopwatch.StartNew();
                var first = db.Foos.First(x => x.Text == "1");
                sw.Stop();
                Console.WriteLine($"Selecting item {first.Id} took {sw.ElapsedMilliseconds:N1} ms.");
                
                sw.Restart();
                first = db.Foos.First(x => x.Text == "2");
                sw.Stop();
                Console.WriteLine($"Selecting item {first.Id} took {sw.ElapsedMilliseconds:N1} ms.");

                sw.Restart();
                first = db.Foos.First(x => x.Text == "3");
                sw.Stop();
                Console.WriteLine($"Selecting item {first.Id} took {sw.ElapsedMilliseconds:N1} ms.");

                sw.Restart();
                first = db.Foos.First(x => x.Text == "4");
                sw.Stop();
                Console.WriteLine($"Selecting item {first.Id} took {sw.ElapsedMilliseconds:N1} ms.");
            }
        }

        internal static void Ado()
        {
            Console.WriteLine("ADO");
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                var sw = Stopwatch.StartNew();
                db.Open();
                var match = db.Select(1);
                sw.Stop();
                Console.WriteLine($"Selecting item {match.Id} took {sw.ElapsedMilliseconds:N1} ms.");
                sw.Restart();

                match = db.Select(2);
                sw.Stop();
                Console.WriteLine($"Selecting item {match.Id} took {sw.ElapsedMilliseconds:N1} ms.");
                sw.Restart();

                match = db.Select(3);
                sw.Stop();
                Console.WriteLine($"Selecting item {match.Id} took {sw.ElapsedMilliseconds:N1} ms.");
                sw.Restart();

                match = db.Select(4);
                sw.Stop();
                Console.WriteLine($"Selecting item {match.Id} took {sw.ElapsedMilliseconds:N1} ms.");
                sw.Restart();
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