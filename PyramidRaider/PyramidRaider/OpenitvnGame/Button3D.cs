using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenitvnGame
{
    class Button3D : CModel
    {
        public bool Visible { get; set; }
        public bool IsActive { get; private set; }

        public Button3D(Model model)
            : base(model)
        {
            Deactive();
        }

        public Button3D(Model model, Vector3 position)
            : base(model, position)
        {
            Deactive();
        }

        public Button3D(Model model, Vector3 position, Vector3 rotation)
            : base(model, position, rotation)
        {
            Deactive();
        }

        public Button3D(Model model, Vector3 position, Vector3 rotation, Vector3 scale)
            : base(model, position, rotation, scale)
        {
            Deactive();
        }

        public new void Draw(Matrix view, Matrix projection)
        {
            if (Visible) base.Draw(view, projection);
        }

        public void Active()
        {
            HideMesh(0);
            UnhideMesh(1);
            IsActive = true;
        }

        public void Deactive()
        {
            HideMesh(1);
            UnhideMesh(0);
            IsActive = false;
        }

        public bool IsPicking(Ray pickRay)
        {
            if (!Visible) return false;

            BoundingBox bounding = new BoundingBox(
                new Vector3(Position.X - 5, 0, Position.Z - 5),
                new Vector3(Position.X + 5, 0, Position.Z + 5)
            );
            Nullable<float> result = pickRay.Intersects(bounding);
            return result.HasValue;
        }
    }
}
