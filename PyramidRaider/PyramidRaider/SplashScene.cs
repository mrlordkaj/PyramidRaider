using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using OpenitvnGame.Helpers;
using OpenitvnGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PyramidRaider
{
    class SplashScene : GameScene
    {
        const int FLAG_LOGO = 1;
        const int FLAG_SPLASH = 2;
        const int FLAG_PRESS = 3;
        const int FLAG_MENU = 4;
        const int FLAG_BACK = 5;
        const int COMMAND_EXIT = 1;
        const int COMMAND_BUY = 2;

        int _timeline;
        SpriteFont fntAncient;
        Texture2D texBackground, texLogo, texLogoLight, texPharaoh, texFlash, texTitle, texTitleMask, texTitleLight;
        Texture2D texMusic, texSound;
        Texture2D texVersionFree, texVersionPremium;
        float backgroundScale, pharaohScale;
        float backgroundAlpha, pharaohAlpha;
        Vector2 vtBackgroundCenter = new Vector2(400, 240);
        Vector2 vtLogoCenter, vtPharaohCenter;
        Vector2 vtTitle, vtTitleLight, vtTouch, vtPharaoh;
        Rectangle recVersion = new Rectangle(560, 330, 240, 150);
        bool showTouch;
        float logoLightAlpha, sloganAlpha, logoAlpha, versionAlpha;
        int _flag = 0;
        SoundEffect sndThunder;
        UIDialog uiDialog;

        Rectangle recMusic, recSound;
        float musicAlpha, soundAlpha;

        public SplashScene(bool isBack)
            : base(Main.Instance)
        {
            if (isBack) SwitchFlag(FLAG_BACK);
            else SwitchFlag(FLAG_LOGO);
        }

        protected override void prepareContent(ContentManager content)
        {
            texBackground = content.Load<Texture2D>("Images/splashBackground");
            texLogo = content.Load<Texture2D>("Images/logo");
            texLogoLight = content.Load<Texture2D>("Images/logoLight");
            texPharaoh = content.Load<Texture2D>("Images/pharaohSkull");
            texFlash = content.Load<Texture2D>("Images/whiteScreen");
            texTitle = content.Load<Texture2D>("Images/title");
            texTitleMask = content.Load<Texture2D>("Images/titleMask");
            texTitleLight = content.Load<Texture2D>("Images/titleLight");
            texMusic = content.Load<Texture2D>("Images/btnMusic");
            texSound = content.Load<Texture2D>("Images/btnSound");
            texVersionFree = content.Load<Texture2D>("Images/versionFree");
            texVersionPremium = content.Load<Texture2D>("Images/versionPremium");
            fntAncient = content.Load<SpriteFont>("Fonts/ancientLarge");
            sndThunder = content.Load<SoundEffect>("Sounds/thunder");

            SoundController.GetInstance().SetBackgroundMusic(content.Load<Song>("Musics/menu"));

            vtLogoCenter = new Vector2(texLogo.Width / 2, texLogo.Height / 2);
            vtPharaohCenter = new Vector2(texPharaoh.Width / 2, 154);

            int touchLeft = 400 - (int)Main.FontNormal.MeasureString(Localize.Instance.TouchToContinue).X / 2;
            vtTouch = new Vector2(touchLeft, 380);
            uiDialog = new UIDialog(content);
        }

        public void SwitchFlag(int flag)
        {
            _flag = flag;
            _timeline = -1;
            switch (_flag)
            {
                case FLAG_LOGO:
                    logoLightAlpha = logoAlpha = sloganAlpha = 0;
                    backgroundAlpha = 0;
                    backgroundScale = 2;
                    vtTitle = new Vector2(0, -92);
                    vtPharaoh = vtBackgroundCenter;
                    break;

                case FLAG_SPLASH:
                    backgroundAlpha = 0.5f;
                    backgroundScale = 1.5f;
                    pharaohAlpha = 0;
                    pharaohScale = 3;
                    vtTitle = new Vector2(0, -92);
                    vtPharaoh = vtBackgroundCenter;
                    recMusic = new Rectangle(10, 380, 60, 60);
                    recSound = new Rectangle(90, 380, 60, 60);
                    musicAlpha = soundAlpha = 0;
                    break;

                case FLAG_PRESS:
                    SoundController.GetInstance().Play();
                    backgroundAlpha = 1;
                    backgroundScale = 1;
                    pharaohAlpha = 0;
                    pharaohScale = 1;
                    vtTitle = Vector2.Zero;
                    vtPharaoh = vtBackgroundCenter;
                    vtTitleLight = new Vector2(-100, 0);
                    showTouch = false;
                    recMusic = new Rectangle(40, 380, 60, 60);
                    recSound = new Rectangle(120, 380, 60, 60);
                    musicAlpha = soundAlpha = 1;
                    versionAlpha = 0;
                    break;

                case FLAG_MENU:
                    backgroundAlpha = 1;
                    backgroundScale = 1;
                    pharaohScale = 1;
                    vtTitle = Vector2.Zero;
                    vtPharaoh = vtBackgroundCenter;
                    recMusic = new Rectangle(40, 380, 60, 60);
                    recSound = new Rectangle(120, 380, 60, 60);
                    musicAlpha = soundAlpha = 1;
                    versionAlpha = 1;
                    break;

                case FLAG_BACK:
                    backgroundAlpha = 1;
                    backgroundScale = 1.5f;
                    pharaohScale = 1.8f;
                    vtTitle = new Vector2(0, -92);
                    vtPharaoh = new Vector2(400 - 18 * 20, 240);
                    recMusic = new Rectangle(40, 380, 60, 60);
                    recSound = new Rectangle(120, 380, 60, 60);
                    musicAlpha = soundAlpha = 1;
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            _timeline++;
            switch (_flag)
            {
                case FLAG_LOGO:
                    if (_timeline < 20)
                    {
                    }
                    else if (_timeline < 28)
                    {
                        logoLightAlpha += 0.1f;
                    }
                    else if (_timeline < 38)
                    {
                        logoLightAlpha += 0.02f;
                        if (_timeline == 37) logoAlpha = 1;
                    }
                    else if (_timeline < 88)
                    {
                        logoLightAlpha -= 0.02f;
                    }
                    else if (_timeline < 108)
                    {
                        sloganAlpha += 0.05f;
                    }
                    else if (_timeline < 168)
                    {

                    }
                    else if (_timeline < 188)
                    {
                        logoAlpha -= 0.05f;
                        sloganAlpha -= 0.05f;
                        backgroundAlpha += 0.025f;
                        backgroundScale -= 0.025f;
                    }
                    else
                    {
                        SwitchFlag(FLAG_SPLASH);
                    }
                    break;

                case FLAG_SPLASH:
                    if (_timeline < 20)
                    {
                        pharaohScale -= 0.1f;
                        pharaohAlpha += 0.05f;
                        backgroundAlpha += 0.025f;
                        backgroundScale -= 0.025f;
                    }
                    else if (_timeline < 26)
                    {
                        if (_timeline == 25) SoundController.PlaySound(sndThunder);
                    }
                    else if (_timeline < 28)
                    {
                        pharaohScale -= 0.05f;
                        pharaohAlpha -= 0.05f;
                    }
                    else if (_timeline < 46)
                    {
                        pharaohScale += 0.05f;
                        pharaohAlpha -= 0.075f;
                    }
                    else if (_timeline < 69)
                    {
                        vtTitle.Y += 4;
                    }
                    else if (_timeline < 79) //bat dau di chuyen nut music va sound
                    {
                        recMusic.X += 3;
                        musicAlpha += 0.1f;
                    }
                    else if (_timeline < 89)
                    {
                        recSound.X += 3;
                        soundAlpha += 0.1f;
                    }
                    else
                    {
                        SwitchFlag(FLAG_PRESS);
                    }
                    break;

                case FLAG_PRESS:
                    if (_timeline % 15 == 0) showTouch = !showTouch;
                    if (vtTitleLight.X < 800) vtTitleLight.X += 20;
                    if (_timeline % 150 == 0)
                    {
                        _timeline = 0;
                        vtTitleLight.X = -100;
                    }
                    break;

                case FLAG_MENU:
                    if (_timeline < 20)
                    {
                        backgroundScale += 0.025f;
                        pharaohScale += 0.04f;
                        vtPharaoh.X -= 18;
                        vtTitle.Y -= 4.6f;
                    }
                    else
                    {
                        Main.Instance.GotoMainMenu();
                    }
                    break;

                case FLAG_BACK:
                    if (_timeline < 20)
                    {
                        backgroundScale -= 0.025f;
                        pharaohScale -= 0.04f;
                        vtPharaoh.X += 18;
                        vtTitle.Y += 4.6f;
                    }
                    else
                    {
                        SwitchFlag(FLAG_PRESS);
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texBackground, vtBackgroundCenter, null, Color.White * backgroundAlpha, 0, vtBackgroundCenter, backgroundScale, SpriteEffects.None, 1);
            if (_flag == FLAG_LOGO)
            {
                spriteBatch.Draw(texLogo, vtBackgroundCenter, null, Color.White * logoAlpha, 0, vtLogoCenter, 1, SpriteEffects.None, 1);
                spriteBatch.Draw(texLogoLight, vtBackgroundCenter, null, Color.White * logoLightAlpha, 0, vtLogoCenter, 1, SpriteEffects.None, 1);
                spriteBatch.DrawString(fntAncient, "Everything is open!", new Vector2(340, 264), Color.White * sloganAlpha);
            }
            if (_flag == FLAG_SPLASH)
            {
                if (_timeline >= 20)
                {
                    spriteBatch.Draw(texPharaoh, vtPharaoh, null, Color.White, 0, vtPharaohCenter, 1, SpriteEffects.None, 1);
                }
                spriteBatch.Draw(texPharaoh, vtPharaoh, null, Color.White * pharaohAlpha, 0, vtPharaohCenter, pharaohScale, SpriteEffects.None, 1);
                spriteBatch.Draw(texMusic, recMusic, Color.White * musicAlpha);
                spriteBatch.Draw(texSound, recSound, Color.White * soundAlpha);
                if (_timeline == 20 || _timeline == 21 || _timeline == 23 || _timeline == 24 | _timeline == 26 || _timeline == 27) spriteBatch.Draw(texFlash, Vector2.Zero, Color.White);
            }
            if (_flag == FLAG_PRESS)
            {
                spriteBatch.Draw(texPharaoh, vtPharaoh, null, Color.White, 0, vtPharaohCenter, 1, SpriteEffects.None, 1);
                if (showTouch) spriteBatch.DrawString(Main.FontNormal, Localize.Instance.TouchToContinue, vtTouch, Color.White);
                spriteBatch.Draw(texMusic, recMusic, Color.White * musicAlpha);
                spriteBatch.Draw(texSound, recSound, Color.White * soundAlpha);
                if (versionAlpha < 1) versionAlpha += 0.05f;
                spriteBatch.Draw(Main.IsPremium ? texVersionPremium : texVersionFree, recVersion, Color.White * versionAlpha);
                SoundController.GetInstance().Draw(spriteBatch, recMusic, recSound);
            }
            if (_flag == FLAG_MENU || _flag == FLAG_BACK)
            {
                spriteBatch.Draw(texPharaoh, vtPharaoh, null, Color.White, 0, vtPharaohCenter, pharaohScale, SpriteEffects.None, 1);
                spriteBatch.Draw(texMusic, recMusic, Color.White * musicAlpha);
                spriteBatch.Draw(texSound, recSound, Color.White * soundAlpha);
                if (_flag == FLAG_MENU)
                {
                    if (versionAlpha > 0) versionAlpha -= 0.05f;
                    spriteBatch.Draw(Main.IsPremium ? texVersionPremium : texVersionFree, recVersion, Color.White * versionAlpha);
                }
                SoundController.GetInstance().Draw(spriteBatch, recMusic, recSound);
            }
            spriteBatch.Draw(texTitle, vtTitle, Color.White);

            if (_flag == FLAG_PRESS)
            {
                spriteBatch.Draw(texTitleLight, vtTitleLight, Color.White);
                spriteBatch.Draw(texTitleMask, Vector2.Zero, Color.White);
            }

            if (uiDialog.Visible) uiDialog.Draw(spriteBatch);
        }

        protected override void pointerPressed(int x, int y)
        {
            if (uiDialog.Visible)
            {
                uiDialog.TestHit(x, y);
                return;
            }

#if !WP7
            if (!Main.IsPremium && recVersion.Contains(x, y))
            {
                uiDialog.Title = Localize.Instance.Disclaimer;
                uiDialog.Content = Localize.Instance.DisclaimerDescription;
                uiDialog.CommandCode = COMMAND_BUY;
                uiDialog.FadeIn(UIDialogType.Message);
                return;
            }
#endif
        }

        protected override void pointerDragged(int x, int y)
        {
            if (uiDialog.Visible)
            {
                uiDialog.TestHit(x, y);
                return;
            }
        }

        protected override void pointerReleased(int x, int y)
        {
            if (uiDialog.Visible)
            {
                UIDialogResult returnCode = uiDialog.CheckHit(x, y);
                if ((short)returnCode > -1)
                {
                    uiDialog.FadeOut();
                    switch (uiDialog.CommandCode)
                    {
                        case COMMAND_EXIT:
                            if (returnCode == UIDialogResult.Yes)
                            {
#if ANDROID
                                if (!Main.IsPremium)
                                {
                                    (Main.Activity as MainActivity).DisplayAd();
                                }
                                SettingHelper.SaveSetting();
                                Main.Instance.Exit();
#endif
#if WINDOWS_PHONE && SILVERLIGHT
                                SettingHelper.SaveSetting();
                                SoundController.GetInstance().FadeOut();
                                GamePage.Instance.GotoExitPage();
#endif
#if WINDOWS || (WINDOWS_PHONE && !SILVERLIGHT)
                                SettingHelper.SaveSetting();
                                Main.Instance.Exit();
#endif
                            }
                            break;

                        case COMMAND_BUY:
#if WP8
                            GamePage.Instance.RemoveAd();
#endif
#if ANDROID
                            (Main.Activity as MainActivity).RemoveAd();
#endif
                            break;
                    }
                }
                return;
            }

            if (recMusic.Contains(x, y))
            {
                SoundController.GetInstance().ToggleMusic();
                return;
            }

            if (recSound.Contains(x, y))
            {
                SoundController.GetInstance().ToggleSound();
                return;
            }

            if (_flag == FLAG_PRESS) SwitchFlag(FLAG_MENU);
        }

        protected override void performBack()
        {
            if (uiDialog.Visible)
            {
                uiDialog.FadeOut();
                return;
            }

            if (_flag == FLAG_PRESS)
            {
                uiDialog.CommandCode = COMMAND_EXIT;
                uiDialog.Title = Localize.Instance.ExitGame;
                uiDialog.Content = Localize.Instance.ExitGameDescription;
                uiDialog.FadeIn(UIDialogType.Confirm);
            }
        }
    }
}
