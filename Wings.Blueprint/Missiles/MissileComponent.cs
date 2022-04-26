using System;
using Elfisk.ECS.Core;

namespace Wings.Blueprint.Missiles
{
  public class MissileComponent : Component
  {
    public MissileComponent(EntityId id)
      : base(id)
    {
    }


    public void Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
    }
  }
}
