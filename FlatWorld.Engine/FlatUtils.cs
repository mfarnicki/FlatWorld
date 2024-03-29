using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FlatWorld.Engine;

public static class FlatUtils
{
    public static void Normalize(ref float x, ref float y)
    {
        float invLen = 1f / MathF.Sqrt(x * x + y * y);

        x *= invLen;
        y *= invLen;
    }

    public static void ToggleFullScreen(GraphicsDeviceManager graphics)
    {
        graphics.HardwareModeSwitch = false;
        graphics.ToggleFullScreen();
    }

    public static Vector2 Transform(Vector2 position, FlatTransform transform)
    {
        float rx = position.X * transform.CosScaleX - position.Y * transform.SinScaleY + transform.PosX;
        float ry = position.X * transform.SinScaleX + position.Y * transform.CosScaleY + transform.PosY;

        return new Vector2(
            position.X * transform.CosScaleX - position.Y * transform.SinScaleY + transform.PosX,
            position.X * transform.SinScaleX + position.Y * transform.CosScaleY + transform.PosY);
    }

    public static float RandomSingle(Random rand, float min, float max)
    {
        if (min > max)
        {
            throw new ArgumentOutOfRangeException(nameof(min));
        }

        return min + rand.NextSingle() * (max - min);
    }

    public static Vector2 RandomDirection(Random rand)
    {
        float angle = FlatUtils.RandomSingle(rand, 0f, MathHelper.TwoPi);
        return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
    }

    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    public static T GetItem<T>(T[] array, int index)
    {
        if (index >= array.Length)
        {
            return array[index % array.Length];
        }

        if (index < 0)
        {
            return array[index % array.Length + array.Length];
        }

        return array[index];
    }
    public static T GetItem<T>(List<T> list, int index)
    {
        if (index >= list.Count)
        {
            return list[index % list.Count];
        }

        if (index < 0)
        {
            return list[index % list.Count + list.Count];
        }

        return list[index];
    }
}