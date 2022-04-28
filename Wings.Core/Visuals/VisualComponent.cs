using System;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Wings.Core.Visuals
{
  public abstract class VisualComponent : Component
  {
    public VisualComponent(EntityId id)
      : base(id)
    {
    }

    public virtual void LoadContent(WingsEnvironment environment)
    {
    }

    
    public virtual void Initialize(WingsEnvironment environment)
    {
    }

    public abstract void Draw(WingsEnvironment environment, SpriteBatch spriteBatch);
  }
}
