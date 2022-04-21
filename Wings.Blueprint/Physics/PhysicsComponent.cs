using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;

namespace Wings.Blueprint.Physics
{
  public class PhysicsComponent : Component
  {
    public Vector3 Velocity { get; set; }

    public Vector3 Acceleration { get; set; }

    public Vector3 RotationalVelocity { get; set; }


    public PhysicsComponent(EntityId id, Vector3 velocity, Vector3 acceleration, Vector3 rotationalVelocity)
      : base(id)
    {
      Velocity = velocity;
      Acceleration = acceleration;
      RotationalVelocity = rotationalVelocity;
    }
  }
}
