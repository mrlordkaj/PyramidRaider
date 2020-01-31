using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PyramidRaiderLevelEditor
{
    class Explorer
    {
        CreateScene parent;
        public int[] Position;

        public Explorer(int row, int col, CreateScene parent)
        {
            this.Position = new int[] { row, col };
            this.parent = parent;
        }

        public bool TestMoveUp()
        {
            if (Position[0] > 0)
            {
                var row = Position[0] - 1;
                var col = Position[1];
                if (parent.Gate != null)
                {
                    if (parent.Gate[0] == row && parent.Gate[1] == col && parent.GateIsBlock) return false;
                }
                if (!parent.Cell[row, col, 1] && !isCellHasEnemyOrTrap(Position[0] - 1, Position[1]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TestMoveRight()
        {
            if (Position[1] < parent.MazeSize - 1)
            {
                var row = Position[0];
                var col = Position[1];
                if (!parent.Cell[row, col, 0] && !isCellHasEnemyOrTrap(Position[0], Position[1] + 1))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TestMoveDown()
        {
            if (Position[0] < parent.MazeSize - 1)
            {
                var row = Position[0];
                var col = Position[1];
                if (parent.Gate != null)
                {
                    if (parent.Gate[0] == row && parent.Gate[1] == col && parent.GateIsBlock) return false;
                }
                if (!parent.Cell[row, col, 1] && !isCellHasEnemyOrTrap(Position[0] + 1, Position[1]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TestMoveLeft()
        {
            if (Position[1] > 0)
            {
                var row = Position[0];
                var col = Position[1] - 1;
                if (!parent.Cell[row, col, 0] && !isCellHasEnemyOrTrap(Position[0], Position[1] - 1))
                {
                    return true;
                }
            }
            return false;
        }

        private bool isCellHasEnemyOrTrap(int row, int col)
        {
            foreach (Enemy mummy in parent.Mummies)
            {
                if (mummy.Position[0] == row && mummy.Position[1] == col) return true;
            }
            foreach (Enemy scorpion in parent.Scorpions)
            {
                if (scorpion.Position[0] == row && scorpion.Position[1] == col) return true;
            }
            foreach (int[] trap in parent.Traps)
            {
                if (trap[0] == row && trap[1] == col) return true;
            }
            return false;
        }
    }
}
