using System;
using Microsoft.Xna.Framework.Input;

namespace FlatWorld.Engine.Input;

public sealed class FlatKeyboard
{
    private static readonly Lazy<FlatKeyboard> Lazy = new Lazy<FlatKeyboard>(() => new FlatKeyboard());

    public static FlatKeyboard Instance => Lazy.Value;

    private KeyboardState prevKeyboardState;
    private KeyboardState currKeyboardState;

    private FlatKeyboard()
    {
        this.prevKeyboardState = Keyboard.GetState();
        this.currKeyboardState = this.prevKeyboardState;
    }

    public void Update()
    {
        this.prevKeyboardState = this.currKeyboardState;
        this.currKeyboardState = Keyboard.GetState();
    }

    public bool IsKeyDown(Keys key)
    {
        return this.currKeyboardState.IsKeyDown(key);
    }

    public bool IsKeyClicked(Keys key)
    {
        return this.currKeyboardState.IsKeyDown(key) && !this.prevKeyboardState.IsKeyDown(key);
    }
}