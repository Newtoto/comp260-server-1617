using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{

    class Program
    {
        static bool quit = false;
        static LinkedList<Message> incommingMessages = new LinkedList<Message>();

        class ReceiveThreadLaunchInfo
        {
            public ReceiveThreadLaunchInfo(int ID, Socket socket)
            {
                this.ID = ID;
                this.socket = socket;
            }

            public int ID;
            public Socket socket;
        }

        static void acceptClientThread(Object obj)
        {
            Socket s = obj as Socket;

            int ID = 0;

            while (quit == false)
            {
                var newClientSocket = s.Accept();

                var myThread = new Thread(clientReceiveThread);
                myThread.Start(new ReceiveThreadLaunchInfo(ID, newClientSocket));

                lock (incommingMessages)
                {
                    incommingMessages.AddLast(new Message(ID, newClientSocket, "New client connected"));
                }

                // Create player and add to list
                Player player = new Player(ID, newClientSocket);

                // Send client welcome message
                String Msg = "Greetings traveller, my name is Falconhoof and I will be your guide." + "\nState your name.";
                ASCIIEncoding sendEncoder = new ASCIIEncoding();
                byte[] sendBuffer = sendEncoder.GetBytes(Msg);

                try
                {
                    // Send greeting message to client
                    int bytesSent = newClientSocket.Send(sendBuffer);
                }
                catch (System.Exception ex)
                {
                    Console.Write(ex);
                }

                ID++;
            }
        }

        static void clientReceiveThread(Object obj)
        {
            ReceiveThreadLaunchInfo receiveInfo = obj as ReceiveThreadLaunchInfo;
            bool socketLost = false;

            while ((quit == false) && (socketLost == false))
            {
                byte[] buffer = new byte[4094];

                try
                {
                    int result = receiveInfo.socket.Receive(buffer);

                    if (result > 0)
                    {
                        ASCIIEncoding encoder = new ASCIIEncoding();

                        lock (incommingMessages)
                        {
                            Message msg = new Message(receiveInfo.ID, receiveInfo.socket, encoder.GetString(buffer, 0, result));
                            incommingMessages.AddLast(msg);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                    socketLost = true;
                }
            }
        }

        static Socket getSocketFromID(int playerID, Player [] players)
        {
            return players[playerID].userSocket;
        }


        static void Main(string[] args)
        {
            // Create and initialise the dungeon once
            var dungeon = new Dungeon();
            dungeon.Init();

            // Create player list
            Player[] players;
            

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipLocal = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8221);

            s.Bind(ipLocal);
            s.Listen(4);

            Console.WriteLine("Waiting for client ...");

            var myThread = new Thread(acceptClientThread);
            myThread.Start(s);


            int tick = 0;
            int itemsProcessed = 0;
            while (true)
            {
                Message userMessage = new Message(0, s, "");

                lock (incommingMessages)
                {
                    if (incommingMessages.First != null)
                    {
                        // Get message info from first item in message array
                        userMessage = incommingMessages.First.Value;

                        // Remove read message
                        incommingMessages.RemoveFirst();

                        itemsProcessed++;
                    }
                }

                if (userMessage.message != "")
                {
                    Console.WriteLine(userMessage.userID.ToString() + "   "+ tick + ":" + itemsProcessed + " " + userMessage.message);

                    // Send client welcome message
                    String Msg = "Greetings traveller, my name is Falconhoof and I will be your guide." + "\nState your name.";
                    ASCIIEncoding sendEncoder = new ASCIIEncoding();
                    byte[] sendBuffer = sendEncoder.GetBytes(Msg);

                    try
                    {
                        // Send greeting message to client
                        int bytesSent = userMessage.socket.Send(sendBuffer);
                    }
                    catch (System.Exception ex)
                    {
                        Console.Write(ex);
                    }
                }

                tick++;

                Thread.Sleep(1);
            }
        }
    }
}
