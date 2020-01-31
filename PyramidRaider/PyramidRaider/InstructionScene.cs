using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpenitvnGame;
using Microsoft.Xna.Framework.Content;

namespace PyramidRaider
{
    class InstructionScene : GameScene
    {
        enum InstructionState { FadeIn, Show, FadeOut }

        Texture2D texPage, texBackground, texPharaoh;
        Vector2 vtBackgroundCenter = new Vector2(400, 240);
        Vector2 vtPharaohCenter, vtPharaoh;
        InstructionState _state;
        float _pageAlpha;

        public InstructionScene(Texture2D page)
            : base(Main.Instance)
        {
            texPage = page;
        }

        protected override void prepareContent(ContentManager content)
        {
            texBackground = content.Load<Texture2D>("Images/splashBackground");
            texPharaoh = content.Load<Texture2D>("Images/pharaohSkull");
            vtPharaohCenter = new Vector2(texPharaoh.Width / 2, 154);
            vtPharaoh = new Vector2(800 / 2 - 18 * 20, 480 / 2);
            _state = InstructionState.FadeIn;
            _pageAlpha = 0;
        }

        public override void  Update(GameTime gameTime)
        {
 	         switch(_state) {
                 case InstructionState.FadeIn:
                     if(_pageAlpha < 1) _pageAlpha += 0.05f;
                     else _state = InstructionState.Show;
                     break;

                 case InstructionState.FadeOut:
                     if(_pageAlpha > 0) _pageAlpha -= 0.05f;
                     else Main.Instance.GotoMainMenu(MainMenuScene.FLAG_INSTRUCTION);
                     break;
             }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texBackground, vtBackgroundCenter, null, Color.White, 0, vtBackgroundCenter, 1.5f, SpriteEffects.None, 1);
            spriteBatch.Draw(texPharaoh, vtPharaoh, null, Color.White, 0, vtPharaohCenter, 1.8f, SpriteEffects.None, 1);
            spriteBatch.Draw(texPage, Vector2.Zero, Color.White * _pageAlpha);
        }

        protected override void performBack()
        {
 	         if(_state == InstructionState.Show) _state = InstructionState.FadeOut;
        }
    }
}
