using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlatWorld.Engine.Graphics;

public sealed class Sprites : IDisposable
{
    private bool isDisposed;
    private Game game;
    private SpriteBatch sprites;
    private BasicEffect effect;

    public Sprites(Game game)
    {
        this.game = game ?? throw new ArgumentNullException(nameof(game));
        this.sprites = new SpriteBatch(this.game.GraphicsDevice);

        this.effect = new BasicEffect(this.game.GraphicsDevice);
        this.effect.FogEnabled = false;
        this.effect.TextureEnabled = true;
        this.effect.LightingEnabled = false;
        this.effect.VertexColorEnabled = true;
        this.effect.World = Matrix.Identity;
        this.effect.Projection = Matrix.Identity;
        this.effect.View = Matrix.Identity;

        this.isDisposed = false;
    }

    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.sprites?.Dispose();
        this.effect?.Dispose();
        this.isDisposed = true;
    }

    public void Begin(bool textureFiltering = false)
    {
        SamplerState sampler = textureFiltering ? SamplerState.LinearClamp : SamplerState.PointClamp;
        Viewport vp = this.game.GraphicsDevice.Viewport;
        this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0f, 1f);

        this.sprites.Begin(blendState: BlendState.AlphaBlend, samplerState: sampler, rasterizerState: RasterizerState.CullNone, effect: this.effect);
    }

    public void End()
    {
        this.sprites.End();
    }

    public void Draw(Texture2D texture, Vector2 origin, Vector2 position, Color color)
    {
        this.sprites.Draw(texture, position, null, color, 0f, origin, 1f, SpriteEffects.FlipVertically, 0f);
    }

    public void Draw(Texture2D texture, Rectangle? sourceRectangle, Vector2 origin, Vector2 position, float rotation, Vector2 scale, Color color)
    {
        this.sprites.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, SpriteEffects.FlipVertically, 0f);
    }

    public void Draw(Texture2D texture, Rectangle? sourceRectangle, Rectangle destinationRectangle, Color color)
    {
        this.sprites.Draw(texture, destinationRectangle, sourceRectangle, color, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
    }
}