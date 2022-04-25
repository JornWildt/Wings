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
      Yaw = MathHelper.ToRadians(yawDeg);
      Pitch = MathHelper.ToRadians(pitchDeg);
      Scale = scale;
      CenterBody = centerBody;
    }


    public override void LoadContent(ContentManager content)
    {
      ItemTexture = content.Load<Texture2D>(TextureName);
      ItemCenter = new Vector2(ItemTexture.Width / 2, ItemTexture.Height / 2);
      base.LoadContent(content);
    }

    static readonly float ViewportFOVx = MathHelper.ToRadians(70);
    static readonly float ViewportFOVy = MathHelper.ToRadians(50);

    public override void Draw(SpriteBatch spriteBatch)
    {
      var roll = -CenterBody.Rotation.X;
      var yawDiff = CenterBody.Rotation.Z - Yaw;
      var pitchDiff = CenterBody.Rotation.Y - Pitch;

      var sizeX = spriteBatch.GraphicsDevice.Viewport.Width;
      var sizeY = spriteBatch.GraphicsDevice.Viewport.Height;

      if (yawDiff > -ViewportFOVx && yawDiff < ViewportFOVx && pitchDiff> -ViewportFOVy && pitchDiff < ViewportFOVy)
      {
        //yawDiff = yawDiff * Angles.QuarterCircle / ViewportFOVx;
        //pitchDiff = pitchDiff * Angles.QuarterCircle / ViewportFOVy;

        Vector2 location = new Vector2(
          (yawDiff / ViewportFOVx) * sizeX / 2,
          (pitchDiff / ViewportFOVy) * sizeY / 2);

        float dist = location.Length();

        location = new Vector2(
          location.X * MathF.Cos(roll) - location.Y * MathF.Sin(roll),
          location.X * MathF.Sin(roll) + location.Y * MathF.Cos(roll));

        location = new Vector2(
          sizeX / 2 + location.X,
          sizeY / 2 + location.Y);

        spriteBatch.Draw(ItemTexture, location, null, Color.White, roll, ItemCenter, new Vector2(Scale,Scale), SpriteEffects.None, 0f);
      }
    }
  }
}
