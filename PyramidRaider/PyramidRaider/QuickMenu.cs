using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using OpenitvnGame;
using OpenitvnGame.Helpers;
using System.Collections.Generic;

namespace PyramidRaider
{
    class QuickMenu
    {
        enum QuickMenuState { SlideUp, Show, SlideDown, Hide }
        const int COMMAND_NONE = -1;
        const int COMMAND_RESET = 1;
        const int COMMAND_SOLUTION = 2;
        const int COMMAND_ABANDON = 3;
        const int COMMAND_MAIN = 4;

        QuickMenuState _state;
        Texture2D texMenu, texBackground;
        float _backgroundAlpha;
        Vector2 vtMenu;
        PlayScene _parent;
        Button2D btnReset, btnSolution, btnAbandon, btnMain;
        int _scheduledCommand;
        UIDialog confirmDialog;

        Rectangle recMusic, recSound;

        public QuickMenu(PlayScene parent)
        {
            _parent = parent;
            ContentManager content = Main.Instance.Content;
            texMenu = content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/quickMenu");
            texBackground = content.Load<Texture2D>("Images/whiteScreen");
            vtMenu = new Vector2(0, 480);

            //nut reset maze
            btnReset = new Button2D(
                content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/qmnuResetMazeLight")
            );
            btnReset.Position = (Localize.Instance.Language == Language.English) ? new Vector2(157, 347) : new Vector2(172, 343);
            //nut get solution
            btnSolution = new Button2D(
                content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/qmnuGetSolutionLight")
            );
            btnSolution.Position = (Localize.Instance.Language == Language.English) ? new Vector2(427, 347) : new Vector2(458, 343);
            //nut abandon
            btnAbandon = new Button2D(
                content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/qmnuAbandonLight")
            );
            btnAbandon.Position = (Localize.Instance.Language == Language.English) ? new Vector2(179, 422) : new Vector2(177, 417);
            //nut main menu
            btnMain = new Button2D(
                content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/qmnuMainMenuLight")
            );
            btnMain.Position = (Localize.Instance.Language == Language.English) ? new Vector2(436, 422) : new Vector2(440, 417);

            _backgroundAlpha = 0;
            _state = QuickMenuState.Hide;
            confirmDialog = new UIDialog(content);

            recMusic = new Rectangle(531, 132 + 480, 60, 60);
            recSound = new Rectangle(624, 132 + 480, 60, 60);
        }

