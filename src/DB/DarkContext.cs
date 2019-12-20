using Common;
using Microsoft.EntityFrameworkCore;

namespace DB
{
    public class DarkContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.GetConnectionString("dark-maria"));
        }

        public DbSet<Tables.Post> Posts { get; set; }
        public DbSet<Tables.Comment> Comments { get; set; }
        public DbSet<Tables.File> Files { get; set; }
    }
}
