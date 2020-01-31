using Microsoft.Xna.Framework;
using OpenitvnGame;

namespace PyramidRaider
{
    enum TrapState { Inactive, Hole, Rock }

    class Trap
    {
        private CModel trap, rock;
        public int[] Position { get; private set; }
        private TrapState _state = TrapState.Inactive;
        private PlayScene _parent;
        private int timeline = -1;

        public Trap(int[] position, PlayScene parent)
        {
            _parent = parent;
            trap = new CModel(PlayContentHolder.Instance.ModelTrap, new Vector3(position[1] * 10 + 5, 0.02f, position[0] * 10 + 5));

            Position = position;
        }

        public void Update()
        {
            switch (_state)
            {
                case TrapState.Rock:
                    if (timeline > 0) timeline--;
                    if (timeline < 40 && timeline > 30) rock.Position.Y -= 4;
                    break;
            }
        }

        public void Draw(Matrix view, Matrix projection)
        {
            switch (_state)
            {
                case TrapState.Inactive:
                    if (_parent.Explorer.LightToObject(trap.Position)) trap.Draw(view, projection);
                    break;

                case TrapState.Rock:
                    trap.Draw(view, projection);
                    if (timeline <= 40) rock.Draw(view, projection);
                    break;
            }
        }

        public bool TriggerTrap(int[] explorerPosition)
        {
            if (Position[0] != explorerPosition[0] || Position[1] != explorerPosition[1]) return false;

            if (PlayScene.Random.Next(1, 3) == 1)
            {
#if DEBUG
                Main.InsertLog("Explorer: Agrrrr!!!");
#endif
                SoundController.PlaySound(PlayContentHolder.Instance.SoundRock);
                rock = new CModel(PlayContentHolder.Instance.ModelRock, new Vector3(Position[1] * 10 + 5, 36, Position[0] * 10 + 5));
                _state = TrapState.Rock;
                _parent.Explorer.Shock();
                timeline = 90;
            }
            else
            {
#if DEBUG
                Main.InsertLog("Explorer: Aaaaa....");
#endif
                SoundController.PlaySound(PlayContentHolder.Instance.SoundPit);
                _state = TrapState.Hole;
                _parent.Floor.HideMesh(Position[0] * _parent.MazeSize + Position[1]);
                _parent.Explorer.Fall();
            }

            return true;
        }

        public void Rewind()
        {
            _state = TrapState.Inactive;
        }
    }
}
