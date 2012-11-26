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
    public class Explosion : triangleEntity
    {
        float timeLeft;
        float initialLifeTime;
        float initialWidth;
        float initialLength;

        static float collisionFudge = 1.0f;

        public static void CreateExplosionAtPoint(Vector2 pos, bool destroyThings)
        {
            for (int i = 0; i < 20; i++)
            {
                float size = ((float)Game1.rand.NextDouble() + 1) / (2.0f * collisionFudge);
                Game1.instance.entities.Add(new Explosion(pos, Game1.randUnitVector(), 0.1f * size, 0.4f * size, new Color(0, (byte)Game1.rand.Next(64), (byte)Game1.rand.Next(128)), destroyThings));
            }
            Game1.instance.timeSinceExplosion = 0;
        }


        public Explosion(Vector2 pos, Vector2 direction, float width, float length, Color color, bool destroyThings)
            : base(pos, direction, width, length, color, EntityType.Explosion)
        {
            initialLifeTime = 0.3f;
            timeLeft = initialLifeTime;
            initialWidth = width * collisionFudge;
            initialLength = length * collisionFudge;

            bool destroyedShip = false;
            Vector2 shipPos = Vector2.Zero;

            if (destroyThings)
            {
                foreach (triangleEntity entity in Game1.instance.entities)
                {
                    if (entity.type == EntityType.Obstacle)
                    {
                        if (IsOverlapping(entity))
                        {
                            entity.destroyed = true;
                        }
                    }
                }
            }

            this.width *= collisionFudge;
            this.length *= collisionFudge;
            recalcEverything();

            foreach (triangleEntity entity in Game1.instance.entities)
            {
                if (entity.type == EntityType.Ship && !entity.destroyed)
                {
                    if (IsOverlapping(entity))
                    {
                        entity.destroyed = true;
                        destroyedShip = true;
                        shipPos = entity.pos;
                        Game1.instance.GameOver(entity);
                    }
                }
            }
            if (destroyedShip)
            {
                CreateExplosionAtPoint(shipPos, false);
            }

        }

        public override void Update(float dt)
        {
            timeLeft -= dt;

            if (timeLeft < 0)
            {
                destroyed = true;
            }
            else
            {

                float fade = timeLeft / initialLifeTime;
                color.A = (byte)(fade * fade * 255);
                width = initialWidth * fade;
                length = initialLength * (0.9f + fade * 0.1f);
                recalcEverything();
            }

            base.Update(dt);
        }
    }
}
