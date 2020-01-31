using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpenitvnGame;

namespace PyramidRaider
{
    class Scorpion : Enemy
    {
        public Scorpion(Model model, int type, int[] position, PlayScene maze)
            : base(model, type, position, maze)
        {
        }

        public override bool IsLive()
        {
            if (!parent.Scorpions.Contains(this)) return false;
            return true;
        }

        public new void Draw(Matrix view, Matrix projection)
        {
            if (State != CharacterState.Catch) base.Draw(view, projection);
        }

        public override void MoveUp()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundScorpionWalk);
            base.MoveUp();
        }

        public override void MoveRight()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundScorpionWalk);
            base.MoveRight();
        }

        public override void MoveDown()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundScorpionWalk);
            base.MoveDown();
        }

        public override void MoveLeft()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundScorpionWalk);
            base.MoveLeft();
        }
    }
}
