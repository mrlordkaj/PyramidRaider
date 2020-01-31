using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OpenitvnGame
{
    public class Sprite2D
    {
        protected Texture2D texture;
        private int numRow, numCol;
        private Rectangle srcRect, desRect;

        public int CurrentFrame { get; private set; }
        public int NumFrames { get; private set; }

        private int _x;
        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                desRect.X = value;
            }
        }

        private int _y;
        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                desRect.Y = value;
            }
        }
        public int Width
        {
            get
            {
                return desRect.Width;
            }
        }
        public int Height
        {
            get
            {
                return desRect.Height;
            }
        }

        private float _factor;
        public float Factor
        {
            get
            {
                return _factor;
            }
            set
            {
                _factor = value;
                desRect.Width = (int)(srcRect.Width * value);
                desRect.Height = (int)(srcRect.Height * value);
            }
        }

        public Sprite2D(Texture2D texture, int width, int height)
        {
            X = 0;
            Y = 0;
            this.texture = texture;
            CurrentFrame = 0;

            numCol = texture.Width / width;
            numRow = texture.Height / height;
            NumFrames = numCol * numRow;
            srcRect = new Rectangle(0, 0, width, height);
            desRect = new Rectangle(0, 0, width, height);
        }

        public void NextFrame()
        {
            if (++CurrentFrame == NumFrames) CurrentFrame = 0;

            UpdateTexture();
        }

        public void SetFrame(int targetFrame)
        {
            if (targetFrame >= NumFrames) throw new Exception("Invalid keyfram.");

            CurrentFrame = targetFrame;

            UpdateTexture();
        }

        private void UpdateTexture()
        {
            int col = CurrentFrame % numCol;
            int row = CurrentFrame / numCol;

            srcRect.X = col * srcRect.Width;
            srcRect.Y = row * srcRect.Height;
        }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentFrame < 0) return;

            spriteBatch.Draw(texture, desRect, srcRect, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, float alpha)
        {
            if (CurrentFrame < 0) return;

            spriteBatch.Draw(texture, desRect, srcRect, Color.White*alpha);
        }
    }
}
