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
    public class Ship: triangleEntity
    {
        public float minX;
        public float minY;
        public float maxX;
        public float maxY;
        public float moveSpeed;

        public int playerNum;

        public bool explode = true;

        Keys up;
        Keys down;
        Keys left;
        Keys right;
        Keys fire1;
        Keys fire2;

        float chargeRate;
        float fire1Charge;
        float fire2Charge;
        bool wasChargingFire1;
        bool wasChargingFire2;

        public SoundEffectInstance charge1SoundInstance;
        public SoundEffectInstance charge2SoundInstance;

        public Ship(Vector2 pos, float minX, float maxX, Vector2 direction, Color color, Keys up, Keys down, Keys left, Keys right, Keys fire1, Keys fire2, int playerNum)
            : base(pos, direction, 0.05f, 0.075f, color, EntityType.Ship)
        {
            this.minX = minX;
            this.minY = -1;
            this.maxX = maxX;
            this.maxY = 1;
            this.moveSpeed = 2.0f;
            this.chargeRate = 1.0f;
            this.up = up;
            this.down = down;
            this.right = right;
            this.left = left;
            this.fire1 = fire1;
            this.fire2 = fire2;
            this.playerNum = playerNum;

            charge1SoundInstance = Game1.instance.charge.CreateInstance();
            charge2SoundInstance = Game1.instance.charge.CreateInstance();
            charge1SoundInstance.Volume = 0.65f;
            charge2SoundInstance.Volume = 0.65f;
        }

        public override void Update(float dt)
        {
            KeyboardState ks = Game1.instance.keyboardState;
            KeyboardState lastKs = Game1.instance.lastKeyboardState;

            Vector2 moveDir = Vector2.Zero;

            if (ks.IsKeyDown(up))
            {
                moveDir.Y += 1;
                Game1.instance.timeIdling = 0;
            }
            if (ks.IsKeyDown(down))
            {
                moveDir.Y -= 1;
                Game1.instance.timeIdling = 0;
            }
            if (ks.IsKeyDown(left))
            {
                moveDir.X -= 1;
                Game1.instance.timeIdling = 0;
            }
            if (ks.IsKeyDown(right))
            {
                moveDir.X += 1;
                Game1.instance.timeIdling = 0;
            }

            if (moveDir != Vector2.Zero)
            {
                moveDir.Normalize();
                pos += moveDir * moveSpeed * dt;

                pos.X = Math.Min(maxX, Math.Max(minX, pos.X));
                pos.Y = Math.Min(maxY, Math.Max(minY, pos.Y));
            }

            if (ks.IsKeyDown(fire1) && !lastKs.IsKeyDown(fire1))
            {
                wasChargingFire1 = true;
                charge1SoundInstance.Play();
            }
            if (ks.IsKeyDown(fire1) && wasChargingFire1)
            {
                fire1Charge += chargeRate * dt;
                fire1Charge = Math.Min(1, fire1Charge);
                charge1SoundInstance.Pan = pos.X * (3.0f / 4.0f);
                charge1SoundInstance.Volume = 0.65f * (0.5f + Math.Abs(pos.X / (3.0f / 4.0f))/2.0f);
                Game1.instance.timeSinceButtonPress = 0;
                Game1.instance.timeIdling = 0;
            }
            else
            {
                if (wasChargingFire1)
                {
                    charge1SoundInstance.Stop();
                    Game1.instance.playSound(Game1.instance.shoot, pos);
                    Game1.instance.entities.Add(new Projectile(pos + direction * length, direction, moveDir * moveSpeed, fire1Charge, new Color(0, 128, 128), EntityType.Explosion, this));
                }

                wasChargingFire1 = false;
                fire1Charge = 0;
            }

            if (ks.IsKeyDown(fire2) && !lastKs.IsKeyDown(fire2))
            {
                wasChargingFire2 = true;
                charge2SoundInstance.Play();
            }
            if (ks.IsKeyDown(fire2) && wasChargingFire2)
            {
                fire2Charge += chargeRate * dt;
                fire2Charge = Math.Min(1, fire2Charge);
                charge2SoundInstance.Pan = pos.X * (3.0f / 4.0f);
                charge2SoundInstance.Volume = 0.65f * (0.5f + Math.Abs(pos.X / (3.0f / 4.0f))/2.0f);
                Game1.instance.timeSinceButtonPress = 0;
                Game1.instance.timeIdling = 0;
            }
            else
            {
                if (wasChargingFire2)
                {
                    charge2SoundInstance.Stop();
                    Game1.instance.playSound(Game1.instance.shoot, pos);
                    Game1.instance.entities.Add(new Projectile(pos + direction * length, direction, moveDir * moveSpeed, fire2Charge, new Color(128, 128, 0), EntityType.Obstacle, this));
                }

                wasChargingFire2 = false;
                fire2Charge = 0;
            }


            foreach (triangleEntity entity in Game1.instance.entities)
            {
                if (entity.type == EntityType.Obstacle)
                {
                    if (IsOverlapping(entity))
                    {
                        destroyed = true;
                        Game1.instance.GameOver(this);
                    }
                }
            }
            if (destroyed && explode)
            {
                Explosion.CreateExplosionAtPoint(pos);
            }
            if (destroyed)
            {
                charge1SoundInstance.Dispose();
                charge2SoundInstance.Dispose();
            }


            base.Update(dt);
        }
    }
}
