using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenitvnGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using PyramidRaiderLevelEditor.Solver;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using Microsoft.Xna.Framework.Audio;

namespace PyramidRaiderLevelEditor
{
    class CreateScene : GameScene
    {
        enum CreateState { Edit, Move, Batch }
        enum MazeType { Six = 6, Eight = 8, Ten = 10 }
        enum Element { Wall, Escape, Trap, Key, Gate, Explorer, NormalMummy, BloodMummy, NormalScorpion, BloodScorpion }

        Texture2D texRadioSelect, texRadioUnselect;
        Texture2D texSave, texStart, texUp, texDown, texHack, texBatch;
        Texture2D texBlank;

        Texture2D texWall, texEscape, texTrap, texKey, texGate;
        Texture2D texExplorer, texNormalMummy, texBloodMummy, texNormalScorpion, texBloodScorpion;
        Texture2D texFloor6, texFloor8, texFloor10;

        SoundEffect sndShutdown;

        Dictionary<string, Texture2D> texTiles = new Dictionary<string, Texture2D>();
        SpriteFont fntSmall;

        CreateState _state = CreateState.Edit;

        int elementIconSize = 30;
        int radioSpan = 6;

        Rectangle rec6x6 = new Rectangle(370, 10, 70, 30);
        Rectangle rec8x8 = new Rectangle(445, 10, 70, 30);
        Rectangle rec10x10 = new Rectangle(520, 10, 70, 30);

        Rectangle recPack = new Rectangle(370, 60, 40, 30);
        Rectangle recPackUp = new Rectangle(410, 60, 25, 15);
        Rectangle recPackDown = new Rectangle(410, 75, 25, 15);

        Rectangle recLevel = new Rectangle(445, 60, 40, 30);
        Rectangle recLevelUp = new Rectangle(485, 60, 25, 15);
        Rectangle recLevelDown = new Rectangle(485, 75, 25, 15);

        Rectangle recHack = new Rectangle(520, 60, 70, 30);
        Rectangle recStart = new Rectangle(370, 320, 70, 30);
        Rectangle recSave = new Rectangle(445, 320, 70, 30);
        Rectangle recBatch = new Rectangle(520, 320, 70, 30);

        Rectangle recWall = new Rectangle(370, 120, 80, 30);
        Rectangle recEscape = new Rectangle(370, 160, 80, 30);
        Rectangle recTrap = new Rectangle(370, 200, 80, 30);
        Rectangle recKey = new Rectangle(370, 240, 80, 30);
        Rectangle recGate = new Rectangle(370, 280, 80, 30);

        Rectangle recExplorer = new Rectangle(500, 120, 80, 30);
        Rectangle recNormalMummy = new Rectangle(500, 160, 80, 30);
        Rectangle recBloodMummy = new Rectangle(500, 200, 80, 30);
        Rectangle recNormalScorpion = new Rectangle(500, 240, 80, 30);
        Rectangle recBloodScorpion = new Rectangle(500, 280, 80, 30);

        Rectangle recMaze = new Rectangle(0, 0, 360, 360);

        MazeType _mazeSize;
        int tileWidth, tileHeight;
        Element currentElement;

        bool[, ,] mazeData;
        int[] escapeCell, escapeData, keyData, gateData, explorerData;
        List<int[]> trapData, mummyData, scorpionData;

        Random rand = new Random();

        Vector2 vtTileCenter, vtEscapeCenter;

        //List<string> mazeDataRecords = new List<string>();

        public CreateScene()
            : base(Main.GetInstance())
        {
            readProcess();

            switchMazeSize(MazeType.Six);
            currentElement = Element.Wall;
        }

