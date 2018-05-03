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
    public class Dungeon
    {        
        Dictionary<int, Room> roomMap;
        sqliteConnection dungeonDbConnection = null;
		sqliteCommand dungeonCommand;
        sqliteConnection playerDbConnection = null;
		sqliteCommand playerCommand;


        public void Init()
        {
            roomMap = new Dictionary<int, Room>();

            OpenPlayerAndDungeonDataBase();

			dungeonCommand = new sqliteCommand("select * from " + "Rooms", dungeonDbConnection);
			playerCommand = new sqliteCommand("select * from " + "PlayerInfo", playerDbConnection);
        }

        // Get and open database
        private void OpenPlayerAndDungeonDataBase()
        {
            dungeonDbConnection = new sqliteConnection("Data Source =" + "dungeon.db" + ";Version=3;FailIfMissing=True");
            playerDbConnection = new sqliteConnection("Data Source =" + "players.db" + ";Version=3;FailIfMissing=True");

            try
            {
                dungeonDbConnection.Open();
                Console.WriteLine("opened dungeon db");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open dungeon DB failed: " + ex);
            }

            try
            {
                playerDbConnection.Open();
                Console.WriteLine("opened player db");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open player DB failed: " + ex);
            }
        }

        // Get the roomId of the room the player is in
        public int GetPlayerRoom(int playerID)
        {
            var reader = playerCommand.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader["PlayerID"]);
            }

            return 0;
        }

        // Try to move the player in direction and return 1, if not return 0
        public int MovePlayer(Player player, String direction)
        {
            // Get the room the player is trying to move from
            //Room currentRoom = GetPlayerRoom(player);

            //if (direction == "north")
            //{
            //    // check north
            //    if(currentRoom.north != "X" && currentRoom.north != "B")
            //    {
            //        // Update player room to new room
            //        player.currentRoomID = Int32.Parse(currentRoom.north);
            //        return 1;
            //    }
            //    else
            //    {
            //        return 0;
            //    }
            //}
            //else if (direction == "east")
            //{
            //    // check east
            //    if (currentRoom.east != "X" && currentRoom.east != "B")
            //    {
            //        // Update player room to new room
            //        player.currentRoomID = Int32.Parse(currentRoom.east);
            //        return 1;
            //    }
            //    else
            //    {
            //        return 0;
            //    }
            //}
            //else if (direction == "south")
            //{
            //    // check south
            //    if (currentRoom.south != "X" && currentRoom.south != "B")
            //    {
            //        // Update player room to new room
            //        player.currentRoomID = Int32.Parse(currentRoom.south);
            //        return 1;
            //    }
            //    else
            //    {
            //        return 0;
            //    }
            //}
            //else
            //{
            //    // check west
            //    if (currentRoom.west != "X" && currentRoom.west != "B")
            //    {
            //        // Update player room to new room
            //        player.currentRoomID = Int32.Parse(currentRoom.west);
            //        return 1;
            //    }
            //    else
            //    {
            //        return 0;
            //    }
            //}
            return 0;
        }

        // Take players message and get run appropriate commands if available
        public String ParsePlayerInput(Player player, String playerInput)
        {
            // Split up text part of message into words
            string[] parsedTextMessage = playerInput.Split(new Char[] { ' ' });

            // Get lowercase of first word
            string command = parsedTextMessage[0].ToLower();
            string option = "";
            string additionalText = "";

            Console.WriteLine(parsedTextMessage.Length);

            // Get second word as option
            if (parsedTextMessage.Length > 1)
            {
                option = parsedTextMessage[1].ToLower();
            }

            // Reappend third words and higher
            if (parsedTextMessage.Length > 2)
            {
                additionalText = parsedTextMessage[2];
                if (parsedTextMessage.Length > 2)
                {
                    for (var i = 3; i < parsedTextMessage.Length; i++)
                    {
                        additionalText += " " + parsedTextMessage[i];
                    }
                }

            }

            String msg = "";

            switch (command)
            {
                case "go":
                    // Check for valid direction input
                    if (option == "north" || option == "east" || option == "south" || option == "west")
                    {
                        if (MovePlayer(player, option) == 1)
                        {
                            // Let player know they have moved
                            msg = "You go " + option + ".\n";
                        }
                        else
                        {
                            // Tell player they can't move that way
                            msg = "You cannot go " + option + ".\n";
                        }

                        //Room currentRoom = GetPlayerRoom(player);
                        //string roomNameText = "You find yourself in the " + currentRoom.name + ". ";
                        //string roomDescriptionText = currentRoom.desc;
                        //// Get direction options
                        //string directions = currentRoom.availableExitsText;
                       // msg += roomNameText + roomDescriptionText + directions;
                    }
                    else
                    {
                        msg = "Sorry, you can't go " + option + ".";
                    }
                    break;
                default:
                    msg = "Sorry, I don't understand '" + playerInput + "'";
                    break;
            }

            return msg;
        }
    }
}
