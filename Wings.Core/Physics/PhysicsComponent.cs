using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;

namespace Wings.Core.Physics
{
  public class PhysicsComponent : Component
  {
    public Vector3 Velocity { get; set; }

    public Vector3 Acceleration { get; set; }

    public Vector3 RotationalVelocity { get; set; }

    public Vector3 VelocityUnitVector { get; set; }


    public PhysicsComponent(EntityId id, Vector3 velocity, Vector3 acceleration, Vector3 rotationalVelocity)
      : base(id)
    {
      Velocity = velocity;
      Acceleration = acceleration;
      RotationalVelocity = rotationalVelocity;
      VelocityUnitVector = velocity.Length() > 0 ? Vector3.Normalize(velocity) : new Vector3(0,0,0);
    }
  }
}
