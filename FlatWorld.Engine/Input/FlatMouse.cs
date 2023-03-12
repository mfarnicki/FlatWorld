using System;
using FlatWorld.Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FlatWorld.Engine.Input;

public sealed class FlatMouse
{
    private static readonly Lazy<FlatMouse> Lazy = new Lazy<FlatMouse>(() => new FlatMouse());

    public static FlatMouse Instance => Lazy.Value;

    private MouseState prevMouseState;
    private MouseState currMouseState;

    public Point WindowPosition => this.currMouseState.Position;

    private FlatMouse()
    {
        this.prevMouseState = Mouse.GetState();
        this.currMouseState = this.prevMouseState;
    }

    public void Update()
    {
        this.prevMouseState = this.currMouseState;
        this.currMouseState = Mouse.GetState();
    }

    public bool IsLeftButtonDown()
    {
        return this.currMouseState.LeftButton == ButtonState.Pressed;
    }

    public bool IsRightButtonDown()
    {
        return this.currMouseState.RightButton == ButtonState.Pressed;
    }

    public bool IsMiddleButtonDown()
    {
        return this.currMouseState.MiddleButton == ButtonState.Pressed;
    }

    public bool IsLeftButtonClicked()
    {
        return this.currMouseState.LeftButton == ButtonState.Pressed &&
               this.prevMouseState.LeftButton == ButtonState.Released;
    }

    public bool IsRightButtonClicked()
    {
        return this.currMouseState.RightButton == ButtonState.Pressed &&
               this.prevMouseState.RightButton == ButtonState.Released;
    }

    public bool IsMiddleButtonClicked()
    {
        return this.currMouseState.MiddleButton == ButtonState.Pressed &&
               this.prevMouseState.MiddleButton == ButtonState.Released;
    }

    public Vector2 GetScreenPosition(FlatScreen screen)
    {
        Rectangle screenDestination = screen.CalculateDestinationRectangle();
        Point windowsPosition = this.WindowPosition;

        float sx = windowsPosition.X - screenDestination.X;
        float sy = windowsPosition.Y - screenDestination.Y;

        sx /= (float)screenDestination.Width;
        sy /= (float)screenDestination.Height;

        sx *= screen.Width;
        sy *= screen.Height;

        sy = screen.Height - sy;
        return new Vector2(sx, sy);
    }
}