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

      foreach (var item in Environment.Entities.GetComponents<Visual3DComponent>())
      {
        item.Initialize(Environment);
      }
    }


    protected override void LoadContent()
    {
      Environment = new WingsEnvironment(InitialEnvironment, GraphicsDevice, Content);

      SpriteBatch = new SpriteBatch(GraphicsDevice);

      foreach (var item in Environment.Entities.GetComponents<VisualComponent>())
      {
        item.LoadContent(Environment);
      }

      foreach (var item in Environment.Entities.GetComponents<Visual3DComponent>())
      {
        item.LoadContent(Environment);
      }
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
      GraphicsDevice.Clear(Color.AliceBlue);

      foreach (var item in Environment.Entities.GetComponents<Visual3DComponent>())
      {
        item.Draw(Environment);
      }

      SpriteBatch.Begin(SpriteSortMode.FrontToBack);

      foreach (var item in Environment.Entities.GetComponents<VisualComponent>())
      {
        item.Draw(Environment, SpriteBatch);
      }

      SpriteBatch.End();

      base.Draw(gameTime);
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
