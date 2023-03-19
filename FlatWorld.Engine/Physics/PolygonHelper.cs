using System;
using Microsoft.Xna.Framework;

namespace FlatWorld.Engine.Physics;

public static class PolygonHelper
{
    public static float FindPolygonArea(Vector2[] vertices)
    {
        float totalArea = 0f;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 a = vertices[i];
            Vector2 b = vertices[(i + 1) % vertices.Length];

            float dy = (a.Y + b.Y) / 2f;
            float dx = b.X - a.X;

            float area = dy * dx;
            totalArea += area;
        }

        return MathF.Abs(totalArea);
    }

    public static bool IntersectCircles(FlatCircle a, FlatCircle b)
    {
        float distSquared = Vector2.DistanceSquared(a.Center, b.Center);
        float radiusSquared = a.Radius + b.Radius;
        radiusSquared *= radiusSquared;

        if (distSquared > radiusSquared)
        {
            return false;
        }

        return true;
    }

    public static bool IntersectCircles(FlatCircle a, FlatCircle b, out float depth, out Vector2 normal)
    {
        depth = 0f;
        normal = Vector2.Zero;

        Vector2 n = b.Center - a.Center;
        float distSquared = n.LengthSquared();
        float r2 = a.Radius + b.Radius;
        float radiusSquared = r2 * r2;

        if (distSquared > radiusSquared)
        {
            return false;
        }

        float dist = MathF.Sqrt(distSquared);

        if (dist != 0)
        {
            depth = r2 - dist;
            normal = n / dist;
        }
        else
        {
            depth = r2;
            normal = new Vector2(1f, 0f);
        }

        return true;
    }
}