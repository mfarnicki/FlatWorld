using System;
using Microsoft.Xna.Framework;

namespace FlatWorld.Engine.Graphics;

public sealed class FlatCamera
{
    public const float MinZ = 1f;
    public const float MaxZ = 2048f;

    public const int MinZoom = 1;
    public const int MaxZoom = 20;

    private float aspectRatio;
    private float fieldOfView;

    public Vector2 Position { get; private set; }

    public float Z { get; private set; }

    public float BaseZ { get; private set; }

    public Matrix View { get; private set; }

    public Matrix Projection { get; private set; }

    private int zoom;

    public FlatCamera(FlatScreen screen)
    {
        if (screen is null)
        {
            throw new ArgumentNullException(nameof(screen));
        }

        this.aspectRatio = (float)screen.Width / screen.Height;
        this.fieldOfView = MathHelper.PiOver2;

        this.Position = Vector2.Zero;
        this.BaseZ = this.GetZFromHeight(screen.Height);
        this.Z = this.BaseZ;

        this.UpdateMatrices();

        this.zoom = 1;
    }

    public void UpdateMatrices()
    {
        this.View = Matrix.CreateLookAt(new Vector3(0, 0, this.Z), Vector3.Zero, Vector3.Up);
        this.Projection = Matrix.CreatePerspectiveFieldOfView(this.fieldOfView, this.aspectRatio, FlatCamera.MinZ, FlatCamera.MaxZ);
    }

    public float GetZFromHeight(float height)
    {
        return (0.5f * height) / MathF.Tan(0.5f * this.fieldOfView);
    }

    public float GetHeightFromZ()
    {
        return this.Z * MathF.Tan(0.5f * this.fieldOfView) * 2f;
    }

    public void MoveZ(float amount)
    {
        this.Z += amount;
        this.Z = MathHelper.Clamp(this.Z, FlatCamera.MinZ, FlatCamera.MaxZ);
    }

    public void ResetZ()
    {
        this.Z = this.BaseZ;
    }

    public void Move(Vector2 amount)
    {
        this.Position += amount;
    }

    public void MoveTo(Vector2 position)
    {
        this.Position = position;
    }

    public void IncZoom()
    {
        this.zoom++;
        this.zoom = MathHelper.Clamp(this.zoom, FlatCamera.MinZoom, FlatCamera.MaxZoom);
        this.Z = this.BaseZ / this.zoom;
    }

    public void DecZoom()
    {
        this.zoom--;
        this.zoom = MathHelper.Clamp(this.zoom, FlatCamera.MinZoom, FlatCamera.MaxZoom);
        this.Z = this.BaseZ / this.zoom;
    }

    public void SetZoom(int amount)
    {
        this.zoom = amount;
        this.zoom = MathHelper.Clamp(this.zoom, FlatCamera.MinZoom, FlatCamera.MaxZoom);
        this.Z = this.BaseZ / this.zoom;
    }

    public void GetExtents(out float width, out float height)
    {
        height = this.GetHeightFromZ();
        width = height * this.aspectRatio;
    }

    public void GetExtents(out float left, out float right, out float bottom, out float top)
    {
        this.GetExtents(out float width, out float height);

        left = this.Position.X - width * 0.5f;
        right = left + width;
        bottom = this.Position.Y - height * 0.5f;
        top = bottom + height;
    }

    public void GetExtents(out Vector2 min, out Vector2 max)
    {
        this.GetExtents(out float left, out float right, out float bottom, out float top);
        min = new Vector2(left, bottom);
        max = new Vector2(right, top);
    }
}