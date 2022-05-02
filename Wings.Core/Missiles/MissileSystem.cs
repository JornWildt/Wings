using System;
using System.Threading.Tasks;
using Elfisk.ECS.Core;
using Elfisk.ECS.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Wings.Core.Physics;
using Wings.Core.Visuals;

namespace Wings.Core.Missiles
{
  public class MissileSystem : ISystem
  {
    public Task Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
      foreach (var missile in environment.Entities.GetComponents<MissileComponent>())
      {
        missile.Update((WingsEnvironment)environment, elapsedTime);
      }

      foreach (var puff in environment.Entities.GetComponents<SmokeComponent>())
      {
        puff.Update(environment, elapsedTime);
      }

      return Task.CompletedTask;
    }


    public static Entity CreateMissile(ContentManager content, Vector3 position, Vector3 velocity)
    {
      EntityId id = EntityId.NewId();

      // FIXME: set orientation
      BodyComponent body = new BodyComponent(id, position, Matrix.Identity);

      return new Entity(
        id,
        new Component[]
        {
          new NamedComponent(id, "Missile"),
          body,
          new PhysicsComponent(id, velocity, Vector3.Zero, Matrix.Identity),
          new Sprite3DComponent(id, content, "Missile", 3f),
          new MissileComponent(id, body)
        });
    }
  }
}
