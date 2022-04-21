using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;

namespace Wings.Blueprint.Physics
{
  public class BodyComponent : Component
  {
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 ForwardUnitVector { get; set; }

    public BodyComponent(EntityId id, Vector3 position, Vector3 rotation)
      : base(id)
    {
      Position = position;
      Rotation = rotation;
    }
  }
}
