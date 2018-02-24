using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server
{
    public class Dungeon
    {        
        Dictionary<int, Room> roomMap;

        public void Init()
        {
            roomMap = new Dictionary<int, Room>();
            // Room 1
            {
                var room = new Room(1, "2,B,X,X", "Entrance Hall", "All adventures start here.\n");
                roomMap.Add(room.roomID, room);
            }

            // Room 2
            {
                var room = new Room(2, "5,3,B,X", "Room 2", "You are in room 2\n");
                roomMap.Add(room.roomID, room);
            }

            // Room 3
            {
                var room = new Room(3, "X,8,4,B", "Room 3", "You are in room 3\n");
                roomMap.Add(room.roomID, room);
            }

            // Room 4
            {
                var room = new Room(4, "B,X,X,1", "Room 4", "You are in room 4\n");
                roomMap.Add(room.roomID, room);
            }

            // Room 5
            {
                var room = new Room(5, "X,X,2,6", "Room 5", "You are in room 5\n");
                roomMap.Add(room.roomID, room);
            }

            // Room 6
            {
                var room = new Room(6, "7,5,X,X", "Room 6", "You are in room 6\n");
                roomMap.Add(room.roomID, room);
            }

            // Room 7
            {
                var room = new Room(7, "X,X,6,X", "Room 7", "You are in room7\n");
                roomMap.Add(room.roomID, room);
            }

            // Room 8
            {
                var room = new Room(8, "9,X,X,3", "Room 8", "You are in room 8\n");
                roomMap.Add(room.roomID, room);
            }

            // Room 9
            {
                var room = new Room(9, "10,X,8,X", "Room 9", "You are in room 9\n");
                roomMap.Add(room.roomID, room);
            }

            // Room 10
            {
                var room = new Room(10, "X,X,9,11", "Room 10", "You are in room 10\n");
                roomMap.Add(room.roomID, room);
            }

            // Room 11
            {
                var room = new Room(11, "X,10,X,X", "Room 11", "You are in room 11\n");
                roomMap.Add(room.roomID, room);
            }
        }

        public Room GetPlayerRoom(Player player)
        {
            Room playerRoom;
            roomMap.TryGetValue(player.currentRoomID, out playerRoom);

            return playerRoom;
        }

        // Try to move the player in direction and return 1, if not return 0
        public int MovePlayer(Player player, String direction)
        {
            // Get the room the player is trying to move from
            Room currentRoom = GetPlayerRoom(player);

            if (direction == "north")
            {
                // check north
                if(currentRoom.north != "X" && currentRoom.north != "B")
                {
                    // Update player room to new room
                    player.currentRoomID = Int32.Parse(currentRoom.north);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (direction == "east")
            {
                // check east
                if (currentRoom.east != "X" && currentRoom.east != "B")
                {
                    // Update player room to new room
                    player.currentRoomID = Int32.Parse(currentRoom.east);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (direction == "south")
            {
                // check south
                if (currentRoom.south != "X" && currentRoom.south != "B")
                {
                    // Update player room to new room
                    player.currentRoomID = Int32.Parse(currentRoom.south);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                // check west
                if (currentRoom.west != "X" && currentRoom.west != "B")
                {
                    // Update player room to new room
                    player.currentRoomID = Int32.Parse(currentRoom.west);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
