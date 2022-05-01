using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wings.Core.Physics;
using Wings.Core.Visuals;

namespace Wings.Core.WorldMap
{
  public class WorldMapRenderComponent : Visual3DComponent, IDerivedComponent
  {
    private Model PineTreeModel;

    private Matrix CameraProjection;
    BodyComponent PointOfView;

    public WorldMapRenderComponent(EntityId id, BodyComponent pointOfView)
      : base(id)
    {
      PointOfView = pointOfView;
    }


    public override void LoadContent(WingsEnvironment environment)
    {
      base.LoadContent(environment);
      
      PineTreeModel = environment.Content.Load<Model>("3D/PineTree2");
    }


    public override void Initialize(WingsEnvironment environment)
    {
      base.Initialize(environment);
      
      CameraProjection = Matrix.CreatePerspectiveFieldOfView(
        fieldOfView: MathHelper.ToRadians(90),
        aspectRatio: environment.Graphics.DisplayMode.Width / environment.Graphics.DisplayMode.Height,
        nearPlaneDistance: 0.1f,
        farPlaneDistance: 10000f);
    }


    public override void Draw(WingsEnvironment environment)
    {
      var viewMatrix = Matrix.CreateLookAt(PointOfView.Position, PointOfView.Position + PointOfView.ForwardUnitVector, PointOfView.UpUnitVector);

      for (int x = -10; x < 10; ++x)
      {
        for (int y = -10; y < 10; ++y)
        {
          var world = Matrix.CreateWorld(new Vector3(x * 100, y * 100, 900), new Vector3(0, 0, -1), new Vector3(1, 0, 0));
          DrawModel(PineTreeModel, world, viewMatrix, CameraProjection);
        }
      }

    }
  }
}
