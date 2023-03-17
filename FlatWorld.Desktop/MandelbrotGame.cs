using System;
using FlatWorld.Engine.Graphics;
using FlatWorld.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlatWorld.Desktop;

public class MandelbrotGame : Game
{
    private const int ScreenWidth = 1280;
    private const int ScreenHeight = 720;

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

    private Rectangle selectionRectangle;
    private Vector4 currentRectangle;

    public MandelbrotGame()
    {
        this.graphics = new GraphicsDeviceManager(this);
        this.graphics.SynchronizeWithVerticalRetrace = true;

        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = true;
        this.IsFixedTimeStep = true;
    }

    protected override void Initialize()
    {
        this.graphics.PreferredBackBufferWidth = MandelbrotGame.ScreenWidth;
        this.graphics.PreferredBackBufferHeight = MandelbrotGame.ScreenHeight;
        this.graphics.ApplyChanges();

        this.sprites = new FlatSprites(this);
        this.screen = new FlatScreen(this, MandelbrotGame.ScreenWidth, MandelbrotGame.ScreenHeight);
        this.shapes = new FlatShapes(this);

        this.textureTest = new Texture2D(this.GraphicsDevice, MandelbrotGame.ScreenWidth, MandelbrotGame.ScreenHeight);

        this.colors = new Color[MandelbrotGame.ScreenWidth * MandelbrotGame.ScreenHeight];

        this.results = new float[MandelbrotGame.ScreenWidth * MandelbrotGame.ScreenHeight * 2];
        this.pixelCoords = new float[MandelbrotGame.ScreenWidth * MandelbrotGame.ScreenHeight * 2];

        this.selectionRectangle = Rectangle.Empty;

        this.InitPixels(-2.5f, 1f, 0.5f, -1f);

        base.Initialize();
    }

    private void InitPixels(float left, float top, float right, float bottom)
    {
        this.currentRectangle = new Vector4(left, top, right, bottom);
        float width = right - left;
        float height = top - bottom;

        for (int i = 0; i < this.pixelCoords.Length; i += 2)
        {
            float x = i / 2f % MandelbrotGame.ScreenWidth;
            float y = i / 2f / MandelbrotGame.ScreenWidth;

            float ax = left + (x / MandelbrotGame.ScreenWidth) * width;
            float ay = bottom + (y / MandelbrotGame.ScreenHeight) * height;

            this.pixelCoords[i] = ax;
            this.pixelCoords[i + 1] = ay;

            this.results[i] = 0;
            this.results[i + 1] = 0;

            this.colors[i / 2] = Color.Black;
        }

        iterations = 0;
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

        Vector2 mousePos = mouse.GetScreenPosition(this.screen);
        int oneSixthWidth = ScreenWidth / 6;
        int oneSixthHeight = ScreenHeight / 6;
        int newPosX = MathHelper.Clamp((int)mousePos.X, ScreenWidth / 6, ScreenWidth - ScreenWidth / 6);
        int newPosY = MathHelper.Clamp((int)mousePos.Y, ScreenHeight / 6, ScreenHeight - ScreenHeight / 6);
        this.selectionRectangle = new Rectangle(newPosX - oneSixthWidth, newPosY - oneSixthHeight, newPosX + oneSixthWidth, newPosY + oneSixthHeight);

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
            float left = this.currentRectangle.X;
            float top = this.currentRectangle.Y;
            float right = this.currentRectangle.Z;
            float bottom = this.currentRectangle.W;

            float width = right - left;
            float height = top - bottom;

            float newLeft = left + (float)this.selectionRectangle.Left / ScreenWidth * width;
            float newRight = left + (float)this.selectionRectangle.Width / ScreenWidth * width;
            float newTop = bottom + (float)(480 - this.selectionRectangle.Top) / ScreenHeight * height;
            float newBottom = bottom + (float)this.selectionRectangle.Height / ScreenHeight * height;

            this.InitPixels(newLeft, newTop, newRight, newBottom);
        }

        if (mouse.IsRightButtonClicked())
        {
            this.InitPixels(-2.5f, 1f, 0.5f, -1f);
        }

        Color newColor = new Color((iterations * 8) % 255, (iterations * 8) % 255, 255);

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

        this.sprites.Begin(null);
        // this.sprites.Draw(this.texture, null, new Rectangle(32, 32, 512, 256), Color.White);
        this.sprites.Draw(this.textureTest, Vector2.Zero, Vector2.Zero, Color.White);
        this.sprites.End();

        this.shapes.Begin(null);
        this.shapes.DrawRectangle(this.selectionRectangle.X, this.selectionRectangle.Y, this.selectionRectangle.Width, this.selectionRectangle.Height, 1f, Color.DarkRed);
        this.shapes.End();

        this.screen.UnSet();
        this.screen.Present(this.sprites);

        base.Draw(gameTime);
    }
}
