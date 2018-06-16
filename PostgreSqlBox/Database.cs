namespace PostgreSqlBox
{
    using System.Configuration;
    using Microsoft.EntityFrameworkCore;

    public class Database : DbContext
    {
        private static bool isMigrated;

        public Database()
        {
            if (!isMigrated)
            {
                this.Database.Migrate();
                isMigrated = true;
            }
        }

        public DbSet<Foo> Foos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ConfigurationManager.ConnectionStrings["Database"].ConnectionString);
        }
    }
}