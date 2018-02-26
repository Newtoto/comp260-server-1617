using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using MessageTypes;


namespace Server
{
    class Program
    {
        static Dictionary<String, Player> clientDictionary = new Dictionary<String, Player>();
        static int clientID = 1;


        static void SendClientName(Socket s, String clientName)
        {
            ClientNameMsg nameMsg = new ClientNameMsg();
            nameMsg.name = clientDictionary[clientName].userName;

            MemoryStream outStream = nameMsg.WriteData();

            s.Send(outStream.GetBuffer() );
        }

        static void SendClientList()
        {
            ClientListMsg clientListMsg = new ClientListMsg();

            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Player> s in clientDictionary)
                {
                    clientListMsg.clientList.Add(clientDictionary[s.Key].userName);
                }

                MemoryStream outStream = clientListMsg.WriteData();

                foreach (KeyValuePair<String, Player> s in clientDictionary)
                {
                    s.Value.socket.Send(outStream.GetBuffer());
                }
            }
        }

        static void SendChatMessage(String msg)
        {
            PublicChatMsg chatMsg = new PublicChatMsg();

            chatMsg.msg = msg;

            MemoryStream outStream = chatMsg.WriteData();

            lock (clientDictionary)
            {            
                foreach (KeyValuePair<String, Player> s in clientDictionary)
                {
                    try
                    {
                        s.Value.socket.Send(outStream.GetBuffer());
                    }
                    catch (System.Exception)
                    {
                    	
                    }                    
                }
            }
        }

        static void SendRoomChatMessage(String msg, Room room)
        {
            Console.WriteLine("Sending message to everyone in room " + room.name);

            PublicChatMsg chatMsg = new PublicChatMsg();

            chatMsg.msg = room.name + ": " + msg;

            MemoryStream outStream = chatMsg.WriteData();

            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Player> s in clientDictionary)
                {
                    if (s.Value.currentRoomID == room.roomID)
                    {
                        try
                        {
                            s.Value.socket.Send(outStream.GetBuffer());
                        }
                        catch (System.Exception)
                        {

                        }
                    }
                }
            }
        }

        static void SendPrivateMessage(Socket s, String from, String msg)
        {
            PrivateChatMsg chatMsg = new PrivateChatMsg();
            chatMsg.msg = msg;
            chatMsg.destination = from;
            MemoryStream outStream = chatMsg.WriteData();

            try
            {
                s.Send(outStream.GetBuffer());
            }
            catch (System.Exception)
            {

            }
        }

        static Socket GetSocketFromName(String name)
        {
            lock (clientDictionary)
            {
                return clientDictionary[name].socket;
            }
        }

        static Socket GetSocketFromUsername(String username)
        {
            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Player> o in clientDictionary)
                {
                    if (o.Value.userName == username)
                    {
                        return o.Value.socket;
                    }
                }
            }

            return null;
        }

        static bool UsernameTaken(String username)
        {
            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Player> o in clientDictionary)
                {
                    if (o.Value.userName == username)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static String GetNameFromSocket(Socket s)
        {
            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Player> o in clientDictionary)
                {
                    if (o.Value.socket == s)
                    {
                        return o.Key;
                    }
                }
            }

            return null;
        }

        static String GetUsernameFromSocket(Socket s)
        {
            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Player> o in clientDictionary)
                {
                    if (o.Value.socket == s)
                    {
                        return o.Value.userName;
                    }
                }
            }

            return null;
        }

        static String GetUsernameFromID (String userID)
        {
            lock (clientDictionary)
            {
                return clientDictionary[userID].userName;
            }
        }

        static void RemoveClientBySocket(Socket s)
        {
            string name = GetNameFromSocket(s);

            if (name != null)
            {
                lock (clientDictionary)
                {
                    clientDictionary.Remove(name);
                }
            }
        }

        static void WelcomeNewClient(Socket s)
        {
            SendPrivateMessage(s, "", "Greetings traveller, my name is Falconhoof and I will be your guide. " + "State your name.");
        }

        static void SetNameWithSocket(Socket s, String username)
        {
            string oldName = GetNameFromSocket(s);

            lock (clientDictionary)
            {
                // Save player from dictionary to variable
                clientDictionary[oldName].userName = username;
            }
        }

        static void receiveClientProcess(Object o)
        {
            // Create and initialise dungoen for player
            Dungeon dungeon = new Dungeon();
            dungeon.Init();

            bool bQuit = false;

            Socket chatClient = (Socket)o;

            Console.WriteLine("client receive thread for " + GetNameFromSocket(chatClient));

            SendClientList();

            // Used to set username once
            bool usernameSet = false;

            // Sleep to make sure the client is ready to receive message
            Thread.Sleep(20);
            // Introductory text for client
            WelcomeNewClient(chatClient);

            while (bQuit == false)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = chatClient.Receive(buffer);

                    if (result > 0)
                    {
                        // Get player from dictionary
                        Player thisPlayer = clientDictionary[GetNameFromSocket(chatClient)];

                        MemoryStream stream = new MemoryStream(buffer);
                        BinaryReader read = new BinaryReader(stream);

                        Msg m = Msg.DecodeStream(read);

                        if (m != null)
                        {
                            Console.Write("Got a message: ");
                            switch (m.mID)
                            {
                                // Public messages
                                case PublicChatMsg.ID:
                                    {
                                        PublicChatMsg publicMsg = (PublicChatMsg)m;

                                        String formattedMsg = "<" + GetNameFromSocket(chatClient)+"> " + publicMsg.msg;

                                        Console.WriteLine("public chat - " + formattedMsg);

                                        SendRoomChatMessage(formattedMsg, dungeon.GetPlayerRoom(thisPlayer));
                                    }
                                    break;
                                
                                // Private messages
                                case PrivateChatMsg.ID:
                                    {
                                        PrivateChatMsg privateMsg = (PrivateChatMsg)m;

                                        String formattedMsg = "Private message from " + GetUsernameFromSocket(chatClient) + ": " + privateMsg.msg;

                                        Console.WriteLine("private chat - " + formattedMsg + "to " + privateMsg.destination);

                                        SendPrivateMessage(GetSocketFromUsername(privateMsg.destination), GetNameFromSocket(chatClient), formattedMsg);

                                        formattedMsg = "Sent private message to " + privateMsg.destination + ": " + privateMsg.msg;
                                        SendPrivateMessage(chatClient, "", formattedMsg);
                                    }
                                    break;

                                // Navigation messages
                                case DungeonNavigationMsg.ID:
                                    {
                                        if (!usernameSet)
                                        {
                                            // Get message
                                            DungeonNavigationMsg navigationMsg = (DungeonNavigationMsg)m;

                                            // Set username
                                            if (!UsernameTaken(navigationMsg.msg))
                                            {
                                                thisPlayer.userName = navigationMsg.msg;
                                                usernameSet = true;
                                                // Update client list
                                                SendClientList();

                                                // Send username welcome message
                                                String formattedMsg = "Hello " + thisPlayer.userName;
                                                SendPrivateMessage(chatClient, "", formattedMsg);

                                                Thread.Sleep(20);

                                                // Get player's room
                                                Room enteredRoom = dungeon.GetPlayerRoom(thisPlayer);
                                                // Create text based on room
                                                string roomNameText = "You find yourself in the " + enteredRoom.name + ". ";
                                                string roomDescriptionText = enteredRoom.desc;
                                                string directions = enteredRoom.availableExitsText;
                                                formattedMsg = roomNameText + roomDescriptionText + directions;

                                                // Send room based text
                                                SendPrivateMessage(chatClient, "", formattedMsg);
                                            }
                                            else
                                            {
                                                // Request different username
                                                String formattedMsg = "Sorry, but the username '" + navigationMsg.msg + "' is already taken.";
                                                SendPrivateMessage(chatClient, "", formattedMsg);

                                                formattedMsg = "What else can I call you?"
;                                               SendPrivateMessage(chatClient, "", formattedMsg);
                                            }
                                        }
                                        else
                                        {
                                            DungeonNavigationMsg navigationMsg = (DungeonNavigationMsg)m;

                                            // Respond to the player's navigation movement
                                            String formattedMsg = dungeon.ParsePlayerInput(thisPlayer, navigationMsg.msg);

                                            Console.WriteLine("Navigation - " + formattedMsg);

                                            // Send response
                                            SendPrivateMessage(chatClient, "", formattedMsg);
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }                   
                }
                catch (Exception)
                {
                    bQuit = true;

                    String output = "Lost client: " + GetNameFromSocket(chatClient);
                    Console.WriteLine(output);
                    SendChatMessage(output);

                    RemoveClientBySocket(chatClient);

                    SendClientList();
                }
            }
        }

        static void Main(string[] args)
        {

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
            serverSocket.Listen(32);

            bool bQuit = false;

            Console.WriteLine("Server");

            while (!bQuit)
            {
                Socket serverClient = serverSocket.Accept();

                Thread myThread = new Thread(receiveClientProcess);
                myThread.Start(serverClient);

                lock (clientDictionary)
                {
                    String clientName = "client" + clientID;
                    Player thisPlayer = new Player(clientID, clientName, serverClient);
                    clientDictionary.Add(clientName, thisPlayer);

                    SendClientName(serverClient, clientName);
                    Thread.Sleep(500);
                    SendClientList();

                    clientID++;
                }
                
            }
        }
    }
}
