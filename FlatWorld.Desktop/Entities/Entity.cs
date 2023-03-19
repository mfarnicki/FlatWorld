using System;
using FlatWorld.Engine;
using FlatWorld.Engine.Graphics;
using FlatWorld.Engine.Physics;
using Microsoft.Xna.Framework;

namespace FlatWorld.Desktop.Entities;

public abstract class Entity
{
    protected Vector2[] vertices;
    protected Vector2 position;
    protected Vector2 velocity;

    protected float angle;
    protected Color color;
    protected float radius;

    public Color CircleColor;

    public Vector2 Position => this.position;

    public float CollisionCircleRadius => this.radius;

    public Entity(Vector2[] vertices, Vector2 position, Color color)
    {
        this.vertices = vertices;
        this.position = position;
        this.velocity = Vector2.Zero;
        this.angle = 0f;
        this.color = color;

        if (vertices != null)
        {
            this.radius = Entity.FindCollisionRadius(vertices);
        }
    }

    protected static float FindCollisionRadius(Vector2[] vertices)
    {
        float polygonArea = PolygonHelper.FindPolygonArea(vertices);
        return MathF.Sqrt(polygonArea / MathHelper.Pi);
    }

    public virtual void Update(GameTime gameTime, FlatCamera camera)
    {
        this.position += this.velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        camera.GetExtents(out Vector2 camMin, out Vector2 camMax);
        float camViewWidth = camMax.X - camMin.X;
        float camViewHeight = camMax.Y - camMin.Y;

        if (this.position.X < camMin.X) { this.position.X += camViewWidth; }
        if (this.position.X > camMax.X) { this.position.X -= camViewWidth; }
        if (this.position.Y < camMin.Y) { this.position.Y += camViewHeight; }
        if (this.position.Y > camMax.Y) { this.position.Y -= camViewHeight; }

        this.CircleColor = Color.White;
    }

    public virtual void Draw(FlatShapes shapes)
    {
        FlatTransform transform = new FlatTransform(this.position, this.angle, 1f);
        shapes.DrawPolygon(this.vertices, transform, 1f, this.color);

        shapes.DrawCircle(this.position.X, this.position.Y, this.radius, 1f, 32, this.CircleColor);
    }
}