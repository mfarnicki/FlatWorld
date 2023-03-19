using Microsoft.Xna.Framework;

namespace FlatWorld.Engine;

public readonly struct FlatCircle
{
    public readonly Vector2 Center;
    public readonly float Radius;

    public FlatCircle(Vector2 center, float radius)
    {
        this.Center = center;
        this.Radius = radius;
    }

    public FlatCircle(float x, float y, float radius) : this(new Vector2(x, y), radius)
    { }
}