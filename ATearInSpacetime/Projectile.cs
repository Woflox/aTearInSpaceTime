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
    public class Projectile : triangleEntity
    {
        Vector2 velocity;
        float timeLeft;
        float initialLifetime;
        EntityType effect;
        triangleEntity owner;
        Vector2 launchVelocity;

        public Projectile(Vector2 pos, Vector2 direction, Vector2 externalVelocity, float launchPower, Color color, EntityType effect, triangleEntity owner)
            :base(pos, direction, 0.05f, 0.05f, color, EntityType.Projectile)
        {
            velocity = direction * 8 + externalVelocity;
            launchVelocity = velocity;
            timeLeft = launchPower / 1.5f;
            initialLifetime = timeLeft;
            this.effect = effect;
            this.owner = owner;
        }

        public override void Update(float dt)
        {
            float timeSteps = 3;
            dt /= timeSteps;
            for (int i = 0; i < timeSteps; i++)
            {
                timeLeft -= dt;

                velocity = launchVelocity * (timeLeft / initialLifetime);
                this.pos += velocity * dt;

                recalcVertices();

                bool shouldDoEffect = false;
                if (timeLeft < dt)
                {
                    shouldDoEffect = true;
                }

                if (Math.Abs(pos.X) > 4.0f / 3.0f || Math.Abs(pos.Y) > 1)
                {
                    destroyed = true;
                }



                foreach (triangleEntity entity in Game1.instance.entities)
                {
                    if (entity.type == EntityType.Ship && entity != owner)
                    {
                        if (IsOverlapping(entity))
                        {
                            shouldDoEffect = true;
                            entity.destroyed = true;
                            Game1.instance.GameOver(entity);
                        }
                    }
                    else if (entity.type == EntityType.Obstacle)
                    {
                        if (IsOverlapping(entity))
                        {
                            shouldDoEffect = true;
                        }
                    }
                }

                if (shouldDoEffect)
                {
                    DoEffect();
                    return;
                }

            }
            base.Update(dt);
        }

        public void DoEffect()
        {
            destroyed = true;

            if (effect == EntityType.Explosion)
            {
                Explosion.CreateExplosionAtPoint(pos);
                Game1.instance.playSound(Game1.instance.explode, pos);
            }
            else if (effect == EntityType.Obstacle)
            {
                Obstacle.CreateClump(pos);
                Game1.instance.playSound(Game1.instance.clump, pos);
            }
        }

    }
}
