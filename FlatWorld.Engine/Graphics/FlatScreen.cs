using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatWorld.Engine.Graphics;

public sealed class FlatScreen : IDisposable
{
    private const int MinDim = 64;
    private const int MaxDim = 4096;

    private bool isDisposed;
    private Game game;
    private RenderTarget2D target;

    public int Width => this.target.Width;
    public int Height => this.target.Height;

    public FlatScreen(Game game, int width, int height)
    {
        this.game = game ?? throw new ArgumentNullException(nameof(game));

        width = MathHelper.Clamp(width, FlatScreen.MinDim, FlatScreen.MaxDim);
        height = MathHelper.Clamp(height, FlatScreen.MinDim, FlatScreen.MaxDim);

        this.target = new RenderTarget2D(this.game.GraphicsDevice, width, height);
    }

    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.target?.Dispose();
        this.isDisposed = true;
    }

    public void Set()
    {
        this.game.GraphicsDevice.SetRenderTarget(this.target);
    }

    public void UnSet()
    {
        this.game.GraphicsDevice.SetRenderTarget(null);
    }

    public void Present(FlatSprites sprites, bool textureFiltering = true)
    {
        if (sprites is null)
        {
            throw new ArgumentNullException(nameof(sprites));
        }

#if DEBUG
        this.game.GraphicsDevice.Clear(Color.HotPink);
#else
        this.game.GraphicsDevice.Clear(Color.Black);
#endif

        Rectangle destinationRectangle = this.CalculateDestinationRectangle();

        sprites.Begin(textureFiltering);
        sprites.Draw(this.target, null, destinationRectangle, Color.White);
        sprites.End();
    }

    internal Rectangle CalculateDestinationRectangle()
    {
        Rectangle backBufferBounds = this.game.GraphicsDevice.PresentationParameters.Bounds;
        float backBufferAspectRatio = (float)backBufferBounds.Width / backBufferBounds.Height;
        float screenAspectRatio = (float)this.Width / this.Height;

        float rx = 0;
        float ry = 0;
        float rw = backBufferBounds.Width;
        float rh = backBufferBounds.Height;

        if (backBufferAspectRatio > screenAspectRatio)
        {
            rw = rh * screenAspectRatio;
            rx = ((float)backBufferBounds.Width - rw) / 2f;
        }
        else if (backBufferAspectRatio < screenAspectRatio)
        {
            rh = rw / screenAspectRatio;
            ry = ((float)backBufferBounds.Height - rh) / 2f;
        }

        return new Rectangle((int)rx, (int)ry, (int)rw, (int)rh);
    }
}