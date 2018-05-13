using Humanizer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Context
{
    public class OpenAAPContext : DbContext
    {
        public DbSet<Identity> Identities { get; set; }

        public DbSet<PasswordAuthentication> PasswordAuthentications { get; set; }

        public DbSet<Session> Sessions { get; set; }

        public OpenAAPContext(DbContextOptions<OpenAAPContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            RenameTables(modelBuilder);
            RenameColumns(modelBuilder);
            RenameForeignKeys(modelBuilder);
            RenameIndices(modelBuilder);
        }

        private static void RenameIndices(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var index in entityType.GetIndexes())
                {
                    index.Relational().Name = index.Relational().Name.ToSnakeCase();
                }
            }
        }

        private static void RenameForeignKeys(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    foreach (var fk in entityType.FindForeignKeys(property))
                    {
                        fk.Relational().Name = fk.Relational().Name.ToSnakeCase();
                    }
                }
            }
        }

        private static void RenameColumns(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    property.Relational().ColumnName = property.Relational().ColumnName.ToSnakeCase();
                }
            }
        }

        private static void RenameTables(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.Relational().TableName = entityType.Relational().TableName.Singularize().ToSnakeCase();
            }
        }

        public async Task Seed()
        {
            if (await Identities.AnyAsync() || await PasswordAuthentications.AnyAsync())
            {
                return;
            }

            await Identities.AddRangeAsync(Seeder.Identites);
            await PasswordAuthentications.AddRangeAsync(Seeder.PasswordAuths);

            await SaveChangesAsync();
        }
    }
}
