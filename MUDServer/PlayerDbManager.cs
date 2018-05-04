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
    class PlayerDbManager
    {
        sqliteConnection playerDbConnection = null;
		sqliteCommand playerCommand;
        sqliteConnection userDbConnection = null;
		sqliteCommand userCommand;

        public PlayerDbManager()
        {
            OpenDungeonDatabase();
        }

        // Get and open database
        private void OpenDungeonDatabase()
        {
            // Link databases
            playerDbConnection = new sqliteConnection("Data Source =" + "players.db" + ";Version=3;FailIfMissing=True");
            userDbConnection = new sqliteConnection("Data Source =" + "users.db" + ";Version=3;FailIfMissing=True");

            try
            {
                playerDbConnection.Open();
                Console.WriteLine("opened player db");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open player DB failed: " + ex);
            }

            try
            {
                userDbConnection.Open();
                Console.WriteLine("opened user db");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open user DB failed: " + ex);
            }
        }

        // Get the roomId of the room the player is in
        public int GetPlayerRoom(int playerID)
        {
			playerCommand = new sqliteCommand("select CurrentRoom from " + "PlayerInfo", playerDbConnection);

            var reader = playerCommand.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader["PlayerID"]);
            }

            return 0;
        }

        public int LoginUser(string username, string password)
        {
			userCommand = new sqliteCommand("select Password from Users where Username ='" + username + "'", userDbConnection);

            var reader = userCommand.ExecuteReader();

            while (reader.Read())
            {
                if (reader[0].ToString() == password)
                {
                    Console.WriteLine(reader[0]);
                    // Get and return player ID
					userCommand = new sqliteCommand("select PlayerID from Users where Username ='" + username + "'", userDbConnection);
                    reader = userCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        return int.Parse(string.Format("{0}", reader[0]));
                    }
                }
            }
            // Return failed login
            return 0;
        }

        public int CreateUser(string username, string password)
        {
			userCommand = new sqliteCommand("select Username from " + "Users", userDbConnection);

            var reader = userCommand.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader[0]);
            }

            // Return failed sign up
            return 0;
        }

        public bool CheckForExistingUsername(string username)
        {
            userCommand = new sqliteCommand("select * from Users where Username ='" + username + "'", userDbConnection);

            var reader = userCommand.ExecuteReader();

            return reader.Read();
        }

        public List<String> GetPlayerCharacters(int playerID)
        {
            playerCommand = new sqliteCommand("select Name from PlayerInfo where Owner ='" + playerID + "'", playerDbConnection);
            List<String> characterList = new List<String>();

            var reader = playerCommand.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader[0]);
                characterList.Add(reader[0].ToString());
            }

            return characterList;
        }

        public void CreateNewUser(String username, String password)
        {
            try
            {
                var sql = "insert into " + "Users" + " (Username, Password) values ";
                sql += "('" + username + "'";
                sql += ",";
                sql += "'" + password + "'";
                sql += ")";

                userCommand = new sqliteCommand(sql, userDbConnection);
                userCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add: " + username + " : " + password + " to DB " + ex);
            }
        }
    }
}
