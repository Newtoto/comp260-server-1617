using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Sqlite for databases
#if TARGET_LINUX
using Mono.Data.Sqlite;
using sqliteConnection = Mono.Data.Sqlite.SqliteConnection;
using sqliteCommand = Mono.Date.Sqlite.SqliteCommand;
using sqliteDataReader = Mono.Date.Sqlite.SqliteDataReader;
#endif

//#if TARGET_WINDOWS
using System.Data.SQLite;
using sqliteConnection = System.Data.SQLite.SQLiteConnection;
using sqliteCommand = System.Data.SQLite.SQLiteCommand;
using sqliteDataReader = System.Data.SQLite.SQLiteDataReader;
//#endif

namespace Server
{
    public class Room
    {
        sqliteConnection conn = null;

        public String availableExitsText = "You can go ";
        // How other code references this room
        public int roomID;
        // Display name for the room
        public String name = "";
        // Display description for the room
        public String desc = "";
        // CSV string of 4 values containing exit data in NESW order
        private String exitsCode = "";

        public String north;
        public String east;
        public String south;
        public String west;

        public Room(int roomID, String exitsCode, String roomName, String desc)
        {
            this.roomID = roomID;
            this.name = roomName;
            this.desc = desc;

            String[] exits = exitsCode.Split(new Char[] { ',' });
            north = exits[0];
            east = exits[1];
            south = exits[2];
            west = exits[3];

            GetAvailableExitsText();
        }

        // Create string for describing available exits
        private void GetAvailableExitsText()
        {
            
			
            conn = new sqliteConnection("Data Source =" + "rooms.db" + ";Version=3;FailIfMissing=True");

			try
			{
				conn.Open();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Open existing DB failed: " + ex);
			}
            
            

            List<String> availableExits = new List<String>();
            int numberOfExits = 0;

            // Add north if available
            if (north != "X" && north != "B")
            {
                availableExits.Add("north");
                numberOfExits++;
            }
            // Add east if available
            if (east != "X" && east != "B")
            {
                availableExits.Add("east");
                numberOfExits++;
            }
            // Add south if available
            if (south != "X" && south != "B")
            {
                availableExits.Add("south");
                numberOfExits++;
            }
            // Add west if available
            if (west != "X" && west != "B")
            {
                availableExits.Add("west");
                numberOfExits++;
            }

            // Add first room to output text
            availableExitsText += availableExits[0];

            for (var i = 1; i < numberOfExits; i++)
            {
                if(i == numberOfExits - 1)
                {
                    availableExitsText += " or " + availableExits[i];
                }
                else
                {
                    availableExitsText += ", " + availableExits[i];
                }
            }

            // Add punctuation to end of output text
            availableExitsText += ".\n";
        }

        // Returns value of new roomID if available, 0 if not
        public int Go(String direction)
        {
            string newRoomID = "";

            if(direction == "north")
            {
                newRoomID = north;
            }
            else if (direction == "east")
            {
                newRoomID = east;
            }
            else if (direction == "south")
            {
                newRoomID = south;
            }
            else if (direction == "west")
            {
                newRoomID = west;
            }

            if(newRoomID == "X" || newRoomID == "B"|| newRoomID == "")
            {
                // Return 0 for no exit available
                return 0;
            }
            else
            {
                // Return ID of new room
                return Int32.Parse(newRoomID);
            }
        }
    }

}
