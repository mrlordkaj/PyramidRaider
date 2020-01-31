using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PyramidRaider
{
    class UndoData
    {
        public int[] Explorer { get; set; } //row, col
        public List<int[]> Mummies { get; set; } //row, col, type
        public List<int[]> Scorpions { get; set; } //row, col, type
        public bool GateIsOpen { get; set; }
    }
}
