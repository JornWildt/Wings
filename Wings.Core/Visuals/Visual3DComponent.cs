using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wings.Core.Visuals
{
  public abstract class Visual3DComponent : Component
  {
    public Visual3DComponent(EntityId id)
      : base(id)
    {
    }

    
    public virtual void LoadContent(WingsEnvironment environment)
    {
    }


    public virtual void Initialize(WingsEnvironment environment)
    {
    }

    public abstract void Draw(WingsEnvironment environment);


    
    protected void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
    {
      foreach (ModelMesh mesh in model.Meshes)
      {
        foreach (BasicEffect effect in mesh.Effects)
        {
          effect.EnableDefaultLighting();
          effect.World = world;
          effect.View = view;
          effect.Projection = projection;
        }

        mesh.Draw();
      }
    }
  }
}
