using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
#if WINDOWS_PHONE
using Microsoft.Phone.Tasks;
#endif
using OpenitvnGame.Helpers;
using OpenitvnGame;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace PyramidRaider
{
    class MainMenuScene : GameScene
    {
        public const int FLAG_MAIN = 1;
        public const int FLAG_INSTRUCTION = 2;

        const int COMMAND_NONE = -1;
        const int COMMAND_MAIN = 0;
        const int COMMAND_QUICKPLAY = 1;
        const int COMMAND_ADVENTURE = 2;
        const int COMMAND_INSTRUCTION = 3;
        const int COMMAND_LEADERBOARD = 4;
        const int COMMAND_HOWTO = 5;
        const int COMMAND_TUTORIAL = 6;
        const int COMMAND_ABOUT = 7;
        const int COMMAND_MOREAPP = 8;
        const int COMMAND_SPLASH = 9;

        enum MenuState { FadeIn, Active, FadeOut, Game }

        Texture2D texBackground, texPharaoh, texEmpty, texSidebarClassic, texSidebarAdventure;
        Texture2D texCredits, texHowToPlay;
        Texture2D texMusic, texSound;
        Dictionary<int, Texture2D[][]> texMenuItem;
        MenuItem[] menuItems = new MenuItem[4];
        Vector2 vtBackgroundCenter = new Vector2(400, 240);
        Vector2 vtPharaohCenter, vtPharaoh, vtSidebar;
        int _timeline;
        int _flag;
        MenuState _state;
        int _commandScheduled;
        HellGate hellGate;
        GameMode gameMode;
        bool _returnMenuAfterTutorial;
        UIDialog confirmDialog;

        Rectangle recMusic = new Rectangle(40, 380, 60, 60);
        Rectangle recSound = new Rectangle(120, 380, 60, 60);

        public MainMenuScene()
            : base(Main.Instance)
        {
            switchFlag(FLAG_MAIN);
        }

        public MainMenuScene(int flag)
            : base(Main.Instance)
        {
            switchFlag(flag);
        }

        protected override void prepareContent(ContentManager content)
        {
            texBackground = content.Load<Texture2D>("Images/splashBackground");
            texPharaoh = content.Load<Texture2D>("Images/pharaohSkull");
            texEmpty = content.Load<Texture2D>("Images/empty");
            texHowToPlay = content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/pageHowToPlay");
            texCredits = content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/pageCredits");
            texSidebarClassic = content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/sidebarClassic");
            texSidebarAdventure = content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/sidebarAdventure");
            texMusic = content.Load<Texture2D>("Images/btnMusic");
            texSound = content.Load<Texture2D>("Images/btnSound");

            //menu items
            texMenuItem = new Dictionary<int, Texture2D[][]>();
            texMenuItem.Add(FLAG_MAIN, new Texture2D[][] {
                new Texture2D[] {
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuQuickPlay"),
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuQuickPlayLight")
                },
                new Texture2D[] {
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuAdventure"),
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuAdventureLight")
                },
                new Texture2D[] {
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuInstruction"),
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuInstructionLight")
                },
                new Texture2D[] {
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuLeaderboard"),
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuLeaderboardLight")
                }
            });
            texMenuItem.Add(FLAG_INSTRUCTION, new Texture2D[][] {
                new Texture2D[] {
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuHowToPlay"),
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuHowToPlayLight")
                },
                new Texture2D[] {
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuTutorial"),
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuTutorialLight")
                },
                new Texture2D[] {
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuAbout"),
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuAboutLight")
                },
                new Texture2D[] {
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuMore"),
                    content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/mnuMoreLight")
                }
            });
            vtPharaohCenter = new Vector2(texPharaoh.Width / 2, 154);
            vtPharaoh = new Vector2(800 / 2 - 18 * 20, 480 / 2);
            vtSidebar = new Vector2(800, 0);
            SoundController.GetInstance().SetBackgroundMusic(content.Load<Song>("Musics/menu"));
            SoundController.GetInstance().Play();
            confirmDialog = new UIDialog(content);

            gameMode = GameMode.Classic;
        }

        public override void Update(GameTime gameTime)
        {
            switch (_state)
            {
                case MenuState.FadeIn:
                    if (_timeline < 40) _timeline++;
                    else switchState(MenuState.Active);
                    switch (_timeline)
                    {
                        case 6:
                            buildMenuItem(0);
                            break;

                        case 12:
                            buildMenuItem(1);
                            break;

                        case 18:
                            buildMenuItem(2);
                            break;

                        case 24:
                            buildMenuItem(3);
                            break;
                    }
                    break;

                case MenuState.FadeOut:
                    if (!confirmDialog.Visible)
                    {
                        if (_timeline < 10) _timeline++;
                        else performScheduledCommand();
                    }
                    break;

                case MenuState.Game:
                    if (hellGate.State != HellGateState.Closed) _timeline++;
                    else
                    {
                        Main.Instance.GotoGame(gameMode, _returnMenuAfterTutorial);
                        return;
                    }
                    if (_timeline < 18)
                    {
                        vtSidebar.X -= 16;
                    }
                    else
                    {
                        hellGate.Update();
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texBackground, vtBackgroundCenter, null, Color.White, 0, vtBackgroundCenter, 1.5f, SpriteEffects.None, 1);
            spriteBatch.Draw(texPharaoh, vtPharaoh, null, Color.White, 0, vtPharaohCenter, 1.8f, SpriteEffects.None, 1);
            foreach (MenuItem menuItem in menuItems)
            {
                if (menuItem != null) menuItem.Draw(spriteBatch);
            }
            spriteBatch.Draw(texMusic, recMusic, Color.White);
            spriteBatch.Draw(texSound, recSound, Color.White);
            SoundController.GetInstance().Draw(spriteBatch, recMusic, recSound);
            if (_state == MenuState.Game)
            {
                if (_timeline >= 18)
                {
                    hellGate.Draw(spriteBatch);
                }
                spriteBatch.Draw(gameMode == GameMode.Adventure ? texSidebarAdventure : texSidebarClassic, vtSidebar, Color.White);
            }
            if (confirmDialog.Visible) confirmDialog.Draw(spriteBatch);
        }

        protected override void pointerPressed(int x, int y)
        {
            if (confirmDialog.Visible)
            {
                confirmDialog.TestHit(x, y);
                return;
            }

            if (_state != MenuState.Active) return;

            foreach (MenuItem menuItem in menuItems)
            {
                menuItem.TestHit(x, y);
            }
        }

        protected override void pointerDragged(int x, int y)
        {
            if (confirmDialog.Visible)
            {
                confirmDialog.TestHit(x, y);
                return;
            }

            if (_state != MenuState.Active) return;

            foreach (MenuItem menuItem in menuItems)
            {
                menuItem.TestHit(x, y);
            }
        }

        protected override void pointerReleased(int x, int y)
        {
            if (confirmDialog.Visible)
            {
                UIDialogResult returnCode = confirmDialog.CheckHit(x, y);
                if ((short)returnCode > -1)
                {
                    confirmDialog.FadeOut();
                    switch (confirmDialog.CommandCode)
                    {
                        case COMMAND_QUICKPLAY:
                            gameMode = (returnCode == UIDialogResult.Yes) ? GameMode.Tutorial : GameMode.Classic;
                            _returnMenuAfterTutorial = false;
                            switchState(MenuState.Game);
                            break;

                        case COMMAND_ADVENTURE:
                        case COMMAND_LEADERBOARD:
#if WINDOWS_PHONE
                            if (returnCode == UIDialogResult.Yes)
                            {
                                MarketplaceReviewTask marketplace = new MarketplaceReviewTask();
                                marketplace.Show();
                            }
#endif
                            switchFlag(FLAG_MAIN);
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

            if (_state != MenuState.Active) return;

            foreach (MenuItem menuItem in menuItems)
            {
                menuItem.Active = false;
            }
            _commandScheduled = COMMAND_NONE;

            switch (_flag)
            {
                case FLAG_MAIN:

                    if (menuItems[0].CheckHit(x, y))
                    {
                        _commandScheduled = COMMAND_QUICKPLAY;
                        switchState(MenuState.FadeOut);
                        return;
                    }
                    if (menuItems[1].CheckHit(x, y))
                    {
                        _commandScheduled = COMMAND_ADVENTURE;
                        switchState(MenuState.FadeOut);
                        return;
                    }
                    if (menuItems[2].CheckHit(x, y))
                    {
                        _commandScheduled = COMMAND_INSTRUCTION;
                        switchState(MenuState.FadeOut);
                        return;
                    }
                    if (menuItems[3].CheckHit(x, y))
                    {
                        _commandScheduled = COMMAND_LEADERBOARD;
                        switchState(MenuState.FadeOut);
                        return;
                    }
                    break;

                case FLAG_INSTRUCTION:
                    if (menuItems[0].CheckHit(x, y))
                    {
                        _commandScheduled = COMMAND_HOWTO;
                        switchState(MenuState.FadeOut);
                        return;
                    }
                    if (menuItems[1].CheckHit(x, y))
                    {
                        _commandScheduled = COMMAND_TUTORIAL;
                        switchState(MenuState.FadeOut);
                        return;
                    }
                    if (menuItems[2].CheckHit(x, y))
                    {
                        _commandScheduled = COMMAND_ABOUT;
                        switchState(MenuState.FadeOut);
                        return;
                    }
                    if (menuItems[3].CheckHit(x, y))
                    {
                        _commandScheduled = COMMAND_MOREAPP;
                        switchState(MenuState.FadeOut);
                        return;
                    }
                    break;
            }
        }

        protected override void performBack()
        {
            if (confirmDialog.Visible)
            {
                confirmDialog.FadeOut();
                switchState(MenuState.FadeIn);
                return;
            }

            switch (_state)
            {
                case MenuState.Active:
                    switch (_flag)
                    {
                        case FLAG_MAIN:
                            _commandScheduled = COMMAND_SPLASH;
                            switchState(MenuState.FadeOut);
                            break;

                        case FLAG_INSTRUCTION:
                            _commandScheduled = COMMAND_MAIN;
                            switchState(MenuState.FadeOut);
                            break;
                    }
                    break;
            }
        }

        private void buildMenuItem(int id)
        {
            Texture2D texInactive = texMenuItem[_flag][id][0];
            Texture2D texActive = texMenuItem[_flag][id][1];
            Vector2 vtPosition = new Vector2(560, (id + 1) * 100);
            menuItems[id] = new MenuItem(texInactive, texActive, vtPosition);
        }

        private void switchFlag(int newFlag)
        {
            _flag = newFlag;
            _timeline = -1;
            _state = MenuState.FadeIn;
        }

        private void switchState(MenuState newState)
        {
            _state = newState;
            _timeline = -1;

            switch (_state)
            {
                case MenuState.FadeOut:
                    foreach (MenuItem menuItem in menuItems)
                    {
                        menuItem.FadeOut();
                    }
                    break;

                case MenuState.Game:
                    SoundController.GetInstance().FadeOut();
                    hellGate = new HellGate();
                    hellGate.ContentTexture = texEmpty;
                    break;
            }
        }

        private void performScheduledCommand()
        {
            switch (_commandScheduled)
            {
                case COMMAND_MAIN:
                    for (int i = 0; i < menuItems.Length; i++)
                    {
                        menuItems[i] = null;
                    }
                    switchFlag(FLAG_MAIN);
                    break;

                case COMMAND_QUICKPLAY:
                    bool firstPlay = SettingHelper.GetSetting<bool>(PlayScene.RECORD_FIRST_PLAY, true);
                    if (firstPlay)
                    {
                        confirmDialog.CommandCode = COMMAND_QUICKPLAY;
                        confirmDialog.Title = Localize.Instance.PlayTutorial;
                        confirmDialog.Content = Localize.Instance.PlayTutorialDescription;
                        confirmDialog.FadeIn(UIDialogType.Confirm);
                    }
                    else
                    {
                        gameMode = GameMode.Classic;
                        switchState(MenuState.Game);
                    }
                    break;

                case COMMAND_ADVENTURE:
                    gameMode = GameMode.Adventure;
                    switchState(MenuState.Game);
                    break;

                case COMMAND_LEADERBOARD:
                    Main.Instance.GotoLeaderboard();
                    break;

                case COMMAND_INSTRUCTION:
                    for (int i = 0; i < menuItems.Length; i++)
                    {
                        menuItems[i] = null;
                    }
                    switchFlag(FLAG_INSTRUCTION);
                    break;

                case COMMAND_HOWTO:
                    Main.Instance.GotoInstruction(texHowToPlay);
                    break;

                case COMMAND_TUTORIAL:
                    gameMode = GameMode.Tutorial;
                    _returnMenuAfterTutorial = true;
                    switchState(MenuState.Game);
                    break;

                case COMMAND_ABOUT:
                    Main.Instance.GotoInstruction(texCredits);
                    break;

                case COMMAND_MOREAPP:
                    for (int i = 0; i < menuItems.Length; i++)
                    {
                        menuItems[i] = null;
                    }
                    switchFlag(FLAG_INSTRUCTION);
                    break;

                case COMMAND_SPLASH:
                    Main.Instance.GotoSplash(true);
                    break;
            }
        }
    }
}
