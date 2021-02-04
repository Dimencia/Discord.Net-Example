using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InactiviteRoleRemover
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .Property(u => u.RoleIdsToRestore)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => ulong.Parse(s)).ToList());

            modelBuilder.Entity<User>().HasData(new User() { DiscordId = 123, LastActivity = DateTime.Now, Id = Guid.NewGuid() });
        }
    }
}
