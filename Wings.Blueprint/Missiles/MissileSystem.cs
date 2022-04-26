using System;
using System.Threading.Tasks;
using Elfisk.ECS.Core;
using Elfisk.ECS.Core.Components;
using Microsoft.Xna.Framework;
using Wings.Blueprint.Physics;
using Wings.Blueprint.Visuals;

namespace Wings.Blueprint.Missiles
{
  public class MissileSystem : ISystem
  {
    public Task Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
      foreach (var missile in environment.Entities.GetComponents<MissileComponent>())
      {
        missile.Update(environment, elapsedTime);
      }

      foreach (var puff in environment.Entities.GetComponents<SmokeComponent>())
      {
        puff.Update(environment, elapsedTime);
      }

      return Task.CompletedTask;
    }


    public static Entity CreateMissile(Vector3 position, Vector3 velocity)
    {
      EntityId id = EntityId.NewId();

      BodyComponent body = new BodyComponent(id, position, Vector3.Normalize(velocity));

      return new Entity(
        id,
        new Component[]
        {
          new NamedComponent(id, "Missile"),
          body,
          new PhysicsComponent(id, velocity, Vector3.Zero, Vector3.Zero),
          new Sprite3DComponent(id, "Missile", 3f),
          new MissileComponent(id, body)
        });
    }
  }
}
