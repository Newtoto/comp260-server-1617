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
                var room = new Room(1, "2,B,X,X", "Entrance Hall", "A large stone statue lies before you weilding a sword and shield. ");
                roomMap.Add(room.roomID, room);
            }

            // Room 2
            {
                var room = new Room(2, "5,3,B,X", "Ballroom", "Feel free to stay and have a dance. ");
                roomMap.Add(room.roomID, room);
            }

            // Room 3
            {
                var room = new Room(3, "X,8,4,B", "Dusty Corridor", "Suits of armour line the walls. ");
                roomMap.Add(room.roomID, room);
            }

            // Room 4
            {
                var room = new Room(4, "B,X,X,1", "Taxidermy Room", "You feel like you're being watched. ");
                roomMap.Add(room.roomID, room);
            }

            // Room 5
            {
                var room = new Room(5, "X,X,2,6", "Drawing Room", "Have a seat and play some games. ");
                roomMap.Add(room.roomID, room);
            }

            // Room 6
            {
                var room = new Room(6, "7,5,X,X", "Spiral Staircase", "The stairs weave round from the drawing room, a suspicious door lies to the north at the bottom. ");
                roomMap.Add(room.roomID, room);
            }

            // Room 7
            {
                var room = new Room(7, "X,X,6,X", "Furry Room", "Nothing to see here. ");
                roomMap.Add(room.roomID, room);
            }

            // Room 8
            {
                var room = new Room(8, "9,X,X,3", "Armoury", "You would pick up a sword here, but you're too good for that. ");
                roomMap.Add(room.roomID, room);
            }

            // Room 9
            {
                var room = new Room(9, "10,X,8,X", "Forge", "An assortment of tools are strewn about the room, likely used to make weapons and armour. ");
                roomMap.Add(room.roomID, room);
            }

            // Room 10
            {
                var room = new Room(10, "X,X,9,11", "Training Room", "Test your skills on the dummies, dummy. ");
                roomMap.Add(room.roomID, room);
            }

            // Room 11
            {
                var room = new Room(11, "X,10,X,X", "Storage Room", "There are lots and lots of boxes. ");
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

                        Room currentRoom = GetPlayerRoom(player);
                        string roomNameText = "You find yourself in the " + currentRoom.name + ". ";
                        string roomDescriptionText = currentRoom.desc;
                        // Get direction options
                        string directions = currentRoom.availableExitsText;
                        msg += roomNameText + roomDescriptionText + directions;
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
