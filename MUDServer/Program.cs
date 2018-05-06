using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using MessageTypes;

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
    class Program
    {
        static SocketManager socketManager = new SocketManager();

        static DatabaseController databaseController = new DatabaseController();
        static DungeonDBAccess dungeonDB = databaseController.dungeonDB;
        static UserDBAccess userDB = databaseController.userDB;
        static CharacterDBAccess characterDB = databaseController.characterDB;


        static int clientID = 1;

        // Sends message to socket
        static void SendMessageToSocket(Socket s, Msg message)
        {
            MemoryStream outStream = message.WriteData();

            s.Send(outStream.GetBuffer());
        }

        // Sends private message to socket
        static void SendPrivateMessageToSocket(Socket s, PrivateChatMsg message)
        {
            MemoryStream outStream = message.WriteData();

            s.Send(outStream.GetBuffer());
        }

        // Log in / sign up user process
        static List<String> SendLogInStateToUser(Socket s, String newTitle, String messageType, String loginSuccess, int userID)
        {
            Console.WriteLine("Login " + loginSuccess);

            // Update title for player
            SendClientID(s, newTitle);

			Thread.Sleep(500);

			// Create and send login success message
            LoginStateMsg successMsg = new LoginStateMsg();
            successMsg.type = messageType;
            successMsg.msg = loginSuccess;
            SendLoginStateMsg(s, successMsg);

            // Send player list of characters they own
            return characterDB.GetPlayerCharacters(userID);
        }

        // Character select / create user process
        static void SendCharacterSelectStateToUser(Socket s, String characterName, String messageType, String selectSuccess, int userID)
        {
            // Title displayed on the client's window
            SendClientID(s, characterName);

            // Create and send sign up success message
            LoginStateMsg playerSelectedMsg = new LoginStateMsg();
            playerSelectedMsg.type = messageType;
            playerSelectedMsg.msg = selectSuccess;
            SendLoginStateMsg(s, playerSelectedMsg);

            // Successful selection / creation
            if(selectSuccess == "success")
            {
                // Add the client to logged in socket dictionary
                socketManager.AddClientToLoggedInSockets(userID, s, characterName);

                // Allow previous message to go through
                Thread.Sleep(500);

                // Rejoining character
                if(messageType == "select")
                {
                    SendGlobalChatMessage(characterName + " has just rejoined the dungeon.");
                }
                // First time joining
                else
                {
                    // Create character in database
                    characterDB.CreateNewCharacter(characterName, userID);

                    SendGlobalChatMessage(characterName + " has joined the dungeon, give them a warm welcome!");
                }
				// Allow previous message to go through
                Thread.Sleep(500);

                // Send list of logged in character names
                SendClientList();

                // Allow previous message to go through
                Thread.Sleep(500);

                // Create and send room text
                PublicChatMsg roomText = new PublicChatMsg();
                roomText.msg = GetRoomTextFromCharacterName(characterName);
                SendMessageToSocket(s, roomText);
            }
        }

        // Tells user if login or signup was successful
        static void SendLoginStateMsg(Socket s, LoginStateMsg message)
        {
            MemoryStream outStream = message.WriteData();

            s.Send(outStream.GetBuffer());
        }

        // Sends username to client to update window title
        static void SendClientID(Socket s, string playerID)
        {
            ClientNameMsg nameMsg = new ClientNameMsg();

            // Get message name from sql query
            nameMsg.name = playerID.ToString();

            MemoryStream outStream = nameMsg.WriteData();

            s.Send(outStream.GetBuffer() );
        }

        // Send message to all logged in players
        static void SendToAllLoggedInPlayers(Msg message)
        {
            Console.WriteLine("Sending to all logged in players");

            // Send list to all logged in players
            MemoryStream outStream = message.WriteData();
            lock(socketManager.loggedInSockets)
            {
                foreach (KeyValuePair<int, Socket> s in socketManager.loggedInSockets)
                {
                    s.Value.Send(outStream.GetBuffer());
                }
            }
        }

        // Send message to all players in the same room as the character
        static void SendRoomChatMessage(PublicChatMsg message, string characterName)
        {
            // Get room from characterName
            int roomID = characterDB.GetCharacterRoom(characterName);
            string roomName = dungeonDB.GetRoomNameFromID(roomID);
            message.msg = roomName + " " + message.msg;

            List<string> charactersInRoom = characterDB.GetCharactersInRoom(roomID);
            List<Socket> targetSockets = new List<Socket>();

            foreach (string name in charactersInRoom)
            {
                targetSockets.Add(socketManager.GetSocketFromCharacterName(name));
            }

            Console.WriteLine("Sending to all players in " + roomName);

            // Send list to all characters in room
            //SendToSocketList(message, targetSockets);
            MemoryStream outStream = message.WriteData();

            foreach (Socket s in targetSockets)
            {
                s.Send(outStream.GetBuffer());
            }
        }

        static void SendToSocketList(Msg message, List<Socket> socketList)
        {
            MemoryStream outStream = message.WriteData();

            foreach (Socket s in socketList)
            {
                s.Send(outStream.GetBuffer());
            }
        }

        // Sends list of logged in players
        static void SendClientList()
        {
            ClientListMsg clientListMsg = new ClientListMsg();

            lock (socketManager.socketToCharacterName)
            {
                foreach (KeyValuePair<Socket, string> s in socketManager.socketToCharacterName)
                {

                    // Add user to list
                    clientListMsg.clientList.Add(s.Value);
                }

                SendToAllLoggedInPlayers(clientListMsg);
            }
        }

        // Sends list of playable characters
        static void SendCharacterList(Socket s, int playerID)
        {
            CharacterListMsg characterListMsg = new CharacterListMsg();

            characterListMsg.characterList = characterDB.GetPlayerCharacters(playerID);

            SendMessageToSocket(s, characterListMsg);
        }

        // Send chat message to all users
        static void SendGlobalChatMessage(String msg)
        {
            Console.WriteLine("Sending message to everyone");

            // Create the message
            PublicChatMsg chatMsg = new PublicChatMsg();
            chatMsg.msg = msg;
            MemoryStream outStream = chatMsg.WriteData();

            SendToAllLoggedInPlayers(chatMsg);

        }

        // Returns text for current character room
        static String GetRoomTextFromCharacterName(String characterName)
        {
            // Get room ID from characterName
            int roomID = characterDB.GetCharacterRoom(characterName);

            return dungeonDB.GetRoomText(roomID);
        }

        static void receiveClientProcess(Object o)
        {
            // Create and initialise dungoen for player
            //Dungeon dungeon = new Dungeon();
            //dungeon.Init();

            bool bQuit = false;

            Socket chatClient = (Socket)o;
            int clientID = socketManager.GetClientIDFromSocket(chatClient);
            int userID = 0;

            string characterName = "";
            List<String> ownedCharacters = new List<String>();

            Console.WriteLine("client receive thread for client " + clientID);

            // Used for login and player creation
            bool loggedIn = false;
            bool playerChosen = false;

            // Sleep to make sure the client is ready to receive message
            Thread.Sleep(20);

            while (bQuit == false)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = chatClient.Receive(buffer);

                    if (result > 0)
                    {
                        MemoryStream stream = new MemoryStream(buffer);
                        BinaryReader read = new BinaryReader(stream);

                        Msg m = Msg.DecodeStream(read);

                        // Log in / sign up screen
                        if (!loggedIn)
                        {
                            switch (m.mID)
                            {
                                // Login Attempt
                                case LoginAttempt.ID:
                                    {
                                        {
                                            LoginAttempt loginDetails = (LoginAttempt)m;

                                            Console.WriteLine("Logging in user " + loginDetails.username);

                                            // Update userID using database
                                            userID = userDB.LoginUser(loginDetails.username, loginDetails.password);

                                            // Successful login
                                            if (userID > 0)
                                            {
                                                // Update list of available characters, and send login success info to player
                                                ownedCharacters = SendLogInStateToUser(chatClient, "Character Selection", "login", "success", userID);
                                                SendCharacterList(chatClient, userID);

                                                // Break out of this message receive section
                                                loggedIn = true;
                                            }
                                            // Unsuccessful login
                                            else
                                            {
                                                // Update list of available characters, and send login failure info to player
                                                ownedCharacters = SendLogInStateToUser(chatClient, "Log into your master account", "login", "failed", userID);
                                            }
                                        }
                                    }
                                    break;
                                // Sign up Attempt
                                case SignUpAttempt.ID:
                                    {
                                        {
                                            SignUpAttempt loginDetails = (SignUpAttempt)m;

                                            Console.WriteLine("Signing up user " + loginDetails.username);

                                            // Check if username is in use
                                            bool userExists = userDB.CheckForExistingUsername(loginDetails.username);

                                            if (!userExists)
                                            {
                                                // Create user in database
                                                userID = userDB.CreateNewUser(loginDetails.username, loginDetails.password);

                                                // Update list of available characters, and send sign up success info to player
                                                ownedCharacters = SendLogInStateToUser(chatClient, "Create Your First Character", "signup", "success", userID);

                                                // Log in user
                                                loggedIn = true;
                                            }
                                            else
                                            {
                                                // Update list of available characters, and send sign up failure info to player
                                                ownedCharacters = SendLogInStateToUser(chatClient, "Log into your master account", "signup", "fail", userID);
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        // Player selection screen
                        else if (!playerChosen)
                        {
                            Console.WriteLine("Player not chosen");
                            switch (m.mID)
                            {
                                // select existing player
                                case CharacterSelectionMsg.ID:
                                    {
                                        // Log in player with this character
                                        CharacterSelectionMsg characterSelection = (CharacterSelectionMsg)m;
                                        Console.WriteLine("Player selected: " + characterSelection.msg);

                                        List<String> characterList = databaseController.characterDB.GetPlayerCharacters(userID);

                                        if (characterDB.DoesUserOwnCharacter(userID, characterSelection.msg))
                                        {
                                            // Update variables for this user
                                            characterName = characterSelection.msg;

                                            SendCharacterSelectStateToUser(chatClient, characterName, "select", "success", userID);

                                            // Break out of this message recieve section
                                            playerChosen = true;
                                        }
                                        else
                                        {
                                            // Create and send sign up failure message
                                            SendCharacterSelectStateToUser(chatClient, characterName, "select", "fail", userID);
                                        }
                                    }
                                    break;
                                // create new player
                                case CharacterCreationMsg.ID:
                                    {
                                        CharacterCreationMsg characterCreation = (CharacterCreationMsg)m;

                                        Console.WriteLine("Creating player with name: " + characterCreation.characterName);

                                        if (characterDB.CheckForExistingCharacterName(characterCreation.characterName))
                                        {
                                            // Create and send player create fail message
                                            SendCharacterSelectStateToUser(chatClient, characterName, "create", "fail", userID);
                                        }
                                        else
                                        {
                                            // Update player name
                                            characterName = characterCreation.characterName;

                                            SendCharacterSelectStateToUser(chatClient, characterName, "create", "success", userID);

                                            playerChosen = true;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        // MUD Screen
                        else
                        {
                            if (m != null)
                            {
                                switch (m.mID)
                                {
                                    // Public messages
                                    case PublicChatMsg.ID:
                                        {
                                            PublicChatMsg publicMsg = (PublicChatMsg)m;

                                            // Split the type of public message from the text
                                            String [] parsedMessage = publicMsg.msg.Split(':');
                                            String messageType = parsedMessage[0];
                                            String messageText = parsedMessage[1];

                                            String formattedMsg = characterName + ": " + messageText;

                                            // Send to all players
                                            if(messageType == "global")
                                            {
                                                PublicChatMsg messageOutput = new PublicChatMsg();
                                                messageOutput.msg = "Global Chat from " + formattedMsg;
                                                SendToAllLoggedInPlayers(messageOutput);
                                            }
                                            // Send to players in room
                                            else
                                            {
                                                PublicChatMsg messageOutput = new PublicChatMsg();
                                                messageOutput.msg = "Room Chat from " + formattedMsg;
                                                // Change to room only
                                                SendRoomChatMessage(messageOutput, characterName);
                                            }
                                        }
                                        break;

                                    // Private messages
                                    case PrivateChatMsg.ID:
                                        {
                                            PrivateChatMsg privateMsg = (PrivateChatMsg)m;

                                            Console.WriteLine("Sending private message from " + characterName + " to " + privateMsg.destination);

											Socket targetSocket = socketManager.GetSocketFromCharacterName(privateMsg.destination);
                                            
											// Send feedback to original sender
                                            privateMsg.msg = "You to " + privateMsg.destination + ": " + privateMsg.msg;
                                            SendMessageToSocket(chatClient, privateMsg);

                                            // Send message to target
                                            privateMsg.msg = "Private message from " + characterName + ": " + privateMsg.msg;

                                            SendMessageToSocket(targetSocket, privateMsg);
                                            
										}
                                        break;

                                    // Navigation messages
                                    case DungeonNavigationMsg.ID:
                                        {
                                            {
                                                DungeonNavigationMsg navigationMsg = (DungeonNavigationMsg)m;

                                                // Initialise response message
                                                PublicChatMsg serverResponse = new PublicChatMsg();

                                                int currentRoom = characterDB.GetCharacterRoom(characterName);
                                                int newRoomID = dungeonDB.GetRoomExit(navigationMsg.msg, currentRoom);

                                                // Successful room change
                                                if(newRoomID > 0)
                                                {
                                                    Console.WriteLine("Moved " + characterName + "to room with ID: " + newRoomID);
                                                    characterDB.MoveCharacterLocation(characterName, newRoomID);
                                                }
                                                // Exit not available
                                                else
                                                {
                                                    Console.WriteLine(characterName + " movement failed, the was is closed");

                                                    // Create and send exit blocked message
                                                    serverResponse.msg = "You cannot go " + navigationMsg.msg + ".";
                                                    SendMessageToSocket(chatClient, serverResponse);

                                                    // Allow failed navigation message to send
                                                    Thread.Sleep(500);
                                                }


                                                // Send room text to client
                                                serverResponse.msg = GetRoomTextFromCharacterName(characterName);
                                                SendMessageToSocket(chatClient, serverResponse);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }                   
                }
                catch (Exception e)
                {
                    bQuit = true;
                    String output = "Lost client: " + clientID;
                    Console.WriteLine(output);
                    //SendChatMessage(output);
                    Console.WriteLine(e);

                    // Remove this client's socket from dictionaries
                    socketManager.RemoveClientByID(clientID);
                    if(userID > 0)
                    {
                        socketManager.RemovePlayerByID(userID, chatClient, characterName);
                    }

                    // Update client lists for other players
                    SendClientList();
                }
            }
        }

        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Server IP 138.68.161.95, test 127.0.0.1
			serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
            serverSocket.Listen(32);

            bool bQuit = false;

            Console.WriteLine("Server");

            while (!bQuit)
            {
                Socket serverClient = serverSocket.Accept();

                Thread myThread = new Thread(receiveClientProcess);
                myThread.Start(serverClient);

                socketManager.AddClientToAllSockets(clientID, serverClient);


                SendClientID(serverClient, "Log into your master account");

                Thread.Sleep(500);

                clientID++;

                
            }
        }
    }
}
