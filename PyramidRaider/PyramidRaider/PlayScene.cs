using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Tranquillity;
using System.Threading;
using OpenitvnGame.Helpers;
using OpenitvnGame;
using Cameras;
#if !WINDOWS
using Microsoft.Xna.Framework.Input.Touch;
#endif
using Microsoft.Xna.Framework.Content;

namespace PyramidRaider
{
    enum GameState { Load, Init, Play, Pause, OpenMap, CloseMap, Summary, Losing, Lose, BackMain, SubmitScore }
    enum GameMode { Classic, Adventure, Tutorial }

    class PlayScene : GameScene
    {
        public const string RECORD_CLASSIC_PYRAMID = "classic_pyramid";
        public const string RECORD_CLASSIC_CHAMBER = "classic_chamber";
        public const string RECORD_CLASSIC_SCORE = "classic_score";
        public const string RECORD_ADVENTURE_PYRAMID_PROCESS = "adventure_pyramid_process";
        public const string RECORD_ADVENTURE_PYRAMID_CURRENT = "adventure_pyramid_current";
        public const string RECORD_ADVENTURE_CHAMBER_PROCESS = "adventure_pyramid_chamber";
        public const string RECORD_CLASSIC_HINT_POINT = "hint_point_classic";
        public const string RECORD_ADVENTURE_HINT_POINT = "hint_point_adventure";
        public const string RECORD_FIRST_PLAY = "first_play";
        public const string RECORD_RANDOM_LEVEL = "random_level";

        public const string DEFAULT_ADVENTURE_PYRAMID_PROCESS = "0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1";
        public const int SOLUTION_COST = 20;

        public const float MOVEMENT_STEP = 0.56f;
        const short COMMAND_NONE = -1;
        const short COMMAND_MAINMENU = 1;
        const short COMMAND_ADVENTURE_NOTIFY_2 = 2;

        public static ParticleManager ParticleManager { get; private set; }

        RenderTarget2D viewpot3D;
        public Rectangle viewpot3DBound;
        RotateCamera camera;

        public GameState State { get; private set; }
        int timeline = -1;

        Thread loadContentThread;
        PlayContentHolder contentHolder;
        List<UndoData> undo;
        public LevelManager Level { get; private set; }

        public bool[] explorerDirectionAvailable = new bool[5];

        Texture2D texSidebarClassic, texSidebarAdventure;

        //model objects
        public List<Trap> Traps { get; private set; }
        public CModel Stair { get; private set; }
        public List<CModel> Walls { get; private set; }
        public CModel[] Borders { get; private set; }
        public GateSystem Gate { get; private set; }
        public Explorer Explorer { get; private set; }
        public List<Scorpion> Scorpions { get; private set; }
        public List<Mummy> Mummies { get; private set; }
        public CModel Floor { get; private set; }
        private byte floorModelId;

        //masked sprite for dark mode
        public bool IsDarkness { get; private set; }
        RenderTarget2D darkFloor;
        Vector2 vtFloorMaskCenter;
        //end masked sprite

        //data fields
        public bool[, ,] Cell { get; private set; }
        public int MazeSize { get; private set; }
        public int[] EscapeCell { get; private set; }
        public int EscapeDirection { get; private set; }
        public List<short> SolutionData { get; private set; }

        Vector2 vtSidebar = new Vector2(512, 0);
        Rectangle recSidebar = new Rectangle(600, 0, 288, 300);
        Rectangle recController = new Rectangle(529, 308, 270, 180);
        //compass
        Vector2 vtCompass = new Vector2(0, 320);
        Vector2 vtNeedle = new Vector2(80, 400);
        Rectangle recNeedle = new Rectangle(0, 0, 160, 160);
        float needleAngle;
        Vector2 vtNeedleCenter = new Vector2(80, 80);
        //score
        Vector2 vtScore = new Vector2(667, 196);
        Vector2 vtMapDetail = new Vector2(0, 190);
        Color mapDetailColor = new Color(0x49, 0x32, 0x08);
        //star
        Vector2 vtStarCenter = new Vector2(45, 40);
        float[] starFactor = new float[] { 0, 0, 0 };
        Vector2[] starPosition = new Vector2[] {
            new Vector2(170, 315),
            new Vector2(300, 315),
            new Vector2(430, 315)
        };
        //galaxy
        Vector2 vtGalaxy = new Vector2(300, 240);
        Rectangle recGalaxy = new Rectangle(0, 0, 780, 780);
        float galaxyAngle = 0f;
        Vector2 vtGalaxyCenter = new Vector2(390, 390);
        //summary
        HellGate hellGate;
        QuickMenu quickMenu;
        UIDialog uiDialog;
        AdventureMap adventureMap;
        PyramidMap pyramidMap;
        //rotate
#if WINDOWS
        public static Rectangle recRotateCCW = new Rectangle(10, 10, 120, 88);
        public static Rectangle recRotateCW = new Rectangle(470, 10, 120, 88);
#endif
        //ban do phong
        Vector2[] chambers = new Vector2[] {
            new Vector2(638, 262),
            new Vector2(663, 262),
            new Vector2(688, 262),
            new Vector2(713, 262),
            new Vector2(738, 262),
            new Vector2(726, 244),
            new Vector2(701, 244),
            new Vector2(676, 244),
            new Vector2(651, 244),
            new Vector2(663, 226),
            new Vector2(688, 226),
            new Vector2(713, 226),
            new Vector2(701, 208),
            new Vector2(676, 208),
            new Vector2(688, 190),
        };

        Button2D[] controller;
        Button2D[] btnSummary;
        Button2D btnUndo, btnWorldMap;
        Sprite2D sprLoading;
        Rectangle recTouchNotice = new Rectangle(50, 400, 500, 60);

        int tick = 0, second = 0, basicScore, bonusScore;
        bool pressReminder = false;
        public GameMode Mode { get; private set; }
        bool _returnMenuAfterTutorial;
        public bool ShowSolution { get; private set; }
        float _blackAlpha;
        Sprite2D sprSmallTreasure, sprNiceTreasure;
        bool pyramidCompleted = false;

