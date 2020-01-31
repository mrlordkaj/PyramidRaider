using Microsoft.Xna.Framework;
using OpenitvnGame;

namespace PyramidRaider
{
    enum GateState { Closed, Closing, Opened, Opening }
    class GateSystem
    {
        CModel gate, gateWall, gateKey;

        public int[] GatePosition { get; private set; }
        public int[] KeyPosition { get; private set; }
        public GateState State { get; private set; }

        private PlayScene _parent;

        public GateSystem(int[] gatePosition, int[] keyPosition, PlayScene parent)
        {
            _parent = parent;

            gate = new CModel(PlayContentHolder.Instance.ModelGate);
            gateWall = new CModel(PlayContentHolder.Instance.ModelGateWall);
            gateKey = new CModel(PlayContentHolder.Instance.ModelGateKey);

            GatePosition = gatePosition;
            KeyPosition = keyPosition;
            gate.Position.X = gateWall.Position.X = GatePosition[1] * 10 + 5;
            gate.Position.Z = gateWall.Position.Z = (GatePosition[0] + 1) * 10;
            gateKey.Position.X = KeyPosition[1] * 10 + 5;
            gateKey.Position.Z = KeyPosition[0] * 10 + 5;

            State = GateState.Closed;
        }

        public void Update()
        {
            gateKey.Rotation.Y -= MathHelper.ToRadians(6);

            switch (State)
            {
                case GateState.Closing:
                    if (gate.Position.Y < 0.3f) gate.Position.Y += 0.5f;
                    else State = GateState.Closed;
                    break;

                case GateState.Opening:
                    if (gate.Position.Y > -4.2f) gate.Position.Y -= 0.5f;
                    else State = GateState.Opened;
                    break;
            }
        }

        public void TriggerKey(int[] position)
        {
            if (KeyPosition[0] != position[0] || KeyPosition[1] != position[1]) return;
            SoundController.PlaySound(PlayContentHolder.Instance.SoundGateToggle);
            if (State == GateState.Closed) State = GateState.Opening;
            if (State == GateState.Opened) State = GateState.Closing;
        }

        public void Draw(Matrix view, Matrix projection) {
            if (_parent.Explorer.LightToObject(gateKey.Position)) gateKey.Draw(view, projection);

            if (_parent.Explorer.LightToObject(gate.Position))
            {
                gateWall.Draw(view, projection);
                gate.Draw(view, projection);
            }
        }

        public bool IsBlock()
        {
            return (State == GateState.Closed || State == GateState.Closing);
        }

        public void Rewind(bool isOpen)
        {
            if (isOpen)
            {
                State = GateState.Opened;
                gate.Position.Y = -4.2f;
            }
            else
            {
                State = GateState.Closed;
                gate.Position.Y = 0.3f;
            }
        }
    }
}
