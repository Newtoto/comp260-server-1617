using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Server
{
    class SocketManager
    {
        public Dictionary<int, Socket> playerSockets = new Dictionary<int, Socket>();
        public Dictionary<int, Socket> loggedInSockets = new Dictionary<int, Socket>();
        public Dictionary<Socket, string> socketToCharacterName = new Dictionary<Socket, string>();

        public SocketManager()
        {

        }

        // Uses socket connected to find ID of player
        public int GetClientIDFromSocket(Socket socket)
        {
            foreach (KeyValuePair<int, Socket> i in playerSockets)
            {
                if (playerSockets[i.Key] == socket)
                {
                    return i.Key;
                }
            }

            return 0;
        }

        // Remove client from dictionary
        public void RemoveClientByID(int clientID)
        {
            Console.WriteLine("Removing client " + clientID);
            lock (playerSockets)
            {
                playerSockets.Remove(clientID);
            }
        }


        // Remove player from dictionary
        public void RemovePlayerByID(int playerID)
        {
            Console.WriteLine("Removing player " + playerID);
            lock (loggedInSockets)
            {
                try
                {
                    loggedInSockets.Remove(playerID);
                }
                catch
                {
                    // Player wasn't logged in
                }
            }
        }

        public void AddClientToAllSockets(int userID, Socket s)
        {
            lock (playerSockets)
            {
                // Add new player to logged in socket dictionary
                playerSockets.Add(userID, s);
            }
        }

        public void AddClientToLoggedInSockets(int userID, Socket s, string characterName)
        {
            lock (loggedInSockets)
            {
                // Add new player to logged in socket dictionary
                loggedInSockets.Add(userID, s);
            }

            lock (socketToCharacterName)
            {
                // Add character to dictionary
                socketToCharacterName.Add(s, characterName);
            }
        }
    }
}
