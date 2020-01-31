using OpenitvnGame.Helpers;
using System.IO;
using System;
#if WINDOWS_PHONE
using System.Windows.Resources;
using System.Windows;
#endif
using System.Collections.Generic;

namespace PyramidRaider
{
    class LevelManager
    {
        public short Pyramid { get; protected set; }
        public short Chamber { get; protected set; }

        public bool[, ,] MazeData { get; protected set; }
        public int[,] TrapData { get; protected set; }
        public int[,] ScorpionData { get; protected set; }
        public int[,] MummyData { get; protected set; }
        public int[] GateData { get; protected set; }
        public int[] ExplorerData { get; protected set; }
        public int[] EscapeData { get; protected set; }
        public short[] SolutionData { get; protected set; }

        GameMode _gameMode;

        public LevelManager(GameMode gameMode)
        {
            _gameMode = gameMode;
            if (_gameMode == GameMode.Tutorial)
            {
                Pyramid = 0;
                Chamber = 1;
                buildTutorialData();
            }
            else
            {
                Pyramid = 1;
                Chamber = 1;
                buildLevelData();
            }
        }

        public LevelManager(short pyramid, short chamber, GameMode gameMode)
        {
            if (gameMode == GameMode.Tutorial) throw new Exception("Can't assign the values of pyramid and chamber in tutorial mode.");

            _gameMode = gameMode;
            Pyramid = pyramid;
            Chamber = chamber;
            buildLevelData();

            if (_gameMode == GameMode.Adventure)
            {
                SettingHelper.StoreSetting(PlayScene.RECORD_ADVENTURE_PYRAMID_CURRENT, Pyramid, true);
                SettingHelper.SaveSetting();
            }
        }

        public bool NextLevel()
        {
            bool canUp = true;

            switch (_gameMode)
            {
                case GameMode.Classic:
                    if (Chamber < 15)
                    {
                        Chamber++;
                    }
                    else if (Pyramid < 15)
                    {
                        Chamber = 1;
                        Pyramid++;
                    }
                    else
                    {
                        Chamber = Pyramid = 1;
                        canUp = false;
                    }
                    SettingHelper.StoreSetting(PlayScene.RECORD_CLASSIC_PYRAMID, Pyramid, true);
                    SettingHelper.StoreSetting(PlayScene.RECORD_CLASSIC_CHAMBER, Chamber, true);
                    //short randomLevel = (short)PlayScene.Random.Next((Chamber - 1) * 25, Chamber * 25 + 1);
                    //SettingHelper.StoreSetting(PlayScene.RECORD_RANDOM_LEVEL, randomLevel, true);
                    if (canUp) buildLevelData();
                    break;

                case GameMode.Adventure:
                    short pyramidId = (short)(Pyramid - 1);
                    short chamberId = (short)(Chamber - 1);
                    bool needUpdatePyramidProcess = (AdventureMap.Instance.PyramidProcess[pyramidId] == chamberId);

                    if (needUpdatePyramidProcess) AdventureMap.Instance.PyramidProcess[Pyramid - 1] = Chamber; //that means chamberId + 1

                    if (Chamber < 15) Chamber++;
                    else canUp = false;

                    if (canUp) buildLevelData();
                    else if (needUpdatePyramidProcess)
                    {
                        foreach (byte neigbour in AdventureMap.Instance.NeigbourPyramid[pyramidId])
                        {
                            if (neigbour != 14 && AdventureMap.Instance.PyramidProcess[neigbour] == -1) AdventureMap.Instance.PyramidProcess[neigbour] = 0;
                        }
                        bool done = true;
                        for (byte i = 0; i < 14; i++)
                        {
                            if (AdventureMap.Instance.PyramidProcess[i] < 15)
                            {
                                done = false;
                                break;
                            }
                        }
                        if (done) AdventureMap.Instance.PyramidProcess[14] = 0;
                    }
                    string strOpenedPyramid = string.Join(",", AdventureMap.Instance.PyramidProcess);
                    SettingHelper.StoreSetting(PlayScene.RECORD_ADVENTURE_PYRAMID_PROCESS, strOpenedPyramid, true);
                    break;

                case GameMode.Tutorial:
                    if (Chamber < 5)
                    {
                        Chamber++;
                        buildTutorialData();
                        return true;
                    }
                    else return false;
            }

            return canUp;
        }

