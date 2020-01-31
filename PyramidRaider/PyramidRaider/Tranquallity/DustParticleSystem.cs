using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tranquillity
{
    public class DustParticleSystem : DynamicParticleSystem
    {
        public DustParticleSystem(int maxCapacity, Texture2D texture)
            : base(maxCapacity, texture)
        {

        }

        public override void Update(GameTime gameTime)
        {
            foreach (DynamicParticle particle in liveParticles)
            {
                particle.Color = Color.Lerp(particle.InitialColor, new Color(1.0f, 1.0f, 1.0f, 0.0f), 1.0f - particle.Age.Value);
                particle.Scale += 0.005f;
            }

            base.Update(gameTime);
        }
    }
}
