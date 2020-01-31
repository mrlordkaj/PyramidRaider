using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpenitvnGame;

namespace PyramidRaider
{
    class MummyClip : CharacterClip
    {
        public const string Catch = "Catch";
        public const string Dance = "Dance";
        public const string HearLeft = "Hear_Left";
        public const string HearRight = "Hear_Right";
        public const string Spin = "Spin";
        public const string Look = "Look";
    }

    class Mummy : Enemy
    {
        CModel tile;

        public Mummy(Model model, int type, int[] position, PlayScene maze)
            : base(model, type, position, maze)
        {
        }

        public void SetInit()
        {
            skinnedModel.Position.Y = -15;
            tile = new CModel(
                PlayContentHolder.Instance.ModelTile[parent.IsDarkness ? 2 : (position[0] * parent.MazeSize + position[1]) % 2],
                skinnedModel.Position
            );
        }

        public override bool IsLive()
        {
            if (!parent.Mummies.Contains(this)) return false;
            return true;
        }

        public new void Update(GameTime gameTime)
        {
            switch (State)
            {
                case CharacterState.Catch:
                    if (timeline > 0) timeline--;
                    else if (timeline == 0)
                    {
                        skinnedModel.Rotation.Y = MathHelper.ToRadians(45);
                        skinnedModel.Position.Z -= 2;
                        skinnedModel.Position.X -= 4;
                        skinnedModel.SwitchClip(MummyClip.Catch);
                        timeline--;
                    }
                    break;

                case CharacterState.Stand:
                    if (timeline > 0) timeline--;
                    else if (timeline == 0)
                    {
                        CharacterState rand = (CharacterState)PlayScene.Random.Next((int)CharacterState.Dance, (int)CharacterState.Look + 1);
                        switch (rand)
                        {
                            case CharacterState.Dance:
                                Dance();
                                break;

                            case CharacterState.Spin:
                                Spin();
                                break;

                            case CharacterState.HearLeft:
                                HearLeft();
                                break;

                            case CharacterState.HearRight:
                                HearRight();
                                break;

                            case CharacterState.Look:
                                Look();
                                break;
                        }
                    }
                    break;

                case CharacterState.Dance:
                case CharacterState.Spin:
                case CharacterState.Look:
                case CharacterState.HearLeft:
                case CharacterState.HearRight:
                    if (timeline > 0) timeline--;
                    else if (timeline == 0) Stand();
                    break;
            }

            base.Update(gameTime);
        }

        public override void MoveUp()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundMummyWalk);
            base.MoveUp();
        }

        public override void MoveRight()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundMummyWalk);
            base.MoveRight();
        }

        public override void MoveDown()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundMummyWalk);
            base.MoveDown();
        }

        public override void MoveLeft()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundMummyWalk);
            base.MoveLeft();
        }

        public new void Draw(Matrix view, Matrix projection)
        {
            if (tile != null) tile.Draw(view, projection);

            base.Draw(view, projection);
        }

        public void RisingUp()
        {
            if (skinnedModel.Position.Y < 0)
            {
                skinnedModel.Position.Y += 0.5f;
                tile.Position.Y += 0.5f;
            }
            else
            {
                skinnedModel.Position.Y = 0;
                tile.Position.Y = 0;
            }
        }

        public void RiseUpDone()
        {
            tile = null;
            if (PlayScene.Random.Next(1, 3) == 1)
            {
                Dance();
            }
            else
            {
                Spin();
            }
        }

        public void Dance()
        {
            State = CharacterState.Dance;
            skinnedModel.SwitchClip(MummyClip.Dance);
            timeline = 88;
        }

        public void Spin()
        {
            State = CharacterState.Spin;
            skinnedModel.SwitchClip(MummyClip.Spin);
            timeline = 47;
        }

        public void Look()
        {
            State = CharacterState.Look;
            skinnedModel.SwitchClip(MummyClip.Look);
            timeline = 41;
        }

        public void HearLeft()
        {
            State = CharacterState.HearLeft;
            skinnedModel.SwitchClip(MummyClip.HearLeft);
            timeline = 35;
        }

        public void HearRight()
        {
            State = CharacterState.HearRight;
            skinnedModel.SwitchClip(MummyClip.HearRight);
            timeline = 37;
        }
    }
}
