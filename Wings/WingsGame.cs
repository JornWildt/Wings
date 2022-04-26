using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wings.Blueprint;
using Wings.Blueprint.Visuals;

namespace Wings
{
  public class WingsGame : Game
  {
    private GraphicsDeviceManager Graphics;
    private SpriteBatch SpriteBatch;

    private GameEnvironment Environment { get; set; }


    public WingsGame(GameEnvironment environment)
    {
      Environment = environment;

      Graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;

      // What a hack: expose content externally. Needs better implementation.
      WingsInitializer.GameContent = Content;
    }

    protected override void Initialize()
    {
      base.Initialize();
    }

    protected override void LoadContent()
    {
      SpriteBatch = new SpriteBatch(GraphicsDevice);

      foreach (var item in Environment.Entities.GetComponents<VisualComponent>())
      {
        item.Initialize(Environment, GraphicsDevice);
      }

      foreach (var item in Environment.Entities.GetComponents<VisualComponent>())
      {
        item.LoadContent(Environment, Content);
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
      GraphicsDevice.Clear(Color.CornflowerBlue);
      SpriteBatch.Begin(SpriteSortMode.FrontToBack);

      foreach (var item in Environment.Entities.GetComponents<VisualComponent>())
      {
        item.Draw(Environment, SpriteBatch);
      }

      SpriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
