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
                ReceiveThreadLaunchInfo receiveInfo = obj as ReceiveThreadLaunchInfo;

                myThread.Start(new ReceiveThreadLaunchInfo(ID, newClientSocket));

                Console.WriteLine("New client");

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
                            Message msg = new Message(receiveInfo.socket, receiveInfo.ID+ ":" + encoder.GetString(buffer, 0, result));
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

        static void addPlayerToArray(Player[]players, int userID,String userName)
        {
            players[userID] = new Player(userID, userName);
        }

        static void Main(string[] args)
        {
            // Create playerlist
            List<Player> players = new List<Player>();

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

                Message userMessage = new Message(s, "");

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

                if (userMessage.messageText != "")
                {
                    string[] receivedMessage = userMessage.messageText.Split(new Char[] { ':' });

                    string userID = receivedMessage[0];
                    string numberOfMessages = receivedMessage[1];
                    string userInputString = receivedMessage[2];

                    String Msg = "";

                    // Set username on first message
                    if (Int32.Parse(numberOfMessages) == 0)
                    {
                        // Add player to list
                        players.Add(new Player(0, userInputString));

                        Console.WriteLine("Creating player");
                        Msg = "Hello " + players[Int32.Parse(userID)].userName;

                    }
                    else
                    {
                        // What to do on later messages
                        Msg = "Hello " + players[Int32.Parse(userID)].userName;
                    }

                    Console.WriteLine(userInputString + " " + numberOfMessages + " " + userID);

                    // Parse user input and do action here
                    ASCIIEncoding sendEncoder = new ASCIIEncoding();
                    byte[] sendBuffer = sendEncoder.GetBytes(Msg);

                    try
                    {
                        // Send return message
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
