using System;
using System.Threading.Tasks;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;

namespace Wings.Blueprint.Physics
{
  public class PhysicsSystem : ISystem
  {
    private readonly Vector3 Gravity = new Vector3(0, 0, -9.81f);

    public Task Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
      foreach (var p in environment.Entities.GetComponents<BodyComponent, PhysicsComponent>())
      {
        p.Item2.Velocity += (p.Item2.Acceleration + Gravity) * (float)elapsedTime.TotalSeconds;

        p.Item1.Position += p.Item2.Velocity * (float)elapsedTime.TotalSeconds;
        p.Item1.Rotation += p.Item2.RotationalVelocity * (float)elapsedTime.TotalSeconds;

        p.Item1.Rotation = new Vector3
          (
            MathHelper.WrapAngle(p.Item1.Rotation.X),
            MathHelper.WrapAngle(p.Item1.Rotation.Y),
            MathHelper.WrapAngle(p.Item1.Rotation.Z)
          );

        p.Item1.Direction = new Vector2(p.Item1.Rotation.Y, p.Item1.Rotation.Z);
        p.Item1.ForwardUnitVector = Converters.RotationRadiansToUnitVector(p.Item1.Direction);
        p.Item2.VelocityUnitVector = p.Item2.Velocity.Length() > 0 ? Vector3.Normalize(p.Item2.Velocity) : new Vector3(0, 0, 0);
      }

      return Task.CompletedTask;
    }
  }
}
