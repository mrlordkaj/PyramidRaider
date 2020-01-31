using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PyramidRaider
{
    class MenuItem
    {
        enum MenuItemState { FadeIn, Visible, FadeOut }

        MenuItemState _state;

        Rectangle _bounce;
        Texture2D texActive, texInactive;
        public bool Active { get; set; }
        private float _inactiveAlpha;
        private float _activeAlpha;
        int _timeline = -1;

        public MenuItem(Texture2D inactive, Texture2D active, Vector2 position)
        {
            texActive = active;
            texInactive = inactive;
            _bounce = new Rectangle();
            _bounce.X = (int)(position.X - texActive.Width / 2);
            _bounce.Y = (int)(position.Y - texActive.Height / 2);
            _bounce.Width = texActive.Width;
            _bounce.Height = texActive.Height;
            switchState(MenuItemState.FadeIn);
        }

        private void switchState(MenuItemState state)
        {
            _state = state;
            _timeline = -1;
            switch(_state) {
                case MenuItemState.FadeIn:
                    _inactiveAlpha = 0;
                    _activeAlpha = 0;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (_state)
            {
                case MenuItemState.FadeIn:
                    _timeline++;
                    if (_timeline < 10)
                    {
                        _activeAlpha += 0.1f;
                    }
                    else if (_timeline < 15)
                    {
                        _inactiveAlpha += 0.2f;
                    }
                    else if (_timeline < 35)
                    {
                        _activeAlpha -= 0.05f;
                    }
                    else switchState(MenuItemState.Visible);
                    break;

                case MenuItemState.Visible:
                    if (Active)
                    {
                        if (_activeAlpha < 1) _activeAlpha += 0.1f;
                    }
                    else
                    {
                        if (_activeAlpha > 0) _activeAlpha -= 0.1f;
                    }
                    break;

                case MenuItemState.FadeOut:
                    if (_inactiveAlpha > 0)
                    {
                        _inactiveAlpha -= 0.1f;
                        _activeAlpha -= 0.1f;
                    }
                    break;
            }

            spriteBatch.Draw(texInactive, _bounce, Color.White * _inactiveAlpha);
            spriteBatch.Draw(texActive, _bounce, Color.White * _activeAlpha);
        }

        public bool TestHit(int x, int y)
        {
            if (_state != MenuItemState.Visible) return false;
            Active = _bounce.Contains(x, y);
            return Active;
        }

        public bool CheckHit(int x, int y)
        {
            if (_state != MenuItemState.Visible) return false;
            else return _bounce.Contains(x, y);
        }

        public void FadeOut()
        {
            _state = MenuItemState.FadeOut;
        }
    }
}
