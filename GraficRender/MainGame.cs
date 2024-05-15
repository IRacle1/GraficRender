using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraficRender;

public class MainGame : Game
{
    public static MainGame Instance { get; private set; } = null!;
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
        new() { Color = Color.White, Position = new Vector3(-10000f, 0f, 0f) },
        new() { Color = Color.White, Position = new Vector3(10000f, 0f, 0f) },
        new() { Color = Color.White, Position = new Vector3(0f, -10000f, 0f) },
        new() { Color = Color.White, Position = new Vector3(0f, 10000f, 0f) }
    };

    Dictionary<int, VertexPositionColor[]> loadedGrafics = new();
    public Dictionary<int, FunctionModel> Functions = null!;

    public MainGame()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Instance = this;
    }

    protected override void Initialize()
    {
        Window.KeyDown += Window_KeyDown;
        viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 6), Vector3.Zero, Vector3.Up);

        Coef = (float)Window.ClientBounds.Width /
            (float)Window.ClientBounds.Height;

        projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
            Coef,
            1, 10);

        worldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

        graphics.IsFullScreen = false;
        graphics.PreferredBackBufferWidth = 1080;
        graphics.PreferredBackBufferHeight = 720;
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

        //TODO: поменять уже когда не лень будет
        Functions = LoaderHelper.LoadAll(true);
        UpdateParameters(0.0f);
        LoadGrafs(false);

        base.Initialize();
    }

    private void Window_KeyDown(object? sender, InputKeyEventArgs e)
    {
        int val = (int)e.Key - 48;
        if (val >= 1 && val <= 9)
        {
            if (Functions.ContainsKey(val))
            {
                bool hide = Functions[val].Info.Hide;
                Functions[val].Info.Hide = !hide;
                if (hide)
                {
                    loadedGrafics[val] = GetGrafVertexes(Functions[val]);
                }
                else
                {
                    loadedGrafics.Remove(val);
                }
            }

        }
    }

    private void LoadGrafs(bool checkUpdate = false)
    {
        foreach (var item in Functions)
        {
            if (item.Value.Info.Hide)
                continue;

            if (!checkUpdate || item.Value.Info.ShouldUpdate)
                loadedGrafics[item.Key] = GetGrafVertexes(item.Value);
        }
    }

    private VertexPositionColor[] GetGrafVertexes(FunctionModel model)
    {
        return model.GetVertexBuffer(-5, 5, model.Info.Step).ToArray();
    }

    private void UpdateParameters(float time)
    {
        foreach (DynamicParameter parameter in LoaderHelper.DynamicParameters)
        {
            parameter.Set(time);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
            return;
        }

        UpdateParameters((float)gameTime.TotalGameTime.TotalSeconds);

        LoadGrafs(true);

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