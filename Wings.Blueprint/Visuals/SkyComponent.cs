using System;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Wings.Blueprint.Physics;

namespace Wings.Blueprint.Visuals
{
  public class SkyComponent : VisualComponent, IDerivedComponent
  {
    string TextureName;
    float Yaw;
    float Pitch;
    float Scale;
    BodyComponent CenterBody;

    private Texture2D ItemTexture;
    private Vector2 ItemCenter;


    public SkyComponent(EntityId id, string textureName, float yawDeg, float pitchDeg, float scale, BodyComponent centerBody)
      : base(id)
    {
      TextureName = textureName;
      Yaw = MathHelper.WrapAngle(MathHelper.ToRadians(yawDeg));
      Pitch = MathHelper.WrapAngle(MathHelper.ToRadians(pitchDeg));
      Scale = scale;
      CenterBody = centerBody;
    }


    public override void LoadContent(GameEnvironment environment, ContentManager content)
    {
      ItemTexture = content.Load<Texture2D>(TextureName);
      ItemCenter = new Vector2(ItemTexture.Width / 2, ItemTexture.Height / 2);
      base.LoadContent(environment, content);
    }

    static readonly float ViewportFOVx = MathHelper.ToRadians(45);
    static readonly float ViewportFOVy = MathHelper.ToRadians(45);

    public override void Draw(GameEnvironment environment, SpriteBatch spriteBatch)
    {
      var roll = -CenterBody.Rotation.X;
      var yawDiff = CenterBody.Rotation.Z - Yaw;
      var pitchDiff = CenterBody.Rotation.Y - Pitch;

      var sizeX = spriteBatch.GraphicsDevice.Viewport.Width;
      var sizeY = spriteBatch.GraphicsDevice.Viewport.Height;

      var dx = (sizeX / 2) / MathF.Tan(ViewportFOVx);
      var dy = (sizeY / 2) / MathF.Tan(ViewportFOVy);

      if (yawDiff > -ViewportFOVx && yawDiff < ViewportFOVx && pitchDiff > -ViewportFOVy && pitchDiff < ViewportFOVy)
      {
        Vector2 location = new Vector2(
          MathF.Tan(yawDiff) * dx,
          MathF.Tan(pitchDiff) * dy);

        location = new Vector2(
          location.X * MathF.Cos(roll) - location.Y * MathF.Sin(roll),
          location.X * MathF.Sin(roll) + location.Y * MathF.Cos(roll));

        location = new Vector2(
          sizeX / 2 + location.X,
          sizeY / 2 + location.Y);

        spriteBatch.Draw(ItemTexture, location, null, Color.White, roll, ItemCenter, new Vector2(Scale, Scale), SpriteEffects.None, 0f);
      }
    }
  }
}
