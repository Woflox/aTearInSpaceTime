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
    public class Charge : triangleEntity
    {
        float initialWidth;
        float initialLength;

        bool isBomb;

        Ship ship;

        public static void CreateCharge(Ship ship, bool isBomb)
        {
            for (int i = 0; i < 10; i++)
            {
                float size = (float)Game1.rand.NextDouble();
                Color color = isBomb ? new Color(0, 64,64) : new Color(64, 64, 0);

                Game1.instance.entities.Add(new Charge(0.01f, 0.1f * size, color, ship, isBomb));
            }
        }


        public Charge(float width, float length, Color color, Ship ship, bool isBomb)
            : base(Vector2.Zero, Game1.randUnitVector(), width, length, color, EntityType.Charge)
        {
            initialWidth = width ;
            initialLength = length;
            this.ship = ship;
            this.isBomb = isBomb;
        }

        public void UpdatePos()
        {
            pos = ship.pos + ship.direction * ship.length;
            if (ship.playerNum == 2)
            {
                pos.Y -= 0.0025f;
            }
        }

        public override void Update(float dt)
        {
            float fade = 0;

            direction = Game1.randUnitVector();
            
            if (isBomb)
            {
                destroyed = !ship.chargingFire1 || ship.destroyed;
                fade = 1 - ship.fire1Charge;
            }
            else
            {
                destroyed = !ship.chargingFire2 || ship.destroyed;
                fade = 1 - ship.fire2Charge;
            }

            color.A = (byte)(255*Math.Min(1,(1 - fade) * 10));
            length = initialLength * (fade * 0.9f + 0.1f);

            UpdatePos();
            recalcEverything();

            base.Update(dt);
        }
    }
}
