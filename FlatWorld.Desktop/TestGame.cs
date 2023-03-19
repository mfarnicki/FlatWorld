using System;
using FlatWorld.Engine;
using FlatWorld.Engine.Graphics;
using FlatWorld.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlatWorld.Desktop;

public class TestGame : Game
{
    private const int ScreenWidth = 1280;
    private const int ScreenHeight = 720;

    private GraphicsDeviceManager graphics;
    private FlatSprites sprites;
    private FlatScreen screen;
    private FlatShapes shapes;
    private FlatCamera camera;

    private Texture2D texture;

    private Vector2[] vertices;
    private int[] triangleIndices;
    private float angle = 0f;

    public TestGame()
    {
        this.graphics = new GraphicsDeviceManager(this);
        this.graphics.SynchronizeWithVerticalRetrace = true;

        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = true;
        this.IsFixedTimeStep = true;
    }

    protected override void Initialize()
    {
        this.graphics.PreferredBackBufferWidth = TestGame.ScreenWidth;
        this.graphics.PreferredBackBufferHeight = TestGame.ScreenHeight;
        this.graphics.ApplyChanges();

        this.sprites = new FlatSprites(this);
        this.screen = new FlatScreen(this, TestGame.ScreenWidth, TestGame.ScreenHeight);
        this.shapes = new FlatShapes(this);
        this.camera = new FlatCamera(this.screen);

        this.vertices = new Vector2[5];
        this.vertices[0] = new Vector2(0, 10);
        this.vertices[1] = new Vector2(10, -10);
        this.vertices[2] = new Vector2(3, -6);
        this.vertices[3] = new Vector2(-3, -6);
        this.vertices[4] = new Vector2(-10, -10);

        int triangleCount = this.vertices.Length - 2;

        this.triangleIndices = new int[triangleCount * 3];
        this.triangleIndices[0] = 0;
        this.triangleIndices[1] = 1;
        this.triangleIndices[2] = 2;
        this.triangleIndices[3] = 0;
        this.triangleIndices[4] = 2;
        this.triangleIndices[5] = 3;
        this.triangleIndices[6] = 0;
        this.triangleIndices[7] = 3;
        this.triangleIndices[8] = 4;

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
            this.camera.GetExtents(out Vector2 min, out Vector2 max);
            Console.WriteLine("CamMin: " + min);
            Console.WriteLine("CamMax: " + max);
        }

        if (keyboard.IsKeyClicked(Keys.A))
        {
            this.camera.IncZoom();
        }

        if (keyboard.IsKeyClicked(Keys.Z))
        {
            this.camera.DecZoom();
        }

        if (keyboard.IsKeyClicked(Keys.F))
        {
            FlatUtils.ToggleFullScreen(this.graphics);
        }

        this.angle += MathHelper.PiOver2 * (float)gameTime.ElapsedGameTime.TotalSeconds;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.screen.Set();
        this.GraphicsDevice.Clear(Color.Black);

        this.sprites.Begin(this.camera, false);
        // this.sprites.Draw(texture, null, new Vector2(8, 8), Vector2.Zero, MathHelper.TwoPi / 100f, new Vector2(2f, 2f), Color.White);
        this.sprites.End();

        // Matrix transform = Matrix.CreateScale(1f) * Matrix.CreateRotationZ(MathHelper.TwoPi / 10f) * Matrix.CreateTranslation(0f, 100f, 0f);
        FlatTransform transform = new FlatTransform(new Vector2(0f, 100f), this.angle, 2f);

        this.shapes.Begin(this.camera);
        this.shapes.DrawPolygonFill(this.vertices, triangleIndices, transform, Color.LightGreen);
        this.shapes.DrawCircleFill(-32, -32, 64, 48, Color.White);
        this.shapes.End();

        this.screen.UnSet();
        this.screen.Present(this.sprites);

        base.Draw(gameTime);
    }
}
