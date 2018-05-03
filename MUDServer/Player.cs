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
        public Player(int userID, String userName, Socket socket)
        {
            this.userID = userID;
            this.userName = userName;
            this.socket = socket;
        }

        public int userID;
        public String userName = "";
        public int currentRoomID = 1;
        public Socket socket;
    }
}
