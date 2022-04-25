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
  public class CloudComponent : VisualComponent, IDerivedComponent
  {
    float Yaw;
    float Pitch;
    BodyComponent CenterBody;

    private Texture2D CloudTexture;
    private Vector2 CloudCenter;


    public CloudComponent(EntityId id, float yawDeg, float pitchDeg, BodyComponent centerBody)
      : base(id)
    {
      Yaw = MathHelper.ToRadians(yawDeg);
      Pitch = MathHelper.ToRadians(pitchDeg);
      CenterBody = centerBody;
    }


    public override void LoadContent(ContentManager content)
    {
      CloudTexture = content.Load<Texture2D>("Cloud1");
      CloudCenter = new Vector2(CloudTexture.Width / 2, CloudTexture.Height / 2);
      base.LoadContent(content);
    }

    static readonly float ViewportFOVx = MathHelper.ToRadians(70);
    static readonly float ViewportFOVy = MathHelper.ToRadians(50);

    public override void Draw(SpriteBatch spriteBatch)
    {
      var roll = -CenterBody.Rotation.X;
      var yawDiff = CenterBody.Rotation.Z - Yaw;
      var pitchDiff = Pitch - CenterBody.Rotation.Y;

      var sizeX = spriteBatch.GraphicsDevice.Viewport.Width;
      var sizeY = spriteBatch.GraphicsDevice.Viewport.Height;

      if (yawDiff > -ViewportFOVx && yawDiff < ViewportFOVx && pitchDiff> -ViewportFOVy && pitchDiff < ViewportFOVy)
      {
        //yawDiff = yawDiff * Angles.QuarterCircle / ViewportFOVx;
        //pitchDiff = pitchDiff * Angles.QuarterCircle / ViewportFOVy;

        Vector2 cloudLocation = new Vector2(
          MathF.Sin(yawDiff) * sizeX / 2,
          MathF.Sin(pitchDiff) * sizeY / 2);

        cloudLocation = new Vector2(
          cloudLocation.X * MathF.Cos(roll) - cloudLocation.Y * MathF.Sin(roll),
          cloudLocation.X * MathF.Sin(roll) - cloudLocation.Y * MathF.Cos(roll));

        cloudLocation = new Vector2(
          sizeX / 2 + cloudLocation.X * 1.5f,
          sizeY / 2 + cloudLocation.Y * 1.5f);

        spriteBatch.Draw(CloudTexture, cloudLocation, null, Color.White, roll, CloudCenter, new Vector2(1,1), SpriteEffects.None, 0f);
      }
    }
  }
}
