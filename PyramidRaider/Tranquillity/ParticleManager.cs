using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tranquillity
{
    /// <summary>
    /// Manages the logic and drawing of all particle systems.
    /// This is a DrawableGameComponent; simply add it to the game component collection of your game.
    /// </summary>
    public class ParticleManager
    {
        #region Fields


        /// <summary>
        /// The default blend state of a newly added particle system
        /// </summary>
        public readonly BlendState DefaultBlendState = BlendState.NonPremultiplied;

        readonly Matrix InvertY = Matrix.CreateScale(1, -1, 1);

        BasicEffect basicEffect;

        Dictionary<BlendState, List<IParticleSystem>> particleSystems = new Dictionary<BlendState, List<IParticleSystem>>();


        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the camera view matrix
        /// </summary>
        Matrix View { get; set; }

        /// <summary>
        /// Gets or set the camera projection matrix
        /// </summary>
        Matrix Projection { get; set; }

        /// <summary>
        /// Gets the total current number of particles across all enabled particle systems
        /// </summary>
        public int ParticleCount
        {
            get
            {
                int total = 0;

                foreach (List<IParticleSystem> particleSystemBatch in particleSystems.Values)
                {
                    foreach (IParticleSystem particleSystem in particleSystemBatch)
                    {
                        if (particleSystem.Enabled)
                        {
                            total += particleSystem.ParticleCount;
                        }
                    }
                }

                return total;
            }
        }


        #endregion

        #region Initialization

        /// <summary>
        /// Initializes an instance of the particle manager
        /// </summary>
        /// <param name="game"></param>
        public ParticleManager(GraphicsDevice graphicDevice)
        {
            basicEffect = new BasicEffect(graphicDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
                LightingEnabled = false,
                World = InvertY,
                View = Matrix.Identity
            };
        }


        #endregion

        #region Update


        /// <summary>
        /// Updates the properties of all particle systems
        /// </summary>
        public void Update(GameTime gameTime)
        {
            foreach (List<IParticleSystem> particleSystemBatch in particleSystems.Values)
            {
                foreach (IParticleSystem particleSystem in particleSystemBatch)
                {
                    if (particleSystem.Enabled)
                    {
                        DynamicParticleSystem dynamicParticleSystem = particleSystem as DynamicParticleSystem;

                        if (dynamicParticleSystem != null)
                        {
                            dynamicParticleSystem.Update(gameTime);
                        }
                    }
                }
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws all of the particle systems
        /// </summary>
        /// <remarks>
        /// An efficient SpriteBatch-based method by Shawn Hargreaves.
        /// Thoroughly documented at http://blogs.msdn.com/b/shawnhar/archive/2011/01/12/spritebatch-billboards-in-a-3d-world.aspx
        /// </remarks>
        public void Draw(SpriteBatch spriteBatch)
        {
            basicEffect.Projection = Projection;

            foreach (KeyValuePair<BlendState, List<IParticleSystem>> particleSystemBatch in particleSystems)
            {
                //spriteBatch.Begin(0, particleSystemBatch.Key, null, DepthStencilState.DepthRead, RasterizerState.CullNone, basicEffect);
                spriteBatch.Begin(0, particleSystemBatch.Key, null, DepthStencilState.None, RasterizerState.CullNone, basicEffect);

                foreach (IParticleSystem particleSystem in particleSystemBatch.Value)
                {
                    if (particleSystem.Enabled)
                    {
                        for (int i = 0; i < particleSystem.ParticleCount; i++)
                        {
                            IParticle particle = particleSystem[i];

                            Vector3 viewSpacePosition = Vector3.Transform(particle.Position, View);

                            spriteBatch.Draw(particleSystem.Texture, new Vector2(viewSpacePosition.X, viewSpacePosition.Y), null, particle.Color, particle.Angle, particleSystem.TextureOrigin, particle.Scale, 0, viewSpacePosition.Z);
                        }
                    }
                }

                spriteBatch.End();
            }
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Adds a particle system with default blend state settings
        /// </summary>
        public void AddParticleSystem(IParticleSystem particleSystem)
        {
            AddParticleSystem(particleSystem, DefaultBlendState);
        }

        /// <summary>
        /// Adds a particle system with custom blend state settings
        /// </summary>
        public void AddParticleSystem(IParticleSystem particleSystem, BlendState blendState)
        {
            if (particleSystems.ContainsKey(blendState))
            {
                particleSystems[blendState].Add(particleSystem);
            }
            else
            {
                List<IParticleSystem> particleSystemBatch = new List<IParticleSystem>();
                particleSystemBatch.Add(particleSystem);

                particleSystems.Add(blendState, particleSystemBatch);
            }
        }

        /// <summary>
        /// Sets the view and projection matrices
        /// </summary>
        public void SetMatrices(Matrix view, Matrix projection)
        {
            View = view * InvertY;
            Projection = projection;
        }


        #endregion
    }
}
