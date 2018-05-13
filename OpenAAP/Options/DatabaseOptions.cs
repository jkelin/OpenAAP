using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Options
{
    public enum DatabaseType
    {
        InMemory,
        Sqlite,
        SqlServer,
        Postgres,
    }

    public class DatabaseOptions
    {
        public const string Section = "Database";

        public DatabaseType? Type { get; set; } = Options.DatabaseType.InMemory;

        public string ConnectionStringPostgres { get; set; }
        public string ConnectionStringSqlite { get; set; }
        public string ConnectionStringSqlServer { get; set; }

        public bool? SeedWithTestData { get; set; } = false;
    }
}
