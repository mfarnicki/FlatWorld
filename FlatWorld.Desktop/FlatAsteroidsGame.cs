using System;
using System.Collections.Generic;
using FlatWorld.Desktop.Entities;
using FlatWorld.Engine;
using FlatWorld.Engine.Graphics;
using FlatWorld.Engine.Input;
using FlatWorld.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    private List<Entity> entities;

    private SoundEffect rocketSound;
    private SoundEffectInstance rocketSoundInstance;

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

        Random rand = new Random();
        this.entities = new List<Entity>();

        Vector2[] vertices = new Vector2[5];
        vertices[0] = new Vector2(10, 0);
        vertices[1] = new Vector2(-10, -10);
        vertices[2] = new Vector2(-5, -3);
        vertices[3] = new Vector2(-5, 3);
        vertices[4] = new Vector2(-10, 10);

        MainShip player = new MainShip(vertices, Vector2.Zero, Color.LightGreen);
        this.entities.Add(player);

        int asteroidCount = 20;
        for (int i = 0; i < asteroidCount; i++)
        {
            Asteroid asteroid = new Asteroid(rand, this.camera);
            this.entities.Add(asteroid);
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        this.rocketSound = this.Content.Load<SoundEffect>("explosion");
        this.rocketSoundInstance = this.rocketSound.CreateInstance();
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

        MainShip player = (MainShip)this.entities[0];
        float playerRotationAmount = MathHelper.Pi * (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (keyboard.IsKeyDown(Keys.Left))
        {
            player.Rotate(playerRotationAmount);
        }

        if (keyboard.IsKeyDown(Keys.Right))
        {
            player.Rotate(-playerRotationAmount);
        }

        if (keyboard.IsKeyDown(Keys.Up))
        {
            player.ApplyRocketForce(50f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            if (this.rocketSoundInstance.State != SoundState.Playing)
            {
                this.rocketSoundInstance.Volume = 0.2f;
                this.rocketSoundInstance.Play();
            }
        }
        else
        {
            player.DisableRocketForce();
            if (this.rocketSoundInstance.State == SoundState.Playing)
            {
                this.rocketSoundInstance.Stop();
            }
        }

        this.entities.ForEach(e => e.Update(gameTime, this.camera));

        for (int i = 0; i < this.entities.Count - 1; i++)
        {
            Entity a = this.entities[i];
            FlatCircle ca = new FlatCircle(a.Position, a.CollisionCircleRadius);

            for (int j = i + 1; j < this.entities.Count; j++)
            {
                Entity b = this.entities[j];
                FlatCircle cb = new FlatCircle(b.Position, b.CollisionCircleRadius);

                if (PolygonHelper.IntersectCircles(ca, cb))
                {
                    a.CircleColor = Color.Red;
                    b.CircleColor = Color.Red;
                }
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.screen.Set();
        this.GraphicsDevice.Clear(Color.Black);

        this.shapes.Begin(this.camera);

        this.entities.ForEach(e => e.Draw(this.shapes));

        this.shapes.End();
        this.screen.UnSet();
        this.screen.Present(this.sprites);

        base.Draw(gameTime);
    }
}
