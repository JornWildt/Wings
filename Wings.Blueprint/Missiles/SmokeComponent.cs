using System;
using Elfisk.ECS.Core;
using Wings.Blueprint.Visuals;

namespace Wings.Blueprint.Missiles
{
  public class SmokeComponent : Sprite3DComponent
  {
    readonly TimeSpan LifeTime = TimeSpan.FromSeconds(10);

    DateTime Start;

    public SmokeComponent(EntityId id)
      : base(id, "SmokePuff", 1f)
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

      return t < LifeTime ? (float)(t.TotalSeconds * 2) : (float)LifeTime.TotalSeconds;
    }
  }
}
