using System;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Wings.Blueprint.Physics;
using Wings.Blueprint.Visuals;

namespace Wings.Blueprint.Aircraft
{
  public class HUDComponent : VisualComponent, IDerivedComponent
  {
    private AircraftComponent Aircraft { get; set; }

    private BodyComponent AircraftBody { get; set; }


    private Texture2D HorizonTexture;
    private Texture2D ControlStickFaceTexture;
    private Texture2D ControlStickBallTexture;

    private Vector2 ControlStickLocation;
    private Vector2 ControlStickFaceLocation;
    private Vector2 ControlStickBallLocation;
    private Vector2 ControlStickScale;


    public HUDComponent(EntityId id, AircraftComponent aircraft, BodyComponent aircraftBody)
      : base(id)
    {
      Aircraft = aircraft;
      AircraftBody = aircraftBody;

      ControlStickScale = new Vector2(0.25f, 0.25f);
      ControlStickLocation = new Vector2(600, 300);
      ControlStickFaceLocation = ControlStickLocation - new Vector2(100, 100) * ControlStickScale;
      ControlStickBallLocation = ControlStickLocation - new Vector2(20, 20) * ControlStickScale;
    }


    public override void LoadContent(ContentManager content)
    {
      HorizonTexture = content.Load<Texture2D>("Horizon");
      ControlStickBallTexture = content.Load<Texture2D>("DarkBall");
      ControlStickFaceTexture = content.Load<Texture2D>("ControlStickFace");
      base.LoadContent(content);
    }


    public override void Draw(SpriteBatch spriteBatch)
    {
      DrawControlStick(spriteBatch);
      DrawHorizon(spriteBatch);
    }


    public void DrawControlStick(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(ControlStickFaceTexture, ControlStickFaceLocation, null, Color.White, 0f, new Vector2(0, 0), ControlStickScale, SpriteEffects.None, 0);

      Vector2 stickPosition = ControlStickBallLocation + 100 * ControlStickScale.X * Aircraft.CurrentStickPosition;
      spriteBatch.Draw(ControlStickBallTexture, stickPosition, null, Color.White, 0f, new Vector2(0,0), ControlStickScale, SpriteEffects.None, 0);
    }


    public void DrawHorizon(SpriteBatch spriteBatch)
    {
      float pitchAngle = AircraftBody.Rotation.Y;
      float pitchOffset = Math.Max(Math.Min(pitchAngle * 100, 200), -200);
      Vector2 position = new Vector2(400, 250 + pitchOffset);

      float rollAngle = AircraftBody.Rotation.X;
      var origin = new Vector2(200, 22);

      var scale = new Vector2(1, 1);

      spriteBatch.Draw(HorizonTexture, position, null, Color.White, rollAngle, origin, scale, SpriteEffects.None, 0);
    }
  }
}
