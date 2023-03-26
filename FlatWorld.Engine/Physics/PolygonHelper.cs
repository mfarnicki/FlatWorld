using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace FlatWorld.Engine.Physics;

public static class PolygonHelper
{
    public enum WindingOrder
    {

    }

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

    public static bool Triangulate(Vector2[] vertices, out int[] triangles, out string errorMessage)
    {
        triangles = null;
        errorMessage = string.Empty;

        if (vertices is null)
        {
            errorMessage = "The vertex list is null";
            return false;
        }

        if (vertices.Length < 3)
        {
            errorMessage = "The vertex list must have at least 3 vertices";
            return false;
        }

        if (vertices.Length > 1024)
        {
            errorMessage = "The max vertex list length is 1024";
            return false;
        }

        // PolygonHelper.ComputePolygonArea(vertices, out float area, out WindingOrder windingOrder);

        List<int> indexList = new List<int>();
        indexList.AddRange(Enumerable.Range(0, vertices.Length));

        int totalTriangleCount = vertices.Length - 2;
        int totalTriangleIndexCount = totalTriangleCount * 3;
        triangles = new int[totalTriangleIndexCount];

        int triangleIndex = 0;
        while (indexList.Count > 3)
        {
            for (int i = 0; i < indexList.Count; i++)
            {
                int a = indexList[i];
                int b = FlatUtils.GetItem(indexList, i - 1);
                int c = FlatUtils.GetItem(indexList, i + 1);

                Vector2 va = vertices[a];
                Vector2 vb = vertices[b];
                Vector2 vc = vertices[c];

                Vector2 va_to_vb = vb - va;
                Vector2 va_to_vc = vc - va;

                if (FlatUtils.Cross(va_to_vb, va_to_vc) < 0f)
                {
                    continue;
                }

                bool isEar = true;
                for (int j = 0; j < vertices.Length; j++)
                {
                    if (j == a || j == b || j == c)
                    {
                        continue;
                    }

                    Vector2 p = vertices[j];
                    if (PolygonHelper.IsPointInTriangle(p, vb, va, vc))
                    {
                        isEar = false;
                        break;
                    }
                }

                if (isEar)
                {
                    triangles[triangleIndex++] = b;
                    triangles[triangleIndex++] = a;
                    triangles[triangleIndex++] = c;

                    indexList.RemoveAt(i);
                    break;
                }
            }
        }

        triangles[triangleIndex++] = indexList[0];
        triangles[triangleIndex++] = indexList[1];
        triangles[triangleIndex++] = indexList[2];

        return true;
    }

    public static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        Vector2 ab = b - a;
        Vector2 bc = c - b;
        Vector2 ca = a - c;

        Vector2 ap = p - a;
        Vector2 bp = p - b;
        Vector2 cp = p - c;

        float cross1 = FlatUtils.Cross(ab, ap);
        float cross2 = FlatUtils.Cross(bc, bp);
        float cross3 = FlatUtils.Cross(ca, cp);

        if (cross1 > 0f || cross2 > 0f || cross3 > 0f)
        {
            return false;
        }

        return true;
    }

    public static bool IsSimplePolygon(Vector2[] vertices)
    {
        throw new NotImplementedException();
    }

    public static bool ContainsColinearEdges(Vector2[] vertices)
    {
        throw new NotImplementedException();
    }

    public static void ComputePolygonArea(Vector2[] vertices, out float area, out WindingOrder windingOrder)
    {
        throw new NotImplementedException();
    }
}