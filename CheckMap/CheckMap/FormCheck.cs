using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CheckMap
{
    public partial class FormCheck : Form
    {
        public FormCheck()
        {
            InitializeComponent();
        }

        private void FormCheck_Load(object sender, EventArgs e)
        {
            fixGateError();
            return;


            int error = 0, missing = 0;
            for (int i = 0; i < 100; i++)
            {
                for (int j = 1; j <= 100; j++)
                {
                    string path = string.Format("pack/{0}/{1}.dat", i, j);
                    if (File.Exists(path))
                    {
                        try
                        {
                            StreamReader reader = new StreamReader(path);
                            string maze = reader.ReadLine();
                            reader.ReadLine();
                            reader.ReadLine();
                            string mummy = reader.ReadLine();
                            reader.ReadLine();
                            string explorerAndEscape = reader.ReadLine();
                            string solution = reader.ReadLine();
                            if (maze.Length != 200 && maze.Length != 128 && maze.Length != 72)
                            {
                                txtError.Text += i + "-" + j + ": Maze size wrong\r\n";
                                error++;
                            }
                            if (mummy.Length < 3)
                            {
                                txtError.Text += i + "-" + j + ": Missing mummy\r\n";
                                error++;
                            }
                            if (mummy.Length % 3 != 0)
                            {
                                txtError.Text += i + "-" + j + ": Mummy data wrong\r\n";
                                error++;
                            }
                            if (explorerAndEscape.Length != 4)
                            {
                                txtError.Text += i + "-" + j + ": Eplorer and escape error\r\n";
                                error++;
                            }
                            if (solution.Length == 0)
                            {
                                txtError.Text += i + "-" + j + ": Missing solution\r\n";
                                error++;
                            }
                            reader.Close();
                        }
                        catch
                        {
                            txtError.Text += i + "-" + j + ": Unknow error\r\n";
                            error++;
                        }
                    }
                    else
                    {
                        txtMissing.Text += i + "-" + j + "\r\n";
                        missing++;
                    }
                }
            }
            if (error == 0)
            {
                txtError.Text = "No error found";
            }
            if (missing == 0)
            {
                txtMissing.Text = "No missing found";
            }
        }

        private void fixGateError()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                string pack = i.ToString();
                if (pack.Length == 1) pack = "0" + pack;
                using (var fs = new FileStream("data/mazes/B-" + pack + ".dat",
                              FileMode.Open,
                              FileAccess.ReadWrite))
                {
                    //init infos
                    string strByte = fs.ReadByte().ToString("x");
                    if (strByte.Length == 1) strByte = "0" + strByte;
                    bool isFlip = (strByte[0] == '1');
                    int mapSize = Convert.ToInt32(strByte[1].ToString(), 16);

                    int numMap = fs.ReadByte();
                    int numMummy = fs.ReadByte();
                    int numGate = fs.ReadByte();
                    int numTrap = fs.ReadByte();
                    int numScorpion = fs.ReadByte();

                    int mapCapacity;
                    if (mapSize < 10) mapCapacity = mapSize * 2 + numMummy + numGate * 2 + numTrap + numScorpion + 2;
                    else mapCapacity = mapSize * 4 + numMummy + numGate * 2 + numTrap + numScorpion + 2;

                    for (int j = 1; j <= 100; j++)
                    {
                        string path = string.Format("pack/{0}/{1}.dat", i, j);
                        if (File.Exists(path))
                        {
                            StreamReader reader = new StreamReader(path);
                            string maze = reader.ReadLine();
                            string trap = reader.ReadLine();
                            string scorpion = reader.ReadLine();
                            string mummy = reader.ReadLine();
                            string gate = reader.ReadLine();
                            string explorerAndEscape = reader.ReadLine();
                            string solution = reader.ReadLine();
                            reader.Close();

                            fs.Position = mapCapacity * (j - 1) + 6;
                            int[] escapeData, escapeCell, explorerData;
                            escapeData = new int[2] { -1, -1 };
                            if (!isFlip)
                            {
                                //wall
                                if (mapSize < 10) fs.Position += 2 * mapSize;
                                else fs.Position += 4 * mapSize;
                                //escape
                                strByte = fs.ReadByte().ToString("x");
                                if (strByte.Length == 1) strByte = "0" + strByte;
                                int cellId = Convert.ToInt32(strByte[0].ToString(), 16);

                                switch (Convert.ToInt32(strByte[1].ToString(), 16))
                                {
                                    case 0:
                                        escapeData = new int[2] { 0, cellId };
                                        //if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 3;
                                        //if (cellId == mapSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 1;
                                        escapeCell = new int[2] { 0, cellId };
                                        break;

                                    case 1:
                                        escapeData = new int[2] { 3, cellId };
                                        //if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 3;
                                        //if (cellId == mapSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 2 : 3;
                                        escapeCell = new int[] { cellId, 0 };
                                        break;

                                    case 2:
                                        escapeData = new int[2] { 1, cellId };
                                        //if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 1;
                                        //if (cellId == mapSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 1 : 2;
                                        escapeCell = new int[2] { cellId, mapSize - 1 };
                                        break;

                                    case 3:
                                        escapeData = new int[2] { 2, cellId };
                                        //if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 2 : 3;
                                        //if (cellId == mapSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 1 : 2;
                                        escapeCell = new int[2] { mapSize - 1, cellId };
                                        break;
                                }
                                //explorer
                                strByte = fs.ReadByte().ToString("x");
                                if (strByte.Length == 1) strByte = "0" + strByte;
                                explorerData = new int[2] { Convert.ToInt32(strByte[0].ToString(), 16), Convert.ToInt32(strByte[1].ToString(), 16) };
                                //mummy
                                fs.Position += numMummy;
                                //scorpion
                                fs.Position += numScorpion;
                                //trap
                                fs.Position += numTrap;
                                //gate and key
                                if (numGate == 1)
                                {
                                    strByte = fs.ReadByte().ToString("x");
                                    if (strByte.Length == 1) strByte = "0" + strByte;
                                    gate = Convert.ToInt32(strByte[0].ToString(), 16).ToString() + Convert.ToInt32(strByte[1].ToString(), 16).ToString();
                                    strByte = fs.ReadByte().ToString("x");
                                    if (strByte.Length == 1) strByte = "0" + strByte;
                                    gate = Convert.ToInt32(strByte[0].ToString(), 16).ToString() + Convert.ToInt32(strByte[1].ToString(), 16).ToString() + gate;
                                }
                                else
                                {
                                    gate = "";
                                }
                            }
                            else
                            {
                                //wall
                                if (mapSize < 10) fs.Position += 2 * mapSize;
                                else fs.Position += 4 * mapSize;
                                //escape
                                strByte = fs.ReadByte().ToString("x");
                                if (strByte.Length == 1) strByte = "0" + strByte;
                                int cellId = Convert.ToInt32(strByte[0].ToString(), 16);
                                switch (Convert.ToInt32(strByte[1].ToString(), 16))
                                {
                                    case 0:
                                        escapeData = new int[2] { 1, cellId };
                                        //if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 1;
                                        //if (cellId == mapSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 1 : 2;
                                        escapeCell = new int[2] { cellId, mapSize - 1 };
                                        break;

                                    case 1:
                                        escapeData = new int[2] { 0, mapSize - cellId - 1 };
                                        //if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 1;
                                        //if (cellId == mapSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 3;
                                        escapeCell = new int[2] { 0, mapSize - cellId - 1 };
                                        break;

                                    case 2:
                                        escapeData = new int[2] { 2, mapSize - cellId - 1 };
                                        //if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 1 : 2;
                                        //if (cellId == mapSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 2 : 3;
                                        escapeCell = new int[2] { mapSize - 1, mapSize - cellId - 1 };
                                        break;

                                    case 3:
                                        escapeData = new int[2] { 3, cellId };
                                        //if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 3;
                                        //if (cellId == mapSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 2 : 3;
                                        escapeCell = new int[2] { cellId, 0 };
                                        break;
                                }
                                //explorer
                                strByte = fs.ReadByte().ToString("x");
                                if (strByte.Length == 1) strByte = "0" + strByte;
                                explorerData = new int[2] { Convert.ToInt32(strByte[1].ToString(), 16), mapSize - Convert.ToInt32(strByte[0].ToString(), 16) - 1 };
                                //mummy
                                fs.Position += numMummy;
                                //scorpion
                                fs.Position += numScorpion;
                                //trap
                                fs.Position += numTrap;
                                //gate and key
                                if (numGate == 1)
                                {
                                    strByte = fs.ReadByte().ToString("x");
                                    if (strByte.Length == 1) strByte = "0" + strByte;
                                    gate = Convert.ToInt32(strByte[1].ToString(), 16).ToString() + (mapSize - Convert.ToInt32(strByte[0].ToString(), 16) - 1).ToString();
                                    strByte = fs.ReadByte().ToString("x");
                                    if (strByte.Length == 1) strByte = "0" + strByte;
                                    gate = Convert.ToInt32(strByte[1].ToString(), 16).ToString() + (mapSize - Convert.ToInt32(strByte[0].ToString(), 16) - 1).ToString() + gate;
                                }
                                else
                                {
                                    gate = "";
                                }
                            }

                            StreamWriter writer = new StreamWriter(path);
                            writer.WriteLine(maze);
                            writer.WriteLine(trap);
                            writer.WriteLine(scorpion);
                            writer.WriteLine(mummy);
                            writer.WriteLine(gate);
                            writer.WriteLine(explorerData[0].ToString() + explorerData[1].ToString() + escapeData[0].ToString() + escapeData[1].ToString());
                            writer.Write(solution);
                            writer.Close();
                        }
                    }
                }
            }
        }
    }
}
