using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OpenitvnGame
{
    class CModel
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public float Alpha = 1;
        protected Model model;
        protected Matrix[] modelTransforms;
        protected List<int> _hiddenMeshs;

        public CModel(Model model)
        {
            buildFromModel(model);
        }

        public CModel(Model model, Vector3 position)
        {
            buildFromModel(model);
            Position = position;
        }

        public CModel(Model model, Vector3 position, Vector3 rotation)
        {
            buildFromModel(model);
            Position = position;
            Rotation = rotation;
        }

        public CModel(Model model, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            buildFromModel(model);
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        private void buildFromModel(Model model)
        {
            this.model = model;
            modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            _hiddenMeshs = new List<int>();
        }

        public void HideMesh(int meshIndex)
        {
            if(!_hiddenMeshs.Contains(meshIndex)) _hiddenMeshs.Add(meshIndex);
        }

        public void UnhideMesh(int meshIndex)
        {
            _hiddenMeshs.Remove(meshIndex);
        }

        public void UnhideAllMesh()
        {
            _hiddenMeshs.Clear();
        }

        public void Draw(Matrix view, Matrix projection)
        {
            Matrix baseWorld = Matrix.CreateScale(Scale)
                                * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                                * Matrix.CreateTranslation(Position);

            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                if (_hiddenMeshs.Contains(i++)) continue;
                Matrix localWorld = modelTransforms[mesh.ParentBone.Index] * baseWorld;
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    BasicEffect effect = (BasicEffect)meshPart.Effect;
                    effect.World = localWorld;
                    effect.View = view;
                    effect.Projection = projection;
                    if (Alpha < 1) effect.Alpha = Alpha;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