        public void Update()
        {
            switch (_state)
            {
                case QuickMenuState.SlideUp:
                    if (vtMenu.Y > 0)
                    {
                        _backgroundAlpha += 0.025f;
                        vtMenu.Y -= 20;
                        recMusic.Y -= 20;
                        recSound.Y -= 20;
                    }
                    else
                    {
                        _state = QuickMenuState.Show;
                        btnReset.Visible = btnSolution.Visible = btnAbandon.Visible = btnMain.Visible = true;
                    }
                    break;

                case QuickMenuState.SlideDown:
                    if (vtMenu.Y < 480)
                    {
                        _backgroundAlpha -= 0.025f;
                        vtMenu.Y += 20;
                        recMusic.Y += 20;
                        recSound.Y += 20;
                    }
                    else
                    {
                        _state = QuickMenuState.Hide;
                        performScheduledCommand();
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_state == QuickMenuState.Hide) return;
            spriteBatch.Draw(texBackground, Vector2.Zero, Color.Black * _backgroundAlpha);
            spriteBatch.Draw(texMenu, vtMenu, Color.White);
            SoundController.GetInstance().Draw(spriteBatch, recMusic, recSound);

            if (_state == QuickMenuState.Show)
            {
                btnReset.Draw(spriteBatch);
                btnSolution.Draw(spriteBatch);
                btnAbandon.Draw(spriteBatch);
                btnMain.Draw(spriteBatch);
            }

            if (confirmDialog.Visible) confirmDialog.Draw(spriteBatch);
        }

        public void SlideUp()
        {
            if (_state == QuickMenuState.Hide)
            {
                _scheduledCommand = COMMAND_NONE;
                _state = QuickMenuState.SlideUp;
            }
        }

        public void SlideDown()
        {
            if (_state == QuickMenuState.Show) _state = QuickMenuState.SlideDown;
        }

        public void TestHit(int x, int y)
        {
            if (confirmDialog.Visible)
            {
                confirmDialog.TestHit(x, y);
                return;
            }

            if (_state != QuickMenuState.Show) return;
            btnReset.TestHit(x, y);
            btnSolution.TestHit(x, y);
            btnAbandon.TestHit(x, y);
            btnMain.TestHit(x, y);
        }

        public void CheckHit(int x, int y)
        {
            if (confirmDialog.Visible)
            {
                UIDialogResult resultCode = confirmDialog.CheckHit(x, y);
                if (resultCode == UIDialogResult.None) return;
                if (resultCode == UIDialogResult.Yes)
                {
                    _scheduledCommand = confirmDialog.CommandCode;
                    SlideDown();
                }
                confirmDialog.FadeOut();
                return;
            }

            _scheduledCommand = COMMAND_NONE;
            btnReset.Active = btnSolution.Active = btnAbandon.Active = btnMain.Active = false;
            if (_state != QuickMenuState.Show) return;

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

            if (btnReset.CheckHit(x, y))
            {
                _scheduledCommand = COMMAND_RESET;
                SlideDown();
                return;
            }
            if (btnSolution.CheckHit(x, y))
            {
                if (_parent.HintPoint - PlayScene.SOLUTION_COST < 0)
                {
                    confirmDialog.Title = Localize.Instance.SolutionWarning;
                    confirmDialog.Content = string.Format(Localize.Instance.SolutionWarningDescription, _parent.HintPoint, PlayScene.SOLUTION_COST);
                    btnSolution.Active = false;
                    confirmDialog.FadeIn(UIDialogType.Message);
                }
                else
                {
                    confirmDialog.Title = Localize.Instance.SolutionConfirm;
                    confirmDialog.Content = string.Format(Localize.Instance.SolutionConfirmDescription, _parent.HintPoint, PlayScene.SOLUTION_COST);
                    confirmDialog.CommandCode = COMMAND_SOLUTION;
                    confirmDialog.FadeIn(UIDialogType.Confirm);
                }
                return;
            }
            if (btnAbandon.CheckHit(x, y))
            {
                confirmDialog.Title = Localize.Instance.AbandonConfirm;
                confirmDialog.Content = Localize.Instance.AbandonConfirmDescription;
                confirmDialog.CommandCode = COMMAND_ABANDON;
                confirmDialog.FadeIn(UIDialogType.Confirm);
                return;
            }
            if (btnMain.CheckHit(x, y))
            {
                _scheduledCommand = COMMAND_MAIN;
                SlideDown();
                return;
            }
        }

        public void PerformBack()
        {
            if (confirmDialog.Visible)
            {
                confirmDialog.FadeOut();
                return;
            }

            SlideDown();
        }

        private void performScheduledCommand()
        {
            switch (_scheduledCommand)
            {
                case COMMAND_NONE:
                    _parent.Resume();
                    break;

                case COMMAND_RESET:
                    _parent.Resume();
                    _parent.PerformReset();
                    break;

                case COMMAND_SOLUTION:
                    if (_parent.HintPoint - PlayScene.SOLUTION_COST >= 0)
                    {
                        _parent.HintPoint -= PlayScene.SOLUTION_COST;
                        SettingHelper.SaveSetting();
                        _parent.GetSolution();
                    }
                    else
                    {
                        _parent.Resume();
                    }
                    break;

                case COMMAND_ABANDON:
                    if (_parent.Mode == GameMode.Classic)
                    {
                        SettingHelper.StoreSetting(PlayScene.RECORD_CLASSIC_PYRAMID, (short)1, true);
                        SettingHelper.StoreSetting(PlayScene.RECORD_CLASSIC_CHAMBER, (short)1, true);
                        SettingHelper.StoreSetting(PlayScene.RECORD_CLASSIC_SCORE, 0, true);
                        SettingHelper.StoreSetting(PlayScene.RECORD_CLASSIC_HINT_POINT, 0, true);
                        SettingHelper.StoreSetting(PlayScene.RECORD_RANDOM_LEVEL, (short)(-1), true);
                    }
                    else if (_parent.Mode == GameMode.Adventure)
                    {
                        PyramidMap.ResetChamberProcess();
                        SettingHelper.StoreSetting(PlayScene.RECORD_ADVENTURE_CHAMBER_PROCESS, PyramidMap.ChamberProcess, true);
                        SettingHelper.StoreSetting(PlayScene.RECORD_ADVENTURE_PYRAMID_PROCESS, PlayScene.DEFAULT_ADVENTURE_PYRAMID_PROCESS, true);
                        SettingHelper.StoreSetting(PlayScene.RECORD_ADVENTURE_PYRAMID_CURRENT, (short)1, true);
                        SettingHelper.StoreSetting(PlayScene.RECORD_ADVENTURE_HINT_POINT, 0, true);
                    }
                    SettingHelper.SaveSetting();
                    if (_parent.Score > 0) _parent.GotoSubmitScore();
                    else _parent.GotoMainMenu();
                    break;

                case COMMAND_MAIN:
                    _parent.GotoMainMenu();
                    break;
            }
        }
    }
}
