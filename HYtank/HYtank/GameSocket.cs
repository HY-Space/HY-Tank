using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Microsoft.Xna.Framework;
using System.Diagnostics;

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

                if (responseData.Length > 0 && responseData.Split(':')[0] == "I")
                {
                    info = responseData.Split(':', '#');
                    Game1.ourPlayer = Game1.players[int.Parse(info[1].Substring(1))];//sets our player
                    initializeBricks(info[2]);
                    initializeStones(info[3]);
                    initializeWater(info[4]);
                    initialized = true;
                    //initialize the map
                }
                else if (responseData.Length > 0 && responseData.Split(':')[0] == "S")
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

                //Console.WriteLine("Hi");
            }
        }

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
                if (responseData.Split(':')[0] == "G")
                {
                    info = responseData.Split(':');
                    String[] playerInfo;
                    String[] brickInfo;
                    PlayerInfo p;
                    int tmp;


                    //clearing the current player positions
                    if (p0.coordinates.X > -1)
                    {
                        grid[p0.coordinates.Y, p0.coordinates.X] = '\0';
                    }
                    if (p1.coordinates.X > -1)
                    {
                        grid[p1.coordinates.Y, p1.coordinates.X] = '\0';
                    }
                    if (p2.coordinates.X > -1)
                    {
                        grid[p2.coordinates.Y, p2.coordinates.X] = '\0';
                    }
                    if (p3.coordinates.X > -1)
                    {
                        grid[p3.coordinates.Y, p3.coordinates.X] = '\0';
                    }
                    if (p4.coordinates.X > -1)
                    {
                        grid[p4.coordinates.Y, p4.coordinates.X] = '\0';
                    }



                    for (int i = 1; i < info.Length - 1; i++)
                    {

                        playerInfo = info[i].Split(';', ',');
                        p = null;
                        tmp = 0;
                        switch (playerInfo[0])
                        {

                            case "P0":
                                {
                                    p = p0;
                                    tmp = 1;
                                    break;
                                }
                            case "P1":
                                {
                                    p = p1;
                                    tmp = 2;
                                    break;
                                }
                            case "P2":
                                {
                                    p = p2;
                                    tmp = 3;
                                    break;
                                }
                            case "P3":
                                {
                                    p = p3;
                                    tmp = 4;
                                    break;
                                }
                            case "P4":
                                {
                                    p = p4;
                                    tmp = 5;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }                               

                        }
                        if (p != null)
                        {
                            p.participant = true;
                            //reset the current position on the grid
                            if (p.coordinates.X != -1)
                            {
                                Game1.tankGrid[p.coordinates.Y, p.coordinates.X] = -1;
                            }
                            p.coordinates.X=Int32.Parse(playerInfo[1]);
                            p.coordinates.Y=Int32.Parse(playerInfo[2]);

                            
                            

                            
                            p.position.X = Game1.gridOriginx + (p.coordinates.X + .5f) * gridSize / columnsGrid;
                            p.position.Y = Game1.gridOriginy + (p.coordinates.Y + .5f) * gridSize / columnsGrid;
                            p.direction = Int32.Parse(playerInfo[3]);
                            p.shot = Int32.Parse(playerInfo[4]) == 0 ? false : true;
                            if (p.shot)
                            {
                                new Bullet(new Point(p.coordinates.X, p.coordinates.Y), p.direction);//bullet will be added automatically to the list of bullets
                            }
                            p.health = Int32.Parse(playerInfo[5]);

                            //marks the player on the grid only if alive
                            if (p.health != 0)
                            {
                                grid[p.coordinates.Y, p.coordinates.X] = 't';
                                Game1.tankGrid[p.coordinates.Y, p.coordinates.X] = tmp - 1;
                            }

                            p.coins = Int32.Parse(playerInfo[6]);
                            p.points = Int32.Parse(playerInfo[7]);
                            if (Game1.noPlayers < tmp)
                            {
                                Game1.noPlayers = tmp;
                            }
                        }
                    }

                    brickInfo = info[info.Length - 1].Split(',', ';', '#');
                                
                    for (int j = 0; j+2 < brickInfo.Length;j+=3 )
                    {
                        if (brickInfo[j + 2] =="1")
                            grid[Int32.Parse(brickInfo[j + 1]), Int32.Parse(brickInfo[j])] = '1';//rows of the array corresponds to the y axis. so had to change the order
                        else if (brickInfo[j + 2] == "2")
                            grid[Int32.Parse(brickInfo[j + 1]), Int32.Parse(brickInfo[j])] = '2';
                        else if (brickInfo[j + 2] == "3")
                            grid[Int32.Parse(brickInfo[j + 1]), Int32.Parse(brickInfo[j])] = '3';
                        else if (brickInfo[j + 2] == "4")
                            grid[Int32.Parse(brickInfo[j + 1]), Int32.Parse(brickInfo[j])] = '\0';
                    }


                    //removes the collected coins
                    for (int i = 0; i < Game1.coinsList.Count; i++)
                    {
                        for (int j = 0; j < Game1.noPlayers; j++)
                        {
                            if (Game1.players[j].position == Game1.coinsList.ElementAt(i).position && Game1.players[j].health!=0)
                            {
                                Game1.arena[Game1.coinsList.ElementAt(i).y, Game1.coinsList.ElementAt(i).x] = '\0';
                                Game1.coinsList.RemoveAt(i);
                                break;
                            }
                        }
                    }

                    Game1.game.setNextMove();
                }
                else if (responseData.Split(':')[0] == "C")
                {
                    info = responseData.Split(':', '#',',');
                    grid[Int32.Parse(info[2]), Int32.Parse(info[1])] = 'c';
                    double leaveat = Double.Parse(info[3]) + Game1.time;
                    Game1.coinsList.Add(new CoinsInfo(Game1.gridOriginx + (Int32.Parse(info[1]) + .5f) * gridSize / columnsGrid, Game1.gridOriginy + (Int32.Parse(info[2]) + .5f) * gridSize / columnsGrid, Int32.Parse(info[4]), leaveat, Int32.Parse(info[1]), Int32.Parse(info[2]), Int32.Parse(info[3])));
                    Game1.game.setNextMove();
                }
                else if (responseData.Split(':')[0] == "L")
                {
                    info = responseData.Split(':', '#',',');
                    grid[Int32.Parse(info[2]), Int32.Parse(info[1])] = 'l';
                    double leaveat = Double.Parse(info[3]) + Game1.time;
                    Game1.lifeList.Add(new LifepackInfo(Game1.gridOriginx + (Int32.Parse(info[1]) + .5f) * gridSize / columnsGrid, Game1.gridOriginy + (Int32.Parse(info[2]) + .5f) * gridSize / columnsGrid, leaveat,Int32.Parse(info[1]), Int32.Parse(info[2]), Int32.Parse(info[3])));
                }
                else if (responseData == "TOO_QUICK#")
                {
                    //command reached before 1s after the previous
                }
                else if (responseData == "GAME_HAS_FINISHED#")
                {
                    //command reached after the game is finished
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
