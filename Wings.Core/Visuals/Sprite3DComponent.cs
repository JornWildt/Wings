using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Wings.Core.Visuals
{
  public class Sprite3DComponent : Component
  {
    virtual public Texture2D Texture { get; protected set; }

    private float _scale;
    virtual public float Scale() => _scale;

    virtual public Vector2 CenterOffsetScreen { get; protected set; }

    //private string TextureName { get; set; }


    public Sprite3DComponent(EntityId id, ContentManager content, string textureName, float scale)
      : base(id)
    {
      Texture = content.Load<Texture2D>(textureName);
      CenterOffsetScreen = new Vector2(Texture.Width / 2, Texture.Height / 2);
      _scale = scale;
    }
  }
}
