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
    class DungeonDBAccess
    {
        sqliteConnection dungeonDbConnection = null;
		sqliteCommand dungeonCommand;

        public DungeonDBAccess()
        {
            OpenDungeonDatabase();
        }

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

        public String GetRoomText(int roomID)
        {
            // Get name
            dungeonCommand = new sqliteCommand("select Name from Rooms where ID ='" + roomID + "'", dungeonDbConnection);

            string roomText = "";

            var reader = dungeonCommand.ExecuteReader();

            while (reader.Read())
            {
                roomText += "You find yourself in the " + reader[0].ToString() + ". ";
            }

            // Get description
            dungeonCommand = new sqliteCommand("select Description from Rooms where ID ='" + roomID + "'", dungeonDbConnection);

            reader = dungeonCommand.ExecuteReader();

            while (reader.Read())
            {
                roomText += reader[0].ToString();
            }

            return roomText;
        }

        public String GetRoomNameFromID(int roomID)
        {
            // Get description
            dungeonCommand = new sqliteCommand("select Name from Rooms where ID ='" + roomID + "'", dungeonDbConnection);

            string roomName = "";

            var reader = dungeonCommand.ExecuteReader();

            while (reader.Read())
            {
                roomName = reader[0].ToString();
            }

            return roomName;
        }

        // Returns room ID of the chosen exit
        public int GetRoomExit(string direction, int currentRoomID)
        {
            dungeonCommand = new sqliteCommand("select " + direction + " from Rooms where ID ='" + currentRoomID + "'", dungeonDbConnection);

            var reader = dungeonCommand.ExecuteReader();

            while (reader.Read())
            {
                return int.Parse(string.Format("{0}", reader[0]));
            }

            return 0;
        }
    }
}
