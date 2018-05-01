using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Networking stuff for sockets
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using MessageTypes;

namespace MUDClient
{
    public partial class Mud : MetroFramework.Forms.MetroForm
    {
        // Initialise variables for networking
        Socket client;
        private Thread myThread;
        bool bQuit = false;
        bool bConnected = false;

        List<String> currentClientList = new List<String>();

        // Connect cilent to server
        static void clientProcess(Object o)
        {

            Mud form = (Mud)o;

            while ((form.bConnected == false) && (form.bQuit == false))
            {
                try
                {
                    form.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    // Server IP 138.68.161.95
                    form.client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
                    form.bConnected = true;
                    form.AddText("Connected to server");

                    Thread receiveThread;

                    receiveThread = new Thread(clientReceive);
                    receiveThread.Start(o);

                    while ((form.bQuit == false) && (form.bConnected == true))
                    {
                        if (form.IsDisposed == true)
                        {
                            form.bQuit = true;
                            form.client.Close();
                        }
                    }

                    receiveThread.Abort();
                }
                catch (System.Exception)
                {
                    form.AddText("No server!");
                    Thread.Sleep(1000);
                }
            }
        }

        static void clientReceive(Object o)
        {
            Mud form = (Mud)o;

            while (form.bConnected == true)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = form.client.Receive(buffer);

                    if (result > 0)
                    {
                        MemoryStream stream = new MemoryStream(buffer);
                        BinaryReader read = new BinaryReader(stream);

                        Msg m = Msg.DecodeStream(read);

                        if (m != null)
                        {
                            Console.Write("Got a message: ");
                            switch (m.mID)
                            {
                                case PublicChatMsg.ID:
                                    {
                                        PublicChatMsg publicMsg = (PublicChatMsg)m;

                                        form.AddText(publicMsg.msg);
                                    }
                                    break;

                                case PrivateChatMsg.ID:
                                    {
                                        PrivateChatMsg privateMsg = (PrivateChatMsg)m;
                                        form.AddText(privateMsg.msg);
                                    }
                                    break;

                                case ClientListMsg.ID:
                                    {
                                        ClientListMsg clientList = (ClientListMsg)m;

                                        form.SetClientList(clientList);
                                    }
                                    break;

                                case ClientNameMsg.ID:
                                    {
                                        ClientNameMsg clientName = (ClientNameMsg)m;

                                        form.SetClientName(clientName.name);
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                }
                catch (Exception)
                {
                    form.bConnected = false;
                    Console.WriteLine("Lost server!");
                }

            }
        }

        // Initialisation
        public Mud()
        {
            InitializeComponent();

            myThread = new Thread(clientProcess);
            myThread.Start(this);

            Application.ApplicationExit += delegate { OnExit(); };
        }

        private void Mud_Load(object sender, EventArgs e)
        {

        }

        // WINFORM FUNCTIONS {
        // NAVIGATION {
        // Send north direction
        private void northTile_Click(object sender, EventArgs e)
        {
            // Create navigation message
            createNavigationMessageFromString("go north");
        }

        // Send east direction
        private void eastTile_Click(object sender, EventArgs e)
        {
            // Create navigation message
            createNavigationMessageFromString("go east");
        }

        // Send south direction
        private void southTile_Click(object sender, EventArgs e)
        {
            // Create navigation message
            createNavigationMessageFromString("go south");
        }

        // Send west direction
        private void westTile_Click(object sender, EventArgs e)
        {
            // Create navigation message
            createNavigationMessageFromString("go west");
        }

        // Sends navigation message to server
        private void createNavigationMessageFromString(String direction)
        {
            // Check for server connection
            if(bConnected)
            {
                // Create navigation message
                DungeonNavigationMsg navigationMsg = new DungeonNavigationMsg();
                navigationMsg.msg = direction;

                MemoryStream outStream = navigationMsg.WriteData();
                client.Send(outStream.GetBuffer());
            }
            else
            {
                AddText("No server, navigation disabled.");
            }
        }
        // } NAVIGATION

        private delegate void AddTextDelegate(String s);
        
        // Add to main output text box
        private void AddText(String s)
        {
            if (outputTextBox.InvokeRequired)
            {
                Invoke(new AddTextDelegate(AddText), new object[] { s });
            }
            else
            {
                outputTextBox.Text += s;
                outputTextBox.Text += Environment.NewLine;
            }
        }

        private delegate void SetClientNameDelegate(String s);
        private void SetClientName(String s)
        {
            if (this.InvokeRequired)
            {
                Invoke(new SetClientNameDelegate(SetClientName), new object[] { s });
            }
            else
            {
                Text = s;
            }
        }

        private delegate void SetClientListDelegate(ClientListMsg clientList);
        private void SetClientList(ClientListMsg clientList)
        {
            if (this.InvokeRequired)
            {
                Invoke(new SetClientListDelegate(SetClientList), new object[] { clientList });
            }
            else
            {
                clientListBox.DataSource = null;
                currentClientList.Clear();
                currentClientList.Add("Room");

                foreach (String s in clientList.clientList)
                {
                    currentClientList.Add(s);
                }
                clientListBox.DataSource = currentClientList;
            }
        }

        // } WINFORM FUNCTIONS

        // Send text input
        private void sendButton_Click(object sender, EventArgs e)
        {
            if ((inputBox.Text.Length > 0) && (client != null))
            {
                try
                {
                    if (clientListBox.SelectedIndex == 0)
                    {
                        // Create public message
                        PublicChatMsg publicMsg = new PublicChatMsg();

                        publicMsg.msg = inputBox.Text;
                        MemoryStream outStream = publicMsg.WriteData();
                        client.Send(outStream.GetBuffer());
                    }
                    else
                    {
                        // Create private message
                        PrivateChatMsg privateMsg = new PrivateChatMsg();

                        privateMsg.msg = inputBox.Text;
                        privateMsg.destination = currentClientList[clientListBox.SelectedIndex];
                        MemoryStream outStream = privateMsg.WriteData();
                        client.Send(outStream.GetBuffer());
                    }

                }
                catch (System.Exception)
                {
                }

                inputBox.Text = "";
            }
        }

        private void OnExit()
        {
            bQuit = true;
            Thread.Sleep(500);
            if (myThread != null)
            {
                myThread.Abort();
            }
        }

        private void clientListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
