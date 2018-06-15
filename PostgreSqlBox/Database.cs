namespace PostgreSqlBox
{
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    public class Database : DbContext
    {
        public Database()
        {
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
                          {
                              Host = "localhost",
                              Database = "test",
                          };

            builder.UseNpgsql(connectionStringBuilder.ToString());
        }
 
        public DbSet<Foo> Foos { get; set; }
    }
}