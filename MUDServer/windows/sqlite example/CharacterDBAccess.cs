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
    class CharacterDBAccess
    {
        sqliteConnection characterDBConnection = null;
		sqliteCommand characterCommand;

        public CharacterDBAccess()
        {
            OpenDatabases();
        }

        // Get and open database
        private void OpenDatabases()
        {
            // Link databases
            characterDBConnection = new sqliteConnection("Data Source =" + "players.db" + ";Version=3;FailIfMissing=True");

            try
            {
                characterDBConnection.Open();
                Console.WriteLine("opened player db");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open player DB failed: " + ex);
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
