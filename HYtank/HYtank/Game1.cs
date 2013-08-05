using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Timers;
using System.Diagnostics;

namespace HYtank
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static char[,] arena;
        public static int[,] tankGrid;//this contains the player no (0 to 4) at the occupied cell else -1
        public static Game1 game;
        public static PlayerInfo ourPlayer;
        public static PlayerInfo[] players = new PlayerInfo[5];
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        Texture2D backgroundTexture;
        Texture2D waterTexture, stoneTexture, brickTexture, brick1Texture, brick2Texture,brick3Texture, tankTexture,bulletTexture, p1Texture, p2Texture, p3Texture, p4Texture, p5Texture,coinsTexture,lifepackTexture;
        int screenWidth;
        int screenHeight;
        int rowsGrid = 20;
        public static int columnsGrid = 20;
        public static int gridSize = 660;//600
        public static int gridOriginx = 6;
        public static int gridOriginy = 6;
        int cellWidth, cellHeight;
        double lastCmdAt = 0;
        public static int noPlayers = 0;
        public static double time;
        public static List<CoinsInfo> coinsList = new List<CoinsInfo>();
        public static List<LifepackInfo> lifeList = new List<LifepackInfo>();
        public static LinkedList<Bullet> bullets = new LinkedList<Bullet>();

        int bulletCount; //used to fire more
        int allowFireCount=0;//used to control gaps between bursts

        SpriteFont title,body,celltext;

        GameSocket gs = new GameSocket();
        AI ai = new AI(columnsGrid,columnsGrid);

        
        PlayerInfo p0 = new PlayerInfo(-1, -1), p1 = new PlayerInfo(-1, -1), p2 = new PlayerInfo(-1, -1), p3 = new PlayerInfo(-1, -1), p4 = new PlayerInfo(-1, -1);
        
        Vector2 tankCentre,bulletCentre,playerCentre, coinsCentre, lifepackCentre;
        float tankScale, bulletScale, playerScale, coinsScale, lifepackScale;
       
        float angle;

        //Changing the timer should also change the timePerStep value to calculate correctly the time to coin piles
        int timePerStep=1300;
        //Timer timer = new Timer(1003);//original
        Timer timer = new Timer(1300);//testing
        Timer bulletTimer = new Timer(2);//testing
        String nextCommand = "";

        Cell[,] myDistances=new Cell[columnsGrid, columnsGrid];// has the format [y,x];
        HashSet<char> barriers= new HashSet<char>{'w','b','s','1','2','3'};// add 't' here if tanks need to be considered as obstacles
        GameTime gt;

        public Game1()
        {
            game = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            arena = new char[columnsGrid, columnsGrid];
            tankGrid = new int[columnsGrid, columnsGrid];
            cellWidth = gridSize / columnsGrid;
            cellHeight = gridSize / rowsGrid;
            //arena[0, 1] = 's'; arena[9, 0] = 's'; arena[9, 1] = 's'; arena[9, 2] = 's'; arena[8, 1] = 's'; arena[8, 0] = 's'; arena[5, 0] = 'w'; arena[5, 1] = 'w'; arena[4, 0] = 'w'; arena[4, 1] = 'w'; arena[6, 9] = 'b'; arena[6, 8] = 'b'; arena[5, 9] = 'b'; arena[5, 8] = 'b';

            players[0] = p0; players[1] = p1; players[2] = p2; players[3] = p3; players[4] = p4;

            for (int i = 0; i < tankGrid.GetLength(0); i++)//initializing tankgrid for all -1s
            {
                for (int j = 0; j < tankGrid.GetLength(0); j++)
                {
                    tankGrid[i, j] = -1;
                }
            }

            for (int i = 0; i < columnsGrid; i++)
            {
                for (int j = 0; j < columnsGrid; j++)
                {
                    myDistances[i, j]=new Cell();
                }
            }

            gs.setGrid(arena, p0, p1, p2, p3, p4, gridSize, columnsGrid);
            

            tankCentre = new Vector2(49, 49);//origin needs to be defined with respect to the original image
            tankScale = cellWidth * 1f / 100;
            bulletCentre = new Vector2(500, 500);//origin needs to be defined with respect to the original image
            bulletScale = cellWidth * 1f / 1000;
            playerCentre = new Vector2(50, 50);//origin needs to be defined with respect to the original image
            coinsCentre = new Vector2(50, 50);
            lifepackCentre = new Vector2(50, 50);
            coinsScale = lifepackScale = cellWidth * 1f / 100; 

            timer.Elapsed+=new ElapsedEventHandler(timer_Elapsed);
            bulletTimer.Elapsed += new ElapsedEventHandler(bulletTimer_Elapsed);
            
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1100;//default 1000
            graphics.PreferredBackBufferHeight = 671;//default 610
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "TANKS";
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            backgroundTexture = Content.Load<Texture2D>("BG");
            waterTexture = Content.Load<Texture2D>("water");
            stoneTexture = Content.Load<Texture2D>("stone");
            brickTexture = Content.Load<Texture2D>("brick");
            brick1Texture = Content.Load<Texture2D>("brick25");
            brick2Texture = Content.Load<Texture2D>("brick50");
            brick3Texture = Content.Load<Texture2D>("brick75");
            tankTexture = Content.Load<Texture2D>("tank");
            bulletTexture = Content.Load<Texture2D>("bullet");
            coinsTexture = Content.Load<Texture2D>("coins");
            lifepackTexture = Content.Load<Texture2D>("lifepack");
            p1Texture = Content.Load<Texture2D>("p1");
            p2Texture = Content.Load<Texture2D>("p2");
            p3Texture = Content.Load<Texture2D>("p3");
            p4Texture = Content.Load<Texture2D>("p4");
            p5Texture = Content.Load<Texture2D>("p5");
            title = Content.Load<SpriteFont>("ScoreTitle");
            body = Content.Load<SpriteFont>("ScoreBody");
            celltext = Content.Load<SpriteFont>("cellbody");
            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;
            // TODO: use this.Content to load your game content here


            gs.connectToServer();
            gs.joinGame();

            //test
            GameTime temptime = new GameTime();
            Console.WriteLine(temptime.TotalGameTime);
            gs.initialize();
            Console.WriteLine(temptime.TotalGameTime);
            timer.Start();


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            gs.update();

            ai.nextMove(arena);


            //uncomment the following to allow keyboard handling of the tank

            /*
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up))
            {
                nextCommand = "UP#";
                gs.command(nextCommand);
            }
            else if (keyState.IsKeyDown(Keys.Down))
            {
                nextCommand = "DOWN#";
                gs.command(nextCommand);
            }
            else if (keyState.IsKeyDown(Keys.Left))
            {
                nextCommand = "LEFT#";
                gs.command(nextCommand);
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                nextCommand = "RIGHT#";
                gs.command(nextCommand);
            }
            else if (keyState.IsKeyDown(Keys.Space))
            {
                nextCommand = "SHOOT#";
                //Console.WriteLine(gameTime.TotalGameTime.TotalMilliseconds);
                //gs.command(nextCommand);
                bulletCount = 0;
                bulletTimer.Start();
            }
            */
            //keyboar controlling ends here

            base.Update(gameTime);
            time = gameTime.TotalGameTime.TotalMilliseconds;
        }



        private void timer_Elapsed(Object sender, ElapsedEventArgs arg)
        {

            if (allowFireCount < 0)//use this to control the delay between bursts
            {
                allowFireCount++;
            }

            else
            {
                //These check for inline oponents to fire

                if (ourPlayer.direction == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (players[i].health > 0 && players[i].coordinates.X == ourPlayer.coordinates.X && players[i].coordinates.Y < ourPlayer.coordinates.Y /*&& (players[i].direction==2 || players[i].direction == 0)*/)
                        {
                            bulletTimer.Start();
                            allowFireCount = 0;
                            return;
                        }
                    }
                }
                if (ourPlayer.direction == 1)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (players[i].health > 0 && players[i].coordinates.Y == ourPlayer.coordinates.Y && players[i].coordinates.X > ourPlayer.coordinates.X /*&& (players[i].direction == 3 || players[i].direction == 1)*/)
                        {
                            bulletTimer.Start();
                            allowFireCount = 0;
                            return;
                        }
                    }
                }
                if (ourPlayer.direction == 2)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (players[i].health > 0 && players[i].coordinates.X == ourPlayer.coordinates.X && players[i].coordinates.Y > ourPlayer.coordinates.Y /*&& (players[i].direction == 0 || players[i].direction == 2)*/)
                        {
                            bulletTimer.Start();
                            allowFireCount = 0;
                            return;
                        }
                    }
                }
                if (ourPlayer.direction == 3)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (players[i].health > 0 && players[i].coordinates.Y == ourPlayer.coordinates.Y && players[i].coordinates.X < ourPlayer.coordinates.X /*&& (players[i].direction == 1 || players[i].direction == 3)*/)
                        {
                            bulletTimer.Start();
                            allowFireCount = 0;
                            return;
                        }
                    }
                }
            }

            if ("".Equals(nextCommand))
            {
                setNextMove();
            }


            if (!("".Equals(nextCommand)))
            {
                timer.Stop();
                gs.command(nextCommand);
                timer.Start();
                nextCommand = "";
            }

            setNextMove();
               
        }

        private void bulletTimer_Elapsed(Object sender, ElapsedEventArgs arg)
        {
            
            gs.command("SHOOT#");//test

            bulletCount++;

            if (bulletCount == 4)
            {
                bulletTimer.Stop();
                bulletCount = 0;
            }
        }

        public void setNextMove()
        {
            //Console.WriteLine(gt.TotalGameTime.TotalMilliseconds);// test
            findDistances(ourPlayer.coordinates.X, ourPlayer.coordinates.Y, ourPlayer.direction);
            //Console.WriteLine(gt.TotalGameTime.TotalMilliseconds);// test
            CoinsInfo nextTarget = null;
            LifepackInfo nextLife = null;
            foreach (CoinsInfo coin in coinsList)
            {
                if ((ourPlayer.coordinates.X != coin.x || ourPlayer.coordinates.Y != coin.y) && (nextTarget == null || myDistances[coin.y, coin.x].min < myDistances[nextTarget.y, nextTarget.x].min) && myDistances[coin.y, coin.x].min *timePerStep < coin.leaveat - gt.TotalGameTime.TotalMilliseconds)
                {
                    nextTarget = coin;
                }
            }
            if (nextTarget != null)// used in case no coins are in the arena
            {
                //Console.WriteLine(nextTarget.x + "," + nextTarget.y);//test
                switch (myDistances[nextTarget.y, nextTarget.x].moveTo)
                {
                    case 0:
                        {
                            nextCommand = "UP#";
                            break;
                        }
                    case 1:
                        {
                            nextCommand = "RIGHT#";
                            break;
                        }
                    case 2:
                        {
                            nextCommand = "DOWN#";
                            break;
                        }
                    case 3:
                        {
                            nextCommand = "LEFT#";
                            break;
                        }
                }
            }
            if (nextTarget == null)//go for life packs if no coins are there
            {
                foreach (LifepackInfo lifePack in lifeList)
                {
                    if ((ourPlayer.coordinates.X != lifePack.x || ourPlayer.coordinates.Y != lifePack.y) && (nextLife == null || myDistances[lifePack.y, lifePack.x].min < myDistances[nextLife.y, nextLife.x].min) && myDistances[lifePack.y, lifePack.x].min * timePerStep < lifePack.leaveat - gt.TotalGameTime.TotalMilliseconds)
                    {
                        nextLife = lifePack;
                    }
                }
                if (nextLife != null)
                {
                    switch (myDistances[nextLife.y, nextLife.x].moveTo)
                    {
                        case 0:
                            {
                                nextCommand = "UP#";
                                break;
                            }
                        case 1:
                            {
                                nextCommand = "RIGHT#";
                                break;
                            }
                        case 2:
                            {
                                nextCommand = "DOWN#";
                                break;
                            }
                        case 3:
                            {
                                nextCommand = "LEFT#";
                                break;
                            }
                    }
                }
            }

            if (nextTarget == null)//this segment is used to move the tank without going nowhere
            {
                foreach (CoinsInfo coin in coinsList)
                {
                    if ((ourPlayer.coordinates.X != coin.x || ourPlayer.coordinates.Y != coin.y) && (nextTarget == null || myDistances[coin.y, coin.x].min < myDistances[nextTarget.y, nextTarget.x].min) && myDistances[coin.y, coin.x].min * timePerStep < coin.leaveat - gt.TotalGameTime.TotalMilliseconds)
                    {
                        nextTarget = coin;
                    }

                    if (nextTarget != null)// used in case no coins are in the arena
                    {
                        //Console.WriteLine(nextTarget.x + "," + nextTarget.y);//test
                        switch (myDistances[nextTarget.y, nextTarget.x].moveTo)
                        {
                            case 0:
                                {
                                    nextCommand = "UP#";
                                    break;
                                }
                            case 1:
                                {
                                    nextCommand = "RIGHT#";
                                    break;
                                }
                            case 2:
                                {
                                    nextCommand = "DOWN#";
                                    break;
                                }
                            case 3:
                                {
                                    nextCommand = "LEFT#";
                                    break;
                                }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            gt = gameTime;//this has the game's time info
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            DrawBackground();               //draw the basic background
            DrawGrid();                     //draw the map according to the map size
            fillGrid();                     //update the objects on the map
            DrawScoreboard();               //draw the scoreboard considering no of players
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private void DrawBackground()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
        }
        private void DrawGrid()
        {
            Texture2D blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            int rowGap = gridSize / rowsGrid;
            int columnGap = gridSize / columnsGrid;
            Color lineColour = new Color(210, 200, 180);
            for (int i = 1; i < rowsGrid; i++)
            {
                spriteBatch.Draw(blank, new Vector2(gridOriginx, gridOriginy + (gridSize * i) / rowsGrid), null, lineColour, 0, Vector2.Zero, new Vector2(gridSize, 1), SpriteEffects.None, 0);
            }
            for (int i = 1; i < columnsGrid; i++)
            {
                spriteBatch.Draw(blank, new Vector2(gridOriginx + (gridSize * i) / columnsGrid, gridOriginy), null, lineColour, 0, Vector2.Zero, new Vector2(1, gridSize), SpriteEffects.None, 0);
            }
        }
        private void fillGrid()//s-stone, b-brick, w-water
        {
            Rectangle screenRectangle;
            for (int i = 0; i < rowsGrid; i++)
            {
                for (int j = 0; j < columnsGrid; j++)
                {
                    switch (arena[i, j])
                    {
                        case 's':
                            {
                                screenRectangle = new Rectangle(gridOriginx + j * gridSize / columnsGrid, gridOriginy + i * gridSize / rowsGrid, cellWidth, cellHeight);
                                spriteBatch.Draw(stoneTexture, screenRectangle, Color.White);
                                break;
                            }
                        case 'w':
                            {
                                screenRectangle = new Rectangle(gridOriginx + j * gridSize / columnsGrid, gridOriginy + i * gridSize / rowsGrid, cellWidth, cellHeight);
                                spriteBatch.Draw(waterTexture, screenRectangle, Color.White);
                                break;
                            }
                        case 'b':
                            {
                                screenRectangle = new Rectangle(gridOriginx + j * gridSize / columnsGrid, gridOriginy + i * gridSize / rowsGrid, cellWidth, cellHeight);
                                spriteBatch.Draw(brickTexture, screenRectangle, Color.White);
                                break;
                            }
                        case '1':
                            {
                                screenRectangle = new Rectangle(gridOriginx + j * gridSize / columnsGrid, gridOriginy + i * gridSize / rowsGrid, cellWidth, cellHeight);
                                spriteBatch.Draw(brick1Texture, screenRectangle, Color.White);
                                spriteBatch.DrawString(celltext, "75%", getTextLocationCenter(j,i,"75%"), Color.Black);
                                break;
                            }
                        case '2':
                            {
                                screenRectangle = new Rectangle(gridOriginx + j * gridSize / columnsGrid, gridOriginy + i * gridSize / rowsGrid, cellWidth, cellHeight);
                                spriteBatch.Draw(brick2Texture, screenRectangle, Color.White);
                                spriteBatch.DrawString(celltext, "50%", getTextLocationCenter(j, i, "50%"), Color.Black);
                                break;
                            }
                        case '3':
                            {
                                screenRectangle = new Rectangle(gridOriginx + j * gridSize / columnsGrid, gridOriginy + i * gridSize / rowsGrid, cellWidth, cellHeight);
                                spriteBatch.Draw(brick3Texture, screenRectangle, Color.White);
                                spriteBatch.DrawString(celltext, "25%", getTextLocationCenter(j, i, "25%"), Color.Black);
                                break;
                            }
                    }
                }
            }

            for (int i = 0; i < coinsList.Count; i++)
            {
                if (time > coinsList.ElementAt(i).leaveat)
                {
                    arena[coinsList.ElementAt(i).y, coinsList.ElementAt(i).x] = '\0';
                    coinsList.RemoveAt(i);
                }
                else
                {
                    bool removed = false;
                    for (int j = 0; j < noPlayers; j++)
                    {
                        if (players[j].position == coinsList.ElementAt(i).position)
                        {
                            arena[coinsList.ElementAt(i).y, coinsList.ElementAt(i).x] = '\0';
                            coinsList.RemoveAt(i);
                            removed = true;
                            break;
                        }
                    }
                    if (!removed)
                    {
                        spriteBatch.Draw(coinsTexture, coinsList.ElementAt(i).position, null, Color.White, 0, coinsCentre, coinsScale, SpriteEffects.None, 1);
                        spriteBatch.DrawString(celltext, "$" + coinsList.ElementAt(i).value + "\n" + ((int)Math.Round(coinsList.ElementAt(i).leaveat - gt.TotalGameTime.TotalMilliseconds))/*coinsList.ElementAt(i).lifetime*/, getTextLocationCenter(coinsList.ElementAt(i).x, coinsList.ElementAt(i).y, "$" + coinsList.ElementAt(i).value + "\n" + coinsList.ElementAt(i).lifetime), Color.Black);
                    }
                }
            }

            for (int i = 0; i < lifeList.Count; i++)
            {
                if (time > lifeList.ElementAt(i).leaveat)
                {
                    arena[lifeList.ElementAt(i).y, lifeList.ElementAt(i).x] = '\0';
                    lifeList.RemoveAt(i);
                }
                else
                {
                    bool removed = false;
                    for (int j = 0; j < noPlayers; j++)
                    {
                        if (players[j].position == lifeList.ElementAt(i).position)
                        {
                            arena[lifeList.ElementAt(i).y, lifeList.ElementAt(i).x] = '\0';
                            lifeList.RemoveAt(i);
                            removed = true;
                            break;
                        }
                    }
                    if (!removed)
                    {
                        spriteBatch.Draw(lifepackTexture, lifeList.ElementAt(i).position, null, Color.White, 0, lifepackCentre, lifepackScale, SpriteEffects.None, 1);
                        spriteBatch.DrawString(celltext, ((int)Math.Round(lifeList.ElementAt(i).leaveat - gt.TotalGameTime.TotalMilliseconds)).ToString(), getTextLocationCenter(lifeList.ElementAt(i).x, lifeList.ElementAt(i).y, lifeList.ElementAt(i).lifetime.ToString()), Color.Black);
                    }
                }
            }
            
            foreach (Bullet bullet in bullets)
            {
                spriteBatch.Draw(bulletTexture, bullet.position, null, Color.White, bullet.direction * 1.57f, bulletCentre, bulletScale, SpriteEffects.None, 1);
            }

            if (p0.position.X != -1 && p0.health > 0)
            {
                spriteBatch.Draw(tankTexture, p0.position, null, Color.DeepSkyBlue, p0.direction * 1.57f, tankCentre, tankScale, SpriteEffects.FlipVertically, 1);
            }
            if (p1.position.X != -1 && p1.health > 0)
            {
                spriteBatch.Draw(tankTexture, p1.position, null, Color.Crimson, p1.direction * 1.57f, tankCentre, tankScale, SpriteEffects.FlipVertically, 1);
            }
            if (p2.position.X != -1 && p2.health > 0)
            {
                spriteBatch.Draw(tankTexture, p2.position, null, Color.Yellow, p2.direction * 1.57f, tankCentre, tankScale, SpriteEffects.FlipVertically, 1);
            }
            if (p3.position.X != -1 && p3.health > 0)
            {
                spriteBatch.Draw(tankTexture, p3.position, null, Color.LawnGreen, p3.direction * 1.57f, tankCentre, tankScale, SpriteEffects.FlipVertically, 1);
            }
            if (p4.position.X != -1 && p4.health > 0)
            {
                spriteBatch.Draw(tankTexture, p4.position, null, Color.Violet, p4.direction * 1.57f, tankCentre, tankScale, SpriteEffects.FlipVertically, 1);
            }

        }

        private void DrawScoreboard()
        {
            Texture2D blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            int rowGap = 40;
            int columnGap = 80;
            int scoreorgy = 150;
            int rowlength = 320;
            int scoreorgx = (gridSize + gridOriginx * 2) + ((screenWidth - gridSize - gridOriginx * 2 - rowlength)/2);
            playerScale = (rowGap-5) * 1f / 100; 
            Color lineColour = new Color(210, 200, 180);
            spriteBatch.Draw(blank, new Vector2(scoreorgx, scoreorgy), null, lineColour, 0, Vector2.Zero, new Vector2(rowlength, 1), SpriteEffects.None, 0);
            
            //draw rows and place player symbols
            for (int i = 1; i < noPlayers+2; i++)
            {
                spriteBatch.Draw(blank, new Vector2(scoreorgx, scoreorgy+ 30 + (rowGap * (i-1))), null, lineColour, 0, Vector2.Zero, new Vector2(rowlength, 1), SpriteEffects.None, 0);
                Vector2 position;
                if(i>1)
                {
                    position=new Vector2(scoreorgx + columnGap / 2, scoreorgy + 30 + (rowGap * (i - 1)) - rowGap/2); 
                    if(i==2)
                        spriteBatch.Draw(p1Texture, position, null, Color.White, 0, playerCentre, playerScale, SpriteEffects.FlipVertically, 1);
                    if(i==3)
                        spriteBatch.Draw(p2Texture, position, null, Color.White, 0, playerCentre, playerScale, SpriteEffects.FlipVertically, 1);
                    if(i==4)
                        spriteBatch.Draw(p3Texture, position, null, Color.White, 0, playerCentre, playerScale, SpriteEffects.FlipVertically, 1);
                    if(i==5)
                        spriteBatch.Draw(p4Texture, position, null, Color.White, 0, playerCentre, playerScale, SpriteEffects.FlipVertically, 1);
                    if(i==6)
                        spriteBatch.Draw(p5Texture, position, null, Color.White, 0, playerCentre, playerScale, SpriteEffects.FlipVertically, 1);
                }
            }
            //draw columns
            for (int i = 1; i < 6; i++)
            {
                spriteBatch.Draw(blank, new Vector2(scoreorgx + (columnGap*(i-1)), scoreorgy), null, lineColour, 0, Vector2.Zero, new Vector2(1, (noPlayers)*(rowGap)+30), SpriteEffects.None, 0);
            }
            
            //fill with text

            //titles alligned center
            spriteBatch.DrawString(title, "Player", new Vector2(scoreorgx + columnGap/2 -title.MeasureString("Player").X/2, scoreorgy), Color.WhiteSmoke);
            spriteBatch.DrawString(title, "Points", new Vector2(scoreorgx + columnGap*3/2 - title.MeasureString("Points").X / 2, scoreorgy), Color.WhiteSmoke);
            spriteBatch.DrawString(title, "Coins $", new Vector2(scoreorgx + columnGap*5/2 - title.MeasureString("Coins $").X / 2, scoreorgy), Color.WhiteSmoke);
            spriteBatch.DrawString(title, "Life %", new Vector2(scoreorgx + columnGap*7/2 - title.MeasureString("Life %").X / 2, scoreorgy), Color.WhiteSmoke);
            for (int i = 1; i < noPlayers + 1; i++)
            {
                if (i == 1)
                    fillscore(p0, 1,scoreorgx,scoreorgy,rowGap,columnGap);
                else if (i == 2)
                    fillscore(p1, 2, scoreorgx, scoreorgy, rowGap, columnGap);
                else if (i == 3)
                    fillscore(p2, 3, scoreorgx, scoreorgy, rowGap, columnGap);
                else if (i == 4)
                    fillscore(p3, 4, scoreorgx, scoreorgy, rowGap, columnGap);
                else if (i == 5)
                    fillscore(p4, 5, scoreorgx, scoreorgy, rowGap, columnGap);
            }
        }

        private void fillscore(PlayerInfo player, int playerno,int scoreorgx,int scoreorgy,int rowGap,int columnGap)
        {
            //Allignment: Right
            spriteBatch.DrawString(body, player.points.ToString(), new Vector2(scoreorgx + columnGap * 2 - body.MeasureString(player.points.ToString()).X-5, scoreorgy + rowGap * playerno), Color.WhiteSmoke);
            spriteBatch.DrawString(body, player.coins.ToString(), new Vector2(scoreorgx + columnGap * 3 - body.MeasureString(player.coins.ToString()).X-5, scoreorgy + rowGap * playerno), Color.WhiteSmoke);
            if (player.health > 0)
                spriteBatch.DrawString(body, player.health.ToString(), new Vector2(scoreorgx + columnGap * 4 - body.MeasureString(player.health.ToString()).X - 5, scoreorgy + rowGap * playerno), Color.WhiteSmoke);
            else
            {
                spriteBatch.DrawString(body, "Dead", new Vector2(scoreorgx + columnGap * 4 - body.MeasureString("Dead").X - 5, scoreorgy + rowGap * playerno), Color.WhiteSmoke);
                
            }
        }


        private void updateGrid()
        {
            //String msg = gs.getMsg();       
        }

        private Vector2 getTextLocationCenter(int x, int y, string text)//align center
        {
            return new Vector2(Game1.gridOriginx + (x + .5f) * gridSize / columnsGrid - celltext.MeasureString(text).X / 2, Game1.gridOriginy + (y + .5f) * gridSize / columnsGrid - celltext.MeasureString(text).Y / 2);
        }

        private void findDistances(int sourceX, int sourceY, int sourceOrientation)
        {
            for (int i = 0; i < columnsGrid; i++)
            {
                for (int j = 0; j < columnsGrid; j++)
                {
                    myDistances[i, j].distances[0] = myDistances[i, j].distances[1] = myDistances[i, j].distances[2] = myDistances[i, j].distances[3] = myDistances[i, j].min = int.MaxValue;
                }
            }
            Queue<int[]> q = new Queue<int[]>();
            q.Enqueue(new int[] { sourceX, sourceY });

            myDistances[sourceY, sourceX].setDistance(0, sourceOrientation == 0 ? 0 : 1, -1);
            myDistances[sourceY, sourceX].setDistance(1, sourceOrientation == 1 ? 0 : 1, -1);
            myDistances[sourceY, sourceX].setDistance(2, sourceOrientation == 2 ? 0 : 1, -1);
            myDistances[sourceY, sourceX].setDistance(3, sourceOrientation == 3 ? 0 : 1, -1);

            int[] tmp;
            int parentDistance;
            int tmpMov;

            while (q.Count > 0)
            {
                tmp = q.Dequeue();
                parentDistance = myDistances[tmp[1], tmp[0]].getDistance(0);
                if (tmp[1] > 0 && !barriers.Contains(arena[tmp[1] - 1, tmp[0]]))
                {
                    if (myDistances[tmp[1] - 1, tmp[0]].getDistance(0) > parentDistance + 2)
                    {
                        if (myDistances[tmp[1], tmp[0]].moveTo == -1)
                        {
                            tmpMov = 0;
                        }
                        else
                        {
                            tmpMov = myDistances[tmp[1], tmp[0]].move[0];
                        }

                        myDistances[tmp[1] - 1, tmp[0]].setDistance(0, parentDistance + 1, tmpMov);
                        myDistances[tmp[1] - 1, tmp[0]].setDistance(1, parentDistance + 2, tmpMov);
                        myDistances[tmp[1] - 1, tmp[0]].setDistance(2, parentDistance + 2, tmpMov);
                        myDistances[tmp[1] - 1, tmp[0]].setDistance(3, parentDistance + 2, tmpMov);
                        
                        q.Enqueue(new int[] { tmp[0], tmp[1] - 1 });
                    }
                    else if (myDistances[tmp[1] - 1, tmp[0]].getDistance(0) > parentDistance + 1)
                    {
                        myDistances[tmp[1] - 1, tmp[0]].setDistance(0, parentDistance + 1, myDistances[tmp[1], tmp[0]].move[0]);
                        q.Enqueue(new int[] { tmp[0], tmp[1] - 1 });
                    }
                }

                parentDistance = myDistances[tmp[1], tmp[0]].getDistance(1);
                if (tmp[0] < columnsGrid - 1 && !barriers.Contains(arena[tmp[1], tmp[0] + 1]))
                {
                    if (myDistances[tmp[1], tmp[0] + 1].getDistance(1) > parentDistance + 2)
                    {
                        if (myDistances[tmp[1], tmp[0]].moveTo == -1)
                        {
                            tmpMov = 1;
                        }
                        else
                        {
                            tmpMov = myDistances[tmp[1], tmp[0]].move[1];
                        }

                        myDistances[tmp[1], tmp[0] + 1].setDistance(0, parentDistance + 2, tmpMov);
                        myDistances[tmp[1], tmp[0] + 1].setDistance(1, parentDistance + 1, tmpMov);
                        myDistances[tmp[1], tmp[0] + 1].setDistance(2, parentDistance + 2, tmpMov);
                        myDistances[tmp[1], tmp[0] + 1].setDistance(3, parentDistance + 2, tmpMov);

                        q.Enqueue(new int[] { tmp[0] + 1, tmp[1] });
                    }
                    else if (myDistances[tmp[1], tmp[0] + 1].getDistance(1) > parentDistance + 1)
                    {
                        myDistances[tmp[1], tmp[0] + 1].setDistance(1, parentDistance + 1, myDistances[tmp[1], tmp[0]].move[1]);
                        q.Enqueue(new int[] { tmp[0] + 1, tmp[1] });
                    }
                }
                
                parentDistance = myDistances[tmp[1], tmp[0]].getDistance(2);
                if (tmp[1] < columnsGrid - 1 && !barriers.Contains(arena[tmp[1] + 1, tmp[0]]))
                {
                    if (myDistances[tmp[1] + 1, tmp[0]].getDistance(2) > parentDistance + 2)
                    {
                        if (myDistances[tmp[1], tmp[0]].moveTo == -1)
                        {
                            tmpMov = 2;
                        }
                        else
                        {
                            tmpMov = myDistances[tmp[1], tmp[0]].move[2];
                        }

                        myDistances[tmp[1] + 1, tmp[0]].setDistance(0, parentDistance + 2, tmpMov);
                        myDistances[tmp[1] + 1, tmp[0]].setDistance(1, parentDistance + 2, tmpMov);
                        myDistances[tmp[1] + 1, tmp[0]].setDistance(2, parentDistance + 1, tmpMov);
                        myDistances[tmp[1] + 1, tmp[0]].setDistance(3, parentDistance + 2, tmpMov);

                        q.Enqueue(new int[] { tmp[0], tmp[1] + 1 });
                    }
                    else if (myDistances[tmp[1] + 1, tmp[0]].getDistance(2) > parentDistance + 1)
                    {
                        myDistances[tmp[1] + 1, tmp[0]].setDistance(2, parentDistance + 1, myDistances[tmp[1], tmp[0]].move[2]);
                        q.Enqueue(new int[] { tmp[0], tmp[1] + 1 });
                    }
                }
                

                parentDistance = myDistances[tmp[1], tmp[0]].getDistance(3);
                if (tmp[0] > 0 && !barriers.Contains(arena[tmp[1], tmp[0] - 1]))
                {
                    if (myDistances[tmp[1], tmp[0] - 1].getDistance(3) > parentDistance + 2)
                    {
                        if (myDistances[tmp[1], tmp[0]].moveTo == -1)
                        {
                            tmpMov = 3;
                        }
                        else
                        {
                            tmpMov = myDistances[tmp[1], tmp[0]].move[3];
                        }

                        myDistances[tmp[1], tmp[0] - 1].setDistance(0, parentDistance + 2, tmpMov);
                        myDistances[tmp[1], tmp[0] - 1].setDistance(1, parentDistance + 2, tmpMov);
                        myDistances[tmp[1], tmp[0] - 1].setDistance(2, parentDistance + 2, tmpMov);
                        myDistances[tmp[1], tmp[0] - 1].setDistance(3, parentDistance + 1, tmpMov);

                        q.Enqueue(new int[] { tmp[0] - 1, tmp[1] });
                    }
                    else if (myDistances[tmp[1], tmp[0] - 1].getDistance(3) > parentDistance + 1)
                    {
                        myDistances[tmp[1], tmp[0] - 1].setDistance(3, parentDistance + 1, myDistances[tmp[1], tmp[0]].move[3]);
                        q.Enqueue(new int[] { tmp[0] - 1, tmp[1] });
                    }
                }

            }
        }

    }

    class Cell
    {
        public int[] distances = { int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue };
        public int[] move= new int[4];
        public int min = int.MaxValue;
        public int moveTo;//this is the move need to approch the cell. 0- up, 1-right..... 

        public void setDistance(int orientation, int distance,int mov)
        {
            distances[orientation] = distance;
            move[orientation]=mov;
            if (distance < min)
            {
                min = distance;
                moveTo = mov;

            }
        }
        public int getDistance(int orientation)
        {
            return distances[orientation];
        }
    }



    public class PlayerInfo
    {
        public Vector2 position;
        public Point coordinates;
        public int direction, health = 0, coins = 0, points = 0;
        public Boolean shot = false;
        public Boolean participant = false;
        public PlayerInfo(int x, int y)
        {
            position = new Vector2(x, y);
        }
    }
    public class CoinsInfo
    {
        public Vector2 position;
        public int value;
        public double leaveat;
        public int x, y, lifetime;
        public CoinsInfo(float xpos, float ypos,int val, double lt,int x,int y,int lifetime)
        {
            value= val;
            leaveat=lt;
            position = new Vector2(xpos, ypos);
            this.x = x; this.y = y; this.lifetime = lifetime;
        }
    }
    public class LifepackInfo
    {
        public Vector2 position;
        public double leaveat;
        public int x, y, lifetime;
        public LifepackInfo(float xpos, float ypos, double lt, int x, int y, int lifetime)
        {
            leaveat = lt;
            position = new Vector2(xpos, ypos);
            this.x = x; this.y = y; this.lifetime = lifetime;
        }
    }
}
