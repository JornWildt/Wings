using System;
using System.Threading.Tasks;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;

namespace Wings.Blueprint.Physics
{
  public class PhysicsSystem : ISystem
  {
    public Task Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
      foreach (var p in environment.Entities.GetComponents<BodyComponent, PhysicsComponent>())
      {
        p.Item2.Velocity += p.Item2.Acceleration;

        p.Item1.Position += p.Item2.Velocity * (float)elapsedTime.TotalSeconds;
        p.Item1.Rotation += p.Item2.RotationalVelocity * (float)elapsedTime.TotalSeconds;

        p.Item1.Rotation = new Vector3
          (
            MathHelper.WrapAngle(p.Item1.Rotation.X),
            MathHelper.WrapAngle(p.Item1.Rotation.Y),
            MathHelper.WrapAngle(p.Item1.Rotation.Z)
          );
      }

      return Task.CompletedTask;
    }
  }
}
