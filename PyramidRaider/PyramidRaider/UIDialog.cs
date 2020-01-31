using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using OpenitvnGame;
using System.Text;
using OpenitvnGame.Helpers;

namespace PyramidRaider
{
    enum UIDialogType
    {
        Confirm = 0,
        Message = 1
    }
    enum UIDialogResult
    {
        None = -1,
        No = 0,
        Yes = 1,
        Ok = 2
    }

    class UIDialog
    {
        const int CONTENT_MARGIN_LEFT = 24;

        public bool Visible { get; private set; }
        public string Title { get; set; }
        public short CommandCode { get; set; }
        public UIDialogType Type { get; private set; }

        string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                string[] phrases = _content.Split('\n');
                StringBuilder text = new StringBuilder();
                //int maxWidth = texForeground[Type].Width - CONTENT_MARGIN_LEFT * 2;
                foreach (string phrase in phrases) text.Append(StringHelper.WordWrap(Main.FontSmall, phrase, 370) + "\r\n");
                _content = text.ToString();
            }
        }

        Texture2D texBackground;
        Dictionary<UIDialogType, Texture2D> texForeground = new Dictionary<UIDialogType, Texture2D>();
        Vector2 vtForeground;

        Button2D btnYes, btnNo, btnOk;
        float alpha;
        bool fadeIn;

        public UIDialog(ContentManager content)
        {
            prepareResource(content);
        }

        private void prepareResource(ContentManager contentManager)
        {
            texBackground = contentManager.Load<Texture2D>("Images/whiteScreen");

            try
            {
                texForeground.Add(UIDialogType.Confirm, contentManager.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/confirmDialog"));
            }
            catch { }
            try
            {
                texForeground.Add(UIDialogType.Message, contentManager.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/messageDialog"));
            }
            catch { }

            btnYes = new Button2D(contentManager.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/btnYesLight"));
            btnYes.FadeAtActive = true;

            btnNo = new Button2D(contentManager.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/btnNoLight"));
            btnNo.FadeAtActive = true;

            btnOk = new Button2D(contentManager.Load<Texture2D>("Images/" + Localize.Instance.LanguageCode + "/btnOkLight"));
            btnOk.FadeAtActive = true;

            fadeIn = Visible = false;
            alpha = 0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (fadeIn)
            {
                if (alpha < 1f)
                {
                    alpha += 0.1f;
                    vtForeground.Y += 4;
                }
                else
                {
                    alpha = 1;
                    switch (Type)
                    {
                        case UIDialogType.Confirm:
                            btnYes.Visible = btnNo.Visible = true;
                            break;

                        case UIDialogType.Message:
                            btnOk.Visible = true;
                            break;
                    }
                }
            }
            else
            {
                if (alpha > 0f)
                {
                    alpha -= 0.1f;
                    vtForeground.Y -= 4;
                }
                else
                {
                    alpha = 0;
                    Visible = false;
                }
            }
            spriteBatch.Draw(texBackground, Vector2.Zero, Color.Black * alpha * 0.6f);
            spriteBatch.Draw(texForeground[Type], vtForeground, Color.White * alpha);
            int titleY = (int)vtForeground.Y + 14;
            int titleX = 400 - (int)Main.FontLarge.MeasureString(Title).X / 2;
            spriteBatch.DrawString(Main.FontLarge, Title, new Vector2(titleX, titleY), Color.Black * alpha);
            spriteBatch.DrawString(Main.FontSmall, Content, new Vector2(vtForeground.X + CONTENT_MARGIN_LEFT, vtForeground.Y + 72), Color.Black * alpha);
            switch (Type)
            {
                case UIDialogType.Confirm:
                    btnYes.Draw(spriteBatch);
                    btnNo.Draw(spriteBatch);
                    break;

                case UIDialogType.Message:
                    btnOk.Draw(spriteBatch);
                    break;
            }

        }

        public void FadeIn(UIDialogType dialogType)
        {
            if (!fadeIn && !Visible)
            {
                Type = dialogType;
                vtForeground = new Vector2(400 - texForeground[Type].Width / 2, 200 - texForeground[Type].Height / 2);
                switch (Type)
                {
                    case UIDialogType.Confirm:
                        btnYes.Position = new Vector2(vtForeground.X + 24, vtForeground.Y + 330);
                        btnNo.Position = new Vector2(vtForeground.X + 210, vtForeground.Y + 330);
                        break;

                    case UIDialogType.Message:
                        btnOk.Position = new Vector2(vtForeground.X + 117, vtForeground.Y + 330);
                        break;
                };
                fadeIn = Visible = true;
            }
        }

        public void FadeOut()
        {
            if (fadeIn)
            {
                fadeIn = false;
                switch (Type)
                {
                    case UIDialogType.Confirm:
                        btnYes.ForceInactive();
                        btnNo.ForceInactive();
                        btnYes.Visible = btnNo.Visible = false;
                        break;

                    case UIDialogType.Message:
                        btnOk.ForceInactive();
                        btnOk.Visible = false;
                        break;
                }
            }
        }

        public void TestHit(int x, int y)
        {
            if (alpha < 1) return;
            switch (Type)
            {
                case UIDialogType.Confirm:
                    btnYes.TestHit(x, y);
                    btnNo.TestHit(x, y);
                    break;

                case UIDialogType.Message:
                    btnOk.TestHit(x, y);
                    break;
            }

        }

        public UIDialogResult CheckHit(int x, int y)
        {
            UIDialogResult code = UIDialogResult.None;
            switch (Type)
            {
                case UIDialogType.Confirm:
                    if (btnYes.CheckHit(x, y)) code = UIDialogResult.Yes;
                    else if (btnNo.CheckHit(x, y)) code = UIDialogResult.No;
                    btnYes.Visible = btnNo.Visible = false;
                    break;

                case UIDialogType.Message:
                    if (btnOk.CheckHit(x, y)) code = UIDialogResult.Ok;
                    btnOk.Visible = false;
                    break;
            }

            return code;
        }
    }
}
