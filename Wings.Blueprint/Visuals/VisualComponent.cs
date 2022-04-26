using System;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Wings.Blueprint.Visuals
{
  public abstract class VisualComponent : Component
  {
    public VisualComponent(EntityId id)
      : base(id)
    {
    }

    public virtual void LoadContent(GameEnvironment environment, ContentManager content)
    {
    }

    
    public virtual void Initialize(GameEnvironment environment, GraphicsDevice graphics)
    {
    }

    public abstract void Draw(GameEnvironment environment, SpriteBatch spriteBatch);
  }
}
