using Microsoft.EntityFrameworkCore;
using UserRegFormHWv01.DAL.Entities;

namespace UserRegFormHWv01.DAL.Context
{
    public class IntroContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public IntroContext(DbContextOptions options)
            :base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
