using System;
using Elfisk.ECS.Core;
using Elfisk.ECS.Core.Components;
using Microsoft.Xna.Framework;
using Wings.Core.Physics;

namespace Wings.Core.Missiles
{
  public class MissileComponent : Component
  {
    BodyComponent MissileBody;

    DateTime LastSmokePuffTime;
    DateTime TargetTime;

    public MissileComponent(EntityId id, BodyComponent missileBody)
      : base(id)
    {
      MissileBody = missileBody;
      LastSmokePuffTime = DateTime.Now;
      TargetTime = DateTime.Now.AddSeconds(8);
    }


    public void Update(WingsEnvironment environment, TimeSpan elapsedTime)
    {
      if (DateTime.Now - LastSmokePuffTime > TimeSpan.FromSeconds(0.25))
      {
        EntityId id = EntityId.NewId();

        Entity puff = new Entity(
          id,
          new Component[]
          {
            new NamedComponent(id, "Smoke"),
            new BodyComponent(id, MissileBody.Position, Vector3.Zero),
            new SmokeComponent(id, environment.Content)
          });

        environment.Entities.AddEntity(puff);

        LastSmokePuffTime = DateTime.Now;
      }

      if (DateTime.Now > TargetTime)
      {
        environment.Entities.RemoveEntity(EntityId);
      }
    }
  }
}