        protected override void prepareContent()
        {
            fntSmall = content.Load<SpriteFont>("Fonts/small");
            texRadioSelect = content.Load<Texture2D>("Images/radioSelect");
            texRadioUnselect = content.Load<Texture2D>("Images/radioUnselect");
            texTiles["00"] = content.Load<Texture2D>("Images/tile00");
            texTiles["10"] = content.Load<Texture2D>("Images/tile10");
            texTiles["01"] = content.Load<Texture2D>("Images/tile01");
            texTiles["11"] = content.Load<Texture2D>("Images/tile11");
            texWall = content.Load<Texture2D>("Images/wall");
            texEscape = content.Load<Texture2D>("Images/escape");
            texTrap = content.Load<Texture2D>("Images/trap");
            texKey = content.Load<Texture2D>("Images/key");
            texGate = content.Load<Texture2D>("Images/gate");
            texExplorer = content.Load<Texture2D>("Images/explorer");
            texNormalMummy = content.Load<Texture2D>("Images/normalMummy");
            texBloodMummy = content.Load<Texture2D>("Images/bloodMummy");
            texNormalScorpion = content.Load<Texture2D>("Images/normalScorpion");
            texBloodScorpion = content.Load<Texture2D>("Images/bloodScorpion");
            texSave = content.Load<Texture2D>("Images/btnSave");
            texStart = content.Load<Texture2D>("Images/btnStart");
            texBatch = content.Load<Texture2D>("Images/btnBatch");
            texUp = content.Load<Texture2D>("Images/btnUp");
            texDown = content.Load<Texture2D>("Images/btnDown");
            texHack = content.Load<Texture2D>("Images/btnHack");
            texFloor6 = content.Load<Texture2D>("Images/floor6");
            texFloor8 = content.Load<Texture2D>("Images/floor8");
            texFloor10 = content.Load<Texture2D>("Images/floor10");
            texBlank = content.Load<Texture2D>("Images/blank");
            sndShutdown = content.Load<SoundEffect>("Shutdown");

            vtEscapeCenter = new Vector2(texEscape.Width / 2, texEscape.Height / 2);
            RegisteredKeys = new Keys[] {
                Keys.Escape,
                Keys.Space,
                Keys.Up,
                Keys.Right,
                Keys.Down,
                Keys.Left
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            if (_state != CreateState.Batch)
            {
                //floor
                switch (MazeSize)
                {
                    case 6:
                        spriteBatch.Draw(texFloor6, recMaze, Color.White);
                        break;

                    case 8:
                        spriteBatch.Draw(texFloor8, recMaze, Color.White);
                        break;

                    case 10:
                        spriteBatch.Draw(texFloor10, recMaze, Color.White);
                        break;
                }
                //end floor

                //escape
                if (escapeCell != null) spriteBatch.Draw(texEscape, new Rectangle(escapeCell[1] * tileWidth + (int)vtTileCenter.X, escapeCell[0] * tileHeight + (int)vtTileCenter.Y, tileWidth, tileHeight), null, Color.White, MathHelper.ToRadians(escapeData[0] * 90), vtEscapeCenter, SpriteEffects.None, 1);
                //end escape

                //traps
                foreach (int[] trap in trapData)
                {
                    spriteBatch.Draw(texTrap, new Rectangle(trap[1] * tileWidth, trap[0] * tileHeight, tileWidth, tileHeight), Color.White);
                }
                //end traps

                switch (_state)
                {
                    case CreateState.Edit:
                    case CreateState.Batch:
                        //explorer
                        if (explorerData != null) spriteBatch.Draw(texExplorer, new Rectangle(explorerData[1] * tileWidth, explorerData[0] * tileHeight, tileWidth, tileHeight), Color.White);
                        //end explorer

                        //mummies
                        foreach (int[] mummy in mummyData)
                        {
                            spriteBatch.Draw(mummy[2] == 0 ? texNormalMummy : texBloodMummy, new Rectangle(mummy[1] * tileWidth, mummy[0] * tileHeight, tileWidth, tileHeight), Color.White);
                        }
                        //end mummies

                        //scorpions
                        foreach (int[] scorpion in scorpionData)
                        {
                            spriteBatch.Draw(scorpion[2] == 0 ? texNormalScorpion : texBloodScorpion, new Rectangle(scorpion[1] * tileWidth, scorpion[0] * tileHeight, tileWidth, tileHeight), Color.White);
                        }
                        //end scorpions

                        //key and gate
                        if (keyData != null) spriteBatch.Draw(texKey, new Rectangle(keyData[1] * tileWidth, keyData[0] * tileHeight, tileWidth, tileHeight), Color.White);
                        if (gateData != null) spriteBatch.Draw(texGate, new Rectangle(gateData[1] * tileWidth - (int)(tileWidth * 0.05f), gateData[0] * tileHeight - (int)(tileHeight * 0.25f), (int)(tileWidth * 1.2f), (int)(tileHeight * 1.3f)), Color.White);
                        //end key and gate
                        break;

                    case CreateState.Move:
                        //explorer
                        if (explorerData != null) spriteBatch.Draw(texExplorer, new Rectangle(Explorer.Position[1] * tileWidth, Explorer.Position[0] * tileHeight, tileWidth, tileHeight), Color.White);
                        //end explorer

                        //mummies
                        foreach (Enemy mummy in Mummies)
                        {
                            spriteBatch.Draw(mummy.Type == 0 ? texNormalMummy : texBloodMummy, new Rectangle(mummy.Position[1] * tileWidth, mummy.Position[0] * tileHeight, tileWidth, tileHeight), Color.White);
                        }
                        //end mummies

                        //scorpions
                        foreach (Enemy scorpion in Scorpions)
                        {
                            spriteBatch.Draw(scorpion.Type == 0 ? texNormalScorpion : texBloodScorpion, new Rectangle(scorpion.Position[1] * tileWidth, scorpion.Position[0] * tileHeight, tileWidth, tileHeight), Color.White);
                        }
                        //end scorpions

                        //key and gate
                        if (keyData != null) spriteBatch.Draw(texKey, new Rectangle(keyData[1] * tileWidth, keyData[0] * tileHeight, tileWidth, tileHeight), Color.White);
                        if (Gate != null && GateIsBlock) spriteBatch.Draw(texGate, new Rectangle(gateData[1] * tileWidth - (int)(tileWidth * 0.05f), gateData[0] * tileHeight - (int)(tileHeight * 0.25f), (int)(tileWidth * 1.2f), (int)(tileHeight * 1.3f)), Color.White);
                        //end key and gate
                        break;
                }

                //maze
                int size = (int)_mazeSize;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (mazeData[i, j, 0] || mazeData[i, j, 1])
                        {
                            Rectangle drawPosition = new Rectangle(j * tileWidth - (int)(tileWidth * 0.05f), i * tileHeight - (int)(tileHeight * 0.25f), (int)(tileWidth * 1.2f), (int)(tileHeight * 1.3f));
                            string tileType = (mazeData[i, j, 0] ? "1" : "0") + (mazeData[i, j, 1] ? "1" : "0");
                            spriteBatch.Draw(texTiles[tileType], drawPosition, Color.White);
                        }
                    }
                }
                //end maze

                spriteBatch.Draw(texBlank, new Rectangle(360, 0, 240, 360), Color.CornflowerBlue);

                //solution
                if (solution != null)
                {
                    StringBuilder strSolution = new StringBuilder();
                    foreach (int step in solution)
                    {
                        strSolution.Append(step);
                    }
                    spriteBatch.DrawString(fntSmall, strSolution, Vector2.Zero, Color.Orange);
                }
                //end solution

                //step
                if (steps != null)
                {
                    StringBuilder strStep = new StringBuilder();
                    foreach (int step in steps)
                    {
                        strStep.Append(step);
                    }
                    spriteBatch.DrawString(fntSmall, strStep, new Vector2(0, 20), Color.Violet);
                }
                //step

                //maze size setting
                spriteBatch.Draw(_mazeSize == MazeType.Six ? texRadioSelect : texRadioUnselect, new Rectangle((int)rec6x6.X, (int)rec6x6.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.DrawString(fntSmall, "6", new Vector2((int)rec6x6.X + elementIconSize + radioSpan, (int)rec6x6.Y), Color.White);
                spriteBatch.Draw(_mazeSize == MazeType.Eight ? texRadioSelect : texRadioUnselect, new Rectangle((int)rec8x8.X, (int)rec8x8.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.DrawString(fntSmall, "8", new Vector2((int)rec8x8.X + elementIconSize + radioSpan, (int)rec8x8.Y), Color.White);
                spriteBatch.Draw(_mazeSize == MazeType.Ten ? texRadioSelect : texRadioUnselect, new Rectangle((int)rec10x10.X, (int)rec10x10.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.DrawString(fntSmall, "10", new Vector2((int)rec10x10.X + elementIconSize + radioSpan, (int)rec10x10.Y), Color.White);
                //end maze size setting

                //element setting
                spriteBatch.Draw(currentElement == Element.Wall ? texRadioSelect : texRadioUnselect, new Rectangle((int)recWall.X, (int)recWall.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(texWall, new Rectangle(recWall.X + elementIconSize + radioSpan, recWall.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(currentElement == Element.Escape ? texRadioSelect : texRadioUnselect, new Rectangle((int)recEscape.X, (int)recEscape.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(texEscape, new Rectangle(recEscape.X + elementIconSize + radioSpan, recEscape.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(currentElement == Element.Trap ? texRadioSelect : texRadioUnselect, new Rectangle((int)recTrap.X, (int)recTrap.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(texTrap, new Rectangle(recTrap.X + elementIconSize + radioSpan, recTrap.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(currentElement == Element.Key ? texRadioSelect : texRadioUnselect, new Rectangle((int)recKey.X, (int)recKey.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(texKey, new Rectangle(recKey.X + elementIconSize + radioSpan, recKey.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(currentElement == Element.Gate ? texRadioSelect : texRadioUnselect, new Rectangle((int)recGate.X, (int)recGate.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(texGate, new Rectangle(recGate.X + elementIconSize + radioSpan, recGate.Y, elementIconSize, elementIconSize), Color.White);

                spriteBatch.Draw(currentElement == Element.Explorer ? texRadioSelect : texRadioUnselect, new Rectangle((int)recExplorer.X, (int)recExplorer.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(texExplorer, new Rectangle(recExplorer.X + elementIconSize + radioSpan, recExplorer.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(currentElement == Element.NormalMummy ? texRadioSelect : texRadioUnselect, new Rectangle((int)recNormalMummy.X, (int)recNormalMummy.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(texNormalMummy, new Rectangle(recNormalMummy.X + elementIconSize + radioSpan, recNormalMummy.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(currentElement == Element.BloodMummy ? texRadioSelect : texRadioUnselect, new Rectangle((int)recBloodMummy.X, (int)recBloodMummy.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(texBloodMummy, new Rectangle(recBloodMummy.X + elementIconSize + radioSpan, recBloodMummy.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(currentElement == Element.NormalScorpion ? texRadioSelect : texRadioUnselect, new Rectangle((int)recNormalScorpion.X, (int)recNormalScorpion.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(texNormalScorpion, new Rectangle(recNormalScorpion.X + elementIconSize + radioSpan, recNormalScorpion.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(currentElement == Element.BloodScorpion ? texRadioSelect : texRadioUnselect, new Rectangle((int)recBloodScorpion.X, (int)recBloodScorpion.Y, elementIconSize, elementIconSize), Color.White);
                spriteBatch.Draw(texBloodScorpion, new Rectangle(recBloodScorpion.X + elementIconSize + radioSpan, recBloodScorpion.Y, elementIconSize, elementIconSize), Color.White);
                //end element setting

                //pack button
                spriteBatch.Draw(texHack, recHack, Color.White);
                spriteBatch.Draw(texSave, recSave, Color.White);
                spriteBatch.Draw(texStart, recStart, (_state == CreateState.Move) ? Color.Red : Color.White);

                //pack setting
                spriteBatch.Draw(texUp, recPackUp, Color.White);
                spriteBatch.Draw(texDown, recPackDown, Color.White);
                spriteBatch.Draw(texUp, recLevelUp, Color.White);
                spriteBatch.Draw(texDown, recLevelDown, Color.White);
            }
            spriteBatch.Draw(texBlank, recPack, Color.Black);
            spriteBatch.DrawString(fntSmall, currentPack.ToString(), new Vector2(recPack.X + 4, recPack.Y), Color.White);
            spriteBatch.Draw(texBlank, recLevel, Color.Black);
            spriteBatch.DrawString(fntSmall, currentLevel.ToString(), new Vector2(recLevel.X + 4, recLevel.Y), Color.White);
            //end level setting
            spriteBatch.Draw(texBatch, recBatch, (_state == CreateState.Batch) ? Color.Red : Color.White);
            //end pack button

            spriteBatch.End();
        }

        protected override void performBack()
        {
            Main.GetInstance().Exit();
        }

        protected override void pointerReleased(int x, int y)
        {
            switch (_state)
            {
                case CreateState.Edit:
                    if (getSelectedTile(x, y)) return;

                    if (recLevelUp.Contains(x, y))
                    {
                        if (currentLevel < 100) currentLevel++;
                        return;
                    }

                    if (recLevelDown.Contains(x, y))
                    {
                        if (currentLevel > 1) currentLevel--;
                        return;
                    }

                    if (recPackUp.Contains(x, y))
                    {
                        if (currentPack < 99) currentPack++;
                        return;
                    }

                    if (recPackDown.Contains(x, y))
                    {
                        if (currentPack > 0) currentPack--;
                        return;
                    }

                    if (recHack.Contains(x, y))
                    {
                        hackMummyProcess();
                        return;
                    }

                    if (recBatch.Contains(x, y))
                    {
                        _state = CreateState.Batch;
                        batchThread = new Thread(beginBatch);
                        batchThread.Start();
                        return;
                    }

                    if (getSelectedMazeSize(x, y)) return;

                    if (getSelectedElement(x, y)) return;
                    break;

                case CreateState.Batch:
                    if (recBatch.Contains(x, y))
                    {
                        _state = CreateState.Edit;
                        StopBatch();
                    }
                    return;

                case CreateState.Move:
                    if (recSave.Contains(x, y))
                    {
                        save();
                        return;
                    }
                    break;
            }

            if (recStart.Contains(x, y))
            {
                if (_state == CreateState.Edit) beginPlay();
                else
                {
                    steps = new List<int>();
                    solution = new List<int>();
                    _state = CreateState.Edit;
                }
                return;
            }
        }

        Thread batchThread;

        private void beginBatch()
        {
            while (currentPack < 99 || currentLevel < 100)
            {
                string pack = currentPack.ToString();
                if (currentPack < 10) pack = "0" + pack;
                readMazeInformation(pack);

                if (!generateSolution() || !save())
                {
                    StreamWriter writer = new StreamWriter("error.log", true);
                    writer.WriteLine(currentPack + "-" + currentLevel);
                    writer.Close();
                }

                if (++currentLevel > 100)
                {
                    if (++currentPack < 100) currentLevel = 1;
                }

                sndShutdown.Play();
            }
            
            System.Windows.Forms.MessageBox.Show("Batch done!", "Success");
        }

        public void StopBatch()
        {
            if (batchThread != null && batchThread.ThreadState == System.Threading.ThreadState.Running)
            {
                batchThread.Abort();
            }
        }

        protected override void keyPressed(Keys key)
        {
            if (!isExplorerTurn || _state != CreateState.Move) return;

            switch (key)
            {
                case Keys.Up:
                    if (Explorer.TestMoveUp())
                    {
                        Explorer.Position[0]--;
                        steps.Add(0);
                        if (Gate != null)
                        {
                            if (Key[0] == Explorer.Position[0] && Key[1] == Explorer.Position[1]) GateIsBlock = !GateIsBlock;
                        }
                        switchToEnemyTurn();
                    }
                    break;

                case Keys.Right:
                    if (Explorer.TestMoveRight())
                    {
                        Explorer.Position[1]++;
                        steps.Add(1);
                        if (Gate != null)
                        {
                            if (Key[0] == Explorer.Position[0] && Key[1] == Explorer.Position[1]) GateIsBlock = !GateIsBlock;
                        }
                        switchToEnemyTurn();
                    }
                    break;

                case Keys.Down:
                    if (Explorer.TestMoveDown())
                    {
                        Explorer.Position[0]++;
                        steps.Add(2);
                        if (Gate != null)
                        {
                            if (Key[0] == Explorer.Position[0] && Key[1] == Explorer.Position[1]) GateIsBlock = !GateIsBlock;
                        }
                        switchToEnemyTurn();
                    }
                    break;

                case Keys.Left:
                    if (Explorer.TestMoveLeft())
                    {
                        Explorer.Position[1]--;
                        steps.Add(3);
                        if (Gate != null)
                        {
                            if (Key[0] == Explorer.Position[0] && Key[1] == Explorer.Position[1]) GateIsBlock = !GateIsBlock;
                        }
                        switchToEnemyTurn();
                    }
                    break;

                case Keys.Space:
                    switchToEnemyTurn();
                    steps.Add(4);
                    return;

                default:
                    return;
            }
        }

        private void switchMazeSize(MazeType newSize)
        {
            //create new maze
            _mazeSize = newSize;
            int size = (int)_mazeSize;
            tileWidth = recMaze.Width / size;
            tileHeight = recMaze.Height / size;
            mazeData = new bool[size, size, 2];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    mazeData[i, j, 0] = false;
                    mazeData[i, j, 1] = false;
                }
            }
            vtTileCenter = new Vector2(tileWidth / 2, tileHeight / 2);

            //reset all datas
            if (_state == CreateState.Move) _state = CreateState.Edit;
            currentElement = Element.Wall;
            escapeCell = null;
            escapeData = null;
            trapData = new List<int[]>();
            keyData = null;
            gateData = null;
            explorerData = null;
            mummyData = new List<int[]>();
            scorpionData = new List<int[]>();
            steps = new List<int>();
            solution = new List<int>();
        }

        private bool getSelectedTile(int x, int y)
        {
            if (recMaze.Contains(x, y))
            {
                int row = y / tileHeight;
                int col = x / tileWidth;
                switch (currentElement)
                {
                    case Element.Wall:
                        editWall(row, col);
                        break;

                    case Element.Escape:
                        editEscape(row, col);
                        break;

                    case Element.Trap:
                        editTrap(row, col);
                        break;

                    case Element.Key:
                        editKey(row, col);
                        break;

                    case Element.Gate:
                        editGate(row, col);
                        break;

                    case Element.Explorer:
                        editExplorer(row, col);
                        break;

                    case Element.NormalMummy:
                        editMummy(row, col, 0);
                        break;

                    case Element.BloodMummy:
                        editMummy(row, col, 1);
                        break;

                    case Element.NormalScorpion:
                        editScorpion(row, col, 0);
                        break;

                    case Element.BloodScorpion:
                        editScorpion(row, col, 1);
                        break;
                }
                return true;
            }
            return false;
        }

        private bool getSelectedMazeSize(int x, int y)
        {
            if (rec6x6.Contains(x, y))
            {
                switchMazeSize(MazeType.Six);
                return true;
            }
            if (rec8x8.Contains(x, y))
            {
                switchMazeSize(MazeType.Eight);
                return true;
            }
            if (rec10x10.Contains(x, y))
            {
                switchMazeSize(MazeType.Ten);
                return true;
            }
            return false;
        }

        private bool getSelectedElement(int x, int y)
        {
            if (recWall.Contains(x, y))
            {
                currentElement = Element.Wall;
                return true;
            }
            if (recEscape.Contains(x, y))
            {
                currentElement = Element.Escape;
                return true;
            }
            if (recTrap.Contains(x, y))
            {
                currentElement = Element.Trap;
                return true;
            }
            if (recKey.Contains(x, y))
            {
                currentElement = Element.Key;
                return true;
            }
            if (recGate.Contains(x, y))
            {
                currentElement = Element.Gate;
                return true;
            }
            if (recExplorer.Contains(x, y))
            {
                currentElement = Element.Explorer;
                return true;
            }
            if (recNormalMummy.Contains(x, y))
            {
                currentElement = Element.NormalMummy;
                return true;
            }
            if (recBloodMummy.Contains(x, y))
            {
                currentElement = Element.BloodMummy;
                return true;
            }
            if (recNormalScorpion.Contains(x, y))
            {
                currentElement = Element.NormalScorpion;
                return true;
            }
            if (recBloodScorpion.Contains(x, y))
            {
                currentElement = Element.BloodScorpion;
                return true;
            }
            return false;
        }

        #region remove laws
        private bool removeExplorer(int row, int col)
        {
            if (explorerData != null && explorerData[0] == row && explorerData[1] == col)
            {
                explorerData = null;
                return true;
            }
            return false;
        }

        private bool removeTrap(int row, int col)
        {
            foreach (int[] trap in trapData)
            {
                if (trap[0] == row && trap[1] == col)
                {
                    trapData.Remove(trap);
                    return true;
                }
            }
            return false;
        }

        private bool removeEscape(int row, int col)
        {
            if (escapeCell != null && escapeCell[0] == row && escapeCell[1] == col)
            {
                escapeCell = null;
                escapeData = null;
                return true;
            }
            return false;
        }

        private bool removeMummy(int row, int col, int type = -1)
        {
            foreach (int[] mummy in mummyData)
            {
                if (mummy[0] == row && mummy[1] == col)
                {
                    mummyData.Remove(mummy);
                    if (type == -1 || mummy[2] == type) return true;
                    else return false;
                }
            }
            return false;
        }

        private bool removeScorpion(int row, int col, int type = -1)
        {
            foreach (int[] scorpion in scorpionData)
            {
                if (scorpion[0] == row && scorpion[1] == col)
                {
                    scorpionData.Remove(scorpion);
                    if (type == -1 || scorpion[2] == type) return true;
                    else return false;
                }
            }
            return false;
        }

        private bool removeKey(int row, int col)
        {
            if (keyData != null && keyData[0] == row && keyData[1] == col)
            {
                keyData = null;
                return true;
            }
            return false;
        }
        #endregion

        #region edit
        private void editWall(int row, int col)
        {
            string tileType = (mazeData[row, col, 0] ? "1" : "0") + (mazeData[row, col, 1] ? "1" : "0");
            string nextTileType = string.Empty;
            switch (tileType)
            {
                case "00":
                    //if last col and not last row, skip to 01
                    if (col == MazeSize - 1 && row < MazeSize - 1) nextTileType = "01";
                    //if last col and last row, return
                    else if (col == MazeSize - 1 && row == MazeSize - 1) return;
                    //else next to 10
                    else nextTileType = "10";
                    break;

                case "10":
                    //if last row, skip to 00
                    if (row == MazeSize - 1) nextTileType = "00";
                    //else next to 11
                    else nextTileType = "01";
                    break;

                case "01":
                    //if last col, skip to 00
                    if (col == MazeSize - 1) nextTileType = "00";
                    //else next to 11
                    else nextTileType = "11";
                    break;

                case "11":
                    nextTileType = "00";
                    break;
            }
            mazeData[row, col, 0] = (nextTileType[0] == '1');
            mazeData[row, col, 1] = (nextTileType[1] == '1');

            string strMazeData = "";
            for (int i = 0; i < MazeSize; i++)
            {
                for (int j = 0; j < MazeSize; j++)
                {
                    strMazeData += mazeData[i, j, 0] ? "1" : "0";
                    strMazeData += mazeData[i, j, 1] ? "1" : "0";
                }
            }
            //if (mazeDataRecords.Contains(strMazeData))
            //{
            //    System.Windows.Forms.MessageBox.Show("Maze is exist", "Duplicate");
            //}
        }

        private void editEscape(int row, int col)
        {
            if (row != 0 && row != ((int)_mazeSize - 1) && col != 0 && col != ((int)_mazeSize - 1)) return;

            if (escapeData == null ||
                (escapeCell != null && (escapeCell[0] != row || escapeCell[1] != col)))
            {
                escapeData = new int[] { -1, -1 };
            }
            else return;

            escapeCell = new int[] { row, col };

            removeExplorer(row, col);
            removeTrap(row, col);

            if (row == 0) //first row
            {
                if (col == 0) //first row, first col
                {
                    //if (escapeData[0] == -1 || escapeData[0] == 3) escapeData = new int[] { 0, col };
                    //else escapeData = new int[] { 3, row };
                    if (rand.Next(0, 2) == 1) escapeData = new int[] { 0, col };
                    else escapeData = new int[] { 3, row };
                }
                else if (col == ((int)_mazeSize - 1)) //first row, last col
                {
                    //if (escapeData[0] == -1 || escapeData[0] == 1) escapeData = new int[] { 0, col };
                    //else escapeData = new int[] { 1, row };
                    if (rand.Next(0, 2) == 1) escapeData = new int[] { 0, col };
                    else escapeData = new int[] { 1, row };
                }
                else
                {
                    escapeData = new int[] { 0, col };
                }
                return;
            }
            if (row == ((int)_mazeSize - 1)) //last row
            {
                if (col == 0) //last row, first col
                {
                    //if (escapeData[0] == -1 || escapeData[0] == 3) escapeData = new int[] { 2, col };
                    //else escapeData = new int[] { 3, row };
                    if (rand.Next(0, 2) == 1) escapeData = new int[] { 2, col };
                    else escapeData = new int[] { 3, row };
                }
                else if (col == ((int)_mazeSize - 1)) //last row, last col
                {
                    //if (escapeData[0] == -1 || escapeData[0] == 2) escapeData = new int[] { 1, row };
                    //else escapeData = new int[] { 2, col };
                    if (rand.Next(0, 2) == 1) escapeData = new int[] { 1, row };
                    else escapeData = new int[] { 2, col };
                }
                else
                {
                    escapeData = new int[] { 2, col };
                }
                return;
            }
            //for col, not need check first or last row anymore
            escapeData = new int[] { col == 0 ? 3 : 1, row };
        }

        private void editTrap(int row, int col)
        {
            removeExplorer(row, col);
            removeEscape(row, col);
            if (removeTrap(row, col)) return;

            //add new trap
            trapData.Add(new int[] { row, col });
        }

        private void editKey(int row, int col)
        {
            removeExplorer(row, col);
            removeMummy(row, col);
            removeScorpion(row, col);
            //add key
            if (keyData == null || keyData[0] != row || keyData[1] != col) keyData = new int[] { row, col };
            else keyData = null;
        }

        private void editGate(int row, int col)
        {
            if (row == MazeSize - 1) return;

            if (gateData == null || gateData[0] != row || gateData[1] != col)
            {
                gateData = new int[] { row, col };
                if (mazeData[row, col, 1]) mazeData[row, col, 1] = false;
            }
            else gateData = null;
        }

        private void editExplorer(int row, int col)
        {
            if (explorerData == null || explorerData[0] != row || explorerData[1] != col) explorerData = new int[] { row, col };

            removeEscape(row, col);
            removeKey(row, col);
            removeTrap(row, col);
            removeMummy(row, col);
            removeScorpion(row, col);
        }

        private void editMummy(int row, int col, int type)
        {
            removeKey(row, col);
            removeExplorer(row, col);
            if (removeMummy(row, col, type)) return;
            removeScorpion(row, col);

            mummyData.Add(new int[] { row, col, type });
        }

        private void editScorpion(int row, int col, int type)
        {
            removeKey(row, col);
            removeExplorer(row, col);
            if (removeScorpion(row, col, type)) return;
            removeMummy(row, col);

            scorpionData.Add(new int[] { row, col, type });
        }
        #endregion

        int currentPack = 0;
        int currentLevel = 1;
        private bool save()
        {
            if (!validatedData()) return false;

            if (_state != CreateState.Batch)
            {
                //if (solution.Count == 0 && (Escape[0] != Explorer.Position[0] || Escape[1] != Explorer.Position[1]))
                //{
                //    System.Windows.Forms.MessageBox.Show("Not escaped yet", "Missing data");
                //    return false;
                //}
            }

            //mazeData
            string strMazeData = "";
            for (int i = 0; i < MazeSize; i++)
            {
                for (int j = 0; j < MazeSize; j++)
                {
                    strMazeData += mazeData[i, j, 0] ? "1" : "0";
                    strMazeData += mazeData[i, j, 1] ? "1" : "0";
                }
            }
            //if (mazeDataRecords.Contains(strMazeData))
            //{
            //    System.Windows.Forms.MessageBox.Show("Maze is exist", "Duplicate");
            //    return;
            //}

            string dirPack = "packs/" + currentPack;
            if (!Directory.Exists(dirPack)) Directory.CreateDirectory(dirPack);
            StreamWriter file = new StreamWriter(string.Format("{0}/{1}.dat", dirPack, currentLevel));
            //mazeDataRecords.Add(strMazeData);
            file.WriteLine(strMazeData);

            //trapData
            foreach (int[] trap in trapData)
            {
                file.Write(trap[0]);
                file.Write(trap[1]);
            }
            file.WriteLine();

            //scorpionData
            foreach (int[] scorpion in scorpionData)
            {
                file.Write(scorpion[0]);
                file.Write(scorpion[1]);
                file.Write(scorpion[2]);
            }
            file.WriteLine();

            //mummyData
            foreach (int[] mummy in mummyData)
            {
                file.Write(mummy[0]);
                file.Write(mummy[1]);
                file.Write(mummy[2]);
            }
            file.WriteLine();

            //gateData
            if (gateData != null && gateData != null)
            {
                file.Write(keyData[0]);
                file.Write(keyData[1]);
                file.Write(gateData[0]);
                file.Write(gateData[1]);
            }
            file.WriteLine();

            //explorerData
            file.Write(explorerData[0]);
            file.Write(explorerData[1]);
            //escapeData
            file.Write(escapeData[0]);
            file.Write(escapeData[1]);
            file.WriteLine();

            //step
            if (solution.Count > 0)
            {
                foreach (int step in solution)
                {
                    file.Write(step);
                }
            }
            else
            {
                foreach (int step in steps)
                {
                    file.Write(step);
                }
            }

            file.Close();

            if (_state != CreateState.Batch)
            {
                if (++currentLevel > 100)
                {
                    if (++currentPack < 100)
                    {
                        currentLevel = 1;
                    }
                    else return true;
                }
                switchMazeSize(_mazeSize);
                hackMummyProcess();
            }

            saveProcess();

            return true;
        }

        private void saveProcess()
        {
            StreamWriter writer = new StreamWriter("process.dat");
            writer.WriteLine(currentPack);
            writer.WriteLine(currentLevel);
            writer.Close();
        }

        private void readProcess()
        {
            try
            {
                StreamReader reader = new StreamReader("process.dat");
                currentPack = int.Parse(reader.ReadLine());
                currentLevel = int.Parse(reader.ReadLine());
                reader.Close();
            }
            catch
            {
                currentPack = 0;
                currentLevel = 1;
            }
        }

        private void hackMummyProcess()
        {
            string pack = currentPack.ToString();
            if (currentPack < 10) pack = "0" + pack;
            readMazeInformation(pack);

            //kill mummy process
            Process[] hackmm = Process.GetProcessesByName("hackmm");
            foreach (System.Diagnostics.Process process in hackmm)
            {
                process.CloseMainWindow();
                process.WaitForExit();
            }

            File.Delete("data/saves/classic.sav");

            using (var fs = new FileStream("hackmm.exe",
                              FileMode.Open,
                              FileAccess.ReadWrite))
            {
                //level
                fs.Position = 0xE4C5;
                fs.WriteByte(Convert.ToByte(currentLevel));
                //pack
                fs.Position = 0x91EFB;
                fs.WriteByte(Convert.ToByte(pack[0]));
                fs.Position = 0x91EFC;
                fs.WriteByte(Convert.ToByte(pack[1]));

            }

            Process newHackmm = Process.Start("hackmm.exe");
            //if (newHackmm.WaitForInputIdle(15000))
            //    User32.SetWindowPos((uint)newHackmm.MainWindowHandle, 0, 0, 100, 640, 480, 0);
        }

        private void readMazeInformation(string pack)
        {

            using (var fs = new FileStream("data/mazes/B-" + pack + ".dat",
                              FileMode.Open,
                              FileAccess.ReadWrite))
            {
                //init infos
                string strByte = fs.ReadByte().ToString("x");
                if (strByte.Length == 1) strByte = "0" + strByte;
                bool isFlip = (strByte[0] == '1');
                int mapSize = Convert.ToInt32(strByte[1].ToString(), 16);
                switchMazeSize((MazeType)mapSize);

                int numMap = fs.ReadByte();
                int numMummy = fs.ReadByte();
                int numGate = fs.ReadByte();
                int numTrap = fs.ReadByte();
                int numScorpion = fs.ReadByte();

                int mapCapacity;
                if (MazeSize < 10) mapCapacity = mapSize * 2 + numMummy + numGate * 2 + numTrap + numScorpion + 2;
                else mapCapacity = mapSize * 4 + numMummy + numGate * 2 + numTrap + numScorpion + 2;

                fs.Position = mapCapacity * (currentLevel - 1) + 6;
                if (!isFlip)
                {
                    #region IF NOT FLIP
                    //vertical walls
                    for (int i = 0; i < MazeSize; i++)
                    {
                        byte[] bytes;
                        if (MazeSize < 10) bytes = new byte[] { (byte)fs.ReadByte() };
                        else bytes = new byte[] { (byte)fs.ReadByte(), (byte)fs.ReadByte() };

                        BitArray bits = new BitArray(bytes);
                        for (int j = 1; j < MazeSize; j++) mazeData[i, j - 1, 0] = bits[j];
                    }
                    //horizonal walls
                    for (int i = 0; i < MazeSize; i++)
                    {
                        byte[] bytes;
                        if (MazeSize < 10) bytes = new byte[] { (byte)fs.ReadByte() };
                        else bytes = new byte[] { (byte)fs.ReadByte(), (byte)fs.ReadByte() };
                        if (i == 0) continue;

                        BitArray bits = new BitArray(bytes);
                        for (int j = 0; j < MazeSize; j++) mazeData[i - 1, j, 1] = bits[j];
                    }
                    //escape
                    strByte = fs.ReadByte().ToString("x");
                    if (strByte.Length == 1) strByte = "0" + strByte;
                    int cellId = Convert.ToInt32(strByte[0].ToString(), 16);
                    switch (Convert.ToInt32(strByte[1].ToString(), 16))
                    {
                        case 0:
                            escapeData = new int[2] { 0, cellId };
                            escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 3;
                            if (cellId == MazeSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 1;
                            escapeCell = new int[2] { 0, cellId };
                            break;

                        case 1:
                            escapeData = new int[2] { 3, cellId };
                            if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 3;
                            if (cellId == MazeSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 2 : 3;
                            escapeCell = new int[] { cellId, 0 };
                            break;

                        case 2:
                            escapeData = new int[2] { 1, cellId };
                            if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 1;
                            if (cellId == MazeSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 1 : 2;
                            escapeCell = new int[2] { cellId, MazeSize - 1 };
                            break;

                        case 3:
                            escapeData = new int[2] { 2, cellId };
                            if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 2 : 3;
                            if (cellId == MazeSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 1 : 2;
                            escapeCell = new int[2] { MazeSize - 1, cellId };
                            break;
                    }
                    //explorer
                    strByte = fs.ReadByte().ToString("x");
                    if (strByte.Length == 1) strByte = "0" + strByte;
                    explorerData = new int[2] { Convert.ToInt32(strByte[0].ToString(), 16), Convert.ToInt32(strByte[1].ToString(), 16) };
                    //white mummy
                    for (int i = 0; i < numMummy; i++)
                    {
                        strByte = fs.ReadByte().ToString("x");
                        if (strByte.Length == 1) strByte = "0" + strByte;
                        mummyData.Add(new int[3] { Convert.ToInt32(strByte[0].ToString(), 16), Convert.ToInt32(strByte[1].ToString(), 16), 0 });
                    }
                    //white scorpion
                    for (int i = 0; i < numScorpion; i++)
                    {
                        strByte = fs.ReadByte().ToString("x");
                        if (strByte.Length == 1) strByte = "0" + strByte;
                        scorpionData.Add(new int[3] { Convert.ToInt32(strByte[0].ToString(), 16), Convert.ToInt32(strByte[1].ToString(), 16), 0 });
                    }
                    //trap
                    for (int i = 0; i < numTrap; i++)
                    {
                        strByte = fs.ReadByte().ToString("x");
                        if (strByte.Length == 1) strByte = "0" + strByte;
                        trapData.Add(new int[2] { Convert.ToInt32(strByte[0].ToString(), 16), Convert.ToInt32(strByte[1].ToString(), 16) });
                    }
                    //gate and key
                    if (numGate == 1)
                    {
                        strByte = fs.ReadByte().ToString("x");
                        if (strByte.Length == 1) strByte = "0" + strByte;
                        gateData = new int[2] { Convert.ToInt32(strByte[0].ToString(), 16), Convert.ToInt32(strByte[1].ToString(), 16) };
                        strByte = fs.ReadByte().ToString("x");
                        if (strByte.Length == 1) strByte = "0" + strByte;
                        keyData = new int[2] { Convert.ToInt32(strByte[0].ToString(), 16), Convert.ToInt32(strByte[1].ToString(), 16) };
                    }
                    #endregion
                }
                else
                {
                    #region IF FLIP
                    //horizonal walls
                    for (int i = MazeSize - 1; i >= 0; i--)
                    {
                        byte[] bytes;
                        if (MazeSize < 10) bytes = new byte[] { (byte)fs.ReadByte() };
                        else bytes = new byte[] { (byte)fs.ReadByte(), (byte)fs.ReadByte() };

                        BitArray bits = new BitArray(bytes);
                        for (int j = 1; j < MazeSize; j++) mazeData[j - 1, i, 1] = bits[j];
                    }
                    //vertical walls
                    for (int i = MazeSize - 2; i >= -1; i--)
                    {
                        byte[] bytes;
                        if (MazeSize < 10) bytes = new byte[] { (byte)fs.ReadByte() };
                        else bytes = new byte[] { (byte)fs.ReadByte(), (byte)fs.ReadByte() };
                        if (i == MazeSize - 2) continue;

                        BitArray bits = new BitArray(bytes);
                        for (int j = 0; j < MazeSize; j++) mazeData[j, i + 1, 0] = bits[j];
                    }
                    //escape
                    strByte = fs.ReadByte().ToString("x");
                    if (strByte.Length == 1) strByte = "0" + strByte;
                    int cellId = Convert.ToInt32(strByte[0].ToString(), 16);
                    switch (Convert.ToInt32(strByte[1].ToString(), 16))
                    {
                        case 0:
                            escapeData = new int[2] { 1, cellId };
                            if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 1;
                            if (cellId == MazeSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 1 : 2;
                            escapeCell = new int[2] { cellId, MazeSize - 1 };
                            break;

                        case 1:
                            escapeData = new int[2] { 0, cellId };
                            if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 1;
                            if (cellId == MazeSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 3;
                            escapeCell = new int[] { 0, MazeSize - cellId - 1 };
                            break;

                        case 2:
                            escapeData = new int[2] { 2, cellId };
                            if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 1 : 2;
                            if (cellId == MazeSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 2 : 3;
                            escapeCell = new int[2] { MazeSize - 1, MazeSize - cellId - 1 };
                            break;

                        case 3:
                            escapeData = new int[2] { 3, cellId };
                            if (cellId == 0) escapeData[0] = (rand.Next(0, 2) == 1) ? 0 : 3;
                            if (cellId == MazeSize - 1) escapeData[0] = (rand.Next(0, 2) == 1) ? 2 : 3;
                            escapeCell = new int[2] { cellId, 0 };
                            break;
                    }
                    //explorer
                    strByte = fs.ReadByte().ToString("x");
                    if (strByte.Length == 1) strByte = "0" + strByte;
                    explorerData = new int[2] { Convert.ToInt32(strByte[1].ToString(), 16), MazeSize - Convert.ToInt32(strByte[0].ToString(), 16) - 1 };
                    //red mummy
                    for (int i = 0; i < numMummy; i++)
                    {
                        strByte = fs.ReadByte().ToString("x");
                        if (strByte.Length == 1) strByte = "0" + strByte;
                        mummyData.Add(new int[3] { Convert.ToInt32(strByte[1].ToString(), 16), MazeSize - Convert.ToInt32(strByte[0].ToString(), 16) - 1, 1 });
                    }
                    //red scorpion
                    for (int i = 0; i < numScorpion; i++)
                    {
                        strByte = fs.ReadByte().ToString("x");
                        if (strByte.Length == 1) strByte = "0" + strByte;
                        scorpionData.Add(new int[3] { Convert.ToInt32(strByte[1].ToString(), 16), MazeSize - Convert.ToInt32(strByte[0].ToString(), 16) - 1, 1 });
                    }
                    //trap
                    for (int i = 0; i < numTrap; i++)
                    {
                        strByte = fs.ReadByte().ToString("x");
                        if (strByte.Length == 1) strByte = "0" + strByte;
                        trapData.Add(new int[2] { Convert.ToInt32(strByte[1].ToString(), 16), MazeSize - Convert.ToInt32(strByte[0].ToString(), 16) - 1 });
                    }
                    //gate and key
                    if (numGate == 1)
                    {
                        strByte = fs.ReadByte().ToString("x");
                        if (strByte.Length == 1) strByte = "0" + strByte;
                        gateData = new int[2] { Convert.ToInt32(strByte[1].ToString(), 16), MazeSize - Convert.ToInt32(strByte[0].ToString(), 16) - 1 };
                        strByte = fs.ReadByte().ToString("x");
                        if (strByte.Length == 1) strByte = "0" + strByte;
                        keyData = new int[2] { Convert.ToInt32(strByte[1].ToString(), 16), MazeSize - Convert.ToInt32(strByte[0].ToString(), 16) - 1 };
                    }
                    #endregion
                }
            }
        }

        private bool validatedData()
        {
            if (escapeData == null)
            {
                System.Windows.Forms.MessageBox.Show("Require escape data.", "Missing data");
                return false;
            }
            if (explorerData == null)
            {
                System.Windows.Forms.MessageBox.Show("Require explorer data.", "Missing data");
                return false;
            }
            if (mummyData.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("Require at least one of mummy.", "Missing data");
                return false;
            }
            if (keyData != null && gateData == null)
            {
                System.Windows.Forms.MessageBox.Show("Key without gate.", "Missing data");
                return false;
            }
            if (keyData == null && gateData != null)
            {
                System.Windows.Forms.MessageBox.Show("Gate without key.", "Missing data");
                return false;
            }
            return true;
        }

        public bool[, ,] Cell;
        public int MazeSize { get { return (int)_mazeSize; } }
        public int[] Escape, Key, Gate;
        public bool GateIsBlock = true;
        public List<Enemy> Mummies, Scorpions;
        public List<int[]> Traps;
        public Explorer Explorer;
        private List<int> steps;
        private List<int> solution;
        private bool isExplorerTurn;

        private bool generateSolution()
        {
            short[][][] mazee = new short[MazeSize][][];
            for (int i = 0; i < MazeSize; i++)
            {
                mazee[i] = new short[MazeSize][];
                for (int j = 0; j < MazeSize; j++)
                {
                    mazee[i][j] = new short[10000];

                    short wallWeight = 0;

                    //check top wall, if exist add 8 to wallWeight
                    if (i == 0) wallWeight += 8;
                    else if (mazeData[i - 1, j, 1]) wallWeight += 8;

                    //check right wall, if exist add 4 to wallWeight
                    if (j == MazeSize - 1) wallWeight += 4;
                    else if (mazeData[i, j, 0]) wallWeight += 4;

                    //check bottom wall, if exist add 2 to wallWeight
                    if (i == MazeSize - 1) wallWeight += 2;
                    else if (mazeData[i, j, 1]) wallWeight += 2;

                    //check left wall, if exist add 1 to wallWeight
                    if (j == 0) wallWeight += 1;
                    else if (mazeData[i, j - 1, 0]) wallWeight += 1;

                    mazee[i][j][0] = wallWeight;
                }
            }

            if (trapData != null)
            {
                for (int i = 0; i < trapData.Count; i++)
                {
                    mazee[trapData[i][0]][trapData[i][1]][0] += 16;
                }
            }

            Keyd[] keye = new Keyd[keyData == null ? 0 : 1];
            if (keyData != null)
            {
                keye[0] = new Keyd(keyData[0], keyData[1], gateData[0], gateData[1], 2);
                //add wallWeight for gate tile and it bottom
                mazee[gateData[0]][gateData[1]][0] += 2;
                //System.Diagnostics.Debug.WriteLine(string.Format("mazee[{0}][{1}][0] + 2 = {2}", gateData[0], gateData[1], mazee[gateData[0]][gateData[1]][0]));
                mazee[gateData[0] + 1][gateData[1]][0] += 8;
                //System.Diagnostics.Debug.WriteLine(string.Format("mazee[{0}][{1}][0] + 2 = {2}", gateData[0]+1, gateData[1], mazee[gateData[0]+1][gateData[1]][0]));
                mazee[keyData[0]][keyData[1]][0] += 32;
                //System.Diagnostics.Debug.WriteLine(string.Format("mazee[{0}][{1}][0] + 2 = {2}", keyData[0] + 1, keyData[1], mazee[keyData[0]][keyData[1]][0]));
            }

            int whiteNum = 0, redNum = 0;
            for (int i = 0; i < mummyData.Count; i++)
            {
                if (mummyData[i][2] == 0) whiteNum++;
                else if (mummyData[i][2] == 1) redNum++;
            }
            MummyWhite[] mummyWhitee = new MummyWhite[whiteNum];
            MummyRed[] mummyRede = new MummyRed[redNum];
            int whiteId = 0, redId = 0;
            foreach (int[] mummy in mummyData)
            {
                if (mummy[2] == 0)
                {
                    mummyWhitee[whiteId] = new MummyWhite(mummy[0], mummy[1]);
                    whiteId++;
                }
                else if (mummy[2] == 1)
                {
                    mummyRede[redId] = new MummyRed(mummy[0], mummy[1]);
                    redId++;
                }
            }

            whiteNum = redNum = 0;
            for (int i = 0; i < scorpionData.Count; i++)
            {
                if (scorpionData[i][2] == 0) whiteNum++;
                else if (scorpionData[i][2] == 1) redNum++;
            }
            ScorpionWhite[] ske = new ScorpionWhite[whiteNum];
            ScorpionRed[] skRede = new ScorpionRed[redNum];
            whiteId = redId = 0;
            foreach (int[] scorpion in scorpionData)
            {
                if (scorpion[2] == 0)
                {
                    ske[whiteId] = new ScorpionWhite(scorpion[0], scorpion[1]);
                    whiteId++;
                }
                else if (scorpion[2] == 1)
                {
                    skRede[redId] = new ScorpionRed(scorpion[0], scorpion[1]);
                    redId++;
                }
            }

            Human human = new Human(explorerData[0], explorerData[1]);
            mazee[human.getX()][human.getY()][1] = 1; //Mark the first step
            State goo = new State(mazee, keye, mummyWhitee, mummyRede, ske, skRede, human, (short)1);
            solution = new List<int>();
            MainProcess thunghiem = new MainProcess(goo, (short)escapeCell[0], (short)escapeCell[1], solution);

            return (solution.Count > 0);
        }

        private void beginPlay()
        {
            if (validatedData())
            {
                //generateSolution();

                //begin play
                Cell = mazeData;
                Escape = escapeCell;
                Traps = trapData;
                Gate = gateData;
                Key = keyData;
                Mummies = new List<Enemy>();
                GateIsBlock = true;
                foreach (int[] mummy in mummyData)
                {
                    Mummies.Add(new Enemy(mummy[0], mummy[1], mummy[2], this));
                }
                Scorpions = new List<Enemy>();
                foreach (int[] scorpion in scorpionData)
                {
                    Scorpions.Add(new Enemy(scorpion[0], scorpion[1], scorpion[2], this));
                }
                Explorer = new Explorer(explorerData[0], explorerData[1], this);
                steps = new List<int>();
                isExplorerTurn = true;
                _state = CreateState.Move;
            }
        }

        private void switchToEnemyTurn()
        {
            foreach (Enemy mummy in Mummies)
            {
                mummy.MovementLeft = 2;
            }
            foreach (Enemy scorpion in Scorpions)
            {
                scorpion.MovementLeft = 1;
            }
            isExplorerTurn = false;

            while (!isExplorerTurn)
            {
                bool endEnemyTurn = true;
                foreach (Enemy scorpion in Scorpions)
                {
                    bool moved = scorpion.TakeMove();
                    if (scorpion.MovementLeft > 0) endEnemyTurn = false;
                    checkCollision(scorpion);
                    if (moved) checkUseKey(scorpion);
                }
                foreach (Enemy mummy in Mummies)
                {
                    bool moved = mummy.TakeMove();
                    if (mummy.MovementLeft > 0) endEnemyTurn = false;
                    checkCollision(mummy);
                    if (moved) checkUseKey(mummy);
                }
                if (endEnemyTurn) isExplorerTurn = true;
            }
        }

        private void checkUseKey(Enemy enemy)
        {
            if (Gate == null) return;
            if (!Scorpions.Contains(enemy) && !Mummies.Contains(enemy)) return;
            if (Key[0] != enemy.Position[0] || Key[1] != enemy.Position[1]) return;
            GateIsBlock = !GateIsBlock;
        }

        private bool checkCollision(Enemy enemy)
        {
            if (!Scorpions.Contains(enemy) && !Mummies.Contains(enemy)) return false;

            List<Enemy> characters = new List<Enemy>();

            //kiem tra nha tham hiem co dung do ko
            if (Explorer.Position[0] == enemy.Position[0] && Explorer.Position[1] == enemy.Position[1])
            {
                System.Windows.Forms.MessageBox.Show("Catched!", "Wrong turn!");
                _state = CreateState.Edit;
                return true;
            }

            //liet ke danh sach bo cap dung do
            for (int i = 0; i < Scorpions.Count; i++)
            {
                if (Scorpions[i].Position[0] == enemy.Position[0] && Scorpions[i].Position[1] == enemy.Position[1]) characters.Add(Scorpions[i]);
            }

            //liet ke danh sach xac uop dung do
            for (int i = 0; i < Mummies.Count; i++)
            {
                if (Mummies[i].Position[0] == enemy.Position[0] && Mummies[i].Position[1] == enemy.Position[1]) characters.Add(Mummies[i]);
            }

            bool haveCollision = false;

            //phan loai va xu ly dung do
            if (characters.Count > 1)
            {
                haveCollision = true;
                while (characters.Count > 1)
                {
                    Enemy curChar = characters[0];
                    if (Scorpions.Contains(curChar)) Scorpions.Remove(curChar);
                    if (Mummies.Contains(curChar)) Mummies.Remove(curChar);
                }
            }
            return haveCollision;
        }
    }
}
