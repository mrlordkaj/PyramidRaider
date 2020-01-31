using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#if !WINDOWS
using Microsoft.Xna.Framework.Input.Touch;
using System;
#endif
#if WP7
using PyramidRaider;
#endif

namespace OpenitvnGame
{
    abstract class GameScene
    {
        protected Game main;
        protected GraphicsDevice graphicsDevice;
        private ContentManager content;

        private static Random _random;
        public static Random Random
        {
            get
            {
                if (_random == null) _random = new Random();
                return _random;
            }
        }

        public GameScene(Game parent)
        {
            main = parent;
            graphicsDevice = main.GraphicsDevice;
            content = main.Content;
            prepareContent(main.Content);
        }

        protected abstract void prepareContent(ContentManager content);
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
        protected virtual void pointerPressed(int x, int y) { }
        protected virtual void pointerDragged(int x, int y) { }
        protected virtual void pointerReleased(int x, int y) { }
        protected virtual void performBack() { }

        bool isTouching = false;
        int pointerX, pointerY;
#if WINDOWS
        private bool waitKeyUp = false;
#endif

        public void PerformStandardInput()
        {
#if WINDOWS
            //nhan phim ESC
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyUp(Keys.Escape))
            {
                if (waitKeyUp)
                {
                    performBack();
                    waitKeyUp = false;
                }
            }
            if (keyboardState.IsKeyDown(Keys.Escape)) waitKeyUp = true;

            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                int x = mouse.X;
                int y = mouse.Y;
#else
            //nhan phim back
#if !WP7
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) performBack();
#endif

            TouchCollection touches = TouchPanel.GetState();
            if (touches.Count > 0)
            {
                int x = (int)touches[0].Position.X;
                int y = (int)touches[0].Position.Y;
#endif
                Resolution.ConvertX(ref x);
                Resolution.ConvertY(ref y);
                if (!isTouching)
                {
                    isTouching = true;
                    pointerPressed(x, y);
                }
                else if (pointerX != x || pointerY != y)
                {
                    pointerDragged(x, y);
                }
                pointerX = x;
                pointerY = y;
            }
            else if (isTouching)
            {
                isTouching = false;
                pointerReleased(pointerX, pointerY);
            }
        }

#if WP7
        public void PerformBack() { performBack(); }
#endif
    }
}
