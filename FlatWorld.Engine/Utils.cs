using System;

namespace FlatWorld.Engine;

public static class Utils
{
    public static void Normalize(ref float x, ref float y)
    {
        float invLen = 1f / MathF.Sqrt(x * x + y * y);

        x *= invLen;
        y *= invLen;
    }
}