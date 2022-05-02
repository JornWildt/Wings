using System;
using System.Threading.Tasks;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;

namespace Wings.Core.Physics
{
  public class PhysicsSystem : ISystem
  {
    private readonly Vector3 Gravity = new Vector3(0, 0, -9.81f);

    public Task Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
      foreach (var p in environment.Entities.GetComponents<BodyComponent, PhysicsComponent>())
      {
        p.Item2.Update(
          deltaVelocity: Vector3.Zero,
          deltaAccelleration: Gravity * (float)elapsedTime.TotalSeconds,
          deltaRotationalVelocity: Matrix.Identity);

        p.Item1.Update(
          movement: p.Item2.Velocity * (float)elapsedTime.TotalSeconds,
          rotation: p.Item2.RotationalVelocity);// * (float)elapsedTime.TotalSeconds);
      }

      return Task.CompletedTask;
    }
  }
}
