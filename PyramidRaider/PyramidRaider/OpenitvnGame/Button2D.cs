using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenitvnGame
{
    class Button2D
    {
        Vector2 _position = Vector2.Zero;
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (!fixedBounce)
                {
                    int oldX = (int)_position.X;
                    int oldY = (int)_position.Y;
                    _bounce.X += (int)(value.X - _position.X);
                    _bounce.Y += (int)(value.Y - _position.Y);
                }
                _position = value;
            }
        }

        public bool Active { get; set; }
        public bool Visible { get; set; }
        public bool FadeAtVisible { get; set; }
        public bool FadeAtActive { get; set; }

        Rectangle _bounce = new Rectangle();
        bool fixedBounce = false;

        Texture2D texActive, texInactive, texDisabled;
        
        float _visibleAlpha = 0f;
        float _activeAlpha = 0f;

        public Button2D(Texture2D textureActive)
        {
            texActive = textureActive;
            _bounce.Width = texActive.Width;
            _bounce.Height = texActive.Height;
        }

        public Button2D(Texture2D textureActive, Texture2D textureInactive)
        {
            texActive = textureActive;
            texInactive = textureInactive;
            _bounce.Width = texActive.Width;
            _bounce.Height = texActive.Height;
        }

        public Button2D(Texture2D textureActive, Texture2D textureInactive, Texture2D textureDisabled)
        {
            texActive = textureActive;
            texInactive = textureInactive;
            texDisabled = textureDisabled;
            _bounce.Width = texActive.Width;
            _bounce.Height = texActive.Height;
        }

        public Button2D(Texture2D textureActive, Texture2D textureInactive, Texture2D textureDisabled, Vector2 position)
        {
            texActive = textureActive;
            texInactive = textureInactive;
            texDisabled = textureDisabled;
            _bounce.Width = texActive.Width;
            _bounce.Height = texActive.Height;
            Position = position;
        }

        public Button2D(Texture2D textureActive, Texture2D textureInactive, Texture2D textureDisabled, Vector2 position, Rectangle bounce)
        {
            texActive = textureActive;
            texInactive = textureInactive;
            texDisabled = textureDisabled;
            _bounce = bounce;
            fixedBounce = true;
            Position = position;
        }

        public Button2D(Texture2D textureActive, Texture2D textureInactive, Texture2D textureDisabled, Vector2 position, Rectangle bounce, bool bounceIsFixed)
        {
            texActive = textureActive;
            texInactive = textureInactive;
            texDisabled = textureDisabled;
            _bounce = bounce;
            fixedBounce = bounceIsFixed;
            Position = position;
        }

        public bool TestHit(int x, int y)
        {
            if (Visible)
            {
                Active = _bounce.Contains(x, y);
                return Active;
            }
            return false;
        }

        public bool CheckHit(int x, int y)
        {
            if (Visible) return _bounce.Contains(x, y);
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (texDisabled != null) spriteBatch.Draw(texDisabled, _position, Color.White);

            if (!Visible)
            {
                if (_visibleAlpha > 0)
                {
                    _visibleAlpha -= 0.1f;
                    if (texInactive != null && FadeAtVisible)
                        spriteBatch.Draw(texInactive, _position, Color.White * _visibleAlpha);
                }
            }
            else
            {
                if (texInactive != null)
                {
                    if (_visibleAlpha < 1) _visibleAlpha += 0.1f;
                    spriteBatch.Draw(texInactive, _position, Color.White * _visibleAlpha);
                }
                if (!Active)
                {
                    if (_activeAlpha > 0)
                    {
                        _activeAlpha -= 0.1f;
                        if (FadeAtActive)
                            spriteBatch.Draw(texActive, _position, Color.White * _activeAlpha);
                    }
                }
                else
                {
                    if (_activeAlpha < 1) _activeAlpha += 0.1f;
                    if(FadeAtActive) spriteBatch.Draw(texActive, _position, Color.White * _activeAlpha);
                    else spriteBatch.Draw(texActive, _position, Color.White);
                }
            }
        }

        public void ForceInactive()
        {
            Active = false;
            _activeAlpha = 0;
        }

        public void ForceInvisible()
        {
            Visible = false;
            _visibleAlpha = 0;
        }
    }
}