        private void buildLevelData()
        {
            if (_gameMode == GameMode.Tutorial) throw new Exception("The buildLevelData() method is not available in tutorial mode.");

            string mazeFolder = string.Empty;
            short pack, level;
            if (_gameMode == GameMode.Adventure)
            {
                mazeFolder = "Adventures";
                pack = Pyramid;
                level = Chamber;
            }
            else
            {
                mazeFolder = "Mazes";
                pack = Pyramid;

                level = SettingHelper.GetSetting<short>(PlayScene.RECORD_RANDOM_LEVEL, -1);
                int levelMin = (Chamber - 1) * 25;
                int levelMax = Chamber * 25;
                if (level < 1 || level < levelMin || level > levelMax)
                {
                    level = (short)PlayScene.Random.Next(levelMin, levelMax + 1);
                    SettingHelper.StoreSetting(PlayScene.RECORD_RANDOM_LEVEL, level, true);
                }

                //List<short> randomLevel = SettingHelper.GetSetting<List<short>>(PlayScene.RECORD_RANDOM_LEVEL, new List<short>());
                //if (randomLevel.Count == 25) randomLevel.Clear();
                //level = -1;
                //while (level < 1 || randomLevel.Contains(level))
                //{
                //    level = (short)PlayScene.Random.Next((Chamber - 1) * 25, Chamber * 25 + 1);
                //}
                //randomLevel.Add(level);
                //SettingHelper.StoreSetting(PlayScene.RECORD_RANDOM_LEVEL, randomLevel, true);
            }

            //string dataPath = mazeFolder + "/" + pack + "/" + level + ".dat";
            string dataPath = string.Format("{0}/{1}.dat", mazeFolder, pack);
#if WINDOWS_PHONE
            StreamResourceInfo resource = Application.GetResourceStream(new Uri(dataPath, UriKind.Relative));
            StreamReader reader = new StreamReader(resource.Stream);
#endif
#if ANDROID
            Stream resource = Main.Activity.Assets.Open(dataPath);
            StreamReader reader = new StreamReader(resource);
#endif
#if WINDOWS
            StreamReader reader = new StreamReader(dataPath);
#endif
            //skip previous levels
            for (short i = 0; i < level - 1; i++) reader.ReadLine();
            string[] levelData = reader.ReadLine().Split('|');

            //string mazeData = reader.ReadLine();
            string mazeData = levelData[0];
            int mazeSize = (int)Math.Sqrt(mazeData.Length / 2);
            MazeData = new bool[mazeSize, mazeSize, 2];
            int codeId = 0;
            for (int i = 0; i < mazeSize; i++)
            {
                for (int j = 0; j < mazeSize; j++)
                {
                    MazeData[i, j, 0] = (mazeData[codeId++] == '1');
                    MazeData[i, j, 1] = (mazeData[codeId++] == '1');
                }
            }

            //string trapData = reader.ReadLine();
            string trapData = levelData[1];
            int numTrap = trapData.Length / 2;
            TrapData = new int[numTrap, 2];
            codeId = 0;
            for (int i = 0; i < numTrap; i++)
            {
                TrapData[i, 0] = (int)Char.GetNumericValue(trapData[codeId++]);
                TrapData[i, 1] = (int)Char.GetNumericValue(trapData[codeId++]);
            }

            //string scorpionData = reader.ReadLine();
            string scorpionData = levelData[2];
            int numScorpion = scorpionData.Length / 3;
            ScorpionData = new int[numScorpion, 3];
            codeId = 0;
            for (int i = 0; i < numScorpion; i++)
            {
                ScorpionData[i, 0] = (int)Char.GetNumericValue(scorpionData[codeId++]);
                ScorpionData[i, 1] = (int)Char.GetNumericValue(scorpionData[codeId++]);
                ScorpionData[i, 2] = (int)Char.GetNumericValue(scorpionData[codeId++]);
            }

            //string mummyData = reader.ReadLine();
            string mummyData = levelData[3];
            int numMummy = mummyData.Length / 3;
            MummyData = new int[numMummy, 3];
            codeId = 0;
            for (int i = 0; i < numMummy; i++)
            {
                MummyData[i, 0] = (int)Char.GetNumericValue(mummyData[codeId++]);
                MummyData[i, 1] = (int)Char.GetNumericValue(mummyData[codeId++]);
                MummyData[i, 2] = (int)Char.GetNumericValue(mummyData[codeId++]);
            }

            //string gateData = reader.ReadLine();
            string gateData = levelData[4];
            GateData = new int[gateData.Length];
            codeId = 0;
            if (gateData.Length == 4)
            {
                GateData[0] = (int)Char.GetNumericValue(gateData[codeId++]);
                GateData[1] = (int)Char.GetNumericValue(gateData[codeId++]);
                GateData[2] = (int)Char.GetNumericValue(gateData[codeId++]);
                GateData[3] = (int)Char.GetNumericValue(gateData[codeId++]);
            }

            //string explorerAndEscapeData = reader.ReadLine();
            string explorerAndEscapeData = levelData[5];
            ExplorerData = new int[] { (int)Char.GetNumericValue(explorerAndEscapeData[0]), (int)Char.GetNumericValue(explorerAndEscapeData[1]) };
            EscapeData = new int[] { (int)Char.GetNumericValue(explorerAndEscapeData[2]), (int)Char.GetNumericValue(explorerAndEscapeData[3]) };

            //string solutionData = reader.ReadLine();
            string solutionData = levelData[6];
            SolutionData = new short[solutionData.Length];
            for (byte i = 0; i < solutionData.Length; i++)
            {
                SolutionData[i] = short.Parse(solutionData[i].ToString());
            }
        }

