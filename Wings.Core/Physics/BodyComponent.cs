using System;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;

namespace Wings.Core.Physics
{
  public class BodyComponent : Component
  {
    // Primary values
    public Vector3 Position { get; protected set; } // X, Y, Z
    public Vector3 Rotation { get; protected set; } // Roll (around X), pitch (around Y), Yaw (around Z)

    // Derived values
    public Vector2 ForwardDirection { get; protected set; }
    public Vector3 ForwardUnitVector { get; protected set; }
    public Vector3 UpUnitVector { get; protected set; }

    public BodyComponent(EntityId id, Vector3 position, Vector3 rotation)
      : base(id)
    {
      Position = position;
      Rotation = rotation;
      UpdateDerivedValues();
    }

    
    public void Update(Vector3 movement, Vector3 rotation)
    {
      Position = movement;

      rotation.Deconstruct(out float roll, out float pitch, out float yaw);

      if (pitch > Angles.QuarterCircle || pitch < -Angles.QuarterCircle)
      {
        pitch = Angles.HalfCircle - pitch;
        roll = MathHelper.WrapAngle(roll + Angles.HalfCircle);
        yaw = MathHelper.WrapAngle(yaw + Angles.HalfCircle);
      }
      else
      {
        roll = MathHelper.WrapAngle(roll);
        pitch = MathHelper.WrapAngle(pitch);
        yaw = MathHelper.WrapAngle(yaw);
      }

      Rotation = new Vector3(roll, pitch, yaw);
      
      ForwardDirection = new Vector2(pitch, yaw);
      ForwardUnitVector = Converters.RotationRadiansToUnitVector(ForwardDirection);

      Matrix rotationMatrix = Matrix.CreateRotationX(roll) * Matrix.CreateRotationZ(yaw);// * Matrix.CreateRotationY(-pitch);
      UpUnitVector = Vector3.Transform(new Vector3(0, 0, 1), rotationMatrix);
    }


    protected void UpdateDerivedValues()
    {

    }
  }
}
