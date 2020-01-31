using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cameras
{
    abstract class Camera
    {
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        private float aspectRatio;

        public Camera(float aspectRatio)
        {
            this.aspectRatio = aspectRatio;
            generatePerspectiveProjectionMatrix(MathHelper.PiOver4);
        }

        private void generatePerspectiveProjectionMatrix(float FieldOfView)
        {
            //tinh ma tran projection
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(28), //goc nhin
                aspectRatio, //ti le man hinh
                1, 400); //khoang cach gan va khoang cach xa
        }

        public virtual void Update() { }
    }
}