        private List<int> direction;
        private List<string> description;

        public KeyValuePair<int, string> GetStepInfo(int step)
        {
            if (_gameMode != GameMode.Tutorial) throw new InvalidOperationException("The getStepInfo() method is only available in tutorial mode.");

            if (step >= 0 && step < direction.Count)
            {
                return new KeyValuePair<int, string>(direction[step], StringHelper.WordWrap(Main.FontSmall, description[step], 380));
            }
            return new KeyValuePair<int, string>(-1, "");
        }

        private void buildTutorialData()
        {
            if (_gameMode != GameMode.Tutorial) throw new InvalidOperationException("The buildTutorialData() method is only available in tutorial mode.");

            direction = new List<int>();
            description = new List<string>();

            switch (Chamber)
            {
                case 1:
                    MazeData = new bool[,,] { { { false, false }, { false, true }, { false, false }, { false, false }, { false, false }, { false, false } }, { { true, false }, { false, false }, { false, true }, { true, false }, { false, false }, { false, true } }, { { false, true }, { false, true }, { true, false }, { true, false }, { false, false }, { false, false } }, { { true, false }, { false, false }, { false, false }, { false, false }, { false, true }, { false, true } }, { { true, false }, { false, false }, { true, false }, { true, true }, { false, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } } };
                    EscapeData = new int[] { 0, 0 };
                    ExplorerData = new int[] { 4, 3 };
                    MummyData = new int[,] { { 2, 0, 0 } };
                    ScorpionData = new int[,] { };
                    TrapData = null;
                    GateData = null;
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_1_1);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_1_2);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_1_3);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_1_4);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_1_5);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_1_6);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_1_6);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_1_6);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_1_6);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_1_7);
                    break;

                case 2:
                    MazeData = new bool[,,] { { { false, false }, { false, false }, { false, true }, { false, false }, { false, true }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } }, { { false, false }, { false, false }, { true, false }, { true, true }, { false, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, true } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } } };
                    EscapeData = new int[] { 0, 0 };
                    ExplorerData = new int[] { 3, 3 };
                    MummyData = new int[,] { { 1, 3, 0 } };
                    ScorpionData = new int[,] { };
                    TrapData = null;
                    GateData = null;
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_2_1);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_2_2);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_2_3);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_2_4);
                    direction.Add(4);
                    description.Add(Localize.Instance.Tutorial_2_5);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_2_6);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_2_6);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_2_6);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_2_6);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_2_6);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_2_7);
                    break;

                case 3:
                    MazeData = new bool[,,] { { { false, false }, { false, false }, { false, false }, { false, false }, { true, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } }, { { false, false }, { false, false }, { true, false }, { false, true }, { false, true }, { false, false } }, { { false, false }, { false, true }, { false, true }, { false, true }, { true, true }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { true, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } } };
                    EscapeData = new int[] { 0, 0 };
                    ExplorerData = new int[] { 3, 4 };
                    MummyData = new int[,] { { 1, 1, 0 } };
                    ScorpionData = new int[,] { };
                    TrapData = null;
                    GateData = null;
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_3_1);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_3_2);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_3_3);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_3_4);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_3_5);
                    direction.Add(4);
                    description.Add(Localize.Instance.Tutorial_3_6);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_3_7);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_3_7);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_3_7);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_3_7);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_3_7);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_3_7);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_3_8);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_3_9);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_3_10);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_3_11);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_3_12);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_3_13);
                    break;

                case 4:
                    MazeData = new bool[,,] { { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } }, { { false, false }, { false, false }, { true, false }, { false, true }, { false, false }, { false, true } }, { { false, false }, { false, false }, { false, false }, { false, false }, { true, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } } };
                    EscapeData = new int[] { 0, 0 };
                    ExplorerData = new int[] { 3, 2 };
                    MummyData = new int[,] { { 0, 3, 0 }, { 2, 3, 0 } };
                    ScorpionData = new int[,] { { 3, 5, 0 } };
                    TrapData = null;
                    GateData = new int[] { 4, 3, 3, 5 };
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_4_1);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_4_2);
                    direction.Add(1);
                    description.Add(Localize.Instance.Tutorial_4_3);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_4_4);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_4_5);
                    direction.Add(2);
                    description.Add(Localize.Instance.Tutorial_4_6);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_4_7);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_4_8);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_4_9);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_4_9);
                    direction.Add(3);
                    description.Add(Localize.Instance.Tutorial_4_9);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_4_9);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_4_9);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_4_9);
                    direction.Add(0);
                    description.Add(Localize.Instance.Tutorial_4_9);
                    break;

                case 5:
                    MazeData = new bool[,,] { { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, true }, { false, false }, { false, false } }, { { false, false }, { false, false }, { true, false }, { false, true }, { false, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } }, { { false, false }, { false, false }, { false, false }, { false, false }, { false, false }, { false, false } } };
                    EscapeData = new int[] { 0, 0 };
                    ExplorerData = new int[] { 2, 2 };
                    MummyData = new int[,] { { 2, 4, 1 } };
                    ScorpionData = new int[,] { };
                    TrapData = new int[,] { { 0, 4 }, { 0, 5 } };
                    GateData = null;
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_5_1);
                    direction.Add(2);
                    description.Add(Localize.Instance.Tutorial_5_2);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_5_3);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_5_4);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_5_5);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_5_6);
                    direction.Add(-7);
                    description.Add(Localize.Instance.Tutorial_5_7);
                    break;
            }
        }
    }
}
