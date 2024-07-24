using Microsoft.EntityFrameworkCore;

using WebApplication1.Model;

namespace WebApplication1.Database
{
    public class PostgresDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }

        public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>().Property(u => u.Id).HasColumnName("id");
            modelBuilder.Entity<User>().HasIndex(u => u.Email).HasMethod("hash");
        }
    }
}
