using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace HYtank
{
    class GameSocket
    {
        char[,] grid;

        Byte[] data;
        NetworkStream stream;
        NetworkStream stream1;
        String responseData = "";
        bool playersFull = false, gameAlreadyStarted = false, initialized = false, positioned = false;
        IPAddress ipc = new IPAddress(new byte[] { 127, 0, 0, 1 });
        IPAddress ips = new IPAddress(new byte[] { 127, 0, 0, 1 });
        //IPAddress ipc = new IPAddress(new byte[] { 101, 2, 179, 32 });
        //IPAddress ips = new IPAddress(new byte[] { 10,224,58,225});
        Socket s1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        TcpListener serverSocket;

        PlayerInfo p0, p1, p2, p3, p4;
        int gridSize, columnsGrid;

        public GameSocket()
        {
            serverSocket = new TcpListener(ipc, 7000);
            serverSocket.Start();
        }

        //connect to server port to write to it to join the game
        //if server is not available, try until it is...

        public void connectToServer()
        {
            while (true)
            {
                try
                {
                    s1.Connect(ips, 6000);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void joinGame()
        {//Join the game
            String message = "JOIN#";
            data = System.Text.Encoding.ASCII.GetBytes(message);
            stream = new NetworkStream(s1);
            stream.Write(data, 0, data.Length);
            s1.Close();
        }

        public void setGrid(char[,] g, PlayerInfo p0, PlayerInfo p1, PlayerInfo p2, PlayerInfo p3, PlayerInfo p4, int gridSize, int columnsGrid)
        {
            grid = g;
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
            this.gridSize = gridSize;
            this.columnsGrid = columnsGrid;
        }

        private void initializeBricks(string positions)
        {
            String[] xy = positions.Split(',', ';');
            for (int i = 0; i < xy.Length; i += 2)
            {
                grid[Int32.Parse(xy[i + 1]), Int32.Parse(xy[i])] = 'b';//rows of the array corresponds to the y axis. so had to change the order
            }
        }
        private void initializeStones(string positions)
        {
            String[] xy = positions.Split(',', ';');
            for (int i = 0; i < xy.Length; i += 2)
            {
                grid[Int32.Parse(xy[i + 1]), Int32.Parse(xy[i])] = 's';//rows of the array corresponds to the y axis. so had to change the order
            }
        }
        private void initializeWater(string positions)
        {
            String[] xy = positions.Split(',', ';');
            for (int i = 0; i < xy.Length; i += 2)
            {
                grid[Int32.Parse(xy[i + 1]), Int32.Parse(xy[i])] = 'w';//rows of the array corresponds to the y axis. so had to change the order
            }
        }

        public void initialize()
        {//manage responses until the game starts (do only reading)
            while (true)
            {
                try
                {
                    stream1 = new NetworkStream(serverSocket.AcceptSocket());
                    responseData = new StreamReader(stream1).ReadToEnd().Trim();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                String[] info;

                if (responseData.Length > 0 && responseData.ElementAt(0) == 'I')
                {
                    info = responseData.Split(':', '#');
                    initializeBricks(info[2]);
                    initializeStones(info[3]);
                    initializeWater(info[4]);
                    initialized = true;
                    //initialize the map
                }
                else if (responseData.Length > 0 && responseData.ElementAt(0) == 'S')
                {
                    info = responseData.Split(':', ';');
                    positioned = true;
                    //position you in the map
                }
                else if (responseData == "PLAYERS_FULL#")
                {
                    playersFull = true;
                    //need to exit and return to main menu
                }
                else if (responseData == "ALREADY_ADDED#")
                {
                    //do nothing till game starts
                }
                else if (responseData == "GAME_ALREADY_STARTED#")
                {
                    gameAlreadyStarted = true;
                    //if join successfully play, else exit and return
                }

                if (initialized && positioned)
                    break;
            }
        }
        //for each second try to make a move... 

        public void update()
        {
            String[] info;
            if (serverSocket.Pending())
            {
                try
                {
                    stream1 = new NetworkStream(serverSocket.AcceptSocket());
                    responseData = new StreamReader(stream1).ReadToEnd().Trim();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                if (responseData.ElementAt(0) == 'G')
                {
                    info = responseData.Split(':','#');
                    String[] playerInfo;
                    for (int i = 1; i < info.Length - 1; i++)
                    {
                        playerInfo = info[i].Split(';', ',');
                        switch (playerInfo[0])
                        {
                            case "P0":
                                {
                                    p0.participant = true;
                                    p0.position.X = Game1.gridOriginx + (Int32.Parse(playerInfo[1]) + .5f) * gridSize / columnsGrid;
                                    p0.position.Y = Game1.gridOriginy + (Int32.Parse(playerInfo[2]) + .5f) * gridSize / columnsGrid;
                                    p0.direction = Int32.Parse(playerInfo[3]);
                                    p0.shot = Int32.Parse(playerInfo[4]) == 0 ? false : true;
                                    p0.health = Int32.Parse(playerInfo[5]);
                                    p0.coins = Int32.Parse(playerInfo[6]);
                                    p0.points = Int32.Parse(playerInfo[7]);
                                    if (Game1.noPlayers < 1)
                                        Game1.noPlayers = 1;
                                    break;
                                }
                            case "P1":
                                {
                                    p1.participant = true;
                                    p1.position.X = Game1.gridOriginx + (Int32.Parse(playerInfo[1]) + .5f) * gridSize / columnsGrid;
                                    p1.position.Y = Game1.gridOriginy + (Int32.Parse(playerInfo[2]) + .5f) * gridSize / columnsGrid;
                                    p1.direction = Int32.Parse(playerInfo[3]);
                                    p1.shot = Int32.Parse(playerInfo[4]) == 0 ? false : true;
                                    p1.health = Int32.Parse(playerInfo[5]);
                                    p1.coins = Int32.Parse(playerInfo[6]);
                                    p1.points = Int32.Parse(playerInfo[7]);
                                    if (Game1.noPlayers < 2)
                                        Game1.noPlayers = 2;
                                    break;
                                }
                            case "P2":
                                {
                                    p2.participant = true;
                                    p2.position.X = Game1.gridOriginx + (Int32.Parse(playerInfo[1]) + .5f) * gridSize / columnsGrid;
                                    p2.position.Y = Game1.gridOriginy + (Int32.Parse(playerInfo[2]) + .5f) * gridSize / columnsGrid;
                                    p2.direction = Int32.Parse(playerInfo[3]);
                                    p2.shot = Int32.Parse(playerInfo[4]) == 0 ? false : true;
                                    p2.health = Int32.Parse(playerInfo[5]);
                                    p2.coins = Int32.Parse(playerInfo[6]);
                                    p2.points = Int32.Parse(playerInfo[7]);
                                    if (Game1.noPlayers < 3)
                                        Game1.noPlayers = 3;
                                    break;
                                }
                            case "P3":
                                {
                                    p3.participant = true;
                                    p3.position.X = Game1.gridOriginx + (Int32.Parse(playerInfo[1]) + .5f) * gridSize / columnsGrid;
                                    p3.position.Y = Game1.gridOriginy + (Int32.Parse(playerInfo[2]) + .5f) * gridSize / columnsGrid;
                                    p3.direction = Int32.Parse(playerInfo[3]);
                                    p3.shot = Int32.Parse(playerInfo[4]) == 0 ? false : true;
                                    p3.health = Int32.Parse(playerInfo[5]);
                                    p3.coins = Int32.Parse(playerInfo[6]);
                                    p3.points = Int32.Parse(playerInfo[7]);
                                    if (Game1.noPlayers < 4)
                                        Game1.noPlayers = 4;
                                    break;
                                }
                            case "P4":
                                {
                                    p4.participant = true;
                                    p4.position.X = Game1.gridOriginx + (Int32.Parse(playerInfo[1]) + .5f) * gridSize / columnsGrid;
                                    p4.position.Y = Game1.gridOriginy + (Int32.Parse(playerInfo[2]) + .5f) * gridSize / columnsGrid;
                                    p4.direction = Int32.Parse(playerInfo[3]);
                                    p4.shot = Int32.Parse(playerInfo[4]) == 0 ? false : true;
                                    p4.health = Int32.Parse(playerInfo[5]);
                                    p4.coins = Int32.Parse(playerInfo[6]);
                                    p4.points = Int32.Parse(playerInfo[7]);
                                    if (Game1.noPlayers < 5)
                                        Game1.noPlayers = 5;
                                    break;
                                }
                            default :
                                {
                                    for (int j = 0; j+2 < playerInfo.Length;j+=3 )
                                    {
                                        if (playerInfo[j + 2] =="1")
                                            grid[Int32.Parse(playerInfo[j + 1]), Int32.Parse(playerInfo[j])] = '1';//rows of the array corresponds to the y axis. so had to change the order
                                        else if (playerInfo[j + 2] == "2")
                                            grid[Int32.Parse(playerInfo[j + 1]), Int32.Parse(playerInfo[j])] = '2';
                                        else if (playerInfo[j + 2] == "3")
                                            grid[Int32.Parse(playerInfo[j + 1]), Int32.Parse(playerInfo[j])] = '3';
                                        else if (playerInfo[j + 2] == "4")
                                            grid[Int32.Parse(playerInfo[j + 1]), Int32.Parse(playerInfo[j])] = '\0';
                                     }
                                    break;
                                }
                        }
                    }

                }

            }
        }
        public void command(String cmd)
        {
            s1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connectToServer();
            data = System.Text.Encoding.ASCII.GetBytes(cmd);
            stream = new NetworkStream(s1);
            stream.Write(data, 0, data.Length);
            s1.Close();
        }

    }

}
