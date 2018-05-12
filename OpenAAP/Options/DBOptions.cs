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

    public class DBOptions
    {
        public DatabaseType? DatabaseType { get; set; }

        public string ConnectionStringPostgres { get; set; }
        public string ConnectionStringSqlite { get; set; }
        public string ConnectionStringSqlServer { get; set; }

        public bool? SeedDBWithTestData { get; set; }
    }
}
