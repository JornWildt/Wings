using System;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wings.Core.Physics;

namespace Wings.Core.Visuals
{
  public class Sprite3DRenderComponent : VisualComponent, IDerivedComponent
  {
    static readonly float ViewportFOVx = MathHelper.ToRadians(45);
    static readonly float ViewportFOVy = MathHelper.ToRadians(60);
    static readonly float MaxVisibleDistance = 1000f;

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


    public override void Initialize(WingsEnvironment environment)
    {
      base.Initialize(environment);

      ViewportSizeX = environment.Graphics.Viewport.Width;
      ViewportSizeY = environment.Graphics.Viewport.Height;

      ViewportSizeX_2 = ViewportSizeX / 2f;
      ViewportSizeY_2 = ViewportSizeY / 2f;

      ViewpointDistanceX = (ViewportSizeX / 2) / MathF.Tan(ViewportFOVx);
      ViewpointDistanceY = (ViewportSizeY / 2) / MathF.Tan(ViewportFOVy);
    }


    public override void Draw(WingsEnvironment environment, SpriteBatch spriteBatch)
    {
#if false
      var roll = -OriginBody.Rotation.X;

      foreach (var p in environment.Entities.GetComponents<BodyComponent, Sprite3DComponent>())
      {
        Vector3 relativeSpritePosition = p.Item1.Position - OriginBody.Position;
        Vector2 relativeSpriteDir = Converters.VectorToRotationRadians(relativeSpritePosition, 0);
        float distance = relativeSpritePosition.Length();

        var yawDiff = OriginBody.Rotation.Z - relativeSpriteDir.Y;
        var pitchDiff = OriginBody.Rotation.Y - relativeSpriteDir.X;

        if (yawDiff > -ViewportFOVx && yawDiff < ViewportFOVx && pitchDiff > -ViewportFOVy && pitchDiff < ViewportFOVy && distance < MaxVisibleDistance)
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

          float scale = p.Item2.Scale() * (MathHelper.Clamp((MaxVisibleDistance - distance) / MaxVisibleDistance, 0, MaxVisibleDistance));

          spriteBatch.Draw(p.Item2.Texture, screenPosition, null, Color.White, roll, p.Item2.CenterOffsetScreen, new Vector2(scale, scale), SpriteEffects.None, 0.1f);
        }
      }
#endif
    }
  }
}