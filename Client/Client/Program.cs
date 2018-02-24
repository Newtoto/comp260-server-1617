using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class client
    {
        static void Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipLocal = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8221);

            bool connected = false;

            while (connected == false)
            {
                try
                {
                    s.Connect(ipLocal);
                    connected = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("No server active");
                    Thread.Sleep(1000);
                }
            }

            int ID = 0;

            while (true)
            {
                // Reading string from server
                byte[] bufferReceive = new byte[4096];

                try
                {
                    int result = s.Receive(bufferReceive);

                    if (result > 0)
                    {
                        ASCIIEncoding encoderSend = new ASCIIEncoding();
                        String recdMsg = encoderSend.GetString(bufferReceive, 0, result);

                        Console.WriteLine(recdMsg);
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                }

                // Send to server
                var entry = Console.ReadLine();

                String Msg = ID.ToString() + ":" + entry;
                ID++;
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(Msg);

                try
                {
                    // Send message to server
                    int bytesSent = s.Send(buffer);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Thread.Sleep(1000);
            }
        }
    }
}
