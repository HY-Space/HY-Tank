using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HYtank
{
    class AI
    {
        public static char[,] fullarena;
        public AI(int rows,int columns)
        {
            fullarena = new char[rows, columns];
        }
        public int nextMove(char[,] arena)
        {
            fullarena = arena;
            return bellmansValueIteration();//return the direction
        }
        private int bellmansValueIteration()
        {
            

            return 0;//return the direction
        }
    }
}
