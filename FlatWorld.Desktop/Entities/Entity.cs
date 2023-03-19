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

    protected float density;
    protected float mass;
    protected float invMass;
    protected float restitution;

    protected float area;

    public Vector2 Position => this.position;

    public float CollisionCircleRadius => this.radius;

    public Vector2 Velocity
    {
        get => this.velocity;
        set => this.velocity = value;
    }

    public float Restitution => this.restitution;

    public float InverseMass => this.invMass;

    public Entity(Vector2[] vertices, Vector2 position, Color color, float density, float restitution)
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

        this.area = 0f;
        this.density = Math.Clamp(density, CommonDensities.MinDensity, CommonDensities.MaxDensity);
        this.restitution = Math.Clamp(restitution, 0f, 1f);
        this.mass = 0f;
        this.invMass = 1f;
    }

    protected static float FindCollisionRadius(Vector2[] vertices)
    {
        float polygonArea = PolygonHelper.FindPolygonArea(vertices);
        return MathF.Sqrt(polygonArea / MathHelper.Pi);
    }

    public void Move(Vector2 amount)
    {
        this.position += amount;
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

    public virtual void Draw(FlatShapes shapes, bool displayCollisionCircles = false)
    {
        FlatTransform transform = new FlatTransform(this.position, this.angle, 1f);
        shapes.DrawPolygon(this.vertices, transform, 1f, this.color);

        if (displayCollisionCircles)
        {
            shapes.DrawCircle(this.position.X, this.position.Y, this.radius, 1f, 32, this.CircleColor);
        }
    }
}