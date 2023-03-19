using System;
using FlatWorld.Engine;
using FlatWorld.Engine.Graphics;
using Microsoft.Xna.Framework;

namespace FlatWorld.Desktop.Entities;

public class Asteroid : Entity
{
    public Asteroid(Random rand, FlatCamera camera, float density, float restitution)
        : base(null, Vector2.Zero, Color.Brown, density, restitution)
    {
        const int minPoints = 6;
        const int maxPoints = 10;

        int points = rand.Next(minPoints, maxPoints);

        this.vertices = new Vector2[points];

        float deltaAngle = MathHelper.TwoPi / (float)points;
        float angle = 0f;

        const float minDist = 12f;
        const float maxDist = 24f;

        for (int i = 0; i < this.vertices.Length; i++)
        {
            float dist = FlatUtils.RandomSingle(rand, minDist, maxDist);

            float x = MathF.Cos(angle) * dist;
            float y = MathF.Sin(angle) * dist;

            this.vertices[i] = new Vector2(x, y);

            angle += deltaAngle;
        }

        camera.GetExtents(out Vector2 camMin, out Vector2 camMax);

        camMin *= 0.75f;
        camMax *= 0.75f;

        float px = FlatUtils.RandomSingle(rand, camMin.X, camMax.X);
        float py = FlatUtils.RandomSingle(rand, camMin.Y, camMax.Y);

        this.position = new Vector2(px, py);

        const float minSpeed = 20f;
        const float maxSpeed = 40f;
        Vector2 velocityDirection = FlatUtils.RandomDirection(rand);
        float speed = FlatUtils.RandomSingle(rand, minSpeed, maxSpeed);

        this.velocity = velocityDirection * speed;

        this.radius = Entity.FindCollisionRadius(this.vertices);

        this.area = MathHelper.Pi * this.radius * this.radius;
        this.mass = this.area * density;
        this.invMass = 1f / this.mass;
    }
}