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

        static PlayerDbManager playerDB = new PlayerDbManager();
        static DungeonDbManager dungeonDB = new DungeonDbManager();

        static int clientID = 1;

        // Sends message to socket
        static void SendMessageToSocket(Socket s, Msg message)
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

            // Create and send login success message
            LoginStateMsg successMsg = new LoginStateMsg();
            successMsg.type = messageType;
            successMsg.msg = loginSuccess;
            SendLoginStateMsg(s, successMsg);

            // Send player list of characters they own
            return playerDB.GetPlayerCharacters(userID);
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
                socketManager.AddClientToLoggedInSockets(userID, s);

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
                    playerDB.CreateNewCharacter(characterName, userID);

                    SendGlobalChatMessage(characterName + " has just rejoined the dungeon.");
                }

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

        // TODO remove????
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
            foreach (KeyValuePair<int, Socket> s in socketManager.loggedInSockets)
            {
                s.Value.Send(outStream.GetBuffer());
            }
        }
        
        // Sends list of logged in players
        static void SendClientList()
        {
            ClientListMsg clientListMsg = new ClientListMsg();

            lock (socketManager.loggedInSockets)
            {
                foreach (KeyValuePair<int, Socket> s in socketManager.loggedInSockets)
                {
                    // Get string username from query using s.Key and add
                    string playerUserName = playerDB.GetPlayerCharacters(s.Key)[0];
                    
                    // Add user to list
                    clientListMsg.clientList.Add(playerUserName);
                }

                SendToAllLoggedInPlayers(clientListMsg);
            }
        }

        // Sends list of playable characters
        static void SendCharacterList(Socket s, int playerID)
        {
            CharacterListMsg characterListMsg = new CharacterListMsg();

            characterListMsg.characterList = playerDB.GetPlayerCharacters(playerID);

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

        static void SendRoomChatMessage(String msg, int playerID)
        {
            Console.WriteLine("Sending message to everyone in room.");

            // TODO PlayerDB SQL query with current playerID to get room ID
            int targetRoomID = 0;


            // TODO Use targetRoomID to get all players with same roomID

            // Create chat message
            // TODO Use query with room ID to get name
            string roomName = "";

            PublicChatMsg chatMsg = new PublicChatMsg();
            chatMsg.msg = roomName + ": " + msg;
            MemoryStream outStream = chatMsg.WriteData();

            // Send the message to each player
            //Socket playerSocket;
            //playerSocket.Value.socket.Send(outStream.GetBuffer());

        }

        // Returns text for current character room
        static String GetRoomTextFromCharacterName(String characterName)
        {
            // Get room ID from characterName
            int roomID = playerDB.GetCharacterRoom(characterName);

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
            int characterID = 0;

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
                                            userID = playerDB.LoginUser(loginDetails.username, loginDetails.password);

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
                                            bool userExists = playerDB.CheckForExistingUsername(loginDetails.username);

                                            if (!userExists)
                                            {
                                                // Create user in database
                                                userID = playerDB.CreateNewUser(loginDetails.username, loginDetails.password);

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

                                        if(playerDB.DoesUserOwnCharacter(userID, characterSelection.msg))
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

                                        if (playerDB.CheckForExistingCharacterName(characterCreation.characterName))
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
                                                SendToAllLoggedInPlayers(messageOutput);
                                            }
                                        }
                                        break;

                                    // Private messages
                                    case PrivateChatMsg.ID:
                                        {
                                            PrivateChatMsg privateMsg = (PrivateChatMsg)m;
                                            String formattedMsg = characterName + ": " + privateMsg.msg;
                                            Console.WriteLine("private chat - " + formattedMsg + "to " + privateMsg.destination);

                                            //SendPrivateMessage(GetSocketFromCharacterName(privateMsg.destination), playerID, formattedMsg);
                                            formattedMsg = "Sent private message to " + privateMsg.destination + ": " + privateMsg.msg;
                                            //SendPrivateMessage(chatClient, "", formattedMsg);
                                        }
                                        break;

                                    // Navigation messages
                                    case DungeonNavigationMsg.ID:
                                        {
                                            {
                                                DungeonNavigationMsg navigationMsg = (DungeonNavigationMsg)m;

                                                // Respond to the player's navigation movement
                                                //String formattedMsg = dungeon.ParsePlayerInput(thisPlayer, navigationMsg.msg);

                                                //Console.WriteLine("Navigation - " + formattedMsg);

                                                // Send response
                                                //SendPrivateMessage(chatClient, "", formattedMsg);

                                                String roomText = GetRoomTextFromCharacterName(characterName);
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
                        socketManager.RemovePlayerByID(userID);
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
