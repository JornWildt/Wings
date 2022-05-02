using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;

namespace Wings.Core.Physics
{
  public class PhysicsComponent : Component
  {
    // Primary values
    public Vector3 Velocity { get; protected set; }

    public Vector3 Acceleration { get; protected set; }

    public Matrix RotationalVelocity { get; protected set; }

    // Derived values
    public Vector3 VelocityUnitVector { get; protected set; }


    public PhysicsComponent(EntityId id, Vector3 velocity, Vector3 acceleration, Matrix rotationalVelocity)
      : base(id)
    {
      Velocity = velocity;
      Acceleration = acceleration;
      RotationalVelocity = rotationalVelocity;
      RefreshDerivedValues();
    }


    public void Update(Vector3 deltaVelocity, Vector3 deltaAccelleration, Matrix deltaRotationalVelocity)
    {
      Velocity += deltaVelocity;
      Acceleration += deltaAccelleration;
      RotationalVelocity = deltaRotationalVelocity;
      RefreshDerivedValues();
    }


    protected void RefreshDerivedValues()
    {
      VelocityUnitVector = Velocity.Length() > 0 ? Vector3.Normalize(Velocity) : new Vector3(0, 0, 0);
    }
  }
}
