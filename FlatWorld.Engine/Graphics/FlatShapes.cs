using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatWorld.Engine.Graphics;

public sealed class FlatShapes : IDisposable
{
    private const float MinLineThickness = 1f;
    private const float MaxLineThickness = 10f;

    private Game game;
    private BasicEffect effect;
    private VertexPositionColor[] vertices;
    private int[] indices;
    private int vertexCount;
    private int indexCount;
    private int shapeCount;

    private bool isStarted;
    private bool isDisposed;

    public FlatShapes(Game game)
    {
        this.game = game ?? throw new ArgumentNullException(nameof(game));

        this.effect = new BasicEffect(this.game.GraphicsDevice);
        this.effect.TextureEnabled = false;
        this.effect.FogEnabled = false;
        this.effect.LightingEnabled = false;
        this.effect.VertexColorEnabled = true;
        this.effect.World = Matrix.Identity;
        this.effect.View = Matrix.Identity;
        this.effect.Projection = Matrix.Identity;

        const int MaxVertexCount = 1024;
        const int MaxIndexCount = MaxVertexCount * 3;
        this.vertices = new VertexPositionColor[MaxVertexCount];
        this.indices = new int[MaxIndexCount];

        this.shapeCount = 0;
        this.vertexCount = 0;
        this.indexCount = 0;

        this.isStarted = false;
        this.isDisposed = false;
    }

    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.effect?.Dispose();
        this.isDisposed = true;
    }

    public void Begin()
    {
        if (this.isStarted)
        {
            throw new Exception("Batch already started.");
        }

        Viewport vp = this.game.GraphicsDevice.Viewport;
        this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0f, 1f);

        this.isStarted = true;
    }

    public void End()
    {
        this.Flush();
        this.isStarted = false;
    }

    public void Flush()
    {
        if (this.shapeCount == 0)
        {
            return;
        }

        this.EnsureStarted();

        foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            this.game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleList,
                this.vertices,
                0,
                this.vertexCount,
                this.indices,
                0,
                this.indexCount / 3);
        }

        this.shapeCount = 0;
        this.vertexCount = 0;
        this.indexCount = 0;
    }

    private void EnsureStarted()
    {
        if (!this.isStarted)
        {
            throw new Exception("Batch was never started.");
        }
    }

    private void EnsureSpace(int shapeVertexCount, int shapeIndexCount)
    {
        if (shapeVertexCount > this.vertices.Length ||
            shapeIndexCount > this.indices.Length)
        {
            throw new Exception("Maximum shape vertex/index exceeded");
        }

        if (this.vertexCount + shapeVertexCount > this.vertices.Length ||
            this.indexCount + shapeIndexCount > this.indices.Length)
        {
            this.Flush();
        }
    }

    public void DrawRectangleFill(float x, float y, float width, float height, Color color)
    {
        this.EnsureStarted();
        this.EnsureSpace(4, 6);

        float left = x;
        float right = x + width;
        float bottom = y;
        float top = y + height;

        Vector2 a = new Vector2(left, top);
        Vector2 b = new Vector2(right, top);
        Vector2 c = new Vector2(right, bottom);
        Vector2 d = new Vector2(left, bottom);

        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 1 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 3 + this.vertexCount;

        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), color);

        this.shapeCount++;
    }

    public void DrawLine(float ax, float ay, float bx, float by, float thickness, Color color)
    {
        this.EnsureStarted();
        this.EnsureSpace(4, 6);

        thickness = MathHelper.Clamp(thickness, FlatShapes.MinLineThickness, FlatShapes.MaxLineThickness) + 1;
        float halfThickness = thickness / 2;

        float e1x = bx - ax;
        float e1y = by - ay;

        Utils.Normalize(ref e1x, ref e1y);
        e1x *= halfThickness;
        e1y *= halfThickness;

        float e2x = -e1x;
        float e2y = -e1y;

        float n1x = -e1y;
        float n1y = e1x;

        float n2x = -n1x;
        float n2y = -n1y;

        float q1x = ax + n1x + e2x;
        float q1y = ay + n1y + e2y;

        float q2x = bx + n1x + e1x;
        float q2y = by + n1y + e1y;

        float q3x = bx + n2x + e1x;
        float q3y = by + n2y + e1y;

        float q4x = ax + n2x + e2x;
        float q4y = ay + n2y + e2y;

        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 1 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 3 + this.vertexCount;

        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q1x, q1y, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q2x, q2y, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q3x, q3y, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q4x, q4y, 0f), color);

        this.shapeCount++;
    }

    public void DrawLineSlow(Vector2 a, Vector2 b, float thickness, Color color)
    {
        this.EnsureStarted();
        this.EnsureSpace(4, 6);

        thickness = MathHelper.Clamp(thickness, FlatShapes.MinLineThickness, FlatShapes.MaxLineThickness) + 1;
        float halfThickness = thickness / 2;

        Vector2 e1 = b - a;
        e1.Normalize();
        e1 *= halfThickness;

        Vector2 e2 = -e1;
        Vector2 n1 = new Vector2(-e1.Y, e1.X);
        Vector2 n2 = -n1;

        Vector2 q1 = a + n1 + e2;
        Vector2 q2 = b + n1 + e1;
        Vector2 q3 = b + n2 + e1;
        Vector2 q4 = a + n2 + e2;

        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 1 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 0 + this.vertexCount;
        this.indices[this.indexCount++] = 2 + this.vertexCount;
        this.indices[this.indexCount++] = 3 + this.vertexCount;

        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q1, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q2, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q3, 0f), color);
        this.vertices[this.vertexCount++] = new VertexPositionColor(new Vector3(q4, 0f), color);

        this.shapeCount++;
    }
}