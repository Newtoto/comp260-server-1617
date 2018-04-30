using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if TARGET_LINUX
using Mono.Data.Sqlite;
using sqliteConnection = Mono.Data.Sqlite.SqliteConnection;
using sqliteCommand = Mono.Date.Sqlite.SqliteCommand;
using sqliteDataReader = Mono.Date.Sqlite.SqliteDataReader;

namespace Server.Properties
{
    public class SqliteCommands
    {
        public SqliteCommands()
        {
        }
    }
}
