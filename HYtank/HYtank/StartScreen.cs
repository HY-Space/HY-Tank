using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;

namespace HYtank
{
    public partial class StartScreen : Form
    {

        byte[] serverIP, clientIP;
        String[] tempIP;
        int serverPort, clientPort, gridSize;

        public StartScreen()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tempIP = textBoxServerIP.Text.Split('.');


            if (tempIP.Length != 4)
            {
                MessageBox.Show("Server IP is not of the correct format. Please try again.");
                return;
            }
            int result;
            if (!(Int32.TryParse(tempIP[0], out result) && Int32.TryParse(tempIP[1], out result) && Int32.TryParse(tempIP[2], out result) && Int32.TryParse(tempIP[3], out result)))
            {
                MessageBox.Show("Server IP is not of the correct format. Please try again.");
                return;
            }
            if (!(Int32.TryParse(textBoxServerPort.Text, out result)))
            {
                MessageBox.Show("Server Port is not of the correct format. Please try again");
                return;
            }

            serverIP = new byte[] { byte.Parse(tempIP[0]), byte.Parse(tempIP[1]), byte.Parse(tempIP[2]), byte.Parse(tempIP[3]) };
            serverPort = Int32.Parse(textBoxServerPort.Text);


            tempIP = textBoxClientIP.Text.Split('.');
            if (tempIP.Length != 4)
            {
                MessageBox.Show("Client IP is not of the correct format. Please try again.");
                return;
            }
            if (!(Int32.TryParse(tempIP[0], out result) && Int32.TryParse(tempIP[1], out result) && Int32.TryParse(tempIP[2], out result) && Int32.TryParse(tempIP[3], out result)))
            {
                MessageBox.Show("Client IP is not of the correct format. Please try again.");
                return;
            }

            if (!(Int32.TryParse(textBoxClientPort.Text, out result)))
            {
                MessageBox.Show("Client Port is not of the correct format. Please try again");
                return;
            }

            clientIP = new byte[] { byte.Parse(tempIP[0]), byte.Parse(tempIP[1]), byte.Parse(tempIP[2]), byte.Parse(tempIP[3]) };
            clientPort = Int32.Parse(textBoxClientPort.Text);

            if (!(Int32.TryParse(textBoxGridSize.Text, out result)))
            {
                MessageBox.Show("Grid Size is not of the correct format. Please try again");
                return;
            }

            //Console.WriteLine(byte.Parse(tempIP[0]) + " " + byte.Parse(tempIP[1]) + " " + byte.Parse(tempIP[2]) + " " + byte.Parse(tempIP[3]));

            gridSize = Int32.Parse(textBoxGridSize.Text);

            /*Game1 game = new GameSocket(serverIP, serverPort);

            if (!game.tryConnectJoin())
            {
                textBoxDetails.Text = "Could not connect to the server. Please try again";
            }
                                    
            if (GameSocket.playersFull)
                textBoxDetails.Text = "Players Full";
            else if (GameSocket.gameAlreadyStarted)
                textBoxDetails.Text = "Game already started";
            else if (GameSocket.alreadyAdded)
                textBoxDetails.Text = "Already added to the game";
            if (GameSocket.initialized)
            {
                textBoxDetails.Text = "Please wait, initializing the map";
                Program.game = game;
                Application.Exit();
            }
             */

            Thread theThread = new Thread(StartGame);
            theThread.Start(); 
        }
        public void StartGame()
        {
            Game1 game = new Game1(gridSize,new IPAddress(serverIP), serverPort, new IPAddress(clientIP), clientPort);
            //Game1 game = new Game1(new IPAddress(new byte[] { 127, 0, 0, 1 }), 6000,new IPAddress(new byte[] { 127, 0, 0, 1 }),7000);
            game.Run();
        }
    }
}
