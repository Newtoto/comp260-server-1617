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
    class CharacterDB
    {
        sqliteConnection characterDBConnection = null;
		sqliteCommand characterCommand;
        sqliteConnection userDBConnection = null;
		sqliteCommand userCommand;

        public CharacterDB()
        {
            OpenDatabases();
        }

        // Get and open database
        private void OpenDatabases()
        {
            // Link databases
            characterDBConnection = new sqliteConnection("Data Source =" + "players.db" + ";Version=3;FailIfMissing=True");
            userDBConnection = new sqliteConnection("Data Source =" + "users.db" + ";Version=3;FailIfMissing=True");

            try
            {
                characterDBConnection.Open();
                Console.WriteLine("opened player db");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open player DB failed: " + ex);
            }

            try
            {
                userDBConnection.Open();
                Console.WriteLine("opened user db");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open user DB failed: " + ex);
            }
        }

        // Get the roomId of the room the player is in
        public int GetCharacterRoom(String characterName)
        {
            characterCommand = new sqliteCommand("select CurrentRoom from PlayerInfo where Name ='" + characterName + "'", characterDBConnection);

            var reader = characterCommand.ExecuteReader();

            while (reader.Read())
            {
                return int.Parse(string.Format("{0}", reader[0]));
            }

            return 0;
        }

        // Returns the playerID of user with matching name and password
        public int LoginUser(string username, string password)
        {
			userCommand = new sqliteCommand("select Password from Users where Username ='" + username + "'", userDBConnection);

            var reader = userCommand.ExecuteReader();

            while (reader.Read())
            {
                if (reader[0].ToString() == password)
                {
                    Console.WriteLine(reader[0]);
                    // Get and return player ID
					userCommand = new sqliteCommand("select PlayerID from Users where Username ='" + username + "'", userDBConnection);
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

        // Checks master user database for same username
        public bool CheckForExistingUsername(string username)
        {
            userCommand = new sqliteCommand("select * from Users where Username ='" + username + "'", userDBConnection);

            var reader = userCommand.ExecuteReader();

            return reader.Read();
        }

        // Checks character database for existing playername
        public bool CheckForExistingCharacterName(string characterName)
        {
            characterCommand = new sqliteCommand("select * from PlayerInfo where Name ='" + characterName + "'", characterDBConnection);

            var reader = characterCommand.ExecuteReader();

            return reader.Read();
        }

        // Gets a list of all the characters owned by a player
        public List<String> GetPlayerCharacters(int userID)
        {
            characterCommand = new sqliteCommand("select Name from PlayerInfo where Owner ='" + userID + "'", characterDBConnection);
            List<String> characterList = new List<String>();

            var reader = characterCommand.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader[0]);
                characterList.Add(reader[0].ToString());
            }

            return characterList;
        }

        // Double checks the right owner is accessing the character, used for hacker protection
        public bool DoesUserOwnCharacter(int playerID, String selectedCharacter)
        {
            List<String> characterList = GetPlayerCharacters(playerID);

            foreach (String characterName in characterList)
            {
                if(characterName == selectedCharacter)
                {
                    return true;
                }
            }

            return false;
        }

        // Create new master user in user database
        public int CreateNewUser(String username, String password)
        {
            userCommand = new sqliteCommand("select PlayerID from Users", userDBConnection);
            int largestPlayerID = 0;
            var reader = userCommand.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    int currentInt = int.Parse(string.Format("{0}", reader[0]));

                    if (currentInt > largestPlayerID)
                    {
                        largestPlayerID = currentInt;
                    }
                }
                catch
                {
                    // PlayerID isn't set for some reason in db
                }
            }

            // Set largestPlayerID to 1 more than highest ID
            largestPlayerID += 1;

            try
            {
                var sql = "insert into " + "Users" + " (Username, Password, PlayerID) values ";
                sql += "('" + username + "'";
                sql += ",";
                sql += "'" + password + "'";
                sql += ",";
                sql += "'" + largestPlayerID + "'";
                sql += ")";

                userCommand = new sqliteCommand(sql, userDBConnection);
                userCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add: " + username + " : " + password + " to DB " + ex);
            }

            return largestPlayerID;
        }

        // Makes new character and associates with player
        public void CreateNewCharacter(String characterName, int owner)
        {
            try
            {
                var sql = "insert into " + "PlayerInfo" + " (Owner, Name, CurrentRoom) values ";
                sql += "('" + owner + "'";
                sql += ",";
                sql += "'" + characterName + "'";
                sql += ",";
                sql += "'" + 1 + "'";
                sql += ")";

                characterCommand = new sqliteCommand(sql, characterDBConnection);
                characterCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add: " + characterName + " to DB " + ex);
            }
        }
    }
}
