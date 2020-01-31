using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using OpenitvnGame;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PyramidRaiderLevelEditor
{
    public class User32
    {
        [DllImport("user32.dll")]
        public static extern void SetWindowPos(uint Hwnd, uint Level, int X,
            int Y, int W, int H, uint Flags);
    }

    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        static Main _instance;
        public static Main GetInstance() { return _instance; }

        GameScene scene;

        public Main()
        {
            _instance = this;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 360;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            ////set screen position using the new User32 class  
            //User32.SetWindowPos((uint)this.Window.Handle, 0, 710, 140,
            //       graphics.PreferredBackBufferWidth,
            //       graphics.PreferredBackBufferHeight, 0);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            scene = new CreateScene();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            scene.PerformStandardInput();
            scene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            scene.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (typeof(CreateScene).Equals(scene.GetType()))
            {
                (scene as CreateScene).StopBatch();
            }
            Process[] hackmm = Process.GetProcessesByName("hackmm");
            foreach (System.Diagnostics.Process process in hackmm)
            {
                process.CloseMainWindow();
            }
            base.OnExiting(sender, args);
        }
    }
}
