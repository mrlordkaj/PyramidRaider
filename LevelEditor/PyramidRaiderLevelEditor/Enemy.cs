using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PyramidRaiderLevelEditor
{
    class Enemy
    {
        CreateScene parent;
        public int[] Position;
        public int Type;
        public int MovementLeft;

        public Enemy(int row, int col, int type, CreateScene parent)
        {
            this.Position = new int[] { row, col };
            this.parent = parent;
            this.Type = type;
        }

        private bool testMoveUp()
        {
            int row = Position[0] - 1;
            int col = Position[1];
            if (parent.Gate != null)
            {
                if (parent.Gate[0] == row && parent.Gate[1] == col && parent.GateIsBlock) return false;
            }
            if (!parent.Cell[row, col, 1])
            {
                Position[0]--;
                return true;
            }
            return false;
        }

        private bool testMoveRight()
        {
            int row = Position[0];
            int col = Position[1];
            if (!parent.Cell[row, col, 0])
            {
                Position[1]++;
                return true;
            }
            return false;
        }

        private bool testMoveDown()
        {
            int row = Position[0];
            int col = Position[1];
            if (parent.Gate != null)
            {
                if (parent.Gate[0] == row && parent.Gate[1] == col && parent.GateIsBlock) return false;
            }
            if (!parent.Cell[row, col, 1])
            {
                Position[0]++;
                return true;
            }
            return false;
        }

        private bool testMoveLeft()
        {
            int row = Position[0];
            int col = Position[1] - 1;
            if (!parent.Cell[row, col, 0])
            {
                Position[1]--;
                return true;
            }
            return false;
        }

        public bool TakeMove()
        {
            //neu nhu da het luot di
            if (MovementLeft == 0) return false;

            MovementLeft--;
            int[] explorerPosition = parent.Explorer.Position;
            if (Type == 0)
            {	//neu la loai binh thuong
                //thu di chuyen ngang truoc
                if (explorerPosition[1] < Position[1])
                {
                    if (testMoveLeft()) return true;
                }
                else if (explorerPosition[1] > Position[1])
                {
                    if (testMoveRight()) return true;
                }
                //neu khong di chuyen ngang duoc thi thu chuyen sang doc
                if (explorerPosition[0] < Position[0])
                {
                    if (testMoveUp()) return true;
                }
                else if (explorerPosition[0] > Position[0])
                {
                    if (testMoveDown()) return true;
                }
            }
            else
            { //neu la loai mau do
                //thu di chuyen doc truoc
                if (explorerPosition[0] < Position[0])
                {
                    if (testMoveUp()) return true;
                }
                else if (explorerPosition[0] > Position[0])
                {
                    if (testMoveDown()) return true;
                }
                //neu khong di chuyen doc duoc thi thu chuyen sang ngang
                if (explorerPosition[1] < Position[1])
                {
                    if (testMoveLeft()) return true;
                }
                else if (explorerPosition[1] > Position[1])
                {
                    if (testMoveRight()) return true;
                }
            }
            //neu khong di chuyen duoc them thi bo han
            MovementLeft = 0;
            return false;
        }
    }
}
