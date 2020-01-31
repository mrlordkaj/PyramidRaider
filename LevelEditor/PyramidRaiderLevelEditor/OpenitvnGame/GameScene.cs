using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif

namespace OpenitvnGame
{
    abstract class GameScene
    {
        public static float FACTOR_WIDTH = 0.5f;
        public static float FACTOR_HEIGHT = 0.5f;

        protected Game main;
        protected GraphicsDevice graphicsDevice;
        protected ContentManager content;

        public GameScene(Game parent)
        {
            main = parent;
            graphicsDevice = main.GraphicsDevice;
            content = main.Content;
            prepareContent();
        }

        protected abstract void prepareContent();
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
        protected virtual void pointerPressed(int x, int y) { }
        protected virtual void pointerDragged(int x, int y) { }
        protected virtual void pointerReleased(int x, int y) { }
        protected virtual void performBack() { }
        protected virtual void keyPressed(Keys key) { }

        bool isTouching = false;
        int pointerX, pointerY;
#if WINDOWS
        protected Keys[] RegisteredKeys = new Keys[] { Keys.Escape };
        Keys waitingKey = Keys.None;
#endif

        public void PerformStandardInput()
        {
            int[] point = null;

#if WINDOWS_PHONE
            //nhan phim back
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) performBack();
            //nhan vao man hinh
            TouchCollection touches = TouchPanel.GetState();
            if (touches.Count > 0) point = new int[] { (int)touches[0].Position.X, (int)touches[0].Position.Y };
#endif
#if WINDOWS
            //nhan phim duoc dang ky
            KeyboardState keyboardState = Keyboard.GetState();
            foreach (Keys key in RegisteredKeys)
            {
                if (checkKeyPressed(key, keyboardState)) break;
            }
            //bam chuot vao man hinh
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed) point = new int[] { mouse.X, mouse.Y };
#endif
            if (point != null)
            {
                int x = point[0];
                int y = point[1];

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

#if WINDOWS
        private bool checkKeyPressed(Keys key, KeyboardState keyboard)
        {
            if (keyboard.IsKeyUp(key))
            {
                if (waitingKey == key)
                {
                    if (key == Keys.Escape) performBack();
                    else keyPressed(key);
                    waitingKey = Keys.None;
                    return true;
                }
            }
            if (keyboard.IsKeyDown(key)) waitingKey = key;
            return false;
        }
#endif
    }
}
