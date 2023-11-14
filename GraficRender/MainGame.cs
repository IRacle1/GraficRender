using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace GraficRender;

public class MainGame : Game
{
    public float Coef;
    public const float Step = 0.001f;

    public static Vector2 Offset = Vector2.Zero;

    GraphicsDeviceManager graphics = null!;
    BasicEffect effect = null!;

    Matrix projectionMatrix;
    Matrix viewMatrix;
    Matrix worldMatrix;

    VertexPositionColor[] axes = new VertexPositionColor[]
    {
        new VertexPositionColor { Color = Color.White, Position = new Vector3(-10000f, 0f, 0f) },
        new VertexPositionColor { Color = Color.White, Position = new Vector3(10000f, 0f, 0f) },
        new VertexPositionColor { Color = Color.White, Position = new Vector3(0f, -10000f, 0f) },
        new VertexPositionColor { Color = Color.White, Position = new Vector3(0f, 10000f, 0f) }
    };

    Dictionary<string, VertexPositionColor[]> loadedGrafics = new();
    public Dictionary<string, FunctionModel> Functions = null!;

    public MainGame()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 6), Vector3.Zero, Vector3.Up);

        Coef = (float)Window.ClientBounds.Width /
            (float)Window.ClientBounds.Height;

        projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
            Coef,
            1, 10);

        worldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

        graphics.ApplyChanges();

        effect = new(GraphicsDevice)
        {
            VertexColorEnabled = true,

            World = worldMatrix,
            View = viewMatrix,
            Projection = projectionMatrix
        };

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1d / 60);

        if (!Directory.Exists("Functions"))
        {
            Directory.CreateDirectory("Functions");
            using StreamWriter stream = new StreamWriter(File.Create($"Functions/parabola.cs"));
            stream.Write("""
                    public static float Parabola(float x) 
                    {
                    	return x * x;
                    }
                    """);
        }

        Functions = LoaderHelper.LoadAll();

        foreach (var item in Functions)
        {
            loadedGrafics[item.Key] = item.Value.GetVertexBuffer(0f, -5, 5, Step).ToArray();
        }

        base.Initialize();
    }

    internal void InitGrafs()
    {
        foreach (var item in Functions)
        {
            loadedGrafics[item.Key] = item.Value.GetVertexBuffer(0f, -5, 5, Step).ToArray();
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        foreach (var item in Functions)
        {
            if (item.Value.HasDynamicArgument)
            {
                loadedGrafics[item.Key] = item.Value.GetVertexBuffer((float)gameTime.TotalGameTime.TotalSeconds, -5, 5, Step).ToArray();
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, axes, 0, 2);

            foreach (var grafic in loadedGrafics)
            {
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.PointList, grafic.Value, 0, grafic.Value.Length - 1);
            }
        }
        base.Draw(gameTime);
    }
}