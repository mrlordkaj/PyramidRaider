using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpenitvnGame;

namespace PyramidRaider
{
    enum CharacterState
    {
        None = -1,
        MoveUp = 0,
        MoveRight = 1,
        MoveDown = 2,
        MoveLeft = 3,
        Stand = 4,
        Idle = 5,
        Wait = 6,
        Fight = 7,
        Catched = 8,
        Poisoned = 9,
        Trapped = 10,
        Catch = 11,
        Escaped = 12,
        Dance = 13,
        Spin = 14,
        HearLeft = 15,
        HearRight = 16,
        Look = 17,
        FixFlashlight = 18,
        Sleepy = 19,
        Read = 20,
        Fall = 21,
        Shock = 22
    }

    class CharacterClip
    {
        public const string Stand = "Stand";
        public const string Walk = "Walk";
    }

    abstract class Character
    {
        protected CSkinnedModel skinnedModel;
        protected PlayScene parent;
        protected int desX, desZ;
        protected int[] position;
        public CharacterState State { get; protected set; }
        protected int timeline = -1;

        public Character(Model model, PlayScene parent)
        {
            this.parent = parent;
            skinnedModel = new CSkinnedModel(model);
            State = CharacterState.None;
        }

        public void Update(GameTime gameTime)
        {
            if (State != CharacterState.None) skinnedModel.Update(gameTime.ElapsedGameTime);
        }

        public void Draw(Matrix view, Matrix projection)
        {
            if (State != CharacterState.None) skinnedModel.Draw(view, projection);
        }

        public void Stand()
        {
            State = CharacterState.Stand;
            skinnedModel.SwitchClip(CharacterClip.Stand);
            timeline = PlayScene.Random.Next(120, 360);
        }

        public void Wait()
        {
            State = CharacterState.Wait;
            skinnedModel.SwitchClip(CharacterClip.Stand);
        }

        public virtual void MoveUp()
        {
            desZ = (--position[0]) * 10 + 5;
            State = CharacterState.MoveUp;
            skinnedModel.Rotation.Y = MathHelper.ToRadians(180);
            skinnedModel.SwitchClip(CharacterClip.Walk);
            timeline = -1;
        }

        public virtual void MoveRight()
        {
            desX = (++position[1]) * 10 + 5;
            State = CharacterState.MoveRight;
            skinnedModel.Rotation.Y = MathHelper.ToRadians(90);
            skinnedModel.SwitchClip(CharacterClip.Walk);
            timeline = -1;
        }

        public virtual void MoveDown()
        {
            desZ = (++position[0]) * 10 + 5;
            State = CharacterState.MoveDown;
            skinnedModel.Rotation.Y = MathHelper.ToRadians(0);
            skinnedModel.SwitchClip(CharacterClip.Walk);
            timeline = -1;
        }

        public virtual void MoveLeft()
        {
            desX = (--position[1]) * 10 + 5;
            State = CharacterState.MoveLeft;
            skinnedModel.Rotation.Y = MathHelper.ToRadians(270);
            skinnedModel.SwitchClip(CharacterClip.Walk);
            timeline = -1;
        }

        protected abstract bool testMoveUp();
        protected abstract bool testMoveRight();
        protected abstract bool testMoveDown();
        protected abstract bool testMoveLeft();
        protected abstract void stepDone();
    }
}
