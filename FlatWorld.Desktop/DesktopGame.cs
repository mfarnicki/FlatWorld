using System;
using FlatWorld.Engine.Graphics;
using FlatWorld.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlatWorld.Desktop;

public class DesktopGame : Game
{
    private const int ScreenWidth = 1280;
    private const int ScreenHeight = 720;

    private GraphicsDeviceManager graphics;
    private FlatSprites sprites;
    private FlatScreen screen;
    private FlatShapes shapes;
    private Texture2D texture;

    private int x = 32;

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

        this.sprites = new FlatSprites(this);
        this.screen = new FlatScreen(this, DesktopGame.ScreenWidth, DesktopGame.ScreenHeight);
        this.shapes = new FlatShapes(this);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        this.texture = this.Content.Load<Texture2D>("face");
    }

    protected override void Update(GameTime gameTime)
    {
        FlatKeyboard keyboard = FlatKeyboard.Instance;
        keyboard.Update();

        if (keyboard.IsKeyClicked(Keys.Escape))
        {
            this.Exit();
        }

        if (keyboard.IsKeyClicked(Keys.Right))
        {
            x += 32;
            Console.WriteLine("Right key clicked.");
        }

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.screen.Set();
        Viewport vp = this.GraphicsDevice.Viewport;
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // this.sprites.Begin();
        // this.sprites.Draw(this.texture, null, new Rectangle(x, 32, 512, 256), Color.White);
        // this.sprites.End();

        this.shapes.Begin();
        this.shapes.DrawRectangle(100 + x, 100, 64, 64, Color.Red);
        this.shapes.DrawRectangle(200 - x, 200, 64, 64, Color.Blue);
        this.shapes.DrawRectangle(300 + x, 300, 64 + 2 * x, 64, Color.Green);
        this.shapes.End();

        this.screen.UnSet();
        this.screen.Present(this.sprites);

        base.Draw(gameTime);
    }
}
