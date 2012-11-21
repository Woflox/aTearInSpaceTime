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
    public class Obstacle : triangleEntity
    {
        float averageGrowTime;

        int numGrows = 0;

        float fullWidth;
        float fullLength;

        float tSinceBirth = 0;
        float birthTime = 0;

        bool firstFull = true;



        public static void CreateClump(Vector2 pos)
        {
            Obstacle centerObstacle = new Obstacle(pos, Game1.randUnitVector(), (((float)Game1.rand.NextDouble() + 0.25f) / 1.25f) * 0.3f, (((float)Game1.rand.NextDouble() + 0.25f) / 1.25f) * 0.3f);
            Vector3 relTrianglecenter = (centerObstacle.relativePoints[0] + centerObstacle.relativePoints[1] + centerObstacle.relativePoints[2]) / 3;
            centerObstacle.pos -= new Vector2(relTrianglecenter.X, relTrianglecenter.Y);
            centerObstacle.recalcVertices();

            Game1.instance.entities.Add(centerObstacle);

            Game1.instance.timeSinceClump = 0;
            centerObstacle.numGrows = 4;
        }

        public Obstacle(Vector2 pos, Vector2 direction, float width, float length)
            : base(pos, direction, 0, 0, new Color(100, 32, 0), EntityType.Obstacle)
        {
            averageGrowTime = 5.0f;
            fullWidth = width;
            fullLength = length;
        }

        public override void  Update(float dt)
        {
            if (tSinceBirth < birthTime)
            {
                tSinceBirth += dt;

                width = fullWidth * tSinceBirth / birthTime;
                length = fullLength * tSinceBirth / birthTime;
                recalcEverything();
            }
            else if (firstFull)
            {
                firstFull = false;
                width = fullWidth;
                length = fullLength;
                recalcEverything();
                for (int i = 0; i < numGrows; i++)
                {
                    MakeGrowAttempt();
                }
            }
            float p = dt / averageGrowTime;
 
            if (Game1.rand.NextDouble() <= p)
            {
                if (tSinceBirth >= birthTime)
                {
                    MakeGrowAttempt();
                }
                else
                {
                    numGrows++;
                }
            }

 	        base.Update(dt);
        }

        public void MakeGrowAttempt()
        {
            if (Math.Abs(pos.X) > 4.0f / 3 || Math.Abs(pos.Y) > 1)
            {
                return;
            }

            int vertexOffset = Game1.rand.Next(3);

            Vector3 edgeCenter = (vertices[vertexOffset].Position + vertices[vertexOffset + 1].Position) / 2;
            Vector3 normal = vertices[vertexOffset].Position - vertices[vertexOffset + 1].Position;
            normal = new Vector3(normal.Y, -normal.X, 0);
            normal.Normalize();

            Obstacle obstacle = new Obstacle(new Vector2(edgeCenter.X, edgeCenter.Y), new Vector2(normal.X, normal.Y), (((float)Game1.rand.NextDouble() + 0.25f) / 1.25f) * 0.3f, (((float)Game1.rand.NextDouble() + 0.25f) / 1.25f) * 0.3f);

            int overlapCount = 0;
            foreach (triangleEntity entity in Game1.instance.entities)
            {
                if (entity.type == EntityType.Obstacle && entity != this)
                {
                    if (obstacle.IsOverlapping(entity))
                    {
                        overlapCount++;
                    }
                }
                if (overlapCount >= 2)
                {
                    return;
                }
            }
            Game1.instance.entities.Add(obstacle);
        }
    }
}
