using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    public class Message 
    {
        public Message(int userID, Socket socket, String message)
        {
            this.userID = userID;
            this.message = message;
            this.socket = socket;
        }

        public int userID;
        public String message;
        public Socket socket;
    }

}
