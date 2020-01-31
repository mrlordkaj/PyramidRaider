using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using OpenitvnGame;
using System;
using System.IO;
using System.Text;

namespace PyramidRaider
{
    enum LeaderboardState { Fill, Submit, View }

    class LeaderboardScene : GameScene, ILeaderboardCaller
    {
        const string DEFAULT_NAME = "Unnamed Player";

        Texture2D texLeaderboardBackground, texLeaderboardForeground, texLeaderboardSubmit;
        Texture2D texWhiteScreen;
        Button2D btnSubmit;
        Sprite2D sprLoading;

        Vector2 vtScore = new Vector2(260, 294);
        Vector2 vtSubmiting = new Vector2(350, 210);

        Vector2 vtBackground = new Vector2(0, 131);

        Vector2 vtRank, vtView7, vtViewAll;
        int view7Height, viewAllHeight;
        string strRank;
        StringBuilder strView7, strViewAll;
        bool leftTouching = false, rightTouching = false, allowLeftTouch = false, allowRightTouch = false;
        int vOffset, vTargetLeft, vTargetRight;

        UIDialog uiDialog;

        int tick = 0;
        private int _score;

        private string _username;
        Vector2 vtUsername = new Vector2(260, 204);
        Rectangle recUsername = new Rectangle(250, 200, 500, 60);

        public LeaderboardState State { get; private set; }

        public LeaderboardScene()
            : base(Main.Instance)
        {
            State = LeaderboardState.View;

            Leaderboard.GetRank(this);
            Leaderboard.View7(this);
            Leaderboard.ViewAll(this);
        }

        public LeaderboardScene(int score)
            : base(Main.Instance)
        {
            State = LeaderboardState.Fill;
            _score = score;

            _username = Leaderboard.UserName;
            if (_username.Length < 3 || _username.Length > 14) _username = DEFAULT_NAME;
        }

