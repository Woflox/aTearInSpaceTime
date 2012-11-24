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
    public class Flame : triangleEntity
    {
        const float baseWidth = 0.025f;
        const float baseLength = 0.05f;

        Ship ship;

        public Flame(Ship ship)
            : base(ship.pos, -ship.direction, baseWidth, baseLength, new Color(255, 0, 0), EntityType.Flame)
        {
            this.ship = ship;

        }


        public override void Update(float dt)
        {
            pos = ship.pos;
            float lengthMul = 1;
            if (ship.playerNum == 1)
            {
                pos.Y += 0.0025f;
            }

            float boost =  Math.Abs((direction - ship.moveDir * 0.5f).X);
            if (boost > 1.1f)
            {
                boost = 1 + (boost - 1)*2;
            }

            float flameAmount = (float)Game1.rand.NextDouble() * boost;

            length = flameAmount * baseLength;

            color.G = (byte)(flameAmount * 64);

            destroyed = ship.destroyed;

            recalcEverything();

            base.Update(dt);
        }
    }
}
