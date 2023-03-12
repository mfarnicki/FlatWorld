using System;
using System.Diagnostics;
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

        FlatMouse mouse = FlatMouse.Instance;
        mouse.Update();

        if (keyboard.IsKeyClicked(Keys.Escape))
        {
            this.Exit();
        }

        if (keyboard.IsKeyClicked(Keys.OemTilde))
        {
            Console.WriteLine($"Mouse window pos: {mouse.WindowPosition}");
            Console.WriteLine($"Screen position: {mouse.GetScreenPosition(this.screen)}");
        }

        if (mouse.IsLeftButtonClicked())
        {
            Console.WriteLine("Left mouse clicked");
        }

        if (mouse.IsLeftButtonDown())
        {
            Console.WriteLine("Left mouse down");
        }

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

        this.shapes.Begin();
        this.shapes.End();

        this.screen.UnSet();
        this.screen.Present(this.sprites);

        base.Draw(gameTime);
    }
}
