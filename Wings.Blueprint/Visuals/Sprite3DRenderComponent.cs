using System;
using System.Collections.Generic;
using System.Text;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Wings.Blueprint.Physics;

namespace Wings.Blueprint.Visuals
{
  public class Sprite3DRenderComponent : VisualComponent, IDerivedComponent
  {
    static readonly float ViewportFOVx = MathHelper.ToRadians(45);
    static readonly float ViewportFOVy = MathHelper.ToRadians(45);

    protected float ViewportSizeX;
    protected float ViewportSizeY;

    protected float ViewportSizeX_2;
    protected float ViewportSizeY_2;

    protected float ViewpointDistanceX;
    protected float ViewpointDistanceY;


    protected BodyComponent OriginBody { get; set; }

    public Sprite3DRenderComponent(EntityId id, BodyComponent origin)
      : base(id)
    {
      OriginBody = origin;
    }


    public override void Initialize(GameEnvironment environment, GraphicsDevice graphics)
    {
      base.Initialize(environment, graphics);

      ViewportSizeX = graphics.Viewport.Width;
      ViewportSizeY = graphics.Viewport.Height;

      ViewportSizeX_2 = ViewportSizeX / 2f;
      ViewportSizeY_2 = ViewportSizeY / 2f;

      ViewpointDistanceX = (ViewportSizeX / 2) / MathF.Tan(ViewportFOVx);
      ViewpointDistanceY = (ViewportSizeY / 2) / MathF.Tan(ViewportFOVy);
    }


    //public override void LoadContent(GameEnvironment environment, ContentManager content)
    //{
    //  base.LoadContent(environment, content);

    //  foreach (var p in environment.Entities.GetComponents<Sprite3DComponent>())
    //  {
    //    p.LoadContent(environment, content);
    //  }
    //}


    public override void Draw(GameEnvironment environment, SpriteBatch spriteBatch)
    {
      var roll = -OriginBody.Rotation.X;

      foreach (var p in environment.Entities.GetComponents<BodyComponent, Sprite3DComponent>())
      {
        Vector3 relativeSpritePosition = p.Item1.Position - OriginBody.Position;
        Vector2 relativeSpriteDir = Converters.UnitVectorToRotationRadians(relativeSpritePosition, 0);

        var yawDiff = OriginBody.Rotation.Z - relativeSpriteDir.Y;
        var pitchDiff = OriginBody.Rotation.Y - relativeSpriteDir.X;

        if (yawDiff > -ViewportFOVx && yawDiff < ViewportFOVx && pitchDiff > -ViewportFOVy && pitchDiff < ViewportFOVy)
        {
          Vector2 screenPosition = new Vector2(
            MathF.Tan(yawDiff) * ViewpointDistanceX,
            MathF.Tan(pitchDiff) * ViewpointDistanceY);

          screenPosition = new Vector2(
            screenPosition.X * MathF.Cos(roll) - screenPosition.Y * MathF.Sin(roll),
            screenPosition.X * MathF.Sin(roll) + screenPosition.Y * MathF.Cos(roll));

          screenPosition = new Vector2(
            ViewportSizeX_2 + screenPosition.X,
            ViewportSizeY_2 + screenPosition.Y);

          spriteBatch.Draw(p.Item2.Texture, screenPosition, null, Color.White, roll, p.Item2.CenterOffsetScreen, new Vector2(p.Item2.Scale, p.Item2.Scale), SpriteEffects.None, 0.1f);
        }
      }

    }
  }

}