using System;
using Microsoft.Xna.Framework;

namespace FlatWorld.Engine;

public struct FlatTransform
{
    public float PosX;
    public float PosY;
    public float SinScaleX;
    public float CosScaleX;
    public float SinScaleY;
    public float CosScaleY;

    public FlatTransform(Vector2 position, float angle, Vector2 scale)
    {
        float sin = MathF.Sin(angle);
        float cos = MathF.Cos(angle);

        this.PosX = position.X;
        this.PosY = position.Y;
        this.SinScaleX = sin * scale.X;
        this.CosScaleX = cos * scale.X;
        this.SinScaleY = sin * scale.Y;
        this.CosScaleY = cos * scale.Y;
    }

    public FlatTransform(Vector2 position, float angle, float scale)
    {
        float sin = MathF.Sin(angle);
        float cos = MathF.Cos(angle);

        this.PosX = position.X;
        this.PosY = position.Y;
        this.SinScaleX = sin * scale;
        this.CosScaleX = cos * scale;
        this.SinScaleY = this.SinScaleX;
        this.CosScaleY = this.CosScaleX;
    }

    public Matrix ToMatrix()
    {
        Matrix results = Matrix.Identity;

        results.M11 = this.CosScaleX;
        results.M12 = this.SinScaleY;
        results.M21 = -this.SinScaleX;
        results.M22 = this.CosScaleY;

        results.M41 = this.PosX;
        results.M42 = this.PosY;

        return results;
    }
}