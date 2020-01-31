using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpenitvnGame;

namespace PyramidRaider
{
    abstract class Enemy : Character
    {
        public int Type { get; private set; }
        protected Dust _dust;

        public int MovementLeft { get; set; }
        public int[] Position
        {
            get { return position; }
            set
            {
                position = value;
                skinnedModel.Position = new Vector3(position[1] * 10 + 5, 0, position[0] * 10 + 5);
            }
        }

        public Enemy(Model model, int type, int[] position, PlayScene parent)
            : base(model, parent)
        {
            Type = type;
            Position = position;
            skinnedModel.Dim = parent.IsDarkness;
            Stand();
        }

        //tra ve false neu ko di chuyen them buoc nao
        public bool TakeMove()
        {
            //neu nhu da het luot di
            if (MovementLeft == 0)
            {
                Stand();
                return false;
            }

            MovementLeft--;
            int[] explorerPosition = parent.Explorer.Position;
            if (Type == 0)
            {	//neu la loai binh thuong
                //thu di chuyen ngang truoc
                if (explorerPosition[1] < position[1])
                {
                    if (testMoveLeft()) return true;
                }
                else if (explorerPosition[1] > position[1])
                {
                    if (testMoveRight()) return true;
                }
                //neu khong di chuyen ngang duoc thi thu chuyen sang doc
                if (explorerPosition[0] < position[0])
                {
                    if (testMoveUp()) return true;
                }
                else if (explorerPosition[0] > position[0])
                {
                    if (testMoveDown()) return true;
                }
            }
            else
            {   //neu la loai mau do
                //thu di chuyen doc truoc
                if (explorerPosition[0] < position[0])
                {
                    if (testMoveUp()) return true;
                }
                else if (explorerPosition[0] > position[0])
                {
                    if (testMoveDown()) return true;
                }
                //neu khong di chuyen doc duoc thi thu chuyen sang ngang
                if (explorerPosition[1] < position[1])
                {
                    if (testMoveLeft()) return true;
                }
                else if (explorerPosition[1] > position[1])
                {
                    if (testMoveRight()) return true;
                }
            }
            //neu khong di chuyen duoc them thi bo han
            turnOnly();
            return false;
        }

        public new void Update(GameTime gameTime)
        {
            switch (State)
            {
                case CharacterState.MoveUp:
                    if (skinnedModel.Position.Z > desZ) skinnedModel.Position.Z -= PlayScene.MOVEMENT_STEP;
                    else stepDone();
                    break;

                case CharacterState.MoveRight:
                    if (skinnedModel.Position.X < desX) skinnedModel.Position.X += PlayScene.MOVEMENT_STEP;
                    else stepDone();
                    break;

                case CharacterState.MoveDown:
                    if (skinnedModel.Position.Z < desZ) skinnedModel.Position.Z += PlayScene.MOVEMENT_STEP;
                    else stepDone();
                    break;

                case CharacterState.MoveLeft:
                    if (skinnedModel.Position.X > desX) skinnedModel.Position.X -= PlayScene.MOVEMENT_STEP;
                    else stepDone();
                    break;
            }

            //cap nhat hieu ung danh lon neu co
            if (_dust != null) _dust.Update();

            base.Update(gameTime);
        }

        public new void Draw(Matrix view, Matrix projection)
        {
            skinnedModel.Dim = !parent.Explorer.LightToObject(skinnedModel.Position);
            base.Draw(view, projection);
        }

        public abstract bool IsLive();

        protected override void stepDone()
        {
            Position = position;
            bool haveCollision = parent.CheckCollision(this);
            if (parent.Gate != null && IsLive()) parent.Gate.TriggerKey(position);
            if (!haveCollision)
            {
                if (MovementLeft > 0) State = CharacterState.Idle;
                else
                {
                    Stand();
                    parent.SwitchPlayerTurn();
                }
            }
        }

        private void turnOnly()
        {
            int hDistance = position[0] - parent.Explorer.Position[0];
            int vDistance = position[1] - parent.Explorer.Position[1];
            if (Math.Abs(hDistance) > Math.Abs(vDistance))
            {
                skinnedModel.Rotation.Y = MathHelper.ToRadians(hDistance > 0 ? 180 : 0);
            }
            else
            {
                skinnedModel.Rotation.Y = MathHelper.ToRadians(vDistance > 0 ? 270 : 90);
            }
            MovementLeft = 0;
            Stand();
            parent.SwitchPlayerTurn();
        }

        public void Fight(bool isFinished)
        {
#if DEBUG
            Main.InsertLog(this.GetType().Name + ": Let's fight!");
#endif
            //khoi tao hieu ung danh lon
            SoundController.PlaySound(PlayContentHolder.Instance.SoundFight);
            _dust = new Dust(skinnedModel.Position, this);
            if (isFinished)
            {
                State = CharacterState.Catch;
                timeline = 30;
            }
            else
            {
                State = CharacterState.Fight;
                skinnedModel.SwitchClip(CharacterClip.Stand);
            }
        }

        public void FightingDone()
        {
#if DEBUG
            Main.InsertLog(this.GetType().Name + ": I'm winner!");
#endif
            _dust = null;
            if (State == CharacterState.Fight)
            {
                if (MovementLeft > 0) State = CharacterState.Idle;
                else
                {
                    Stand();
                    parent.SwitchPlayerTurn();
                }
            }
        }

        public void Idle()
        {
            State = CharacterState.Idle;
        }

        protected override bool testMoveUp()
        {
            int row = position[0] - 1;
            int col = position[1];
            if (parent.Gate != null)
            {
                if (parent.Gate.GatePosition[0] == row && parent.Gate.GatePosition[1] == col && parent.Gate.IsBlock()) return false;
            }
            if (!parent.Cell[row, col, 1])
            {
                MoveUp();
                return true;
            }
            return false;
        }

        protected override bool testMoveRight()
        {
            int row = position[0];
            int col = position[1];
            if (!parent.Cell[row, col, 0])
            {
                MoveRight();
                return true;
            }
            return false;
        }

        protected override bool testMoveDown()
        {
            int row = position[0];
            int col = position[1];
            if (parent.Gate != null)
            {
                if (parent.Gate.GatePosition[0] == row && parent.Gate.GatePosition[1] == col && parent.Gate.IsBlock()) return false;
            }
            if (!parent.Cell[row, col, 1])
            {
                MoveDown();
                return true;
            }
            return false;
        }

        protected override bool testMoveLeft()
        {
            int row = position[0];
            int col = position[1] - 1;
            if (!parent.Cell[row, col, 0])
            {
                MoveLeft();
                return true;
            }
            return false;
        }
    }
}
