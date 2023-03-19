using System;
using FlatWorld.Engine;
using FlatWorld.Engine.Graphics;
using Microsoft.Xna.Framework;

namespace FlatWorld.Desktop.Entities;

public class MainShip : Entity
{
    private bool isRocketForce;
    private Vector2[] rocketVertices;
    private float randomRocketTime;
    private float randomRocketStartTime;

    public MainShip(Vector2[] vertices, Vector2 position, Color color)
    : base(vertices, position, color)
    {
        this.isRocketForce = false;

        this.rocketVertices = new Vector2[3];
        this.rocketVertices[0] = this.vertices[3];
        this.rocketVertices[1] = this.vertices[2];
        this.rocketVertices[2] = new Vector2(-24f, 0f);

        this.randomRocketTime = 60f;
        this.randomRocketStartTime = 0f;

    }

    public void Rotate(float amount)
    {
        this.angle += amount;
        if (this.angle < 0f)
        {
            this.angle += MathHelper.TwoPi;
        }

        if (this.angle >= MathHelper.TwoPi)
        {
            this.angle -= MathHelper.TwoPi;
        }
    }

    public override void Update(GameTime gameTime, FlatCamera camera)
    {
        float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
        if (now - this.randomRocketStartTime >= this.randomRocketTime)
        {
            this.randomRocketStartTime = now;

            float rocketMinX = -28f;
            float rocketMaxX = -20f;
            float rocketMinY = -2f;
            float rocketMaxY = 2f;

            this.rocketVertices[2] = new Vector2(
                FlatUtils.RandomSingle(Random.Shared, rocketMinX, rocketMaxX),
                FlatUtils.RandomSingle(Random.Shared, rocketMinY, rocketMaxY));
        }

        base.Update(gameTime, camera);
    }

    public override void Draw(FlatShapes shapes)
    {
        if (this.isRocketForce)
        {
            FlatTransform transform = new FlatTransform(this.position, this.angle, 1f);
            shapes.DrawPolygon(this.rocketVertices, transform, 1f, Color.Yellow);
        }

        base.Draw(shapes);
    }

    public void ApplyRocketForce(float amount)
    {
        Vector2 forceDirection = new Vector2(MathF.Cos(this.angle), MathF.Sin(this.angle));
        this.velocity += forceDirection * amount;
        this.isRocketForce = true;
    }

    public void DisableRocketForce()
    {
        this.isRocketForce = false;
    }
}