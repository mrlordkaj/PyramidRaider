using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Tranquillity;
using Tranquillity.Helpers;
using Microsoft.Xna.Framework;

namespace PyramidRaider
{
    class Dust
    {
        DustParticleSystem _explosionParticleSystem;
        int _timeline = 50; //80
        Enemy _parent;
        public bool IsPlay = true;
        public Vector3 Position;

        public Dust(Vector3 position, Enemy parent)
        {
            position.Y += 4;
            Position = position;
            _parent = parent;
            _explosionParticleSystem = new DustParticleSystem(50, PlayContentHolder.Instance.TextureDust);
            PlayScene.ParticleManager.AddParticleSystem(_explosionParticleSystem, BlendState.AlphaBlend);
        }

        public void Update()
        {
            _timeline--;
            if (_timeline > 25) //50
            {
                _explosionParticleSystem.AddParticle(
                    Position,
                    RandomHelper.ColorBetween(Color.DarkGray, Color.Gray),
                    new Vector3(RandomHelper.FloatBetween(-3, 3), RandomHelper.FloatBetween(3, -1), RandomHelper.FloatBetween(-3, 3)) * 0.01f,
                    RandomHelper.FloatBetween(-0.1f, 0.1f),
                    TimeSpan.FromSeconds(RandomHelper.IntBetween(1, 2)),
                    false,
                    RandomHelper.FloatBetween(0.0f, MathHelper.Pi),
                    0.14f);
            }
            else if(_timeline == 0)
            {
                _parent.FightingDone();
            }
        }
    }
}
