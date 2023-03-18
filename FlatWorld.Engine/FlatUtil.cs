using Microsoft.Xna.Framework;

namespace FlatWorld.Engine;

public static class FlatUtil
{
    public static void ToggleFullScreen(GraphicsDeviceManager graphics)
    {
        graphics.HardwareModeSwitch = false;
        graphics.ToggleFullScreen();
    }

    public static Vector2 Transform(Vector2 position, FlatTransform transform)
    {
        float rx = position.X * transform.CosScaleX - position.Y * transform.SinScaleY + transform.PosX;
        float ry = position.X * transform.SinScaleX + position.Y * transform.CosScaleY + transform.PosY;

        return new Vector2(
            position.X * transform.CosScaleX - position.Y * transform.SinScaleY + transform.PosX,
            position.X * transform.SinScaleX + position.Y * transform.CosScaleY + transform.PosY);
    }
}