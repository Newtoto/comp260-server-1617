using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#if TARGET_LINUX
using Mono.Data.Sqlite;
using sqliteConnection 	=Mono.Data.Sqlite.SqliteConnection;
using sqliteCommand 	=Mono.Data.Sqlite.SqliteCommand;
using sqliteDataReader	=Mono.Data.Sqlite.SqliteDataReader;
#endif

#if TARGET_WINDOWS
using System.Data.SQLite;
using sqliteConnection = System.Data.SQLite.SQLiteConnection;
using sqliteCommand = System.Data.SQLite.SQLiteCommand;
using sqliteDataReader = System.Data.SQLite.SQLiteDataReader;
#endif

namespace Server
{
    class DungeonDbManager
    {
        sqliteConnection dungeonDbConnection = null;
        SQLiteCommand dungeonCommand;

        // Get and open database
        private void OpenDungeonDatabase()
        {
            dungeonDbConnection = new sqliteConnection("Data Source =" + "dungeon.db" + ";Version=3;FailIfMissing=True");

            try
            {
                dungeonDbConnection.Open();
                Console.WriteLine("opened dungeon db");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open dungeon DB failed: " + ex);
            }
        }
    }
}
