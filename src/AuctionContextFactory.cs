using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace InactiviteRoleRemover.Contexts
{
    /// <summary>
    /// This is necessary for design-time connection to our db, like migrations
    /// </summary>
    public class AuctionContextFactory : IDesignTimeDbContextFactory<UserContext>
    {
       

        public UserContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<UserContext>();
            options.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=inactivity;Integrated Security=True;");

            return new UserContext(options.Options);
        }
    }
}
