using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tranquillity
{
    /// <summary>
    /// Shrinks the size of a particle based on its age
    /// </summary>
    public class Shrink : IParticleAffector
    {
        /// <summary>
        /// Affects the size of the particle
        /// </summary>
        /// <param name="particle">Particle to affect</param>
        /// <param name="gameTime">Game time</param>
        public void Affect(GameTime gameTime, DynamicParticle particle)
        {
            if (particle.Age.HasValue)
            {
                particle.Scale = MathHelper.Lerp(particle.InitialScale, 0.0f, 1.0f - particle.Age.Value);
            }
        }
    }
}
