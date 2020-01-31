using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if WP7
using System.Windows;
#endif
#if WINDOWS
using System.Windows.Forms;
#endif
using OpenitvnGame;
using OpenitvnGame.Helpers;
using System;
using System.Collections.Generic;

namespace PyramidRaider
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        public const int DESIGN_WIDTH = 800;
        public const int DESIGN_HEIGHT = 480;
        public const string RECORD_PREMIUM = "premium";

        public static SpriteFont FontNormal, FontSmall, FontLarge;

        public static Main Instance { get; private set; }

#if WP7
        SharedGraphicsDeviceManager graphics;
#else
        GraphicsDeviceManager graphics;
#endif
        SpriteBatch spriteBatch;
        GameScene scene;

        SoundController soundController;

        public static bool IsPremium { get; set; }
        
        public Main()
        {
            Instance = this;

            //original resolution definition
#if WP7
            graphics = SharedGraphicsDeviceManager.Current;
            Content = (Application.Current as App).Content;
#else
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphics.IsFullScreen = true;
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            InactiveSleepTime = TimeSpan.FromSeconds(1);
#endif
            graphics.PreferredBackBufferWidth = DESIGN_WIDTH;
            graphics.PreferredBackBufferHeight = DESIGN_HEIGHT;
#if WP8
            switch (App.Current.Host.Content.ScaleFactor)
            {
                case 160:
                    //wxga
                    graphics.PreferredBackBufferWidth = 1280;
                    graphics.PreferredBackBufferHeight = 768;
                    break;

                case 150:
                    //720p
                    graphics.PreferredBackBufferWidth = 1280;
                    graphics.PreferredBackBufferHeight = 720;
                    break;
            }
#endif
#if ANDROID
            graphics.PreferredBackBufferWidth = Activity.WindowManager.DefaultDisplay.Width;
            graphics.PreferredBackBufferHeight = Activity.WindowManager.DefaultDisplay.Height;
#endif
#if WINDOWS
            renderWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            renderHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#endif

#if WINDOWS
            Window.IsBorderless = true;
            IsMouseVisible = true;
#endif
            Content.RootDirectory = "Content";

            soundController = SoundController.CreateInstance(this);
        }

        protected override void Initialize()
        {
            base.Initialize();

#if ANDROID
            FeedbackHelper.Default.Initialise(Activity as IFeedbackCaller);
#else
            FeedbackHelper.Default.Initialise();
#endif
            IsPremium = SettingHelper.GetSetting<bool>(Main.RECORD_PREMIUM, false);
        }

        protected override void LoadContent()
        {
            //scaled resolution definition
            setupDrawFactor();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            soundController.LoadContent();
            FontNormal = Content.Load<SpriteFont>("Fonts/normal");
            FontSmall = Content.Load<SpriteFont>("Fonts/small");
            FontLarge = Content.Load<SpriteFont>("Fonts/large");
            HellGate.ImageHellGate = Content.Load<Texture2D>("Images/hellGate");

            GotoSplash(false);
            //GotoSubmitScore(100);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (scene != null) scene.PerformStandardInput();
            if (scene != null) scene.Update(gameTime);
            soundController.Update();
            base.Update(gameTime);
        }

#if DEBUG
        private static List<string> debugStack = new List<string>();
        public static void InsertLog(string content)
        {
            System.Diagnostics.Debug.WriteLine(content);
            debugStack.Add(content);
            if (debugStack.Count > 4) debugStack.RemoveAt(0);
        }
        public static void WriteLog(SpriteBatch spriteBatch)
        {
            int i = 0;
            while (i < debugStack.Count) spriteBatch.DrawString(Main.FontSmall, debugStack[i], new Vector2(0, i++ * 24), Color.White * 0.4f);

#if WINDOWS_PHONE
            long memoryInMb = (long)Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage") / 1000000;
            spriteBatch.DrawString(FontSmall, "Peak memory: " + memoryInMb, new Vector2(0, 440), Color.White * 0.4f);
#endif
        }
#endif

        protected override void Draw(GameTime gameTime)
        {
            if (scene != null)
            {
                GraphicsDevice.SetRenderTarget(Resolution.Screen);
                GraphicsDevice.Clear(Color.Black);
                SpriteBatch dSpriteBatch = new SpriteBatch(GraphicsDevice);
                dSpriteBatch.Begin();
                scene.Draw(dSpriteBatch);
                dSpriteBatch.End();
                GraphicsDevice.SetRenderTarget(null);

                spriteBatch.Begin();
                spriteBatch.Draw(Resolution.Screen, Resolution.Viewpot, Color.White);
#if DEBUG
                WriteLog(spriteBatch);
#endif
#if WP7
                spriteBatch.Draw(GamePage.Instance.UiElementRenderer.Texture, Vector2.Zero, Color.White);
#endif
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public void GotoSplash(bool isBack)
        {
            scene = new SplashScene(isBack);
        }

        internal void GotoGame(GameMode gameMode, bool returnMenuAfterTutorial)
        {
            scene = new PlayScene(gameMode, returnMenuAfterTutorial);
        }

        public void GotoMainMenu()
        {
            scene = new MainMenuScene();
        }

        public void GotoMainMenu(int flag)
        {
            scene = new MainMenuScene(flag);
        }

        public void GotoInstruction(Texture2D texPage)
        {
            scene = new InstructionScene(texPage);
        }

        internal void GotoLeaderboard()
        {
            scene = new LeaderboardScene();
        }

        public void GotoSubmitScore(int score)
        {
            scene = new LeaderboardScene(score);
        }

        private void setupDrawFactor()
        {
            Resolution.Init(graphics);
            Resolution.CropMethod = ResolutionCropMethod.Softscale;
            Resolution.DesignWidth = DESIGN_WIDTH;
            Resolution.DesignHeight = DESIGN_HEIGHT;
            Resolution.ApplyResolutionSettings();
        }

        public static void SwitchToPremium()
        {
            IsPremium = true;
            SettingHelper.StoreSetting(RECORD_PREMIUM, IsPremium, true);
            SettingHelper.SaveSetting();
        }

#if WP7
        public void Exit()
        {
            App.Quit();
        }
        public void PerformBack()
        {
            if (scene != null) scene.PerformBack();
        }
#endif
    }
}
