using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wings.Blueprint;
using Wings.Core;
using Wings.Core.Visuals;

namespace Wings
{
  public class WingsGame : Game
  {
    private GraphicsDeviceManager Graphics;
    private SpriteBatch SpriteBatch;

    private Matrix CameraProjection;

    private VertexBuffer VertexBuffer;

    private Model BoxModel;

    private BasicEffect BasicEffect;

    private GameEnvironment InitialEnvironment { get; set; }

    private WingsEnvironment Environment { get; set; }


    public WingsGame(GameEnvironment environment)
    {
      Graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;

      InitialEnvironment = environment;
      
      // Build stuff to show in game - should probably be located somewhere else
      WingsInitializer.Initialize(environment);
    }


    protected override void Initialize()
    {
      base.Initialize();

      foreach (var item in Environment.Entities.GetComponents<VisualComponent>())
      {
        item.Initialize(Environment);
      }

      CameraProjection = Matrix.CreatePerspectiveFieldOfView(
        fieldOfView: MathHelper.ToRadians(90),
        aspectRatio: Graphics.PreferredBackBufferWidth / Graphics.PreferredBackBufferHeight,
        nearPlaneDistance: 0.1f,
        farPlaneDistance: 10000f);

      BasicEffect = new BasicEffect(GraphicsDevice);
      BasicEffect.Alpha = 1f;
      BasicEffect.VertexColorEnabled = true;
      BasicEffect.LightingEnabled = false;
      BasicEffect.World = Matrix.CreateWorld(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1));

      var triangleVertices = new VertexPositionColor[3];
      triangleVertices[0] = new VertexPositionColor(new Vector3(
                            0, 200, 0), Color.Red);
      triangleVertices[1] = new VertexPositionColor(new Vector3(-
                            200, -200, 0), Color.Green);
      triangleVertices[2] = new VertexPositionColor(new Vector3(
                            200, -200, 0), Color.Blue);

      VertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
      VertexBuffer.SetData(triangleVertices);
    }


    protected override void LoadContent()
    {
      Environment = new WingsEnvironment(InitialEnvironment, GraphicsDevice, Content);

      SpriteBatch = new SpriteBatch(GraphicsDevice);

      foreach (var item in Environment.Entities.GetComponents<VisualComponent>())
      {
        item.LoadContent(Environment);
      }

      BoxModel = Content.Load<Model>("3D/PineTree2");
    }


    protected override void Update(GameTime gameTime)
    {
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      foreach (var system in Environment.Systems)
      {
        system.Update(Environment, gameTime.ElapsedGameTime);
      }

      base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
      var camTarget = new Vector3(0f, 0f, 0f);
      var camPosition = new Vector3(0f, 0f, -100f);

      var ab = WingsInitializer.aircraftComponent.AircraftBody;
      var viewMatrix = Matrix.CreateLookAt(ab.Position, ab.Position+ab.ForwardUnitVector, new Vector3(0, 0, 1));
      //var viewMatrix = Matrix.CreateLookAt(new Vector3(0,0,1000), new Vector3(100,0,1000), new Vector3(0,0,1));

      //BasicEffect.Projection = CameraProjection;
      //BasicEffect.View = viewMatrix;

      GraphicsDevice.Clear(Color.AliceBlue);

      for (int x=-10; x<10; ++x)
      {
        for (int y = -10; y < 10; ++y)
        {
          var world = Matrix.CreateWorld(new Vector3(x*100, y*100, 900), new Vector3(0, 0, -1), new Vector3(1, 0, 0));
          DrawModel(BoxModel, world, viewMatrix, CameraProjection);
        }
      }

      SpriteBatch.Begin(SpriteSortMode.FrontToBack);

      foreach (var item in Environment.Entities.GetComponents<VisualComponent>())
      {
        item.Draw(Environment, SpriteBatch);
      }

      SpriteBatch.End();

      base.Draw(gameTime);
    }


    private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
    {
      foreach (ModelMesh mesh in model.Meshes)
      {
        foreach (BasicEffect effect in mesh.Effects)
        {
          effect.EnableDefaultLighting();
          effect.World = world;
          effect.View = view;
          effect.Projection = projection;
        }

        mesh.Draw();
      }
    }

    //protected override void Draw(GameTime gameTime)
    //{
    //  GraphicsDevice.Clear(Color.CornflowerBlue);
    //  SpriteBatch.Begin(SpriteSortMode.FrontToBack);

    //  foreach (var item in Environment.Entities.GetComponents<VisualComponent>())
    //  {
    //    item.Draw(Environment, SpriteBatch);
    //  }

    //  SpriteBatch.End();

    //  base.Draw(gameTime);
    //}
  }
}
