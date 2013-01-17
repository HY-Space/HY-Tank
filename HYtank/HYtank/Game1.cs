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

namespace HYtank
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        static char[,] arena = new char[20, 20];
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        Texture2D backgroundTexture;
        Texture2D waterTexture, stoneTexture, brickTexture, brick1Texture, brick2Texture,brick3Texture, tankTexture, p1Texture, p2Texture, p3Texture, p4Texture, p5Texture;
        int screenWidth;
        int screenHeight;
        int rowsGrid = 20;
        int columnsGrid = 20;
        int gridSize = 660;//600
        public static int gridOriginx = 6;
        public static int gridOriginy = 6;
        int cellWidth, cellHeight;
        double lastCmdAt = 0;
        public static int noPlayers = 0;
        public static GameTime time=new GameTime();
        public static SortedList<CoinsInfo,Int32> coinsList=new SortedList<CoinsInfo,Int32>();
        public static SortedList<LifepackInfo,Int32> lifeList=new SortedList<LifepackInfo,Int32>();

        SpriteFont title,body,celltext;

        GameSocket gs = new GameSocket();

        PlayerInfo p0 = new PlayerInfo(-1, -1), p1 = new PlayerInfo(-1, -1), p2 = new PlayerInfo(-1, -1), p3 = new PlayerInfo(-1, -1), p4 = new PlayerInfo(-1, -1);
        Vector2 tankCentre;
        float tankScale;
        Vector2 playerCentre;
        float playerScale;

        float angle;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            cellWidth = gridSize / columnsGrid;
            cellHeight = gridSize / rowsGrid;
            //arena[0, 1] = 's'; arena[9, 0] = 's'; arena[9, 1] = 's'; arena[9, 2] = 's'; arena[8, 1] = 's'; arena[8, 0] = 's'; arena[5, 0] = 'w'; arena[5, 1] = 'w'; arena[4, 0] = 'w'; arena[4, 1] = 'w'; arena[6, 9] = 'b'; arena[6, 8] = 'b'; arena[5, 9] = 'b'; arena[5, 8] = 'b';




            gs.setGrid(arena, p0, p1, p2, p3, p4, gridSize, columnsGrid);
            gs.connectToServer();
            gs.joinGame();
            gs.initialize();

            tankCentre = new Vector2(49, 49);//origin needs to be defined with respect to the original image
            tankScale = cellWidth * 1f / 100;

            playerCentre = new Vector2(50, 50);//origin needs to be defined with respect to the original image
            
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


            ///////////////////////////////Test
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up))
            {
                if (gameTime.TotalGameTime.TotalSeconds - lastCmdAt > 1)
                {
                    lastCmdAt = gameTime.TotalGameTime.TotalSeconds;
                    gs.command("UP#");
                }
            }
            else if (keyState.IsKeyDown(Keys.Down))
            {
                if (gameTime.TotalGameTime.TotalSeconds - lastCmdAt > 1)
                {
                    lastCmdAt = gameTime.TotalGameTime.TotalSeconds;
                    gs.command("DOWN#");
                }
            }
            else if (keyState.IsKeyDown(Keys.Left))
            {
                if (gameTime.TotalGameTime.TotalSeconds - lastCmdAt > 1)
                {
                    lastCmdAt = gameTime.TotalGameTime.TotalSeconds;
                    gs.command("LEFT#");
                }
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                if (gameTime.TotalGameTime.TotalSeconds - lastCmdAt > 1)
                {
                    lastCmdAt = gameTime.TotalGameTime.TotalSeconds;
                    gs.command("RIGHT#");
                }
            }
            else if (keyState.IsKeyDown(Keys.Space))
            {
                if (gameTime.TotalGameTime.TotalSeconds - lastCmdAt > 1)
                {
                    lastCmdAt = gameTime.TotalGameTime.TotalSeconds;
                    gs.command("SHOOT#");
                }
            }



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
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
                                spriteBatch.DrawString(celltext, "25%", getTextLocationCenter(j,i,"25%"), Color.Black);
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
                                spriteBatch.DrawString(celltext, "75%", getTextLocationCenter(j, i, "75%"), Color.Black);
                                break;
                            }
                    }
                }
            }
            if (p0.position.X != -1 && p0.health>0)
            {
                spriteBatch.Draw(tankTexture, p0.position, null, Color.DeepSkyBlue, p0.direction * 1.57f, tankCentre, tankScale, SpriteEffects.FlipVertically, 1);
            }
            if (p1.position.X != -1 && p1.health>0)
            {
                spriteBatch.Draw(tankTexture, p1.position, null, Color.Crimson, p1.direction * 1.57f, tankCentre, tankScale, SpriteEffects.FlipVertically, 1);
            }
            if (p2.position.X != -1 && p2.health>0)
            {
                spriteBatch.Draw(tankTexture, p2.position, null, Color.Yellow, p2.direction * 1.57f, tankCentre, tankScale, SpriteEffects.FlipVertically, 1);
            }
            if (p3.position.X != -1 && p3.health>0)
            {
                spriteBatch.Draw(tankTexture, p3.position, null, Color.LawnGreen, p3.direction * 1.57f, tankCentre, tankScale, SpriteEffects.FlipVertically, 1);
            }
            if (p4.position.X != -1 && p4.health>0)
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
            if(player.health>0)
                spriteBatch.DrawString(body, player.health.ToString(), new Vector2(scoreorgx + columnGap * 4 - body.MeasureString(player.health.ToString()).X-5, scoreorgy + rowGap * playerno), Color.WhiteSmoke);
            else
                spriteBatch.DrawString(body, "Dead", new Vector2(scoreorgx + columnGap * 4 - body.MeasureString("Dead").X-5, scoreorgy + rowGap * playerno), Color.WhiteSmoke);
        }


        private void updateGrid()
        {
            //String msg = gs.getMsg();       
        }

        private Vector2 getTextLocationCenter(int x, int y, string text)//allign cebter
        {
            return new Vector2(Game1.gridOriginx + (x + .5f) * gridSize / columnsGrid- celltext.MeasureString(text).X / 2,Game1.gridOriginy + (y + .5f) * gridSize / columnsGrid);
        }

    }


    class PlayerInfo
    {
        public Vector2 position;
        public int direction, health = 0, coins = 0, points = 0;
        public Boolean shot = false;
        public Boolean participant = false;
        public PlayerInfo(float x, float y)
        {
            position = new Vector2(x, y);
        }
    }
    public class CoinsInfo
    {
        public Vector2 position;
        public int value, lifetime;
        public CoinsInfo(float x, float y,int val, int lft)
        {
            position = new Vector2(x, y);
            value= val;
            lifetime=lft;
        }
    }
    public class LifepackInfo
    {
        public Vector2 position;
        public int lifetime;
        public LifepackInfo(float x, float y,int lft)
        {
            position = new Vector2(x, y);
            lifetime=lft;
        }
    }
}
