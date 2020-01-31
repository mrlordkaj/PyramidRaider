using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cameras
{
    class TargetCamera : Camera
    {
        public Vector3 Position;
        public Vector3 Target;

        public TargetCamera(Vector3 position, Vector3 target, float aspectRatio)
            : base(aspectRatio)
        {
            Position = position;
            Target = target;
        }

        public override void Update()
        {
            View = Matrix.CreateLookAt(Position, Target, Vector3.Up);
        }
    }
}
