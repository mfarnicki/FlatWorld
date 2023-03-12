using FlatWorld.Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlatWorld.Desktop;

public class DesktopGame : Game
{
    private const int ScreenWidth = 1280;
    private const int ScreenHeight = 720;

    private GraphicsDeviceManager graphics;
    private Sprites sprites;
    private Screen screen;
    private Texture2D texture;

    public DesktopGame()
    {
        this.graphics = new GraphicsDeviceManager(this);
        this.graphics.SynchronizeWithVerticalRetrace = true;

        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = true;
        this.IsFixedTimeStep = true;
    }

    protected override void Initialize()
    {
        this.graphics.PreferredBackBufferWidth = DesktopGame.ScreenWidth;
        this.graphics.PreferredBackBufferHeight = DesktopGame.ScreenHeight;
        this.graphics.ApplyChanges();

        this.sprites = new Sprites(this);
        this.screen = new Screen(this, DesktopGame.ScreenWidth, DesktopGame.ScreenHeight);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        this.texture = this.Content.Load<Texture2D>("face");
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            this.Exit();
        }

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.screen.Set();
        Viewport vp = this.GraphicsDevice.Viewport;
        GraphicsDevice.Clear(Color.CornflowerBlue);

        this.sprites.Begin();
        this.sprites.Draw(this.texture, null, new Rectangle(32, 32, 512, 256), Color.White);
        this.sprites.End();

        this.screen.UnSet();
        this.screen.Present(this.sprites);

        base.Draw(gameTime);
    }
}
