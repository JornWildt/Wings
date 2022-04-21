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

    public virtual void LoadContent(ContentManager content)
    {
    }

    public abstract void Draw(SpriteBatch spriteBatch);
  }
}
