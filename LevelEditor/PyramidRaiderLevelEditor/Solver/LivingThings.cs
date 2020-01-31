using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PyramidRaiderLevelEditor.Solver
{
    public class LivingThings
    {
        private int x, y;

        protected LivingThings(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        protected static int digit(int x, int n)
        { //return the nth digit
            //return ((x / (((int) Math.pow(10, n-1))))) % 10;
            return (x >> (n - 1)) % 2;
        }

        public bool samePlace(LivingThings thing)
        {
            return ((this.getX() == thing.getX()) && (this.getY() == thing.getY()));
        }

        protected static bool eligibleLivingThingsMove(short[][][] maze, int x, int y, int newX, int newY)
        {

            try
            { //out of maze check
                int i = maze[newX][newY][0];
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
            // Wall check 1111 last four digits: up, right, down, left
            if (newX == x + 1)
            { //move down
                if (digit(maze[x][y][0], 2) == 1)
                {
                    return false;
                }
            }
            else if (newX == x - 1)
            { //move up
                if (digit(maze[x][y][0], 4) == 1)
                {
                    return false;
                }
            }
            else if (newY == y + 1)
            {//move right
                if (digit(maze[x][y][0], 3) == 1)
                {
                    return false;
                }
            }
            else if (newY == y - 1)
            {//move left
                if (digit(maze[x][y][0], 1) == 1)
                {
                    return false;
                }
            }
            return true;
        }

        public void move(int newX, int newY)
        {
            this.setX(newX);
            this.setY(newY);
        }

        public int getX()
        {
            return this.x;
        }

        public int getY()
        {
            return this.y;
        }

        public void setX(int x)
        {
            this.x = x;
        }

        public void setY(int y)
        {
            this.y = y;
        }
    }
}
