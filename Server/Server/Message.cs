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
        public Message(Socket socket, String messageText, string receiveInfoID = "")
        {
            if(receiveInfoID != ""){
                string[] playerInfo = receiveInfoID.Split(new Char[] { ':' });
                this.messageNumber = Int32.Parse(playerInfo[0]);
                this.userID = Int32.Parse(playerInfo[1]);
            }

            this.messageText = messageText;
            this.socket = socket;
        }

        public int userID;
        public int messageNumber;
        public String messageText;
        public Socket socket;
    }

}
