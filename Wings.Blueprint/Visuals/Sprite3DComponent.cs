using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Wings.Blueprint.Visuals
{
  public class Sprite3DComponent : Component
  {
    virtual public Texture2D Texture { get; protected set;  }

    virtual public float Scale { get; protected set; }

    virtual public Vector2 CenterOffsetScreen { get; protected set; }

    //private string TextureName { get; set; }


    public Sprite3DComponent(EntityId id, string textureName, float scale)
      : base(id)
    {
      Texture = WingsInitializer.GameContent.Load<Texture2D>(textureName);
      CenterOffsetScreen = new Vector2(Texture.Width / 2, Texture.Height / 2);
      Scale = scale;
    }
    
    
    //public virtual void LoadContent(GameEnvironment environment, ContentManager content)
    //{
    //}
  }
}
