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

    private Texture2D textureTest;

    private Color[] colors;
    private float[] results;
    private float[] pixelCoords;

    private int iterations = 0;

    private Rectangle selectionRectangle;
    private Vector4 currentViewVector;
    private ColorFlags currentColor;

    public MandelbrotGame()
    {
        this.graphics = new GraphicsDeviceManager(this);
        this.graphics.SynchronizeWithVerticalRetrace = true;

        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = false;
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

        this.Reset();

        base.Initialize();
    }

    private void Reset()
    {
        this.currentColor = ColorFlags.All;
        this.currentViewVector = new Vector4(-2.25f, 1f, 1f, -1f);
        this.InitPixels(this.currentViewVector);
    }

    private void InitPixels(Vector4 viewVector)
    {
        this.currentViewVector = viewVector;
        float width = viewVector.Z - viewVector.X;
        float height = viewVector.Y - viewVector.W;

        for (int i = 0; i < this.pixelCoords.Length; i += 2)
        {
            float x = i / 2f % MandelbrotGame.ScreenWidth;
            float y = i / 2f / MandelbrotGame.ScreenWidth;

            float ax = viewVector.X + (x / MandelbrotGame.ScreenWidth) * width;
            float ay = viewVector.W + (y / MandelbrotGame.ScreenHeight) * height;

            this.pixelCoords[i] = ax;
            this.pixelCoords[i + 1] = ay;

            this.results[i] = 0;
            this.results[i + 1] = 0;

            this.colors[i / 2] = Color.Black;
        }

        iterations = 0;
    }

    private void ToggleColor(ColorFlags colorFlag)
    {
        this.currentColor ^= colorFlag;
        this.InitPixels(this.currentViewVector);
    }

    private void MoveView(int topBottom, int leftRight)
    {
        float width = this.currentViewVector.Z - this.currentViewVector.X;
        float height = this.currentViewVector.Y - this.currentViewVector.W;

        this.currentViewVector = new Vector4(
            this.currentViewVector.X + leftRight * width / 10,
            this.currentViewVector.Y + topBottom * height / 10,
            this.currentViewVector.Z + leftRight * width / 10,
            this.currentViewVector.W + topBottom * height / 10);

        this.InitPixels(this.currentViewVector);
    }

    protected override void Update(GameTime gameTime)
    {
        FlatKeyboard keyboard = FlatKeyboard.Instance;
        keyboard.Update();

        FlatMouse mouse = FlatMouse.Instance;
        mouse.Update();

        Vector2 mousePos = mouse.GetScreenPosition(this.screen);
        int oneThirdWidth = ScreenWidth / 3;
        int oneThirdHeight = ScreenHeight / 3;
        int newPosX = MathHelper.Clamp((int)(mousePos.X - oneThirdWidth / 2), 0, ScreenWidth - oneThirdWidth);
        int newPosY = MathHelper.Clamp((int)(mousePos.Y - oneThirdHeight / 2), 0, ScreenHeight - oneThirdHeight);
        this.selectionRectangle = new Rectangle(newPosX, newPosY, oneThirdWidth, oneThirdHeight);

        if (keyboard.IsKeyClicked(Keys.Escape))
        {
            this.Exit();
        }

        if (keyboard.IsKeyClicked(Keys.OemTilde))
        {
            Console.WriteLine($"Mouse window pos: {mouse.WindowPosition}");
            Console.WriteLine($"Screen position: {mouse.GetScreenPosition(this.screen)}");
        }

        if (keyboard.IsKeyClicked(Keys.R))
        {
            this.ToggleColor(ColorFlags.Red);
        }

        if (keyboard.IsKeyClicked(Keys.G))
        {
            this.ToggleColor(ColorFlags.Green);
        }

        if (keyboard.IsKeyClicked(Keys.B))
        {
            this.ToggleColor(ColorFlags.Blue);
        }

        if (keyboard.IsKeyClicked(Keys.Up))
        {
            this.MoveView(1, 0);
        }

        if (keyboard.IsKeyClicked(Keys.Down))
        {
            this.MoveView(-1, 0);
        }

        if (keyboard.IsKeyClicked(Keys.Left))
        {
            this.MoveView(0, 1);
        }

        if (keyboard.IsKeyClicked(Keys.Right))
        {
            this.MoveView(0, -1);
        }

        if (mouse.IsLeftButtonClicked())
        {
            float left = this.currentViewVector.X;
            float top = this.currentViewVector.Y;
            float right = this.currentViewVector.Z;
            float bottom = this.currentViewVector.W;

            float width = right - left;
            float height = top - bottom;

            float newLeft = left + (float)this.selectionRectangle.Left / ScreenWidth * width;
            float newRight = left + (float)this.selectionRectangle.Right / ScreenWidth * width;

            float newBottom = bottom + (float)(ScreenHeight - this.selectionRectangle.Bottom) / ScreenHeight * height;
            float newTop = bottom + (float)(ScreenHeight - this.selectionRectangle.Top) / ScreenHeight * height;

            this.InitPixels(new Vector4(newLeft, newTop, newRight, newBottom));
        }

        if (mouse.IsRightButtonClicked())
        {
            this.Reset();
        }

        int colorComponent = iterations * 8 % 255;
        Color newColor = new Color(
            this.currentColor.HasFlag(ColorFlags.Red) ? colorComponent : 255,
            this.currentColor.HasFlag(ColorFlags.Green) ? colorComponent : 255,
            this.currentColor.HasFlag(ColorFlags.Blue) ? colorComponent : 255);

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
        this.shapes.DrawRectangle(this.selectionRectangle, 1f, Color.HotPink);
        this.shapes.End();

        this.screen.UnSet();
        this.screen.Present(this.sprites);

        base.Draw(gameTime);
    }

    [Flags]
    protected enum ColorFlags
    {
        None = 0,
        Red = 1 << 0,
        Green = 1 << 1,
        Blue = 1 << 2,
        All = Red | Green | Blue
    }
}
