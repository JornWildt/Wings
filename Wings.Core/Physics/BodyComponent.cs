using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;

namespace Wings.Core.Physics
{
  public class BodyComponent : Component
  {
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector2 ForwardDirection { get; set; } // Rotation without X component
    public Vector2 UpDirection { get; set; } // Rotation without X component
    public Vector3 ForwardUnitVector { get; set; }
    public Vector3 UpUnitVector { get; set; }

    public BodyComponent(EntityId id, Vector3 position, Vector3 rotation)
      : base(id)
    {
      Position = position;
      Rotation = rotation;
    }
  }
}
