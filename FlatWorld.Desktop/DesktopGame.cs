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
    // private const int ScreenWidth = 400;
    // private const int ScreenHeight = 300;

    private GraphicsDeviceManager graphics;
    private FlatSprites sprites;
    private FlatScreen screen;
    private FlatShapes shapes;
    private Texture2D texture;

    private Texture2D textureTest;

    private Color[] colors;
    private float[] results;
    private float[] pixelCoords;

    private int iterations = 0;

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

        this.textureTest = new Texture2D(this.GraphicsDevice, DesktopGame.ScreenWidth, DesktopGame.ScreenHeight);

        this.colors = new Color[DesktopGame.ScreenWidth * DesktopGame.ScreenHeight];

        this.results = new float[DesktopGame.ScreenWidth * DesktopGame.ScreenHeight * 2];
        this.pixelCoords = new float[DesktopGame.ScreenWidth * DesktopGame.ScreenHeight * 2];

        float halfWidth = ScreenWidth / 2f;
        float halfHeight = ScreenHeight / 2f;
        float quarterWidth = ScreenWidth / 4f;

        for (int i = 0; i < this.pixelCoords.Length; i += 2)
        {
            int x = i / 2 % ScreenWidth;
            int y = i / 2 / ScreenWidth;

            float ax = (x - halfWidth) / quarterWidth - 1;
            float ay = (y - halfHeight) / halfHeight;

            this.pixelCoords[i] = ax;
            this.pixelCoords[i + 1] = ay;

            this.results[i] = 0;
            this.results[i + 1] = 0;

            this.colors[i / 2] = Color.Black;
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
            Console.WriteLine($"Mouse window pos: {mouse.WindowPosition}");
            Console.WriteLine($"Screen position: {mouse.GetScreenPosition(this.screen)}");
        }

        Color newColor = new Color((iterations * 8) % 255, (iterations * 6) % 255, 255);

        for (int i = 0; i < this.pixelCoords.Length; i += 2)
        {
            float x = this.results[i];
            float y = this.results[i + 1];

            float mul = x * x + y * y;
            if (mul > 4)
            {
                if (this.colors[i / 2] == Color.Black)
                {
                    this.colors[i / 2] = newColor;
                }

                continue;
            }

            float x0 = this.pixelCoords[i];
            float y0 = this.pixelCoords[i + 1];

            float newX = x * x - y * y + x0;
            float newY = 2 * x * y + y0;

            this.results[i] = newX;
            this.results[i + 1] = newY;
        }

        iterations++;

        this.textureTest.SetData<Color>(this.colors);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.screen.Set();

        this.sprites.Begin();
        // this.sprites.Draw(this.texture, null, new Rectangle(32, 32, 512, 256), Color.White);
        this.sprites.Draw(this.textureTest, Vector2.Zero, Vector2.Zero, Color.White);
        this.sprites.End();

        this.screen.UnSet();
        this.screen.Present(this.sprites);

        base.Draw(gameTime);
    }
}
