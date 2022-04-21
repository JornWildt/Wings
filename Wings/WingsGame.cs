using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wings.Blueprint.Visuals;

namespace Wings
{
  public class WingsGame : Game
  {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private GameEnvironment Environment { get; set; }

    public WingsGame(GameEnvironment environment)
    {
      Environment = environment;

      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;
    }

    protected override void Initialize()
    {
      base.Initialize();
    }

    protected override void LoadContent()
    {
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      foreach (var item in Environment.Entities.GetComponents<VisualComponent>())
      {
        item.LoadContent(Content);
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
      _spriteBatch.Begin();

      foreach (var item in Environment.Entities.GetComponents<VisualComponent>())
      {
        item.Draw(_spriteBatch);
      }

      _spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
