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
    public class triangleEntity
    {
        public Vector2 pos;
        public Vector2 direction;
        public float width;
        public float length;
        public Color color;
        public bool destroyed;
        public bool hidden = false;

        public Vector2 minCorner;
        public Vector2 maxCorner;

        public enum EntityType
        {
            Ship,
            Projectile,
            Obstacle,
            Explosion,
            Particle,
            Charge,
            Flame
        }

        public EntityType type;

        public Vector3[] relativePoints = new Vector3[3];

        public VertexPositionColor[] vertices = new VertexPositionColor[4];


        public triangleEntity(Vector2 pos, Vector2 direction, float width, float length, Color color, EntityType type)
        {
            this.pos = pos;
            this.direction = direction;
            this.width = width;
            this.length = length;
            this.type = type;
            this.color = color;

            recalcEverything();
        }

        public virtual void Update(float dt)
        {
            recalcVertices();
        }

        public void recalcEverything()
        {

            Vector2 halfperpdir = new Vector2(direction.Y * width / 2, -direction.X * width / 2);


            relativePoints[0] = new Vector3(direction.X * length, direction.Y * length, 0);
            relativePoints[1] = new Vector3(halfperpdir.X,
                                                halfperpdir.Y,
                                                0);
            relativePoints[2] = new Vector3(-halfperpdir.X,
                                                -halfperpdir.Y,
                                                0);


            recalcVertices();
        }

        public void recalcVertices()
        {
            Vector3 pos3 = new Vector3(pos.X, pos.Y, 0);
            vertices[0].Position = relativePoints[0] + pos3;
            vertices[1].Position = relativePoints[1] + pos3;
            vertices[2].Position = relativePoints[2] + pos3;
            vertices[3].Position = relativePoints[0] + pos3;
            vertices[0].Color = color;
            vertices[1].Color = color;
            vertices[2].Color = color;
            vertices[3].Color = color;
            minCorner.X = Math.Min(vertices[0].Position.X, Math.Min(vertices[1].Position.X, vertices[2].Position.X));
            minCorner.Y = Math.Min(vertices[0].Position.Y, Math.Min(vertices[1].Position.Y, vertices[2].Position.Y));
            maxCorner.X = Math.Max(vertices[0].Position.X, Math.Max(vertices[1].Position.X, vertices[2].Position.X));
            maxCorner.Y = Math.Max(vertices[0].Position.Y, Math.Max(vertices[1].Position.Y, vertices[2].Position.Y));
        }

        public void Draw(GraphicsDevice graphics)
        {
            graphics.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 1);
        }

        public void Draw2(GraphicsDevice graphics)
        {
            graphics.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 3);
        }

        public bool IsOverlapping(triangleEntity entity)
        {
            if (minCorner.X < entity.maxCorner.X && maxCorner.X > entity.minCorner.X
             && minCorner.Y < entity.maxCorner.Y && maxCorner.Y > entity.minCorner.Y)
            {
                return areLinesIntersecting(vertices[0].Position, vertices[1].Position, entity.vertices[0].Position, entity.vertices[1].Position) ||
                    areLinesIntersecting(vertices[1].Position, vertices[2].Position, entity.vertices[0].Position, entity.vertices[1].Position) ||
                    PointInTriangle(vertices[0].Position, entity.vertices[0].Position, entity.vertices[1].Position, entity.vertices[2].Position) ||
                    PointInTriangle(vertices[1].Position, entity.vertices[0].Position, entity.vertices[1].Position, entity.vertices[2].Position) ||
                    PointInTriangle(vertices[2].Position, entity.vertices[0].Position, entity.vertices[1].Position, entity.vertices[2].Position) ||
                    PointInTriangle(entity.vertices[0].Position, vertices[0].Position, vertices[1].Position, vertices[2].Position) ||
                    PointInTriangle(entity.vertices[0].Position, vertices[0].Position, vertices[1].Position, vertices[2].Position) ||
                    PointInTriangle(entity.vertices[0].Position, vertices[0].Position, vertices[1].Position, vertices[2].Position);
            }
            return false;
        }

        private bool areLinesIntersecting(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
        {
            float ua = (point4.X - point3.X) * (point1.Y - point3.Y) - (point4.Y - point3.Y) * (point1.X - point3.X);
            float ub = (point2.X - point1.X) * (point1.Y - point3.Y) - (point2.Y - point1.Y) * (point1.X - point3.X);
            float denominator = (point4.Y - point3.Y) * (point2.X - point1.X) - (point4.X - point3.X) * (point2.Y - point1.Y);

            if (Math.Abs(denominator) <= 0.00001f)
            {
                if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f)
                {
                    return true;
                }
            }
            else
            {
                ua /= denominator;
                ub /= denominator;

                if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                {
                    return true;
                }
            }

            return false;
        }

        private bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
        {
            Vector3 cp1 = Vector3.Cross(b-a, p1-a);
            Vector3 cp2 = Vector3.Cross(b-a, p2-a);
            if (Vector3.Dot(cp1, cp2) >= 0)
                return true;
            else 
                return false;
        }

        private bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
        if (SameSide(p,a, b,c) && SameSide(p,b, a,c) && SameSide(p,c, a,b) )
            return true;
        else 
            return false;
        }
    }
}