        private int _score, scoreShow;
        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
                SettingHelper.StoreSetting(RECORD_CLASSIC_SCORE, _score, true);
            }
        }

        private int _hintPoint, hintPointShow;
        public int HintPoint
        {
            get
            {
                return _hintPoint;
            }
            set
            {
                _hintPoint = value;
                SettingHelper.StoreSetting(Mode == GameMode.Adventure ? RECORD_ADVENTURE_HINT_POINT : RECORD_CLASSIC_HINT_POINT, _hintPoint, true);
            }
        }

        public PlayScene(GameMode gameMode, bool returnMenuAfterTutorial)
            : base(Main.Instance)
        {
            State = GameState.Load;
            Mode = gameMode;
            _returnMenuAfterTutorial = returnMenuAfterTutorial;
            _hintPoint = hintPointShow = SettingHelper.GetSetting<int>(Mode == GameMode.Adventure ? RECORD_ADVENTURE_HINT_POINT : RECORD_CLASSIC_HINT_POINT, 0);
        }

        protected override void prepareContent(ContentManager content)
        {
#if !WINDOWS
            TouchPanel.EnabledGestures = GestureType.Flick;
#endif
            texSidebarClassic = content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/sidebarClassic");
            texSidebarAdventure = content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/sidebarAdventure");
            sprLoading = new Sprite2D(content.Load<Texture2D>("Images/loading"), 60, 60);
            sprLoading.SetPosition(170, 210);

            contentHolder = new PlayContentHolder();

            //nap tai nguyen bang thread
            loadContentThread = new Thread(new ThreadStart(contentHolder.LoadContent));
            loadContentThread.Start();

            ShowSolution = false;

            hellGate = new HellGate(HellGateState.Closed);
            hellGate.ContentTexture = content.Load<Texture2D>("Images/empty");
            quickMenu = new QuickMenu(this);
            uiDialog = new UIDialog(content);
            pyramidMap = new PyramidMap(this);
        }

        #region Update and draw
        public override void Update(GameTime gameTime)
        {
            switch (State)
            {
                case GameState.Load:
                    if (loadContentThread.ThreadState == ThreadState.Stopped) startGame();
                    return;

                case GameState.Init:
                    hellGate.Update();
                    foreach (Mummy mummy in Mummies)
                    {
                        mummy.RisingUp();
                    }
                    if (++timeline == 32)
                    {
                        SoundController.PlaySound(contentHolder.SoundMummyHowl);
                        State = GameState.Play;
                        Floor.UnhideAllMesh();
                        foreach (Mummy mummy in Mummies)
                        {
                            mummy.RiseUpDone();
                        }
                        SwitchPlayerTurn();
                    }
                    break;

                case GameState.OpenMap:
                    if (hellGate.State != HellGateState.Opened)
                    {
                        hellGate.Update();
                    }
                    else
                    {
                        if (timeline < 17)
                        {
                            vtSidebar.X += 16;
                            timeline++;
                        }
                        else
                        {
                            adventureMap.Update();
                        }
                        if (timeline == 17)
                        {
                            if (adventureMap.PyramidProcess[0] == 0) adventureInstruction(1);
                            else if (adventureMap.PyramidProcess[0] == 15 &&
                                adventureMap.PyramidProcess[1] == 0 &&
                                adventureMap.PyramidProcess[6] == 0)
                            {
                                adventureInstruction(3);
                            }
                            timeline++;
                        } else if (timeline == 18)
                        {
                            if (uiDialog.CommandCode != COMMAND_ADVENTURE_NOTIFY_2) timeline++;
                            else if (!uiDialog.Visible)
                            {
                                adventureInstruction(2);
                                timeline++;
                            }
                        }
                    }

                    if (pyramidMap.Visible) pyramidMap.Update();
                    return;

                case GameState.CloseMap:
                    if (adventureMap == null) //mo world map
                    {
                        if (timeline < 0)
                        {
                            hellGate.ContentTexture = null;
                            hellGate.State = HellGateState.Closing;
                            timeline++;
                        }
                        else
                        {
                            if (hellGate.State != HellGateState.Closed) hellGate.Update();
                            else
                            {
                                pyramidCompleted = false;
                                createWorldMap();
                            }
                        }
                    }
                    else //dong world map
                    {
                        if (timeline < 17)
                        {
                            vtSidebar.X -= 16;
                            timeline++;
                        }
                        else if (timeline == 17)
                        {
                            hellGate.ContentTexture = null;
                            hellGate.State = HellGateState.Closing;
                            timeline++;
                        }
                        else
                        {
                            if (hellGate.State != HellGateState.Closed) hellGate.Update();
                            else
                            {
                                adventureMap = null;
                                beginLevel();
                            }
                        }
                    }

                    if (pyramidMap.Visible) pyramidMap.Update();
                    break;

                case GameState.Play:
                    if (++tick == 30)
                    {
                        second++;
                        tick = 0;
                    }
                    break;

                case GameState.Summary:
                    hellGate.Update();
                    if (timeline < 165) timeline++;

                    if (Mode == GameMode.Tutorial && timeline == 60)
                    {
                        _tutorialStep = -1;
                        _tutorialDescription = "";
                        nextLevel();
                    }
                    else if (!pyramidCompleted && timeline >= 90)
                    {
                        bool turnOnButton = (timeline == 90 && !btnSummary[0].Visible && starLevel == 0);

                        if (timeline < 100)
                        {
                            if (timeline == 90 && starLevel >= 1) SoundController.PlaySound(contentHolder.SoundRate);
                            starFactor[0] += 0.12f;
                        }
                        else if (timeline < 104) starFactor[0] -= 0.05f;
                        else if (timeline < 120)
                        {
                            starFactor[0] = 1;
                            if (!btnSummary[0].Visible && starLevel == 1) turnOnButton = true;
                        }
                        else if (timeline < 130)
                        {
                            if (timeline == 120 && starLevel >= 2) SoundController.PlaySound(contentHolder.SoundRate);
                            starFactor[1] += 0.12f;
                        }
                        else if (timeline < 134) starFactor[1] -= 0.05f;
                        else if (timeline < 150)
                        {
                            starFactor[1] = 1;
                            if (!btnSummary[0].Visible && starLevel == 2) turnOnButton = true;
                        }
                        else if (timeline < 160)
                        {
                            if (timeline == 150 && starLevel == 3) SoundController.PlaySound(contentHolder.SoundRate);
                            starFactor[2] += 0.12f;
                        }
                        else if (timeline < 164) starFactor[2] -= 0.05f;
                        else if (timeline == 164)
                        {
                            starFactor[2] = 1;
                            if (!btnSummary[0].Visible && starLevel == 3) turnOnButton = true;
                            //show ad if not premium
                            if (!Main.IsPremium)
                            {
#if ANDROID
                                (Main.Activity as MainActivity).RenderAd();
#endif
#if WINDOWS_PHONE && SILVERLIGHT
                                GamePage.Instance.RenderAd();
#endif
                            }
                        }

                        if (turnOnButton)
                        {
                            if (Mode == GameMode.Adventure)
                            {
                                foreach (Button2D button in btnSummary) button.Visible = true;
                            }
                            else
                            {
                                btnSummary[1].Visible = true;
                            }
                        }

                        if (pyramidMap.Visible) pyramidMap.Update();
                    }
                    break;

                case GameState.Losing:
                    if (timeline < 20) timeline++;
                    else
                    {
                        timeline = -1;
                        State = GameState.Lose;
                        quickMenu.SlideUp();
                    }
                    break;

                case GameState.Pause:
                case GameState.Lose:
                    quickMenu.Update();
                    return;

                case GameState.BackMain:
                case GameState.SubmitScore:
                    if (timeline < 40)
                    {
                        timeline++;
                        _blackAlpha += 0.025f;
                    }
                    else if(State == GameState.BackMain)
                    {
                        if (_returnMenuAfterTutorial) Main.Instance.GotoMainMenu(MainMenuScene.FLAG_INSTRUCTION);
                        else Main.Instance.GotoMainMenu(MainMenuScene.FLAG_MAIN);
                    }
                    else if (State == GameState.SubmitScore)
                    {
                        Main.Instance.GotoSubmitScore(Score);
                    }
                    break;
            }

            if (uiDialog.Visible) return;

            //cap nhat thien ha nen
            if (galaxyAngle < 360) galaxyAngle += 0.05f;
            else galaxyAngle = 0;

            if (adventureMap == null)
            {
                //cap nhat trang thai nha tham hiem
                Explorer.Update(gameTime);

                //cap nhat cong neu co
                if (Gate != null) Gate.Update();

                //cap nhat trang thai bo cap
                for (int i = 0; i < Scorpions.Count; i++)
                {
                    if (Scorpions[i].State == CharacterState.Idle && Scorpions[i].MovementLeft > 0)
                    {
                        if (Scorpions[i].TakeMove()) Explorer.Wait();
                    }
                    Scorpions[i].Update(gameTime);
                }

                //cap nhat trang thai xac uop
                for (int i = 0; i < Mummies.Count; i++)
                {
                    if (Mummies[i].State == CharacterState.Idle && Mummies[i].MovementLeft > 0)
                    {
                        if (Mummies[i].TakeMove()) Explorer.Wait();
                    }
                    Mummies[i].Update(gameTime);
                }

                foreach (Trap trap in Traps)
                {
                    trap.Update();
                }

                //cap nhat trang thai he thong hat
                ParticleManager.SetMatrices(camera.View, camera.Projection);
                ParticleManager.Update(gameTime);

                //di chuyen camera
                if (camera.State != CameraState.Stand)
                {
                    camera.Update();
                    needleAngle = MathHelper.ToRadians(camera.Angle);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (State == GameState.Load)
            {
                hellGate.Draw(spriteBatch);
                spriteBatch.Draw(Mode == GameMode.Adventure ? texSidebarAdventure : texSidebarClassic, vtSidebar, Color.White);
                if (++timeline % 2 == 0) sprLoading.NextFrame();
                sprLoading.Draw(spriteBatch);
                spriteBatch.DrawString(Main.FontNormal, Localize.Instance.Loading, new Vector2(240, 222), Color.White);
                return;
            }

            if (pyramidCompleted)
            {
                //ve thien ha nen
                spriteBatch.Draw(contentHolder.ImageGalaxy, vtGalaxy, recGalaxy, Color.White, MathHelper.ToRadians(galaxyAngle), vtGalaxyCenter, 1, SpriteEffects.None, 0);

                //ve khung hinh
                spriteBatch.Draw(contentHolder.ImagePyramidCompleted, Vector2.Zero, Color.White);
                if (++tick >= 15)
                {
                    tick = 0;
                    pressReminder = !pressReminder;
                }
                if (pressReminder)
                {
                    string press = Localize.Instance.TouchToContinue;
                    int left = 300 - (int)Main.FontNormal.MeasureString(press).X / 2;
                    spriteBatch.DrawString(Main.FontNormal, press, new Vector2(left, 420), Color.White);
                }
                //ve bao vat
                sprNiceTreasure.Draw(spriteBatch);

                hellGate.Draw(spriteBatch);
                spriteBatch.Draw(Mode == GameMode.Adventure ? texSidebarAdventure : texSidebarClassic, vtSidebar, Color.White);
                return;
            }

            if (adventureMap == null)
            {
                if (State == GameState.Init || State == GameState.Play)
                {
                    spriteBatch.End();

                    //cap nhat trang thai nen
                    if (IsDarkness)
                    {
                        graphicsDevice.SetRenderTarget(darkFloor);
                        SpriteBatch floorSpriteBatch = new SpriteBatch(graphicsDevice);

                        floorSpriteBatch.Begin();
                        floorSpriteBatch.Draw(contentHolder.TextureFloorLight[floorModelId], Vector2.Zero, Color.White);
                        floorSpriteBatch.Draw(contentHolder.TextureFloorMask[floorModelId], Explorer.FloorTexturePosition, null, Color.White, 0, vtFloorMaskCenter, 1, SpriteEffects.None, 1);
                        floorSpriteBatch.End();

                        graphicsDevice.SetRenderTarget(Resolution.Screen);
                    }

                    graphicsDevice.SetRenderTarget(viewpot3D);
                    SpriteBatch spriteBatch3D = new SpriteBatch(graphicsDevice);
                    graphicsDevice.Clear(Color.Black);

                    //draw background galaxy
                    spriteBatch3D.Begin();
                    spriteBatch3D.Draw(contentHolder.ImageGalaxy, vtGalaxy, recGalaxy, Color.White, MathHelper.ToRadians(galaxyAngle), vtGalaxyCenter, 1, SpriteEffects.None, 0);
                    spriteBatch3D.End();

                    graphicsDevice.BlendState = BlendState.AlphaBlend;
                    graphicsDevice.DepthStencilState = DepthStencilState.Default;

                    //draw floor
                    Floor.Draw(camera.View, camera.Projection);

                    //draw maze walls
                    foreach (CModel curWall in Walls)
                    {
                        if (Explorer.LightToObject(curWall.Position)) curWall.Draw(camera.View, camera.Projection);
                    }

                    foreach (CModel curBorder in Borders)
                    {
                        curBorder.Draw(camera.View, camera.Projection);
                    }

                    //draw traps
                    foreach (Trap curTrap in Traps)
                    {
                        curTrap.Draw(camera.View, camera.Projection);
                    }

                    //draw upstair
                    Stair.Dim = !Explorer.LightToObject(Stair.Position);
                    Stair.Draw(camera.View, camera.Projection);

                    //draw explorer
                    Explorer.Draw(camera.View, camera.Projection);

                    //draw scorpions
                    foreach (Scorpion scorpion in Scorpions)
                    {
                        scorpion.Draw(camera.View, camera.Projection);
                    }

                    //draw mummies
                    foreach (Mummy mummy in Mummies)
                    {
                        mummy.Draw(camera.View, camera.Projection);
                    }

                    //draw gate and key
                    if (Gate != null) Gate.Draw(camera.View, camera.Projection);

                    //draw particle system manager
                    ParticleManager.Draw(spriteBatch3D);

                    spriteBatch3D.Begin();
                    if (Mode == GameMode.Tutorial)
                    {
                        spriteBatch3D.Draw(contentHolder.ImageLongScroll, new Vector2(80, 330), Color.White * 0.8f);
                        spriteBatch3D.DrawString(Main.FontSmall, _tutorialDescription, new Vector2(156, 340), Color.Black);
                    }
                    spriteBatch3D.Draw(contentHolder.ImageCompass, vtCompass, Color.White);
                    spriteBatch3D.Draw(contentHolder.ImageNeedle, vtNeedle, recNeedle, Color.White, needleAngle, vtNeedleCenter, 1, SpriteEffects.None, 0);
                    spriteBatch3D.End();

                    graphicsDevice.SetRenderTarget(Resolution.Screen);
                    graphicsDevice.Clear(Color.Black);

                    spriteBatch.Begin();
                }

                spriteBatch.Draw(viewpot3D, viewpot3DBound, Color.White);
#if WINDOWS
                spriteBatch.Draw(contentHolder.ImageRotateCCW, recRotateCCW, Color.White);
                spriteBatch.Draw(contentHolder.ImageRotateCW, recRotateCW, Color.White);
#endif
            }
            else
            {
                adventureMap.Draw(spriteBatch);
                if (pyramidMap.Visible) pyramidMap.Draw(spriteBatch);
            }

            switch (State)
            {
                case GameState.Init:
                case GameState.BackMain:
                case GameState.SubmitScore:
                case GameState.OpenMap:
                case GameState.CloseMap:
                    hellGate.Draw(spriteBatch);
                    break;

                case GameState.Summary:
                    hellGate.Draw(spriteBatch);
                    if (Mode != GameMode.Tutorial)
                    {
                        if (timeline >= 60) spriteBatch.DrawString(Main.FontNormal, string.Format(Localize.Instance.NumTurn, undo.Count), new Vector2(350, 100), Color.White);
                        if (timeline >= 70) spriteBatch.DrawString(Main.FontNormal, string.Format(Localize.Instance.NumTurn, Level.SolutionData.Length), new Vector2(350, 158), Color.White);
                        if (timeline >= 80) spriteBatch.DrawString(Main.FontNormal, string.Format(Localize.Instance.NumSec, second), new Vector2(350, 216), Color.White);
                        if (timeline >= 90)
                        {
                            if (starLevel >= 1) spriteBatch.Draw(contentHolder.ImageStar, starPosition[0], null, Color.White, 0, vtStarCenter, starFactor[0], SpriteEffects.None, 1);
                            if (starLevel >= 2) spriteBatch.Draw(contentHolder.ImageStar, starPosition[1], null, Color.White, 0, vtStarCenter, starFactor[1], SpriteEffects.None, 1);
                            if (starLevel == 3) spriteBatch.Draw(contentHolder.ImageStar, starPosition[2], null, Color.White, 0, vtStarCenter, starFactor[2], SpriteEffects.None, 1);
                            foreach (Button2D button in btnSummary)
                            {
                                button.Draw(spriteBatch);
                            }
                        }
                    }
                    break;
            }

            //draw sidebar
            spriteBatch.Draw(Mode == GameMode.Adventure ? texSidebarAdventure : texSidebarClassic, vtSidebar, Color.White);

            if (adventureMap == null)
            {
                //ve cac nut dieu khien
                for (int i = 0; i < controller.Length; i++)
                {
                    controller[i].Draw(spriteBatch);
                }

                //draw minimap
                if (Mode != GameMode.Tutorial)
                {
                    for (int i = 0; i < Level.Chamber - 1; i++)
                    {
                        spriteBatch.Draw(contentHolder.ImageCrackedChamber, chambers[i], Color.White);
                    }
                    sprSmallTreasure.Draw(spriteBatch);
                    spriteBatch.Draw(contentHolder.ImageCurrentChamber, chambers[Level.Chamber - 1], Color.White);
                }
                //draw pyramid id in minimap
                string strMapDetail = Level.Pyramid.ToString();
                vtMapDetail.X = 650 - (int)(Main.FontSmall.MeasureString(strMapDetail).X / 2);
                spriteBatch.DrawString(Main.FontSmall, strMapDetail, vtMapDetail, mapDetailColor);
                //draw hint point in minimap
                if (hintPointShow < HintPoint) hintPointShow++;
                else if (hintPointShow > HintPoint) hintPointShow--;
                strMapDetail = hintPointShow.ToString();
                vtMapDetail.X = 750 - (int)(Main.FontSmall.MeasureString(strMapDetail).X / 2);
                spriteBatch.DrawString(Main.FontSmall, strMapDetail, vtMapDetail, mapDetailColor);

                if (Mode != GameMode.Adventure)
                {
                    //draw score
                    int scoreLeft = 700 - (int)Main.FontNormal.MeasureString(scoreShow.ToString()).X / 2;
                    if (scoreShow < Score) scoreShow++;
                    spriteBatch.DrawString(Main.FontNormal, scoreShow.ToString(), new Vector2(scoreLeft, 110), Color.White);
                }
                else
                {
                    //draw go to world map button
                    btnWorldMap.Draw(spriteBatch);
                }

                //draw undo button
                btnUndo.Draw(spriteBatch);


                if (ShowSolution)
                {
                    if (++tick >= 15)
                    {
                        tick = 0;
                        pressReminder = !pressReminder;
                    }
                    if (pressReminder)
                    {
                        spriteBatch.DrawString(Main.FontNormal, Localize.Instance.ShowingSolution, Vector2.Zero, Color.Red);
                    }
                }

                //draw pause menu
                if (State == GameState.Pause || State == GameState.Lose) quickMenu.Draw(spriteBatch);

                //draw pyramid map if it is visible
                if (pyramidMap.Visible) pyramidMap.Draw(spriteBatch);
            }

            //draw confirm dialog if it is visible
            if (uiDialog.Visible) uiDialog.Draw(spriteBatch);

            if (State == GameState.BackMain || State == GameState.SubmitScore) spriteBatch.Draw(contentHolder.ImageWhiteScreen, Vector2.Zero, Color.Black * _blackAlpha);
        }
        #endregion

        #region Control input
        protected override void pointerPressed(int x, int y)
        {
            if (State == GameState.Load || State == GameState.Init) return;

            if (uiDialog.Visible)
            {
                uiDialog.TestHit(x, y);
                return;
            }

            if (State == GameState.OpenMap) return;

            if (State == GameState.Summary)
            {
#if WP8
                if (!GamePage.Instance.AdLoaded) return;
#endif
                foreach (Button2D button in btnSummary) button.TestHit(x, y);
                return;
            }

            if (State == GameState.Pause || State == GameState.Lose || State == GameState.Losing)
            {
                quickMenu.TestHit(x, y);
                return;
            }

            if (ShowSolution) return;

            if (Mode == GameMode.Adventure)
            {
                if (btnWorldMap.TestHit(x, y)) return;
            }
            if (btnUndo.TestHit(x, y)) return;

            if (recController.Contains(x, y))
            {
                int buttonId = -1;
                for (int i = 0; i < controller.Length; i++)
                {
                    if (controller[i].TestHit(x, y)) buttonId = i;
                }
                Explorer.ActiveButton(syncWith3D(buttonId));
                return;
            }

            if (viewpot3DBound.Contains(x, y) && camera.State == CameraState.Stand)
            {
                int activeButton = getActiveButton3D(x, y);
                Explorer.ActiveButton(activeButton);
                syncWith2D(activeButton);
            }
        }

        protected override void pointerDragged(int x, int y)
        {
            if (State == GameState.Load || State == GameState.Init) return;

            if (uiDialog.Visible)
            {
                uiDialog.TestHit(x, y);
                return;
            }

            if (State == GameState.OpenMap) return;

            if (State == GameState.Summary)
            {
#if WP8
                if (!GamePage.Instance.AdLoaded) return;
#endif
                foreach (Button2D button in btnSummary) button.TestHit(x, y);
                return;
            }

            if (State == GameState.Pause || State == GameState.Lose || State == GameState.Losing)
            {
                quickMenu.TestHit(x, y);
                return;
            }

            if (ShowSolution) return;

            if (Mode == GameMode.Adventure)
            {
                if (btnWorldMap.TestHit(x, y)) return;
            }
            if (btnUndo.TestHit(x, y)) return;

            if (recController.Contains(x, y))
            {
                int buttonId = -1;
                for (int i = 0; i < controller.Length; i++)
                {
                    if (controller[i].TestHit(x, y)) buttonId = i;
                }
                Explorer.ActiveButton(syncWith3D(buttonId));
                return;
            }

            if (viewpot3DBound.Contains(x, y) && camera.State == CameraState.Stand)
            {
                int activeButton = getActiveButton3D(x, y);
                Explorer.ActiveButton(activeButton);
                syncWith2D(activeButton);
            }
        }

        protected override void pointerReleased(int x, int y)
        {
            if (State == GameState.Load || State == GameState.Init) return;

            if (uiDialog.Visible)
            {
                UIDialogResult code = uiDialog.CheckHit(x, y);
                if (code == UIDialogResult.None) return;
                if (code == UIDialogResult.Yes)
                {
                    switch (uiDialog.CommandCode)
                    {
                        case COMMAND_MAINMENU:
                            GotoMainMenu();
                            break;
                    }
                }
                uiDialog.FadeOut();
                return;
            }

            if (State == GameState.OpenMap)
            {
                if (pyramidMap.Visible) pyramidMap.PointerReleased(x, y);
                else adventureMap.PointerReleased(x, y);
                return;
            }

            if (State == GameState.Summary)
            {
#if WP8
                if (!GamePage.Instance.AdLoaded && !Main.IsPremium) return;
#endif
                bool needCloseAd = false;

                if (pyramidMap.Visible)
                {
                    pyramidMap.PointerReleased(x, y);
                }
                else if (btnSummary[0].CheckHit(x, y))
                {
                    OpenPyramidMap((short)(Level.Pyramid - 1));
                    btnSummary[0].Active = false;
                    needCloseAd = true;
                }
                else if (btnSummary[1].CheckHit(x, y))
                {
                    byte[] reward = new byte[] { 0, 1, 3, 6 };

                    int hintReward = 0;
                    if (Mode == GameMode.Classic)
                    {
                        //add score and reward
                        hintReward = reward[starLevel];
                        Score += basicScore + bonusScore;
                    }
                    else if (Mode == GameMode.Adventure)
                    {
                        //save new chamber's record
                        short currentPyramidId = (short)(Level.Pyramid - 1);
                        short currentChamberId = (short)(Level.Chamber - 1);
                        if (PyramidMap.ChamberProcess[currentPyramidId][currentChamberId] < starLevel)
                        {
                            hintReward = reward[starLevel] - reward[PyramidMap.ChamberProcess[currentPyramidId][currentChamberId]];
                            PyramidMap.SetChamberProcess(currentPyramidId, currentChamberId, starLevel);
                        }
                    }
                    HintPoint += (Main.IsPremium) ? hintReward * 2 : hintReward;

                    //next level or open world map
                    if (Mode == GameMode.Adventure && pyramidCompleted && hellGate.State == HellGateState.Opened)
                        openWorldMap();
                    else
                        nextLevel();

                    SettingHelper.SaveSetting();
                    needCloseAd = true;
                }
                else if (btnSummary[2].CheckHit(x, y))
                {
                    resetLevelState();
                    needCloseAd = true;
                }

                if (!Main.IsPremium && needCloseAd)
                {
#if ANDROID
                    (Main.Activity as MainActivity).CloseAd();
#endif
#if WINDOWS_PHONE && SILVERLIGHT
                    GamePage.Instance.CloseAd();
#endif
                }
                return;
            }

            if (State == GameState.Pause || State == GameState.Lose || State == GameState.Losing)
            {
                quickMenu.CheckHit(x, y);
                return;
            }

            if (ShowSolution) return;

            //reset tat ca trang thai button
            Explorer.ActiveButton(-1);
            for (int i = 0; i < controller.Length; i++)
            {
                controller[i].Active = false;
            }
            btnUndo.Active = false;
            if (Mode == GameMode.Adventure) btnWorldMap.Active = false;

            if (Mode != GameMode.Tutorial)
            {
                //dieu khien camera quay
#if WINDOWS_PHONE || ANDROID
                if (TouchPanel.IsGestureAvailable && camera.State == CameraState.Stand)
                {
                    if (camera.PerformRotation(x, y))
                    {
                        calculateVisibleButton();
                    }
                    return;
                }
#endif
#if WINDOWS
                if (recRotateCCW.Contains(x, y) || recRotateCW.Contains(x, y))
                {
                    if (camera.PerformRotation(x, y))
                    {
                        calculateVisibleButton();
                    }
                    return;
                }
#endif

                if (Mode == GameMode.Adventure && btnWorldMap.CheckHit(x, y))
                {
                    openWorldMap();
                    return;
                }

                if (btnUndo.CheckHit(x, y))
                {
                    if (Explorer.CanMove)
                    {
                        performUndo();
                        if (undo.Count == 0) btnUndo.Visible = false;
                    }
                    return;
                }

                if (State == GameState.Lose) return;
            }
            else
            {
                if (_tutorialKey == -7)
                {
                    SwitchPlayerTurn();
                    return;
                }
            }

            if (recController.Contains(x, y))
            {
                for (int i = 0; i < controller.Length; i++)
                {
                    if (controller[i].CheckHit(x, y))
                    {
                        makeMove((CharacterState)syncWith3D(i));
                        return;
                    }
                }
                return;
            }

            //thao tac tren khung nhin
            if (viewpot3DBound.Contains(x, y) && camera.State == CameraState.Stand)
            {
                makeMove((CharacterState)getActiveButton3D(x, y));
            }
        }

        protected override void performBack()
        {
            if (uiDialog.Visible)
            {
                uiDialog.FadeOut();
                return;
            }
            switch (State)
            {
                case GameState.Play:
                    if (Mode == GameMode.Tutorial)
                    {
                        uiDialog.Title = Localize.Instance.StopTutorial;
                        uiDialog.Content = Localize.Instance.StopTutorialDescription;
                        uiDialog.CommandCode = COMMAND_MAINMENU;
                        uiDialog.FadeIn(UIDialogType.Confirm);
                    }
                    else
                    {
                        if (Explorer.CanMove)
                        {
                            State = GameState.Pause;
                            quickMenu.SlideUp();
                        }
                    }
                    break;

                case GameState.Pause:
                    quickMenu.PerformBack();
                    break;

                case GameState.OpenMap:
                    if (pyramidMap.Visible)
                    {
                        pyramidMap.SlideDown();
                    }
                    else if (uiDialog.Visible)
                    {
                        uiDialog.FadeOut();
                    }
                    else
                    {
                        uiDialog.Title = Localize.Instance.QuitGame;
                        uiDialog.Content = Localize.Instance.QuitGameDescription;
                        uiDialog.CommandCode = COMMAND_MAINMENU;
                        uiDialog.FadeIn(UIDialogType.Confirm);
                    }
                    break;

                case GameState.Summary:
                    if (pyramidMap.Visible) pyramidMap.SlideDown();
                    break;
            }
        }

        private int getActiveButton3D(int x, int y)
        {
            if (Explorer.State == CharacterState.Stand || Explorer.State == CharacterState.FixFlashlight
                || Explorer.State == CharacterState.Look || Explorer.State == CharacterState.Read
                || Explorer.State == CharacterState.Sleepy)
            {
                Ray pickRay = getPickingRay(x, y);
                return Explorer.CheckButtonPicking(pickRay);
            }
            return -1;
        }

        private int syncWith3D(int button2DId)
        {
            int activeButton = -1;
            if (button2DId < 0 || button2DId > 3) activeButton = button2DId;
            else
            {
                int[] directionMap = camera.DirectionMap;
                for (int i = 0; i < directionMap.Length; i++)
                {
                    if (directionMap[i] == button2DId)
                    {
                        activeButton = i;
                        break;
                    }
                }
            }
            return activeButton;
        }

        private void syncWith2D(int button3DId)
        {
            int activeButton = -1;
            if (button3DId < 0 || button3DId > 3) activeButton = button3DId;
            else activeButton = camera.DirectionMap[button3DId];
            for (int i = 0; i < controller.Length; i++)
            {
                controller[i].Active = false;
            }
            if (activeButton >= 0) controller[activeButton].Active = true;
        }

        private void makeMove(CharacterState moveDirection)
        {
            if (!ShowSolution && !Explorer.CanMove) return;

            switch (moveDirection)
            {
                case CharacterState.MoveUp:
                    if (Mode != GameMode.Tutorial) createUndoData();
                    Explorer.MoveUp();
                    break;

                case CharacterState.MoveRight:
                    if (Mode != GameMode.Tutorial) createUndoData();
                    Explorer.MoveRight();
                    break;

                case CharacterState.MoveDown:
                    if (Mode != GameMode.Tutorial) createUndoData();
                    Explorer.MoveDown();
                    break;

                case CharacterState.MoveLeft:
                    if (Mode != GameMode.Tutorial) createUndoData();
                    Explorer.MoveLeft();
                    break;

                case CharacterState.Stand:
                    if (Mode != GameMode.Tutorial) createUndoData();
                    Explorer.Wait();
                    SwitchEnemyTurn();
                    break;
            }
        }

        private Ray getPickingRay(int x, int y)
        {
            Vector3 nearSource = new Vector3((float)x, (float)y, 0f);
            Vector3 farSource = new Vector3((float)x, (float)y, 1f);
            Viewport viewpot = new Viewport(viewpot3DBound);
            Vector3 nearPoint = viewpot.Unproject(nearSource, camera.Projection, camera.View, Matrix.Identity);
            Vector3 farPoint = viewpot.Unproject(farSource, camera.Projection, camera.View, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }
        #endregion

        #region Game state control methods
        private void startGame()
        {
            SoundController.GetInstance().SetBackgroundMusic(contentHolder.SongIngame);

            //tao co vat o ban do kim tu thap nho
            sprSmallTreasure = new Sprite2D(contentHolder.ImageSmallTreasures, 38, 38);
            sprSmallTreasure.SetPosition(680, 167);

            //tao co vat o buc tranh hoan thanh kim tu thap
            sprNiceTreasure = new Sprite2D(contentHolder.ImageNiceTreasures, 133, 127);
            sprNiceTreasure.SetPosition(265, 94);

            //tao he thong hat
            ParticleManager = new ParticleManager(main.GraphicsDevice);

            //tao khung ve 3d
            viewpot3D = new RenderTarget2D(graphicsDevice, 600, 480, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            viewpot3DBound = new Rectangle(0, 0, viewpot3D.Width, viewpot3D.Height);

            //tao cac nut dieu khien
            int cX = recController.X;
            int cY = recController.Y;
            Vector2 vtController = new Vector2(cX, cY);
            controller = new Button2D[] {
                new Button2D(contentHolder.ImageUp[0], contentHolder.ImageUp[1], null, vtController, new Rectangle(cX,cY,110,54), true),
                new Button2D(contentHolder.ImageRight[0], contentHolder.ImageRight[1], null, vtController, new Rectangle(cX+160,cY,110,54), true),
                new Button2D(contentHolder.ImageDown[0], contentHolder.ImageDown[1], null, vtController, new Rectangle(cX+180,cY+90,90,90), true),
                new Button2D(contentHolder.ImageLeft[0], contentHolder.ImageLeft[1], null, vtController, new Rectangle(cX,cY+90,90,90), true),
                new Button2D(contentHolder.ImageCenter[0], contentHolder.ImageCenter[1], null, vtController, new Rectangle(cX+90,cY+54,90,70), true)
            };
            foreach (Button2D button in controller)
            {
                button.FadeAtVisible = true;
                button.FadeAtActive = false;
            }

            //tao cac nut tong ket
            btnSummary = new Button2D[] {
                new Button2D(contentHolder.ImageSummarySelect[0], contentHolder.ImageSummarySelect[1], null, new Vector2(140, 388)),
                new Button2D(contentHolder.ImageSummaryNext[0], contentHolder.ImageSummaryNext[1], null, new Vector2(260, 378)),
                new Button2D(contentHolder.ImageSummaryRestart[0], contentHolder.ImageSummaryRestart[1], null, new Vector2(400, 388))
            };
            foreach (Button2D button in btnSummary)
            {
                button.FadeAtActive = true;
            }

            //tao nut undo va map
            btnUndo = new Button2D(contentHolder.ImageUndo[0], contentHolder.ImageUndo[1], null, new Vector2(614, 8));
            btnUndo.FadeAtActive = true;
            if (Mode == GameMode.Adventure)
            {
                btnWorldMap = new Button2D(contentHolder.ImageWorldMap[0], contentHolder.ImageWorldMap[1], null, new Vector2(614, 78));
                btnWorldMap.FadeAtActive = true;
                btnWorldMap.FadeAtVisible = false;
            }

            //tao camera
            float aspectRatio = (float)viewpot3D.Width / (float)viewpot3D.Height;
            camera = new RotateCamera(0, aspectRatio);

            switch (Mode)
            {
                case GameMode.Tutorial:
                    Level = new LevelManager(GameMode.Tutorial);
                    break;

                case GameMode.Classic:
                    short pyramid = SettingHelper.GetSetting<short>(RECORD_CLASSIC_PYRAMID, 1);
                    short chamber = SettingHelper.GetSetting<short>(RECORD_CLASSIC_CHAMBER, 1);
                    Level = new LevelManager(pyramid, chamber, Mode);
                    _score = scoreShow = SettingHelper.GetSetting<int>(RECORD_CLASSIC_SCORE, 0);
                    break;

                case GameMode.Adventure:
                    createWorldMap();
                    SoundController.GetInstance().FadeIn();
                    return;
            }

            beginLevel();
        }

        public void EnterPyramid(short pyramidId, short chamberId)
        {
            if (State == GameState.Summary)
            {
                Level = new LevelManager((short)(pyramidId + 1), (short)(chamberId + 1), Mode);
                beginLevel();
            }
            else if (State == GameState.OpenMap)
            {
                Level = new LevelManager((short)(pyramidId + 1), (short)(chamberId + 1), Mode);
                timeline = -1;
                State = GameState.CloseMap;
            }
        }

        private void openWorldMap()
        {
            timeline = -1;
            State = GameState.CloseMap;
        }

        public void OpenPyramidMap(short pyramidId)
        {
            pyramidMap.PyramidId = pyramidId;
            pyramidMap.Title = Localize.Instance.Pyramid[pyramidId];
            pyramidMap.Description = Localize.Instance.PyramidDescription[pyramidId];
            pyramidMap.ReachedChamberId = AdventureMap.Instance.PyramidProcess[pyramidId];
            pyramidMap.SlideUp();
        }

        private void createWorldMap()
        {
            adventureMap = new AdventureMap(this);
            timeline = -1;
            State = GameState.OpenMap;
            hellGate.State = (hellGate.ContentTexture != null) ? HellGateState.FadingOut : HellGateState.Opening;
        }

        private void beginLevel()
        {
            //reset state of summary buttons
            foreach (Button2D button in btnSummary)
            {
                button.ForceInactive();
                button.ForceInvisible();
            }

            //tao du lieu undo
            undo = new List<UndoData>();
            btnUndo.Visible = false;

            //if in adventure mode and pyramid equal 7
            IsDarkness = (Mode == GameMode.Adventure && Level.Pyramid == 8);

            //tao me cung
            Cell = Level.MazeData;
            MazeSize = (int)Math.Sqrt(Cell.Length / 2);
            int fullMazeSize = MazeSize * 10;
            int halfMazeSize = MazeSize * 5;

            //tao cac buc tuong
            Walls = new List<CModel>();
            for (int i = 0; i < MazeSize; i++)
            {
                for (int j = 0; j < MazeSize; j++)
                {
                    //0 - trai
                    if (Cell[i, j, 0])
                    {
                        Walls.Add(new CModel(
                            PlayContentHolder.Instance.ModelWall,
                            new Vector3((j + 1) * 10, 0, i * 10 + 5),
                            new Vector3(0, MathHelper.ToRadians(90), 0)
                        ));

                    }
                    //1 - duoi
                    if (Cell[i, j, 1])
                    {
                        Walls.Add(new CModel(
                            PlayContentHolder.Instance.ModelWall,
                            new Vector3(j * 10 + 5, 0, (i + 1) * 10)
                        ));
                    }
                }
            }

            //tao camera
            camera.ResetCamera(halfMazeSize);
            needleAngle = MathHelper.ToRadians(camera.Angle);

            //tao nen
            floorModelId = (byte)((MazeSize - 6) / 2);
            Floor = new CModel(contentHolder.ModelFloor[floorModelId], new Vector3(halfMazeSize, 0, halfMazeSize));
            if (IsDarkness)
            {
                darkFloor = new RenderTarget2D(graphicsDevice, 256, 256);
                Floor.CustomTexture = darkFloor;
                vtFloorMaskCenter = new Vector2(contentHolder.TextureFloorMask[floorModelId].Width / 2, contentHolder.TextureFloorMask[floorModelId].Height / 2);
            }

            //tao bay
            Traps = new List<Trap>();
            if (Level.TrapData != null)
            {
                int totalTrap = Level.TrapData.Length / 2;
                for (int i = 0; i < totalTrap; i++)
                {
                    Traps.Add(new Trap(new int[] { Level.TrapData[i, 0], Level.TrapData[i, 1] }, this));
                }
            }

            //tao tuong bao quanh
            Model mBorder = contentHolder.ModelBorder[(MazeSize - 6) / 2];
            Borders = new CModel[] {
                new CModel(mBorder, new Vector3(MazeSize*5, 0, 0)),
                new CModel(mBorder, new Vector3(MazeSize*10, 0, MazeSize*5), new Vector3(0,MathHelper.ToRadians(-90),0)),
                new CModel(mBorder, new Vector3(MazeSize*5, 0, MazeSize*10), new Vector3(0,MathHelper.ToRadians(-180),0)),
                new CModel(mBorder, new Vector3(0, 0, MazeSize*5), new Vector3(0,MathHelper.ToRadians(-270),0))
            };
            foreach (CModel curBorder in Borders)
            {
                curBorder.Dim = IsDarkness;
            }

            //tao cau thang
            int stairX = 0, stairZ = 0;
            switch (Level.EscapeData[0])
            {
                case 0:
                    stairX = Level.EscapeData[1] * 10 + 5;
                    stairZ = -5;
                    EscapeCell = new int[] { 0, Level.EscapeData[1] };
                    Borders[0].HideMesh(Level.EscapeData[1]);
                    break;

                case 1:
                    stairX = fullMazeSize + 5;
                    stairZ = Level.EscapeData[1] * 10 + 5;
                    EscapeCell = new int[] { Level.EscapeData[1], MazeSize - 1 };
                    Borders[1].HideMesh(Level.EscapeData[1]);
                    break;

                case 2:
                    stairX = Level.EscapeData[1] * 10 + 5;
                    stairZ = fullMazeSize + 5;
                    EscapeCell = new int[] { MazeSize - 1, Level.EscapeData[1] };
                    Borders[2].HideMesh(MazeSize - Level.EscapeData[1] - 1);
                    break;

                case 3:
                    stairX = -5;
                    stairZ = Level.EscapeData[1] * 10 + 5;
                    EscapeCell = new int[] { Level.EscapeData[1], 0 };
                    Borders[3].HideMesh(MazeSize - Level.EscapeData[1] - 1);
                    break;
            }
            EscapeDirection = Level.EscapeData[0];
            Stair = new CModel(
                PlayContentHolder.Instance.ModelStair,
                new Vector3(stairX, 0, stairZ),
                new Vector3(0, MathHelper.ToRadians(Level.EscapeData[0] * -90), 0)
            );
            Stair.Dim = IsDarkness;

            //tao bo cap
            Scorpions = new List<Scorpion>();
            if (Level.ScorpionData != null)
            {
                //0 - hang; 1 - cot; 2 - loai
                int totalScorpion = Level.ScorpionData.Length / 3;
                for (int i = 0; i < totalScorpion; i++)
                {
                    int type = Level.ScorpionData[i, 2];
                    int[] position = new int[] { Level.ScorpionData[i, 0], Level.ScorpionData[i, 1] };
                    Scorpion newScorpion = new Scorpion(PlayContentHolder.Instance.ModelScorpions[type], type, position, this);
                    Scorpions.Add(newScorpion);
                }
            }

            //tao xac uop
            Mummies = new List<Mummy>();
            if (Level.MummyData != null)
            {
                //0 - hang; 1 - cot; 2 - loai;
                int totalMummy = Level.MummyData.Length / 3;

                for (var i = 0; i < totalMummy; i++)
                {
                    int type = Level.MummyData[i, 2];
                    int[] position = new int[] { Level.MummyData[i, 0], Level.MummyData[i, 1] };
                    Mummy newMummy = new Mummy(PlayContentHolder.Instance.ModelMummies[type], type, position, this);
                    this.Mummies.Add(newMummy);
                    newMummy.SetInit();
                    Floor.HideMesh(position[0] * MazeSize + position[1]);
                }
            }

            //tao cong chan
            if (Level.GateData != null && Level.GateData.Length == 4) Gate = new GateSystem(new int[] { Level.GateData[2], Level.GateData[3] }, new int[] { Level.GateData[0], Level.GateData[1] }, this);
            else Gate = null;

            //tao nha tham hiem
            Explorer = new Explorer(contentHolder.ModelExplorer, new int[] { Level.ExplorerData[0], Level.ExplorerData[1] }, this);

            //cap nhat frame cho co vat
            sprSmallTreasure.SetFrame(Level.Pyramid - 1);

            SoundController.GetInstance().FadeIn();
            second = 0;
            tick = 0;
            timeline = 0;
            State = GameState.Init;
            hellGate.State = (hellGate.ContentTexture != null) ? HellGateState.FadingOut : HellGateState.Opening;

            if (Mode == GameMode.Adventure) btnWorldMap.Visible = true;
        }

        private void nextLevel()
        {
            switch (Mode)
            {
                case GameMode.Tutorial:
                    if (!Level.NextLevel())
                    {
                        SettingHelper.StoreSetting(RECORD_FIRST_PLAY, false, true);
                        if (_returnMenuAfterTutorial)
                        {
                            GotoMainMenu();
                            return;
                        }
                        else
                        {
                            Mode = GameMode.Classic;
                            Level = new LevelManager(Mode);
                        }
                    }
                    break;

                case GameMode.Classic:
                    if (!Level.NextLevel())
                    {
                        if (Score > 0) GotoSubmitScore();
                        else GotoMainMenu();
                    }
                    break;

                case GameMode.Adventure:
                    if (!Level.NextLevel())
                    {
                        //createWorldMap();
                        sprNiceTreasure.SetFrame(Level.Pyramid - 1);
                        timeline = -1;
                        //State = GameState.PyramidComplete;
                        pyramidCompleted = true;
                        hellGate.State = HellGateState.Opening;
                        return;
                    }
                    break;
            }

            resetLevelState();
        }

        private void resetLevelState()
        {
            //xoa me cung
            Cell = null;
            MazeSize = 0;

            //xoa cac buc tuong
            Walls = null;

            //reset camera
            camera.ResetCamera(0);
            needleAngle = 0f;

            //xoa nen
            Floor = null;

            //xoa bay
            Traps = null;

            //xoa tuong bao quanh
            Borders = null;

            //xoa cau thang
            EscapeCell = null;
            EscapeDirection = -1;
            Stair = null;

            //xoa bo cap
            Scorpions = null;

            //xoa xac uop
            Mummies = null;

            //xoa cong chan
            Gate = null;

            //xoa nha tham hiem
            Explorer = null;

            //xoa du lieu undo
            undo.Clear();

            beginLevel();
        }

        byte starLevel = 1;
        public void WinLevel()
        {
#if DEBUG
            Main.InsertLog("Explorer: I escaped!");
#endif
            SoundController.GetInstance().FadeOutHalf();
            SoundController.PlaySound(contentHolder.SoundVictory);
            btnUndo.Visible = false;
            timeline = 0;
            if (Mode != GameMode.Tutorial)
            {
                SettingHelper.StoreSetting(RECORD_FIRST_PLAY, false, true);

                starFactor = new float[] { 0, 0, 0 };
                if (ShowSolution)
                {
                    ShowSolution = false;
                    basicScore = bonusScore = 0;
                    starLevel = 0;
                }
                else
                {
                    basicScore = (((Level.Pyramid * 15 + Level.Chamber - 1) / 5 - 3) * 5) + 10;
                    int moveWasted = undo.Count - Level.SolutionData.Length;
                    if (moveWasted > 8) starLevel = 1;
                    else if (moveWasted > 4) starLevel = 2;
                    else starLevel = 3;
                    bonusScore = (int)(basicScore * (float)(starLevel / 3));
                }
            }
            State = GameState.Summary;
            hellGate.ContentTexture = Mode == GameMode.Tutorial ? null : contentHolder.ImageLevelCompleted;
            hellGate.State = HellGateState.Closing;

            if (Mode == GameMode.Adventure)
            {
                btnWorldMap.ForceInactive();
                btnWorldMap.Visible = false;
            }
        }

        public void Lose()
        {
#if DEBUG
            Main.InsertLog("Explorer: I lose!");
#endif
            timeline = -1;
            State = GameState.Losing;
        }

        public void Resume()
        {
            if (State == GameState.Pause) State = GameState.Play;
        }

        public void GotoMainMenu()
        {
            timeline = -1;
            _blackAlpha = 0;
            State = GameState.BackMain;
            SoundController.GetInstance().FadeOut();
        }

        public void GotoSubmitScore()
        {
            timeline = -1;
            _blackAlpha = 0;
            State = GameState.SubmitScore;
            SoundController.GetInstance().FadeOut();
        }

        public void GetSolution()
        {
            SolutionData = Level.SolutionData.ToList<short>();
            Resume();
            PerformReset();
            explorerDirectionAvailable = new bool[5];
            calculateVisibleButton();
            ShowSolution = true;
            SwitchPlayerTurn();
        }
        #endregion

        #region Logic invock methods
        //kiem tra va xu ly dung do
        public bool CheckCollision(Enemy enemy)
        {
            //neu doi tuong da "chet" thi khong kiem tra them nua
            if (!enemy.IsLive()) return false;

            List<Character> characters = new List<Character>();

            //kiem tra nha tham hiem co dung do ko
            if (Explorer.Position[0] == enemy.Position[0] && Explorer.Position[1] == enemy.Position[1]) characters.Add(Explorer);

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
                bool isFinished = false;
                while (characters.Count > 1)
                {
                    Type characterType = characters[0].GetType();
                    if (characterType.Equals(typeof(Explorer)))
                    {
                        Explorer.Catched();
                        explorerDirectionAvailable = new bool[5];
                        calculateVisibleButton();
                        isFinished = true;
                    }
                    else if (characterType.Equals(typeof(Scorpion)))
                    {
                        Scorpions.Remove(characters[0] as Scorpion);
                    }
                    else if (characterType.Equals(typeof(Mummy)))
                    {
                        Mummies.Remove(characters[0] as Mummy);
                    }
                    characters.Remove(characters[0]);
                }
                Enemy lastCharacter = characters.First() as Enemy;
                if (isFinished)
                {
                    if (lastCharacter.GetType().Equals(typeof(Mummy)))
                    {
                        Explorer.Catched();
                    }
                    else
                    {
                        Explorer.Poisoned();
                    }
                }
                lastCharacter.Fight(isFinished);
            }
            return haveCollision;
        }

        //kiem tra 1 o xem co ke thu nao o do khong
        public bool CellHasEnemy(int row, int col)
        {
            foreach (Mummy mummy in Mummies)
            {
                if (mummy.Position[0] == row && mummy.Position[1] == col) return true;
            }
            foreach (Scorpion scorpion in Scorpions)
            {
                if (scorpion.Position[0] == row && scorpion.Position[1] == col) return true;
            }
            return false;
        }

        //kiem tra 1 o co bay khong
        public bool TriggerTrap(int[] explorerPosition)
        {
            foreach (Trap trap in Traps)
            {
                if (trap.TriggerTrap(explorerPosition))
                {
                    explorerDirectionAvailable = new bool[5];
                    calculateVisibleButton();
                    return true;
                }
            }
            return false;
        }

        //chuyen sang luot nguoi choi
        public void SwitchPlayerTurn()
        {
            foreach (Mummy mummy in Mummies)
            {
                if (mummy.State == CharacterState.MoveUp ||
                    mummy.State == CharacterState.MoveRight ||
                    mummy.State == CharacterState.MoveDown ||
                    mummy.State == CharacterState.MoveLeft ||
                    mummy.State == CharacterState.Fight ||
                    mummy.State == CharacterState.Idle)
                    return;
            }
            foreach (Scorpion scorpion in Scorpions)
            {
                if (scorpion.State == CharacterState.MoveUp ||
                    scorpion.State == CharacterState.MoveRight ||
                    scorpion.State == CharacterState.MoveDown ||
                    scorpion.State == CharacterState.MoveLeft ||
                    scorpion.State == CharacterState.Fight ||
                    scorpion.State == CharacterState.Idle)
                    return;
            }

            if (Explorer.State != CharacterState.Catched && Explorer.State != CharacterState.Poisoned
                && Explorer.Position[0] == EscapeCell[0] && Explorer.Position[1] == EscapeCell[1])
            {
                if (Mode != GameMode.Tutorial)
                {
                    Explorer.Escaped();
                    explorerDirectionAvailable = new bool[5];
                }
            }
            else
            {
                if (ShowSolution)
                {
                    makeMove((CharacterState)SolutionData[0]);
                    SolutionData.RemoveAt(0);
                    return;
                }
                else
                {
                    Explorer.Stand();
                }
            }
            if (Mode == GameMode.Tutorial)
            {
                nextTutorialStep();
                if (_tutorialKey == -1)
                {
                    if (Explorer.Position[0] == EscapeCell[0] && Explorer.Position[1] == EscapeCell[1]) Explorer.Escaped();
                    else WinLevel();
                    explorerDirectionAvailable = new bool[5];
                }
            }
            calculateVisibleButton();
        }

        int _tutorialStep = -1;
        string _tutorialDescription = "";

        int _tutorialKey;
        public int CurrentTutorialKey { get { return _tutorialKey; } }

        private void nextTutorialStep()
        {
            _tutorialStep++;
            KeyValuePair<int, string> stepInfo = Level.GetStepInfo(_tutorialStep);
            _tutorialKey = stepInfo.Key;
            _tutorialDescription = stepInfo.Value;
            Explorer.UpdateControllerState();
        }

        //chuyen sang luot di cua ke thu
        public void SwitchEnemyTurn()
        {
            //Explorer.Wait();
            foreach (Mummy mummy in Mummies)
            {
                mummy.Idle();
                mummy.MovementLeft = 2;
            }
            foreach (Scorpion scorpion in Scorpions)
            {
                scorpion.Idle();
                scorpion.MovementLeft = 1;
            }
        }

        //dieu chinh lai cac nut bam duoc hien thi
        private void calculateVisibleButton()
        {
            int[] directionMap = camera.DirectionMap;
            for (int i = 0; i < 4; i++)
            {
                controller[directionMap[i]].Visible = explorerDirectionAvailable[i];
            }
            controller[4].Visible = explorerDirectionAvailable[4];
        }
        #endregion

        #region Record steps
        private void createUndoData()
        {
            bool difference = false;
            UndoData lastUndo = null;
            if (undo.Count > 0) lastUndo = undo.Last();
            else difference = true;
            UndoData newUndo = new UndoData();

            newUndo.Explorer = new int[] { Explorer.Position[0], Explorer.Position[1] };
            //check if difference
            if (!difference)
            {
                if (lastUndo.Explorer[0] != newUndo.Explorer[0] || lastUndo.Explorer[1] != newUndo.Explorer[1]) difference = true;
            }

            newUndo.Mummies = new List<int[]>();
            foreach (Mummy mummy in Mummies)
            {
                int[] mummyData = new int[] { mummy.Position[0], mummy.Position[1], mummy.Type };
                newUndo.Mummies.Add(mummyData);
                //check if difference
                if (!difference)
                {
                    bool dif = true;
                    foreach (int[] lastMummy in lastUndo.Mummies)
                    {
                        if (lastMummy[0] == mummyData[0] && lastMummy[1] == mummyData[1] && lastMummy[2] == mummyData[2])
                        {
                            dif = false;
                            break;
                        }
                    }
                    if (dif) difference = true;
                }
            }

            newUndo.Scorpions = new List<int[]>();
            foreach (Scorpion scorpion in Scorpions)
            {
                int[] scorpionData = new int[] { scorpion.Position[0], scorpion.Position[1], scorpion.Type };
                newUndo.Scorpions.Add(scorpionData);
                //check if difference
                if (!difference)
                {
                    bool dif = true;
                    foreach (int[] lastScorpion in lastUndo.Scorpions)
                    {
                        if (lastScorpion[0] == scorpionData[0] && lastScorpion[1] == scorpionData[1] && lastScorpion[2] == scorpionData[2])
                        {
                            dif = false;
                            break;
                        }
                    }
                    if (dif) difference = true;
                }
            }

            newUndo.GateIsOpen = (Gate != null && Gate.State == GateState.Opened);
            //check if difference
            if (!difference)
            {
                if (newUndo.GateIsOpen != lastUndo.GateIsOpen) difference = true;
            }

            if (difference)
            {
                undo.Add(newUndo);
                btnUndo.Visible = true;
            }
        }

        private void performUndo()
        {
            if (undo.Count == 0) return;

            UndoData lastUndo = undo.Last();

            //check if not difference
            bool difference = false;
            if (!difference)
            {
                if (Explorer.Position[0] != lastUndo.Explorer[0] || Explorer.Position[1] != lastUndo.Explorer[1]) difference = true;
            }
            foreach (Mummy mummy in Mummies)
            {
                int[] mummyData = new int[] { mummy.Position[0], mummy.Position[1], mummy.Type };
                if (!difference)
                {
                    bool dif = true;
                    foreach (int[] lastMummy in lastUndo.Mummies)
                    {
                        if (lastMummy[0] == mummyData[0] && lastMummy[1] == mummyData[1] && lastMummy[2] == mummyData[2])
                        {
                            dif = false;
                            break;
                        }
                    }
                    if (dif) difference = true;
                }
            }
            foreach (Scorpion scorpion in Scorpions)
            {
                int[] scorpionData = new int[] { scorpion.Position[0], scorpion.Position[1], scorpion.Type };
                if (!difference)
                {
                    bool dif = true;
                    foreach (int[] lastScorpion in lastUndo.Scorpions)
                    {
                        if (lastScorpion[0] == scorpionData[0] && lastScorpion[1] == scorpionData[1] && lastScorpion[2] == scorpionData[2])
                        {
                            dif = false;
                            break;
                        }
                    }
                    if (dif) difference = true;
                }
            }
            if (!difference)
            {
                bool gateIsOpen = (Gate != null && Gate.State == GateState.Opened);
                if (lastUndo.GateIsOpen != gateIsOpen) difference = true;
            }
            if (!difference)
            {
                undo.Remove(lastUndo);
                lastUndo = undo.Last();
            }

            Mummies.Clear();
            foreach (int[] mummyData in lastUndo.Mummies)
            {
                Mummies.Add(new Mummy(
                    PlayContentHolder.Instance.ModelMummies[mummyData[2]],
                    mummyData[2],
                    new int[] { mummyData[0], mummyData[1] },
                    this
                    ));
            }

            Scorpions.Clear();
            foreach (int[] scorpionData in lastUndo.Scorpions)
            {
                Scorpions.Add(new Scorpion(
                    PlayContentHolder.Instance.ModelScorpions[scorpionData[2]],
                    scorpionData[2],
                    new int[] { scorpionData[0], scorpionData[1] },
                    this
                    ));
            }

            foreach (Trap trap in Traps) trap.Rewind();
            Floor.UnhideAllMesh();

            if (Gate != null) Gate.Rewind(lastUndo.GateIsOpen);

            Explorer.Rewind(lastUndo.Explorer);

            calculateVisibleButton();

            undo.Remove(lastUndo);

            State = GameState.Play;
        }

        public void PerformReset()
        {
            if (undo.Count == 0) return;

            UndoData firstUndo = undo.First();
            undo.Clear();
            undo.Add(firstUndo);
            performUndo();
            btnUndo.Visible = false;
        }
        #endregion

        #region misc
        public void adventureInstruction(byte instructionId)
        {
            uiDialog.Title = Localize.Instance.AdventureMode;
            uiDialog.CommandCode = COMMAND_NONE;
            switch (instructionId)
            {
                case 1:
                    uiDialog.Content = Localize.Instance.AdventureModeDescription_1;
                    uiDialog.CommandCode = COMMAND_ADVENTURE_NOTIFY_2;
                    break;

                case 2:
                    uiDialog.Content = Localize.Instance.AdventureModeDescription_2;
                    break;

                case 3:
                    uiDialog.Content = Localize.Instance.AdventureModeDescription_3;
                    break;
            }

            uiDialog.FadeIn(UIDialogType.Message);
        }
        #endregion
    }
}
