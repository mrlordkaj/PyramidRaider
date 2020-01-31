using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PyramidRaiderLevelEditor.Solver
{
    public class MummyRed : Mummy
    {
        public MummyRed(int x, int y)
            : base(x, y)
        {
        }

        public MummyRed mummyRedMove(short[][][] maze, Human man)
        {// Vertical then horizontal

            MummyRed tam = new MummyRed(this.getX(), this.getY());

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
                    while (tam.getX() != man.getX())
                    {
                        tam = (MummyRed)tam.mummyMoveVertical(maze, man);
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
                    tam = (MummyRed)tam.mummyMoveHorizontal(maze, man);
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
