﻿
namespace FlatWorld.Desktop;

public class Program
{
    public static void Main(string[] args)
    {
        using (var game = new FlatWorld.Desktop.MandelbrotGame())
        {
            game.Run();
        }
    }
}