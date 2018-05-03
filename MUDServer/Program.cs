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
        static int clientID = 1;

        // Uses socket connected to find ID of player
        static int GetPlayerIDFromSocket(Socket socket)
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
        static void RemoveClientByID(int playerID)
        {
            Console.WriteLine("Removing client " + playerID);
            lock (playerSockets)
            {
                playerSockets.Remove(playerID);
            }
        }

        // Catch all send message to socket
        static void SendLoginSuccessMsg(Socket s, LoginSuccessMsg message)
        {
            MemoryStream outStream = message.WriteData();

            s.Send(outStream.GetBuffer());
        }

        static void SendClientID(Socket s, int playerID)
        {
            ClientNameMsg nameMsg = new ClientNameMsg();

            // Get message name from sql query
            nameMsg.name = playerID.ToString();

            MemoryStream outStream = nameMsg.WriteData();

            s.Send(outStream.GetBuffer() );
        }


        static void SendClientList()
        {
            ClientListMsg clientListMsg = new ClientListMsg();

            lock (playerSockets)
            {
                foreach (KeyValuePair<int, Socket> s in playerSockets)
                {
                    // Get string username from query using s.Key and add
                    string playerUserName = "temp";
                    clientListMsg.clientList.Add(playerUserName);
                }

                MemoryStream outStream = clientListMsg.WriteData();

                foreach (KeyValuePair<int, Socket> s in playerSockets)
                {
                    s.Value.Send(outStream.GetBuffer());
                }
            }
        }

        // Send message to all users in the same room
        static void SendGlobalChatMessage(String msg)
        {
            Console.WriteLine("Sending message to everyone");

            // Create the message
            PublicChatMsg chatMsg = new PublicChatMsg();
            chatMsg.msg = msg;
            MemoryStream outStream = chatMsg.WriteData();

            // TODO Send the message to player with ID
            //Socket playerSocket;
            //playerSocket.Value.socket.Send(outStream.GetBuffer());

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
            int playerID = GetPlayerIDFromSocket(chatClient);

            Console.WriteLine("client receive thread for client " + playerID);

            SendClientList();

            // Used for login
            bool loggedIn = false;

            // Sleep to make sure the client is ready to receive message
            Thread.Sleep(20);

            PlayerDbManager playerDb = new PlayerDbManager();
            DungeonDbManager dungeonDb = new DungeonDbManager();

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

                                            if (playerDb.LoginUser(loginDetails.username, loginDetails.password) > 0)
                                            {
                                                Console.WriteLine("Login success");

                                                // Create success message
                                                LoginSuccessMsg successMsg = new LoginSuccessMsg();
                                                successMsg.msg = "success";

                                                SendLoginSuccessMsg(chatClient, successMsg);

                                                loggedIn = true;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Login failed");

                                                // Create fail message
                                                LoginSuccessMsg failMsg = new LoginSuccessMsg();
                                                failMsg.msg = "failed";
                                                SendLoginSuccessMsg(chatClient, failMsg);
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

                                            if (playerDb.CreateUser(loginDetails.username, loginDetails.password) > 0)
                                            {
                                                loggedIn = true;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Sign up failed");
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            if (m != null)
                            {
                                Console.Write("Got a message: ");
                                switch (m.mID)
                                {
                                    // Public messages
                                    case PublicChatMsg.ID:
                                        {
                                            PublicChatMsg publicMsg = (PublicChatMsg)m;
                                            String formattedMsg = "<" + playerID + "> " + publicMsg.msg;
                                            Console.WriteLine("public chat - " + formattedMsg);
                                            //SendRoomChatMessage(formattedMsg, dungeon.GetPlayerRoom(thisPlayer));
                                        }
                                        break;

                                    // Private messages
                                    case PrivateChatMsg.ID:
                                        {
                                            PrivateChatMsg privateMsg = (PrivateChatMsg)m;
                                            String formattedMsg = "Private message from player " + playerID + ": " + privateMsg.msg;
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
                    String output = "Lost client: " + playerID;
                    Console.WriteLine(output);
                    //SendChatMessage(output);
                    Console.WriteLine(e);

                    RemoveClientByID(playerID);

                    SendClientList();
                }
            }
        }

        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			// Server IP 138.68.161.95, test 127.0.0.1
			serverSocket.Bind(new IPEndPoint(IPAddress.Parse("138.68.161.95"), 8500));
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

                    SendClientID(serverClient, clientID);

                    Thread.Sleep(500);
                    SendClientList();

                    clientID++;
                }
                
            }
        }
    }
}
