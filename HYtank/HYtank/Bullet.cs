using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;

namespace HYtank
{
    public class Bullet
    {
        //the main timer which advances the bullet
        Timer timer = new Timer(250);

        //current postion, direction and coordinates
        public Vector2 position;
        public Point coordinates;
        public int direction;//0-up 1-right 2-down 3-left


        //creating and positioning of the bullet
        public Bullet(Point origin, int direction)
        {
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler(timerElapsed);
            this.direction = direction;
            coordinates = origin;
            position.X = Game1.gridOriginx + (coordinates.X + .5f) * Game1.gridSizeInPixels / Game1.columnsGrid;
            position.Y = Game1.gridOriginy + (coordinates.Y + .5f) * Game1.gridSizeInPixels / Game1.columnsGrid;
            Game1.bullets.AddLast(this);// adding the bullet to the list that has all the bullets on the map
            advance();// because we get the msg after the actual shooting occur
        }

        private void timerElapsed(Object sender, ElapsedEventArgs arg)
        {
            advance();
        }

        private void advance()
        {
            switch (direction)
            {
                case 0:
                    {
                        coordinates.Y--;
                        break;
                    }

                case 1:
                    {
                        coordinates.X++;
                        break;
                    }
                case 2:
                    {
                        coordinates.Y++;
                        break;
                    }
                case 3:
                    {
                        coordinates.X--;
                        break;
                    }
            }
            if (coordinates.X < Game1.columnsGrid && coordinates.Y < Game1.columnsGrid && coordinates.X >= 0 && coordinates.Y >= 0)
            {
                if ((Game1.arena[coordinates.Y, coordinates.X] != 'w' && Game1.arena[coordinates.Y, coordinates.X] != 'c' && Game1.arena[coordinates.Y, coordinates.X] != 'l' && Game1.arena[coordinates.Y, coordinates.X] != '\0') || Game1.tankGrid[coordinates.Y, coordinates.X] != -1)// check if it hits an obstacle
                {
                    Game1.bullets.Remove(this);
                    timer.Stop();
                    timer.Dispose();
                }
            }
            else
            {
                Game1.bullets.Remove(this);
                timer.Stop();
                timer.Dispose();
            }
            position.X = Game1.gridOriginx + (coordinates.X + .5f) * Game1.gridSizeInPixels / Game1.columnsGrid;
            position.Y = Game1.gridOriginy + (coordinates.Y + .5f) * Game1.gridSizeInPixels / Game1.columnsGrid;
        }
    }
}
