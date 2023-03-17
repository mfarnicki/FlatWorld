using System;
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

        Random rand = new Random();

        int vertexCount = 5;
        this.vertices = new Vector2[vertexCount];

        for (int i = 0; i < this.vertices.Length; i++)
        {
            float x = rand.Next() % this.screen.Width - this.screen.Width / 2;
            float y = rand.Next() % this.screen.Height - this.screen.Height / 2;

            this.vertices[i] = new Vector2(x, y);
        }

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

        if (keyboard.IsKeyClicked(Keys.S))
        {
            Random rand = new Random();
            for (int i = 0; i < this.vertices.Length; i++)
            {
                float x = rand.Next() % this.screen.Width - this.screen.Width / 2;
                float y = rand.Next() % this.screen.Height - this.screen.Height / 2;

                this.vertices[i] = new Vector2(x, y);
            }
        }

        if (keyboard.IsKeyClicked(Keys.A))
        {
            this.camera.IncZoom();
        }

        if (keyboard.IsKeyClicked(Keys.Z))
        {
            this.camera.DecZoom();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.screen.Set();
        this.GraphicsDevice.Clear(Color.Black);

        this.sprites.Begin(this.camera, false);
        // this.sprites.Draw(texture, null, new Vector2(8, 8), Vector2.Zero, MathHelper.TwoPi / 100f, new Vector2(2f, 2f), Color.White);
        this.sprites.End();

        this.shapes.Begin(this.camera);
        // this.shapes.DrawRectangleFill(32, 0, 24, 77, Color.DarkOliveGreen);
        // this.shapes.DrawLine(new Vector2(-24, 0), new Vector2(15, 35), 3f, Color.DarkGoldenrod);

        // this.shapes.DrawRectangle(-64, 0, 64, 64, 1f, Color.White);
        // this.shapes.DrawCircle(0, 32, 32, 1f, 48, Color.White);
        this.shapes.DrawPolygon(this.vertices, 1f, Color.White);
        this.shapes.End();

        this.screen.UnSet();
        this.screen.Present(this.sprites);

        base.Draw(gameTime);
    }
}
