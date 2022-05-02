using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;

namespace Wings.Core.Physics
{
  public class BodyComponent : Component
  {
    // Primary values
    public Vector3 Position { get; protected set; }
    public Matrix Rotation { get; protected set; }

    // Derieved values
    public Vector3 ForwardUnitVector { get; protected set; }
    public Vector3 UpUnitVector { get; protected set; }


    public BodyComponent(EntityId id, Vector3 position, Matrix rotation)
      : base(id)
    {
      Position = position;
      Rotation = rotation;
      RefreshDerivedValues();
    }


    public void Update(Vector3 movement, Matrix rotation)
    {
      Position += movement;
      Rotation *= rotation;

      RefreshDerivedValues();
    }


    protected void RefreshDerivedValues()
    {
      ForwardUnitVector = Vector3.Transform(Vector3.UnitX, Rotation);
      UpUnitVector = Vector3.Transform(Vector3.UnitZ, Rotation);
    }
  }
}
