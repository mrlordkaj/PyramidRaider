using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SkinnedModel;

namespace OpenitvnGame
{
    class CSkinnedModel : CModel
    {
        AnimationPlayer animationPlayer;
        SkinningData skinningData;
        public string CurrentClip { get; private set; }

        public CSkinnedModel(Model model)
            : base(model)
        {
            prepareAnimation();
        }

        public CSkinnedModel(Model model, Vector3 position)
            : base(model, position)
        {
            prepareAnimation();
        }

        public CSkinnedModel(Model model, Vector3 position, Vector3 rotation)
            : base(model, position, rotation)
        {
            prepareAnimation();
        }

        public CSkinnedModel(Model model, Vector3 position, Vector3 rotation, Vector3 scaling)
            : base(model, position, rotation, scaling)
        {
            prepareAnimation();
        }

        private void prepareAnimation()
        {
            skinningData = model.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException("This model does not contain a SkinningData tag.");

            animationPlayer = new AnimationPlayer(skinningData);
        }

        public void Update(TimeSpan modelTime)
        {
            animationPlayer.Update(modelTime, true, Matrix.Identity);
        }

        public new void Draw(Matrix view, Matrix projection)
        {
            Matrix baseWorld = Matrix.CreateScale(Scale)
                                * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                                * Matrix.CreateTranslation(Position);

            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                if (_hiddenMeshs.Contains(i++)) continue;
                Matrix localWorld = modelTransforms[mesh.ParentBone.Index] * baseWorld;
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(animationPlayer.GetSkinTransforms());
                    effect.World = localWorld;
                    effect.View = view;
                    effect.Projection = projection;
                    if (Alpha < 1) effect.Alpha = Alpha;
                    if (CustomTexture != null) effect.Texture = CustomTexture;
                    effect.EnableDefaultLighting();
                    if (_dim)
                    {
                        effect.DirectionalLight0.Direction = new Vector3(0.3f, 0, 0.3f);
                        effect.DirectionalLight1.Direction = new Vector3(-0.3f, 0, -0.3f);
                        effect.DirectionalLight2.Enabled = false;
                    }
                }
                mesh.Draw();
            }
        }

        public void SwitchClip(string clipName)
        {
            if (CurrentClip != clipName)
            {
                AnimationClip clip = skinningData.AnimationClips[clipName];
                animationPlayer.StartClip(clip);
                CurrentClip = clipName;
            }
        }
    }
}
