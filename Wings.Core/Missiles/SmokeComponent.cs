using System;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework.Content;
using Wings.Core.Visuals;

namespace Wings.Core.Missiles
{
  public class SmokeComponent : Sprite3DComponent
  {
    readonly TimeSpan LifeTime = TimeSpan.FromSeconds(10);

    DateTime Start;

    public SmokeComponent(EntityId id, ContentManager content)
      : base(id, content, "SmokePuff", 1f)
    {
      Start = DateTime.Now;
    }


    public void Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
      TimeSpan t = DateTime.Now - Start;
      if (t > LifeTime)
        environment.Entities.RemoveEntity(EntityId);
    }


    public override float Scale()
    {
      TimeSpan t = DateTime.Now - Start;

      if (t.TotalSeconds < 1)
        return 1;

      return t < LifeTime ? (float)(t.TotalSeconds * 1) : (float)LifeTime.TotalSeconds;
    }
  }
}
