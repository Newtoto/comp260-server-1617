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

namespace Server
{
    class MessageManager
    {
        // Sends message to socket
        public void SendMessageToSocket(Socket s, Msg message)
        {
            MemoryStream outStream = message.WriteData();

            s.Send(outStream.GetBuffer());
        }

        // Sends private message to socket
        public void SendPrivateMessageToSocket(Socket s, PrivateChatMsg message)
        {
            MemoryStream outStream = message.WriteData();

            s.Send(outStream.GetBuffer());
        }
    }
}
