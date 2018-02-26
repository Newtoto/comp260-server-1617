using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Room
    {
        public String availableExitsText = "You can go ";
        // How other code references this room
        public int roomID;
        // Display name for the room
        public String name = "";
        // Display description for the room
        public String desc = "";
        // CSV string of 4 values containing exit data in NESW order
        public String exitsCode = "";

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
        public void GetAvailableExitsText()
        {
            List<String> availableExits = new List<String>();

            // Add north if available
            if (north != "X" && north != "B")
            {
                availableExits.Add("north");
            }
            // Add east if available
            if (east != "X" && east != "B")
            {
                availableExits.Add("east");
            }
            // Add south if available
            if (south != "X" && south != "B")
            {
                availableExits.Add("south");
            }
            // Add west if available
            if (west != "X" && west != "B")
            {
                availableExits.Add("west");
            }

            // Add first room to output text
            availableExitsText += availableExits[0];

            for (var i = 0; i < 5; i++)
            {
                if (availableExits.Capacity < i)
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
