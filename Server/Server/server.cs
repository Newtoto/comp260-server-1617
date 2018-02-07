using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class server
    {
        static void Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipLocal = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8221);
			
            s.Bind(ipLocal);
            s.Listen(4);

            Console.WriteLine("Waiting for client ...");
            
            Socket newConnection = s.Accept();
            if (newConnection != null)
            {            
                while (true)
                {
                    // Receiving from client
                    byte[] bufferReceive = new byte[4096];

                    try
                    {
                        int result = newConnection.Receive(bufferReceive);

                        if (result > 0)
                        {
                            ASCIIEncoding encoderSend = new ASCIIEncoding();
                            String recdMsg = encoderSend.GetString(bufferReceive, 0, result);

                            Console.WriteLine("Received: " + recdMsg);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex);    	
                    }

                    // Sending data to client

                    String Msg = "Testing server to client message";
                    ASCIIEncoding encoderReceive = new ASCIIEncoding();
                    byte[] bufferSend = encoderReceive.GetBytes(Msg);

                    try
                    {
                        Console.WriteLine(Msg);
                        int bytesSent = newConnection.Send(bufferSend);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    // Reading string from server

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
