using Elfisk.ECS.Core;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Wings.Core
{
  public class WingsEnvironment : GameEnvironment
  {
    public GraphicsDevice Graphics { get; set; }

    public ContentManager Content { get; protected set; }


    public WingsEnvironment(GameEnvironment env, GraphicsDevice graphics, ContentManager content)
      : base(env)
    {
      Graphics = graphics;
      Content = content;
    }
  }
}
