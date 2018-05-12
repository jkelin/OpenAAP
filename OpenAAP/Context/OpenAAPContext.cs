using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Context
{
    public class OpenAAPContext : DbContext
    {
        public DbSet<Identity> Identity { get; set; }

        public DbSet<PasswordAuthentication> PasswordAuthentication { get; set; }

        public OpenAAPContext(DbContextOptions<OpenAAPContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public async Task Seed()
        {
            if (await Identity.AnyAsync() || await PasswordAuthentication.AnyAsync())
            {
                return;
            }

            await Identity.AddRangeAsync(Seeder.Identites);
            await PasswordAuthentication.AddRangeAsync(Seeder.PasswordAuths);

            await SaveChangesAsync();
        }
    }
}