        protected override void prepareContent(ContentManager content)
        {
            texLeaderboardBackground = content.Load<Texture2D>("Images/leaderboardBackground");
            texLeaderboardForeground = content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/leaderboardForeground");
            texLeaderboardSubmit = content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/leaderboardSubmit");
            texWhiteScreen = content.Load<Texture2D>("Images/whiteScreen");
            btnSubmit = new Button2D(content.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/btnSubmitLight"), null, null, new Vector2(450, 380));
            btnSubmit.FadeAtActive = true;
            btnSubmit.Visible = true;
            sprLoading = new Sprite2D(content.Load<Texture2D>("Images/loading"), 60, 60);
            sprLoading.SetPosition(274, 210);
            uiDialog = new UIDialog(content);

            strRank = Localize.Instance.FetchingRank;
            vtRank = new Vector2(400 - Main.FontSmall.MeasureString(strRank).X / 2, 70);
            strView7 = new StringBuilder(Localize.Instance.FetchingData);
            vTargetLeft = 134;
            vtView7 = new Vector2(26, vTargetLeft);
            strViewAll = new StringBuilder(Localize.Instance.FetchingData);
            vTargetRight = 134;
            vtViewAll = new Vector2(422, vTargetRight);
        }

        public override void Update(GameTime gameTime)
        {
            if (!leftTouching)
            {
                if (vTargetLeft > vtView7.Y + 20) vtView7.Y += 20;
                else if (vTargetLeft < vtView7.Y - 20) vtView7.Y -= 20;
                else vtView7.Y = vTargetLeft;
            }

            if (!rightTouching)
            {
                if (vTargetRight > vtViewAll.Y + 20) vtViewAll.Y += 20;
                else if (vTargetRight < vtViewAll.Y - 20) vtViewAll.Y -= 20;
                else vtViewAll.Y = vTargetRight;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (State)
            {
                case LeaderboardState.Fill:
                    spriteBatch.Draw(texLeaderboardSubmit, Vector2.Zero, Color.White);
                    spriteBatch.DrawString(Main.FontLarge, _username, vtUsername, Color.White);
                    spriteBatch.DrawString(Main.FontLarge, _score.ToString(), vtScore, Color.White);
                    btnSubmit.Draw(spriteBatch);
                    break;

                case LeaderboardState.Submit:
                    spriteBatch.Draw(texLeaderboardSubmit, Vector2.Zero, Color.White);
                    spriteBatch.DrawString(Main.FontLarge, _username, vtUsername, Color.White);
                    spriteBatch.DrawString(Main.FontLarge, _score.ToString(), vtScore, Color.White);
                    spriteBatch.Draw(texWhiteScreen, Vector2.Zero, Color.Black * 0.8f);
                    if (tick < 2) tick++;
                    else
                    {
                        tick = 0;
                        sprLoading.NextFrame();
                    }
                    sprLoading.Draw(spriteBatch);
                    spriteBatch.DrawString(Main.FontNormal, Localize.Instance.SubmittingData, vtSubmiting, Color.White);
                    break;

                case LeaderboardState.View:
                    spriteBatch.Draw(texLeaderboardBackground, vtBackground, Color.White);
                    spriteBatch.DrawString(Main.FontSmall, strView7.ToString(), vtView7, Color.White);
                    spriteBatch.DrawString(Main.FontSmall, strViewAll, vtViewAll, Color.White);
                    spriteBatch.Draw(texLeaderboardForeground, Vector2.Zero, Color.White);
                    spriteBatch.DrawString(Main.FontSmall, strRank, vtRank, Color.White);
                    break;
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

            switch (State)
            {
                case LeaderboardState.Fill:
                    if (btnSubmit.TestHit(x, y)) return;

                    if(recUsername.Contains(x, y)) {

                        if (!Guide.IsVisible)
                        {
                            Guide.BeginShowKeyboardInput(PlayerIndex.One,
                                Localize.Instance.NameEntry,
                                Localize.Instance.EnterName,
                                (_username != DEFAULT_NAME) ? _username : "",
                                new AsyncCallback(gotName),
                                this);
                        }

                        return;
                    }
                    break;

                case LeaderboardState.View:
                    if (y > 134)
                    {
                        if (x < 400 && allowLeftTouch)
                        {
                            leftTouching = true;
                            vOffset = y - (int)vtView7.Y;
                        }
                        else if (allowRightTouch)
                        {
                            rightTouching = true;
                            vOffset = y - (int)vtViewAll.Y;
                        }
                    }
                    break;
            }
        }

        protected override void pointerDragged(int x, int y)
        {
            if (uiDialog.Visible)
            {
                uiDialog.TestHit(x, y);
                return;
            }

            switch (State)
            {
                case LeaderboardState.Fill:
                    if (btnSubmit.TestHit(x, y)) return;
                    break;

                case LeaderboardState.View:
                    if (leftTouching)
                    {
                        vtView7.Y = vTargetLeft = y - vOffset;
                    }
                    else if (rightTouching)
                    {
                        vtViewAll.Y = vTargetRight = y - vOffset;
                    }
                    break;
            }
        }

        protected override void pointerReleased(int x, int y)
        {
            if (uiDialog.Visible)
            {
                UIDialogResult returnCode = uiDialog.CheckHit(x, y);
                if ((short)returnCode > -1) uiDialog.FadeOut();
                return;
            }

            switch (State)
            {
                case LeaderboardState.Fill:
                    if (State == LeaderboardState.Fill)
                    {
                        btnSubmit.Active = false;
                        if (btnSubmit.CheckHit(x, y))
                        {
                            if (_username.Length < 3 || _username.Length > 14 || _username == DEFAULT_NAME)
                            {
                                uiDialog.Title = Localize.Instance.InvalidName;
                                uiDialog.Content = Localize.Instance.InvalidNameDescription;
                                uiDialog.FadeIn(UIDialogType.Message);
                            }
                            else
                            {
                                State = LeaderboardState.Submit;
                                Leaderboard.SubmitScore(_score, _username, this);
                            }
                            return;
                        }
                    }
                    break;

                case LeaderboardState.View:
                    if (leftTouching)
                    {
                        if (vtView7.Y > 134 || view7Height < 340) vTargetLeft = 134;
                        else if (vtView7.Y < 490 - view7Height) vTargetLeft = 490 - view7Height;
                    }
                    else if (rightTouching)
                    {
                        if (vtViewAll.Y > 134 || viewAllHeight < 340) vTargetRight = 134;
                        else if (vtViewAll.Y < 490 - viewAllHeight) vTargetRight = 490 - viewAllHeight;
                    }
                    leftTouching = rightTouching = false;
                    break;
            }
        }

        protected override void performBack()
        {
            if (uiDialog.Visible)
            {
                uiDialog.FadeOut();
                return;
            }

            Main.Instance.GotoMainMenu();
        }

        public void OnSubmitSuccess()
        {
            Main.Instance.GotoLeaderboard();
        }

        public void OnSubmitFailed()
        {
            State = LeaderboardState.Fill;
            uiDialog.Title = Localize.Instance.SubmitError;
            uiDialog.Content = Localize.Instance.SubmitErrorDescription;
            uiDialog.FadeIn(UIDialogType.Message);
        }

        public void OnGetRankSuccess(int rank)
        {
            strRank = (rank > 0) ? Localize.Instance.YourRank + rank : Localize.Instance.NotRanked;
            vtRank.X = 400 - Main.FontSmall.MeasureString(strRank).X / 2;
        }

        public void OnGetRankFailed()
        {
            strRank = Localize.Instance.CantFetchRank;
            vtRank.X = 400 - Main.FontSmall.MeasureString(strRank).X / 2;
        }

        public void OnView7Success(StreamReader reader)
        {
            strView7.Clear();
            string line;
            string[] data;
            int i = 1;
            while ((line = reader.ReadLine()) != null)
            {
                data = line.Split(':');
                strView7.AppendLine(string.Format("{0,2}. {1,-14} {2,8}", i, data[0], data[1]));
                i++;
            }
            view7Height = (int)Main.FontSmall.MeasureString(strView7).Y;
            allowLeftTouch = true;
        }

        public void OnView7Failed()
        {
            strView7 = new StringBuilder(Localize.Instance.CantFetchLeaderboard);
        }

        public void OnViewAllSuccess(StreamReader reader)
        {
            strViewAll.Clear();
            string line;
            string[] data;
            int i = 1;
            while ((line = reader.ReadLine()) != null)
            {
                data = line.Split(':');
                strViewAll.AppendLine(string.Format("{0,2}. {1,-14} {2,8}", i, data[0], data[1]));
                i++;
            }
            viewAllHeight = (int)Main.FontSmall.MeasureString(strViewAll).Y;
            allowRightTouch = true;
        }

        public void OnViewAllFailed()
        {
            strViewAll = new StringBuilder(Localize.Instance.CantFetchLeaderboard);
        }

        public void OnRemoveSuccess()
        {
            strRank = Localize.Instance.NotRanked;
            vtRank.X = 400 - Main.FontSmall.MeasureString(strRank).X / 2;
        }

        public void OnRemoveFailed()
        {
            uiDialog.Title = Localize.Instance.RemoveError;
            uiDialog.Content = Localize.Instance.RemoveErrorDescription;
            uiDialog.FadeIn(UIDialogType.Message);
        }

        private void gotName(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                _username = Guide.EndShowKeyboardInput(result);
                if (_username == null || _username.Length == 0)
                {
                    _username = DEFAULT_NAME;
                }
                else if (_username.Length > 14)
                {
                    _username = _username.Substring(0, 14);
                }
            }
        }

        ~LeaderboardScene()
        {
            texLeaderboardBackground = null;
            texLeaderboardForeground = null;
            texLeaderboardSubmit = null;
            btnSubmit = null;
        }
    }
}
