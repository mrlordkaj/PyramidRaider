using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenitvnGame
{
    class Button2D
    {
        Rectangle _bounce;
        Vector2 _position;
        Texture2D texActive, texInactive, texDisabled;
        public bool Active { get; set; }
        public bool Visible { get; set; }
        private float _visibleAlpha = 0f;
        public bool FadeAtVisible { get; set; }
        private float _activeAlpha = 0f;
        public bool FadeAtActive { get; set; }

        public Button2D(Texture2D[] texture, Vector2 position, Rectangle bounce)
        {
            _position = position;
            _bounce = bounce;
            texActive = texture[0];
            if (texture.Length >= 2) texInactive = texture[1];
            if (texture.Length >= 3) texDisabled = texture[2];
        }

        public Button2D(Texture2D[] texture, Vector2 position)
        {
            _position = position;
            texActive = texture[0];
            if (texture.Length >= 2) texInactive = texture[1];
            if (texture.Length >= 3) texDisabled = texture[2];
            _bounce = new Rectangle((int)position.X, (int)position.Y, texActive.Width, texActive.Height);
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
    }
}
