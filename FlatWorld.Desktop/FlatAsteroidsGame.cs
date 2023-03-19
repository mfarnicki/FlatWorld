using FlatWorld.Desktop.Entities;
using FlatWorld.Engine;
using FlatWorld.Engine.Graphics;
using FlatWorld.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlatWorld.Desktop;

public class FlatAsteroidsGame : Game
{
    private const int ScreenWidth = 1280;
    private const int ScreenHeight = 720;

    private GraphicsDeviceManager graphics;
    private FlatScreen screen;
    private FlatSprites sprites;
    private FlatShapes shapes;
    private FlatCamera camera;

    private MainShip player;

    public FlatAsteroidsGame()
    {
        this.graphics = new GraphicsDeviceManager(this);
        this.graphics.SynchronizeWithVerticalRetrace = true;

        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = true;
        this.IsFixedTimeStep = true;
    }

    protected override void Initialize()
    {
        DisplayMode dm = this.GraphicsDevice.DisplayMode;
        this.graphics.PreferredBackBufferWidth = (int)(dm.Width * 0.8f);
        this.graphics.PreferredBackBufferHeight = (int)(dm.Height * 0.8f);
        this.graphics.ApplyChanges();

        this.screen = new FlatScreen(this, ScreenWidth, ScreenHeight);
        this.sprites = new FlatSprites(this);
        this.shapes = new FlatShapes(this);
        this.camera = new FlatCamera(this.screen);

        Vector2[] vertices = new Vector2[5];
        vertices[0] = new Vector2(10, 0);
        vertices[1] = new Vector2(-10, -10);
        vertices[2] = new Vector2(-5, -3);
        vertices[3] = new Vector2(-5, 3);
        vertices[4] = new Vector2(-10, 10);

        this.player = new MainShip(vertices, Vector2.Zero, Color.LightGreen);

        base.Initialize();
    }

    protected override void LoadContent()
    {
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = FlatKeyboard.Instance;
        keyboard.Update();

        var mouse = FlatMouse.Instance;
        mouse.Update();

        if (keyboard.IsKeyClicked(Keys.A))
        {
            this.camera.IncZoom();
        }

        if (keyboard.IsKeyClicked(Keys.Z))
        {
            this.camera.DecZoom();
        }

        float playerRotationAmount = MathHelper.Pi * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (keyboard.IsKeyDown(Keys.Left))
        {
            this.player.Rotate(playerRotationAmount);
        }

        if (keyboard.IsKeyDown(Keys.Right))
        {
            this.player.Rotate(-playerRotationAmount);
        }

        if (keyboard.IsKeyDown(Keys.Up))
        {
            this.player.ApplyForce(50f * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        this.player.Update(gameTime, this.camera);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.screen.Set();
        this.GraphicsDevice.Clear(Color.Black);

        this.shapes.Begin(this.camera);
        this.player.Draw(this.shapes);
        this.shapes.End();
        this.screen.UnSet();
        this.screen.Present(this.sprites);

        base.Draw(gameTime);
    }
}
