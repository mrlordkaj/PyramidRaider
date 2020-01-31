using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PyramidRaiderLevelEditor.Solver
{
    public class MummyWhite : Mummy
    {
        public MummyWhite(int x, int y)
            : base(x, y)
        {
        }

        public MummyWhite mummyWhiteMove(short[][][] maze, Human man)
        {//Horizontal then vertical
            //This method was written using a different logic than the current one
            MummyWhite tam = new MummyWhite(this.getX(), this.getY());
            if (tam.samePlace(man))
            {
                return tam;
            }
            else
            {
                Mummy.setAttempt(0);
                Mummy.setCount(0);
                while ((Mummy.getAttempt() < 5) && (Mummy.getCount() < 1))
                {
                    while (tam.getY() != man.getY())
                    {
                        tam = (MummyWhite)tam.mummyMoveHorizontal(maze, man);
                        if (!(Mummy.getCount() < 1))
                        {
                            return tam;
                        }
                        if (Mummy.getAttempt() > 4) break;
                    }
                    if (tam.samePlace(man))
                    {
                        return tam;
                    }
                    tam = (MummyWhite)tam.mummyMoveVertical(maze, man);
                    if ((tam.samePlace(man)) || (!(Mummy.getCount() < 1)))
                    {
                        return tam;
                    }
                }
                return tam;
            }
        }
    }
}
