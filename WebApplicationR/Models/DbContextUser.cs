using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationR.Models;

namespace WebApplicationR.Models
{
    public class DbContextUser:IdentityDbContext<User>
    {
        public DbContextUser(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> User { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        
    }
}
