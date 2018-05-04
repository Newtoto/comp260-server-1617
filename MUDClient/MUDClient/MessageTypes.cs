﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MessageTypes
{
    public abstract class Msg
    {
        public Msg() { mID = 0; }
        public int mID;

        public abstract MemoryStream WriteData();
        public abstract void ReadData(BinaryReader read);

        public static Msg DecodeStream(BinaryReader read)
        {
            int id;
            Msg m = null;

            id = read.ReadInt32();

            switch (id)
            {
                case PublicChatMsg.ID:
                    m = new PublicChatMsg();
                    break;

                case PrivateChatMsg.ID:
                    m = new PrivateChatMsg();
                    break;

                case ClientListMsg.ID:
                    m = new ClientListMsg();
                    break;

                case ClientNameMsg.ID:
                    m = new ClientNameMsg();
                    break;

                case DungeonNavigationMsg.ID:
                    m = new DungeonNavigationMsg();
                    break;

                case LoginAttempt.ID:
                    m = new LoginAttempt();
                    break;

                case SignUpAttempt.ID:
                    m = new SignUpAttempt();
                    break;

                case LoginStateMsg.ID:
                    m = new LoginStateMsg();
                    break;

                case CharacterSelectionMsg.ID:
                    m = new CharacterSelectionMsg();
                    break;

                case CharacterCreationMsg.ID:
                    m = new CharacterCreationMsg();
                    break;

                case CharacterListMsg.ID:
                    m = new CharacterListMsg();
                    break;

                default:
                    throw (new Exception());
            }

            if (m != null)
            {
                m.mID = id;
                m.ReadData(read);
            }

            return m;
        }
    };

    public class PublicChatMsg : Msg
    {
        public const int ID = 1;
        public String msg;

        public PublicChatMsg() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(msg);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
        }
    };

    public class PrivateChatMsg : Msg
    {
        public const int ID = 2;
        public String msg;
        public String destination;

        public PrivateChatMsg() { mID = ID; }
        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(msg);
            write.Write(destination);

            write.Close();

            return stream;
        }
        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
            destination = read.ReadString();
        }
    };

    public class ClientListMsg : Msg
    {
        public const int ID = 3;
        public List<String> clientList;

        public ClientListMsg() 
        { 
            mID = ID;

            clientList = new List<String>();
        }
        public override MemoryStream WriteData()
        {
            
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(clientList.Count);
            foreach (String s in clientList)
            {
                write.Write(s);
            }

            write.Close();

            return stream;
        }
        public override void ReadData(BinaryReader read)
        {
            int count = read.ReadInt32();

            clientList.Clear();

            for (int i = 0; i < count; i++)
            {
                clientList.Add(read.ReadString());
            }
        }
    };


    public class ClientNameMsg : Msg
    {
        public const int ID = 4;

        public String name;

        public ClientNameMsg() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(name);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            name = read.ReadString();
        }
    };

    public class DungeonNavigationMsg : Msg
    {
        public const int ID = 5;
        public String msg;

        public DungeonNavigationMsg() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(msg);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
        }
    };

    public class LoginAttempt : Msg
    {
        public const int ID = 6;
        public String username;
        public String password;

        public LoginAttempt() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(username);
            write.Write(password);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            username = read.ReadString();
            password = read.ReadString();
        }
    };

    public class SignUpAttempt : Msg
    {
        public const int ID = 7;
        public String username;
        public String password;

        public SignUpAttempt() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(username);
            write.Write(password);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            username = read.ReadString();
            password = read.ReadString();
        }
    };

    public class LoginStateMsg : Msg
    {
        public const int ID = 8;
        public String type;
        public String msg;

        public LoginStateMsg() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(type);
            write.Write(msg);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            type = read.ReadString();
            msg = read.ReadString();
        }
    };

    public class CharacterSelectionMsg : Msg
    {
        public const int ID = 9;
        public String msg;
        public int playerID;

        public CharacterSelectionMsg() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(msg);
            write.Write(playerID);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
            playerID = read.ReadInt32();
        }
    };

    public class CharacterCreationMsg : Msg
    {
        public const int ID = 10;
        public String playerName;

        public CharacterCreationMsg() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(playerName);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            playerName = read.ReadString();
        }
    };

    public class CharacterListMsg : Msg
    {
        public const int ID = 11;
        public List<String> characterList;

        public CharacterListMsg()
        {
            mID = ID;

            characterList = new List<String>();
        }
        public override MemoryStream WriteData()
        {

            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(characterList.Count);
            foreach (String s in characterList)
            {
                write.Write(s);
            }

            write.Close();

            return stream;
        }
        public override void ReadData(BinaryReader read)
        {
            int count = read.ReadInt32();

            characterList.Clear();

            for (int i = 0; i < count; i++)
            {
                characterList.Add(read.ReadString());
            }
        }
    };
}