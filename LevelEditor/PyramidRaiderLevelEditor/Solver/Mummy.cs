using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PyramidRaiderLevelEditor.Solver
{
    public class Mummy : LivingThings
    {
        private int x, y;
        private static int attempt, count;

        public Mummy(int x, int y)
            : base(x, y)
        {
        }

        protected Mummy mummyMoveVertical(short[][][] maze, Human man)
        {
            if (count == 2) { return this; }
            { //Vertical move
                int newX;
                newX = this.getX() + sign(man.getX() - this.getX());
                int newY = this.getY();
                if (eligibleLivingThingsMove(maze, this.getX(), this.getY(), newX, newY))
                {
                    this.move(newX, newY);
                    count++;
                    if (count == 2)
                    {
                        return this;
                    }
                }
                else
                {
                    attempt++;
                }
            }
            return this;
        }

        protected Mummy mummyMoveHorizontal(short[][][] maze, Human man)
        {
            if (count == 2) { return this; }
            { //Horizontal move
                int newX = this.getX();
                int newY = this.getY() + sign(man.getY() - this.getY());
                if (eligibleLivingThingsMove(maze, this.getX(), this.getY(), newX, newY))
                {
                    this.move(newX, newY);
                    count++;
                    if (count == 2) { return this; }
                }
                else
                {
                    attempt++;
                }
            }
            return this;
        }

        protected static int sign(int x)
        { //If x = 0, return x;
            if (x == 0) return x;
            return x / Math.Abs(x);
        }

        //Getter and Setter Auto Generated Code

        public static int getAttempt()
        {
            return attempt;
        }

        public static void setAttempt(int attempt)
        {
            Mummy.attempt = attempt;
        }

        public static int getCount()
        {
            return count;
        }

        public static void setCount(int count)
        {
            Mummy.count = count;
        }
    }
}
