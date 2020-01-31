using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PyramidRaider
{
    enum HellGateState { Closing, FadingIn, Closed, FadingOut, Opening, Opened }

    class HellGate
    {
        public static Texture2D ImageHellGate;

        private HellGateState _state;
        public HellGateState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                setGatePosition();
            }
        }
        int _timeline = -1;
        Vector2 vtGateLeft, vtGateRight;
        public Texture2D ContentTexture { get; set; }
        float contentAlpha = 0f;

        public HellGate()
        {
            State = HellGateState.Closing;
        }

        public HellGate(HellGateState startState)
        {
            State = startState;
        }

        public HellGate(HellGateState startState, Texture2D content)
        {
            State = startState;
            ContentTexture = content;
        }

        private void setGatePosition()
        {
            switch (_state)
            {
                case HellGateState.Opened:
                case HellGateState.Closing:
                    vtGateLeft = new Vector2(-300, 0);
                    vtGateRight = new Vector2(600, 0);
                    contentAlpha = 0f;
                    break;

                case HellGateState.Closed:
                case HellGateState.FadingOut:
                    vtGateLeft = new Vector2(0, 0);
                    vtGateRight = new Vector2(300, 0);
                    contentAlpha = 1f;
                    break;

                case HellGateState.Opening:
                case HellGateState.FadingIn:
                    vtGateLeft = new Vector2(0, 0);
                    vtGateRight = new Vector2(300, 0);
                    contentAlpha = 0f;
                    break;
            }
            _timeline = 0;
        }

        public void Update()
        {
            switch (State)
            {
                case HellGateState.Closing:
                    if (_timeline <= 32)
                    {
                        _timeline++;
                        if (_timeline <= 20)
                        {
                            vtGateLeft.X += 15;
                            vtGateRight.X -= 15;
                        }
                        else if (_timeline <= 24)
                        {
                            vtGateLeft.X -= 2;
                            vtGateRight.X += 2;
                        }
                        else if (_timeline <= 32)
                        {
                            vtGateLeft.X += 1;
                            vtGateRight.X -= 1;
                        }
                    }
                    else State = (ContentTexture != null) ? HellGateState.FadingIn : HellGateState.Closed;
                    break;

                case HellGateState.FadingIn:
                    if(_timeline <= 10) _timeline++;
                    else State = HellGateState.Closed;
                    contentAlpha += 0.1f;
                    break;

                case HellGateState.FadingOut:
                    if(_timeline <= 10) _timeline++;
                    else State = HellGateState.Opening;
                    contentAlpha -= 0.1f;
                    break;

                case HellGateState.Opening:
                    if (_timeline <= 20) _timeline++;
                    else State = HellGateState.Opened;
                    if (_timeline <= 20)
                    {
                        vtGateLeft.X -= 15;
                        vtGateRight.X += 15;
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_state != HellGateState.Opened)
            {
                spriteBatch.Draw(ImageHellGate, vtGateLeft, Color.White);
                spriteBatch.Draw(ImageHellGate, vtGateRight, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 1);
                if (contentAlpha > 0f && ContentTexture != null) spriteBatch.Draw(ContentTexture, Vector2.Zero, Color.White * contentAlpha);
            }
        }
    }
}
