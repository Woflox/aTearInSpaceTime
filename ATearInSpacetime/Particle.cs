using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace ATearInSpacetime
{
    public class Particle : triangleEntity
    {
        public static Particle RandomParticle()
        {
            float size = (float)Game1.rand.NextDouble();
            size *= size;
            return new Particle(Vector2.Zero, Game1.randUnitVector(), ((float)Game1.rand.NextDouble() ) *0.25f*size,
                                                                      ((float)Game1.rand.NextDouble()) * 0.25f * size, new Color(0.002f / size, 0.025f / size, 0.015f/size));
        }

        public Particle(Vector2 pos, Vector2 direction, float width, float length, Color color)
            : base(pos, direction, width, length, color, EntityType.Particle)
        {
            RandomizePosition();
        }

        public override void Update(float dt)
        {
            RandomizePosition();
            base.Update(dt);
        }

        public void RandomizePosition()
        {
            pos.X = ((float)Game1.rand.NextDouble() * 2 - 1) * (4.0f / 3);
            pos.Y = (float)Game1.rand.NextDouble() * 2 - 1;

            float moveSpeed = 5.0f;
            float noiseScale = 0.15f;

            float p = Noise3.noise((pos.X + Game1.instance.timeSinceGameStart * moveSpeed) * noiseScale,
                                   (pos.Y + Game1.instance.timeSinceGameStart * moveSpeed) * noiseScale);
            p = (p + 0.5f)/1.5f;
            if (p > 0)
            {
                p = (float)Math.Pow(p, 2);
                p *= 1.5f;
            }
            //color.R = (byte)(p * 255);

            hidden = Game1.rand.NextDouble() > p;
        }


    }
}
