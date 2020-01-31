using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpenitvnGame;
using System;

namespace PyramidRaider
{
    class ExplorerClip : CharacterClip
    {
        public const string Fall = "Fall";
        public const string Shock = "Shock";
        public const string Poisoned = "Poisoned";
        public const string Catched = "Catched";
        public const string Look = "Look";
        public const string FixFlashlight = "Fix_Flashlight";
        public const string Read = "Read";
        public const string Sleepy = "Sleepy";
    }

    class Explorer : Character
    {
        Button3D[] _buttons;

        public int[] Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                //cap nhat vi tri model
                skinnedModel.Position = new Vector3(position[1] * 10 + 5, 0, position[0] * 10 + 5);
                //cap nhat vi tri va trang thai nut bam
                _buttons[0].Position = new Vector3(position[1] * 10 + 5, 0.04f, position[0] * 10 - 5);
                _buttons[1].Position = new Vector3(position[1] * 10 + 15, 0.04f, position[0] * 10 + 5);
                _buttons[2].Position = new Vector3(position[1] * 10 + 5, 0.04f, position[0] * 10 + 15);
                _buttons[3].Position = new Vector3(position[1] * 10 - 5, 0.04f, position[0] * 10 + 5);
                _buttons[4].Position = new Vector3(position[1] * 10 + 5, 0.04f, position[0] * 10 + 5);
                UpdateControllerState();
            }
        }

        float unitFactor;
        Vector2 unitPosition;
        public Vector2 FloorTexturePosition
        {
            get
            {
                switch (State)
                {
                    case CharacterState.Poisoned:
                    case CharacterState.Catched:
                        unitPosition.X = (position[1] * 10 + 5) * unitFactor;
                        unitPosition.Y = (position[0] * 10 + 5) * unitFactor;
                        break;

                    default:
                        unitPosition.X = skinnedModel.Position.X * unitFactor;
                        unitPosition.Y = skinnedModel.Position.Z * unitFactor;
                        break;
                }
                return unitPosition;
            }
        }

        public Explorer(Model model, int[] explorerData, PlayScene parent)
            : base(model, parent)
        {
            Model mArrow = PlayContentHolder.Instance.ModelArrow;

            //tao cac nut dieu khien
            _buttons = new Button3D[5];
            //nut len tren
            _buttons[0] = new Button3D(mArrow);
            //nut sang phai
            _buttons[1] = new Button3D(mArrow);
            _buttons[1].Rotation = new Vector3(0, MathHelper.ToRadians(-90), 0);
            //nut xuong duoi
            _buttons[2] = new Button3D(mArrow);
            _buttons[2].Rotation = new Vector3(0, MathHelper.ToRadians(-180), 0);
            //nut sang trai
            _buttons[3] = new Button3D(mArrow);
            _buttons[3].Rotation = new Vector3(0, MathHelper.ToRadians(-270), 0);
            //nut giua
            _buttons[4] = new Button3D(PlayContentHolder.Instance.ModelCenter);

            //di chuyen vi tri model va cac nut bam
            Position = explorerData;

            //khoi tao vi tri tuong doi so voi texture cua nen
            unitFactor = 25.6f / (float)(parent.MazeSize);
            unitPosition = new Vector2();

            Wait();
        }

        public new void Update(GameTime gameTime)
        {
            switch (State)
            {
                case CharacterState.MoveUp:
                    if (skinnedModel.Position.Z > desZ)
                    {
                        skinnedModel.Position.Z -= PlayScene.MOVEMENT_STEP;
                    }
                    else stepDone();
                    break;

                case CharacterState.MoveRight:
                    if (skinnedModel.Position.X < desX)
                    {
                        skinnedModel.Position.X += PlayScene.MOVEMENT_STEP;
                    }
                    else stepDone();
                    break;

                case CharacterState.MoveDown:
                    if (skinnedModel.Position.Z < desZ)
                    {
                        skinnedModel.Position.Z += PlayScene.MOVEMENT_STEP;
                    }
                    else stepDone();
                    break;

                case CharacterState.MoveLeft:
                    if (skinnedModel.Position.X > desX)
                    {
                        skinnedModel.Position.X -= PlayScene.MOVEMENT_STEP;
                    }
                    else stepDone();
                    break;

                case CharacterState.Catched:
                    if (++timeline == 120) parent.Lose();
                    if (timeline == 30)
                    {
                        skinnedModel.Rotation.Y = MathHelper.ToRadians(45);
                        skinnedModel.Position.Z -= 1;
                        skinnedModel.Position.X -= 3;
                        skinnedModel.SwitchClip(ExplorerClip.Catched);
                    }
                    break;

                case CharacterState.Poisoned:
                    if (timeline > 0) timeline--;
                    if (timeline == 110)
                    {

                        skinnedModel.CustomTexture = PlayContentHolder.Instance.TextureExplorerPoisoned;
                        skinnedModel.SwitchClip(ExplorerClip.Poisoned);
                    }
                    if (timeline > 89 || timeline < 0) return;
                    if (timeline == 60) SoundController.PlaySound(PlayContentHolder.Instance.SoundPoisoned);
                    if (timeline == 0)
                    {
                        parent.Lose();
                        timeline = -1;
                    }
                    break;

                case CharacterState.Fall:
                    if (timeline > 0) timeline--;
                    else if (timeline == 0)
                    {
                        parent.viewpot3DBound.X = 0;
                        parent.viewpot3DBound.Y = 0;
                        State = CharacterState.None;
                        parent.Lose();
                        timeline = -1;
                    }
                    if (timeline < 80) skinnedModel.Position.Y -= 4;
                    if (timeline == 30)
                    {
                        parent.viewpot3DBound.X = -2;
                        parent.viewpot3DBound.Y = -2;
                    }
                    if (timeline < 30 && timeline > 0)
                    {
                        if (timeline % 2 == 1)
                        {
                            parent.viewpot3DBound.X += 4;
                            parent.viewpot3DBound.Y += 4;
                        }
                        else
                        {
                            parent.viewpot3DBound.X -= 4;
                            parent.viewpot3DBound.Y -= 4;
                        }
                    }
                    break;

                case CharacterState.Shock:
                    if (timeline > 0) timeline--;
                    else if (timeline == 0)
                    {
                        parent.viewpot3DBound.X = 0;
                        parent.viewpot3DBound.Y = 0;
                        State = CharacterState.None;
                        parent.Lose();
                        timeline = -1;
                    }
                    if (timeline == 60) skinnedModel.CustomTexture = PlayContentHolder.Instance.TextureExplorerFear;
                    if (timeline == 30)
                    {
                        parent.viewpot3DBound.X = -2;
                        parent.viewpot3DBound.Y = -2;
                    }
                    if (timeline < 30 && timeline > 0)
                    {
                        if (timeline % 2 == 1)
                        {
                            parent.viewpot3DBound.X += 4;
                            parent.viewpot3DBound.Y += 4;
                        }
                        else
                        {
                            parent.viewpot3DBound.X -= 4;
                            parent.viewpot3DBound.Y -= 4;
                        }
                    }
                    break;

                case CharacterState.Escaped:
                    switch ((CharacterState)parent.EscapeDirection)
                    {
                        case CharacterState.MoveUp:
                            skinnedModel.Position.Z -= PlayScene.MOVEMENT_STEP;
                            if (skinnedModel.Position.Z <= 0) skinnedModel.Position.Y += PlayScene.MOVEMENT_STEP;
                            break;

                        case CharacterState.MoveRight:
                            skinnedModel.Position.X += PlayScene.MOVEMENT_STEP;
                            if (skinnedModel.Position.X >= parent.MazeSize * 10) skinnedModel.Position.Y += PlayScene.MOVEMENT_STEP;
                            break;

                        case CharacterState.MoveDown:
                            skinnedModel.Position.Z += PlayScene.MOVEMENT_STEP;
                            if (skinnedModel.Position.Z >= parent.MazeSize * 10) skinnedModel.Position.Y += PlayScene.MOVEMENT_STEP;
                            break;

                        case CharacterState.MoveLeft:
                            skinnedModel.Position.X -= PlayScene.MOVEMENT_STEP;
                            if (skinnedModel.Position.X <= 0) skinnedModel.Position.Y += PlayScene.MOVEMENT_STEP;
                            break;
                    }
                    if (skinnedModel.Position.Y > 10)
                    {
                        State = CharacterState.None;
                        parent.WinLevel();
                    }
                    break;

                case CharacterState.Stand:
                    if (timeline > 0) timeline--;
                    else if (timeline == 0)
                    {
                        CharacterState rand = (CharacterState)PlayScene.Random.Next((int)CharacterState.Look, (int)CharacterState.Sleepy + 1);
                        switch (rand)
                        {
                            case CharacterState.Look:
                                Look();
                                break;

                            case CharacterState.Read:
                                Read();
                                break;

                            case CharacterState.FixFlashlight:
                                FixFlashlight();
                                break;

                            case CharacterState.Sleepy:
                                Sleepy();
                                break;
                        }
                    }
                    break;

                case CharacterState.Read:
                    if (timeline > 0) timeline--;
                    else if (timeline == 0) Stand();
                    if (timeline == 89)
                    {
                        skinnedModel.HideMesh(1);
                        skinnedModel.UnhideMesh(2);
                    }
                    if (timeline == 15)
                    {
                        skinnedModel.HideMesh(2);
                        skinnedModel.UnhideMesh(1);
                    }
                    break;

                case CharacterState.Look:
                case CharacterState.Sleepy:
                case CharacterState.FixFlashlight:
                    if (timeline > 0) timeline--;
                    else if (timeline == 0) Stand();
                    break;
            }

            base.Update(gameTime);
        }

        protected override void stepDone()
        {
            Position = position;
            if (parent.Gate != null) parent.Gate.TriggerKey(position);
            if (!parent.TriggerTrap(position)) parent.SwitchEnemyTurn();
        }

        public bool LightToObject(Vector3 objectPosition)
        {
            if (!parent.IsDarkness) return true;
            if (objectPosition.Z - 16 > skinnedModel.Position.Z) return false;
            if (objectPosition.Z + 16 < skinnedModel.Position.Z) return false;
            if (objectPosition.X - 16 > skinnedModel.Position.X) return false;
            if (objectPosition.X + 16 < skinnedModel.Position.X) return false;
            //int distancePow2 = (int)(Math.Pow(objectPosition.X - skinnedModel.Position.X, 2) + Math.Pow(objectPosition.Z - skinnedModel.Position.Z, 2));
            //if (distancePow2 > 256) return 1;
            return true;
        }

        public new void Wait()
        {
            resetModelState();
            base.Wait();
        }

        public override void MoveUp()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundExplorerWalk);
            resetModelState();
            base.MoveUp();
        }

        public override void MoveRight()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundExplorerWalk);
            resetModelState();
            base.MoveRight();
        }

        public override void MoveDown()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundExplorerWalk);
            resetModelState();
            base.MoveDown();
        }

        public override void MoveLeft()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundExplorerWalk);
            resetModelState();
            base.MoveLeft();
        }

        public new void Stand()
        {
            UpdateControllerState();
            resetModelState();
            base.Stand();
        }

        public void Look()
        {
            State = CharacterState.Look;
            skinnedModel.SwitchClip(ExplorerClip.Look);
            timeline = 69;
        }

        public void Read()
        {
            State = CharacterState.Read;
            skinnedModel.SwitchClip(ExplorerClip.Read);
            timeline = 99;
        }

        public void Sleepy()
        {
            State = CharacterState.Sleepy;
            skinnedModel.SwitchClip(ExplorerClip.Sleepy);
            timeline = 59;
        }

        public void FixFlashlight()
        {
            State = CharacterState.FixFlashlight;
            skinnedModel.SwitchClip(ExplorerClip.FixFlashlight);
            timeline = 39;
        }

        public void Shock()
        {
            State = CharacterState.Shock;
            skinnedModel.SwitchClip(ExplorerClip.Shock);
            timeline = 90;
        }

        public void Fall()
        {
            State = CharacterState.Fall;
            skinnedModel.CustomTexture = PlayContentHolder.Instance.TextureExplorerFear;
            skinnedModel.SwitchClip(ExplorerClip.Fall);
            timeline = 90;
        }

        public void Poisoned()
        {
            State = CharacterState.Poisoned;
            timeline = 140;
        }

        public new void Draw(Matrix view, Matrix projection)
        {
            if (CanMove)
            {
                for (int i = 0; i < _buttons.Length; i++)
                {
                    _buttons[i].Draw(view, projection);
                }
            }

            if (State == CharacterState.Shock && timeline < 33) return;

            base.Draw(view, projection);
        }

        public int CheckButtonPicking(Ray pickRay)
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i].IsPicking(pickRay)) return i;
            }
            return -1;
        }

        public void UpdateControllerState()
        {
            if (parent.Mode == GameMode.Tutorial)
            {
                for (int i = 0; i < _buttons.Length; i++)
                {
                    _buttons[i].Visible = parent.explorerDirectionAvailable[i] = false;
                }
                int key = parent.CurrentTutorialKey;
                if (key >= 0 && key <= 4) _buttons[key].Visible = parent.explorerDirectionAvailable[key] = true;
            }
            else
            {
                _buttons[0].Visible = parent.explorerDirectionAvailable[0] = testMoveUp();
                _buttons[1].Visible = parent.explorerDirectionAvailable[1] = testMoveRight();
                _buttons[2].Visible = parent.explorerDirectionAvailable[2] = testMoveDown();
                _buttons[3].Visible = parent.explorerDirectionAvailable[3] = testMoveLeft();
                _buttons[4].Visible = parent.explorerDirectionAvailable[4] = true;
            }
        }

        protected override bool testMoveUp()
        {
            if (position[0] > 0)
            {
                var row = position[0] - 1;
                var col = position[1];
                if (parent.Gate != null)
                {
                    if (parent.Gate.GatePosition[0] == row && parent.Gate.GatePosition[1] == col && parent.Gate.IsBlock()) return false;
                }
                if (!parent.Cell[row, col, 1] && !parent.CellHasEnemy(position[0] - 1, position[1]))
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool testMoveRight()
        {
            if (position[1] < parent.MazeSize - 1)
            {
                var row = position[0];
                var col = position[1];
                if (!parent.Cell[row, col, 0] && !parent.CellHasEnemy(position[0], position[1] + 1))
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool testMoveDown()
        {
            if (position[0] < parent.MazeSize - 1)
            {
                var row = position[0];
                var col = position[1];
                if (parent.Gate != null)
                {
                    if (parent.Gate.GatePosition[0] == row && parent.Gate.GatePosition[1] == col && parent.Gate.IsBlock()) return false;
                }
                if (!parent.Cell[row, col, 1] && !parent.CellHasEnemy(position[0] + 1, position[1]))
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool testMoveLeft()
        {
            if (position[1] > 0)
            {
                var row = position[0];
                var col = position[1] - 1;
                if (!parent.Cell[row, col, 0] && !parent.CellHasEnemy(position[0], position[1] - 1))
                {
                    return true;
                }
            }
            return false;
        }

        public void Catched()
        {
            State = CharacterState.Catched;
            timeline = -1;
        }

        public void Escaped()
        {
            SoundController.PlaySound(PlayContentHolder.Instance.SoundEscaped);
            State = CharacterState.Escaped;
            skinnedModel.Rotation.Y = MathHelper.ToRadians(180 - parent.EscapeDirection * 90);
            skinnedModel.SwitchClip(ExplorerClip.Walk);
        }

        public void ActiveButton(int buttonId)
        {
            foreach (Button3D button in _buttons)
            {
                button.Deactive();
            }
            if (buttonId >= 0 && buttonId <= 4) _buttons[buttonId].Active();
        }

        private void resetModelState()
        {
            skinnedModel.CustomTexture = PlayContentHolder.Instance.TextureExplorer;
            skinnedModel.HideMesh(2);
            skinnedModel.UnhideMesh(1);
        }

        public bool CanMove
        {
            get
            {
                //if (parent.State != GameState.Play) return false;

                switch (State)
                {
                    case CharacterState.Stand:
                    case CharacterState.Read:
                    case CharacterState.Sleepy:
                    case CharacterState.Look:
                    case CharacterState.FixFlashlight:
                        return true;

                    default:
                        return false;
                }
            }
        }

        public void Rewind(int[] position)
        {
            Position = position;
            skinnedModel.Rotation = Vector3.Zero;
            Stand();
        }
    }
}
