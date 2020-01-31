using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using OpenitvnGame;
using OpenitvnGame.Helpers;

namespace PyramidRaider
{
    class PyramidMap
    {
        enum PyramidMapState { SlideUp, Show, SlideDown, Hide }

        private static byte[][] _chamberProcess;
        public static byte[][] ChamberProcess
        {
            get
            {
                if (_chamberProcess == null)
                {
                    ResetChamberProcess();
                    _chamberProcess = SettingHelper.GetSetting<byte[][]>(PlayScene.RECORD_ADVENTURE_CHAMBER_PROCESS, _chamberProcess);
                }
                return _chamberProcess;
            }
        }

        public static void SetChamberProcess(short pyramidId, short chamberId, byte starLevel)
        {
            _chamberProcess[pyramidId][chamberId] = starLevel;
            SettingHelper.StoreSetting(PlayScene.RECORD_ADVENTURE_CHAMBER_PROCESS, _chamberProcess, true);
        }

        public static void ResetChamberProcess()
        {
            _chamberProcess = new byte[15][];
            for (byte i = 0; i < 15; i++)
            {
                _chamberProcess[i] = new byte[15];
                for (byte j = 0; j < 15; j++)
                {
                    _chamberProcess[i][j] = 0;
                }
            }
        }

        public string Title { get; set; }
        
        public short PyramidId { get; set; }
        public short ReachedChamberId { get; set; }
        public byte[] ChamberStars { get; set; }
        public bool Visible { get { return _state != PyramidMapState.Hide; } }

        string _description;
        public string Description { 
            get {
                return _description;
            }
            set {
                _description = StringHelper.WordWrap(Main.FontSmall, value, 580);
            } 
        }

        short scheduledChamberId = -1;
        Texture2D texPyramidMap, texBackground, texLock;
        Rectangle[] recChamber;
        float _backgroundAlpha;
        PyramidMapState _state = PyramidMapState.Hide;
        Vector2 vtMap = new Vector2(0, 480);
        Sprite2D sprStar;
        PlayScene _parent;

        public PyramidMap(PlayScene parent)
        {
            _parent = parent;

            ContentManager content = Main.Instance.Content;
            texPyramidMap = content.Load<Texture2D>("Images/pyramidMap");
            texBackground = content.Load<Texture2D>("Images/whiteScreen");
            texLock = content.Load<Texture2D>("Images/lockMusic");
            sprStar = new Sprite2D(content.Load<Texture2D>("Images/smallStar"), 74, 22);

            recChamber = new Rectangle[15];
            for (byte i = 0; i < 15; i++)
            {
                byte row = (byte)(i / 5);
                byte col = (byte)(i % 5);
                recChamber[i] = new Rectangle(140 + col * 110, 184 + 85 * row, 80, 60);
            }
        }

        public void Update()
        {
            switch (_state)
            {
                case PyramidMapState.SlideUp:
                    if (vtMap.Y > 0)
                    {
                        _backgroundAlpha += 0.035f;
                        vtMap.Y -= 20;
                    }
                    else
                    {
                        _state = PyramidMapState.Show;
                        scheduledChamberId = -1;
                    }
                    break;

                case PyramidMapState.SlideDown:
                    if (vtMap.Y < 480)
                    {
                        _backgroundAlpha -= 0.035f;
                        vtMap.Y += 20;
                    }
                    else
                    {
                        _state = PyramidMapState.Hide;
                        if (scheduledChamberId > -1) _parent.EnterPyramid(PyramidId, scheduledChamberId);
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_state != PyramidMapState.Hide)
            {
                spriteBatch.Draw(texBackground, Vector2.Zero, Color.Black * _backgroundAlpha);
                spriteBatch.Draw(texPyramidMap, Vector2.Zero + vtMap, Color.White);
                spriteBatch.DrawString(Main.FontNormal, Title, new Vector2(400 - Main.FontNormal.MeasureString(Title).X / 2, 46) + vtMap, Color.Black);
                spriteBatch.DrawString(Main.FontSmall, Description, new Vector2(122, 90) + vtMap, Color.Black);
                for (short i = 0; i < 15; i++)
                {
                    sprStar.SetPosition(recChamber[i].X + 3 + (int)vtMap.X, recChamber[i].Y + 34 + (int)vtMap.Y);
                    sprStar.SetFrame(ChamberProcess[PyramidId][i]);
                    sprStar.Draw(spriteBatch);
                    if (i > ReachedChamberId)
                    {
                        Rectangle chamberRect = recChamber[i];
                        chamberRect.X += (int)vtMap.X;
                        chamberRect.Y += (int)vtMap.Y;
                        spriteBatch.Draw(texBackground, chamberRect, Color.Black * 0.7f);
                        spriteBatch.Draw(texLock, chamberRect, Color.White);
                    }
                }
            }
        }

        public void PointerReleased(int x, int y)
        {
            scheduledChamberId = -1;
            for (byte i = 0; i < 15; i++)
            {
                if (recChamber[i].Contains(x, y) && i <= ReachedChamberId)
                {
                    SlideDown();
                    scheduledChamberId = i;
                    return;
                }
            }
        }

        public void SlideUp()
        {
            if (_state == PyramidMapState.Hide) _state = PyramidMapState.SlideUp;
        }

        public void SlideDown()
        {
            if (_state == PyramidMapState.Show) _state = PyramidMapState.SlideDown;
        }
    }
}
