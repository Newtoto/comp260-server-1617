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
        static Dictionary<int, Socket> playerSockets = new Dictionary<int, Socket>();
        static Dictionary<int, Socket> loggedInSockets = new Dictionary<int, Socket>();

        static PlayerDbManager playerDb = new PlayerDbManager();
        static DungeonDbManager dungeonDb = new DungeonDbManager();

        static int clientID = 1;

        // Sends message to socket
        static void SendMessageToSocket(Socket s, Msg message)
        {
            MemoryStream outStream = message.WriteData();

            s.Send(outStream.GetBuffer());
        }

        // Uses socket connected to find ID of player
        static int GetClientIDFromSocket(Socket socket)
        {
            foreach (KeyValuePair<int, Socket> i in playerSockets)
            {
                if(playerSockets[i.Key] == socket)
                {
                    return i.Key;
                }
            }

            return 0;
        }

        // Remove client from dictionary
        static void RemoveClientByID(int clientID)
        {
            Console.WriteLine("Removing client " + clientID);
            lock (playerSockets)
            {
                playerSockets.Remove(clientID);
            }
        }

        // Remove player from dictionary
        static void RemovePlayerByID(int playerID)
        {
            Console.WriteLine("Removing player " + playerID);
            lock (loggedInSockets)
            {
                try
                {
                    loggedInSockets.Remove(playerID);
                }
                catch
                {
                    // Player wasn't logged in
                }
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
            foreach (KeyValuePair<int, Socket> s in loggedInSockets)
            {
                s.Value.Send(outStream.GetBuffer());
            }
        }
        
        // Sends list of logged in players
        static void SendClientList()
        {
            ClientListMsg clientListMsg = new ClientListMsg();

            lock (loggedInSockets)
            {
                foreach (KeyValuePair<int, Socket> s in loggedInSockets)
                {
                    // Get string username from query using s.Key and add
                    string playerUserName = playerDb.GetPlayerCharacters(s.Key)[0];
                    
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

            characterListMsg.characterList = playerDb.GetPlayerCharacters(playerID);

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

        static void WelcomeNewClient(int playerID)
        {

        }

        static void receiveClientProcess(Object o)
        {
            // Create and initialise dungoen for player
            //Dungeon dungeon = new Dungeon();
            //dungeon.Init();

            bool bQuit = false;

            Socket chatClient = (Socket)o;
            int clientID = GetClientIDFromSocket(chatClient);
            int playerID = 0;
            string playerName = "";
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

                                            Console.WriteLine("Logging in user " + loginDetails.username + " and password " + loginDetails.password);

                                            // Get player ID from database using username
                                            playerID = playerDb.LoginUser(loginDetails.username, loginDetails.password);

                                            if (playerID > 0)
                                            {
                                                Console.WriteLine("Login success");

                                                // Update title for player
                                                SendClientID(chatClient, "Character Selection");

                                                // Create and send login success message
                                                LoginStateMsg successMsg = new LoginStateMsg();
                                                successMsg.type = "login";
                                                successMsg.msg = "success";
                                                SendLoginStateMsg(chatClient, successMsg);

                                                // Send player list of characters they own
                                                ownedCharacters = playerDb.GetPlayerCharacters(playerID);
                                                SendCharacterList(chatClient, playerID);

                                                loggedIn = true;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Login failed");

                                                // Create and send login fail message
                                                LoginStateMsg failMsg = new LoginStateMsg();
                                                failMsg.type = "login";
                                                failMsg.msg = "failed";
                                                SendLoginStateMsg(chatClient, failMsg);
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
                                            bool userExists = playerDb.CheckForExistingUsername(loginDetails.username);

                                            if (!userExists)
                                            {
                                                Console.WriteLine("Sign up success");

                                                // Create user in database
                                                playerID = playerDb.CreateNewUser(loginDetails.username, loginDetails.password);

                                                // Log in user
                                                loggedIn = true;

                                                // Update title for player
                                                SendClientID(chatClient, "Create Your First Character");

                                                // Create and send sign up success message
                                                LoginStateMsg signUpStateMsg = new LoginStateMsg();
                                                signUpStateMsg.type = "signup";
                                                signUpStateMsg.msg = "success";
                                                SendLoginStateMsg(chatClient, signUpStateMsg);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Sign up failed");

                                                // Create and send failed sign up message
                                                LoginStateMsg signUpStateMsg = new LoginStateMsg();
                                                signUpStateMsg.type = "signup";
                                                signUpStateMsg.msg = "failed";
                                                SendLoginStateMsg(chatClient, signUpStateMsg);
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

                                        if(playerDb.DoesUserOwnCharacter(playerID, characterSelection.msg))
                                        {
                                            playerName = characterSelection.msg;

                                            // Title displayed on the client's window
                                            SendClientID(chatClient, playerName);

                                            playerChosen = true;
                                            // Create and send sign up success message
                                            LoginStateMsg playerSelectedMsg = new LoginStateMsg();
                                            playerSelectedMsg.type = "select";
                                            playerSelectedMsg.msg = "success";
                                            SendLoginStateMsg(chatClient, playerSelectedMsg);

                                            lock (loggedInSockets)
                                            {
                                                Console.WriteLine("Added logged in player with id: " + playerID);

                                                // Add new player to logged in socket dictionary
                                                loggedInSockets.Add(playerID, chatClient);

                                                Thread.Sleep(500);
                                                SendClientList();
                                                SendGlobalChatMessage(playerName + " has just rejoined the dungeon.");
                                            }
                                        }
                                        else
                                        {
                                            // Create and send sign up success message
                                            LoginStateMsg playerSelectedMsg = new LoginStateMsg();
                                            playerSelectedMsg.type = "select";
                                            playerSelectedMsg.msg = "fail";
                                            SendLoginStateMsg(chatClient, playerSelectedMsg);
                                        }
                                    }
                                    break;
                                // create new player
                                case CharacterCreationMsg.ID:
                                    {
                                        CharacterCreationMsg characterCreation = (CharacterCreationMsg)m;

                                        Console.WriteLine("Creating player with name: " + characterCreation.playerName);

                                        if (playerDb.CheckForExistingPlayerName(characterCreation.playerName))
                                        {
                                            // Create and send sign up success message
                                            LoginStateMsg playerCreateMsg = new LoginStateMsg();
                                            playerCreateMsg.type = "create";
                                            playerCreateMsg.msg = "fail";
                                            SendLoginStateMsg(chatClient, playerCreateMsg);
                                        }
                                        else
                                        {
                                            // Create character in database
                                            playerDb.CreateNewCharacter(characterCreation.playerName, playerID);

                                            // Update player name
                                            playerName = characterCreation.playerName;

                                            // Title displayed on the client's window
                                            SendClientID(chatClient, playerName);

                                            playerChosen = true;

                                            // Create and send sign up success message
                                            LoginStateMsg playerCreateMsg = new LoginStateMsg();
                                            playerCreateMsg.type = "create";
                                            playerCreateMsg.msg = "success";
                                            SendLoginStateMsg(chatClient, playerCreateMsg);

                                            lock (loggedInSockets)
                                            {
                                                Console.WriteLine("Added logged in player with id: " + playerID);

                                                // Add new player to logged in socket dictionary
                                                loggedInSockets.Add(playerID, chatClient);

                                                Thread.Sleep(500);
                                                SendClientList();
                                                SendGlobalChatMessage(playerName + " has just joined the dungeon, give them a warm welcome!");
                                            }
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
                                            String formattedMsg = "<" + clientID + "> " + publicMsg.msg;
                                            Console.WriteLine("public chat - " + formattedMsg);
                                            //SendRoomChatMessage(formattedMsg, dungeon.GetPlayerRoom(thisPlayer));
                                        }
                                        break;

                                    // Private messages
                                    case PrivateChatMsg.ID:
                                        {
                                            PrivateChatMsg privateMsg = (PrivateChatMsg)m;
                                            String formattedMsg = "Private message from player " + clientID + ": " + privateMsg.msg;
                                            Console.WriteLine("private chat - " + formattedMsg + "to " + privateMsg.destination);
                                            //SendPrivateMessage(GetSocketFromPlayerName(privateMsg.destination), playerID, formattedMsg);
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
                    RemoveClientByID(clientID);
                    if(playerID > 0)
                    {
                        RemovePlayerByID(playerID);
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

                lock (playerSockets)
                {
                    // Add new player to socket dictionary
                    playerSockets.Add(clientID, serverClient);

                    SendClientID(serverClient, "Log into your master account");

                    Thread.Sleep(500);

                    clientID++;
                }
                
            }
        }
    }
}
