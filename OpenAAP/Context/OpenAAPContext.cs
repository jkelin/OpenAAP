using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Context
{
    public class OpenAAPContext : DbContext
    {
        public DbSet<IdentityModel> Identity { get; set; }

        public DbSet<PasswordAuthenticationModel> PasswordAuthentication { get; set; }

        public OpenAAPContext(DbContextOptions<OpenAAPContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityModel>().HasData(Seeder.Identites);
            modelBuilder.Entity<PasswordAuthenticationModel>().HasData(Seeder.PasswordAuths);
        }
    }
}
