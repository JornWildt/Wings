using System;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Wings.Core.Physics;

namespace Wings.Core.Visuals
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


    public override void LoadContent(WingsEnvironment environment)
    {
      ItemTexture = environment.Content.Load<Texture2D>(TextureName);
      ItemCenter = new Vector2(ItemTexture.Width / 2, ItemTexture.Height / 2);

      base.LoadContent(environment);
    }

    static readonly float ViewportFOVx = MathHelper.ToRadians(45);
    static readonly float ViewportFOVy = MathHelper.ToRadians(35);

    public override void Draw(WingsEnvironment environment, SpriteBatch spriteBatch)
    {
#if true
      CenterBody.Rotation.Deconstruct(out float bodyRoll, out float bodyPitch, out float bodyYaw);
#else
      float bodyRoll = 0f;
      float bodyPitch = MathHelper.ToRadians(80);
      float bodyYaw = 0f;
#endif

      float relativeYaw = MathHelper.WrapAngle(bodyYaw - Yaw);

      Vector3 itemVector = new Vector3(
        MathF.Cos(Pitch) * MathF.Cos(relativeYaw),
        MathF.Cos(Pitch) * MathF.Sin(relativeYaw),
        MathF.Sin(Pitch));

      Vector3 unpitchedItemVector = new Vector3(
        MathF.Cos(bodyPitch) * itemVector.X + MathF.Sin(bodyPitch) * itemVector.Z,
        itemVector.Y,
        MathF.Cos(bodyPitch) * itemVector.Z - MathF.Sin(bodyPitch) * itemVector.X);

      Vector2 relativeRadians = Converters.VectorToRotationRadians(unpitchedItemVector, bodyRoll);

#if true
      float roll = -bodyRoll;
      float yawDiff = relativeRadians.Y;
      float pitchDiff = -relativeRadians.X;

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
#endif
    }
  }
}
