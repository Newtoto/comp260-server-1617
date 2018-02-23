using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    public class Player
    {
        public Player(int userID, Socket socket)
        {
            this.userID = userID;
            this.userSocket = socket;
        }

        public int userID;
        public String userName = "";
        public String location = "Room 0";
        public Socket userSocket;
    }

}
