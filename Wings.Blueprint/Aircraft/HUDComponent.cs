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

    private PhysicsComponent AircraftPhysics { get; set; }


    private Texture2D HorizonTexture;
    private Texture2D ControlStickFaceTexture;
    private Texture2D ControlStickBallTexture;
    private Texture2D CompassTexture;

    private Vector2 ControlStickLocation;
    private Vector2 ControlStickFaceLocation;
    private Vector2 ControlStickBallLocation;
    private Vector2 ControlStickScale;

    private SpriteFont DialFont;

    private Vector2 VerticalSpeedLocation;
    private Vector2 AirspeedLocation;
    private Vector2 HeightLocation;
    private Vector2 ThrottleLocation;
    private Vector2 CompassLocation;


    public HUDComponent(EntityId id, AircraftComponent aircraft)
      : base(id)
    {
      Aircraft = aircraft;
      AircraftBody = aircraft.AircraftBody;
      AircraftPhysics = aircraft.AircraftPhysics;

      ControlStickScale = new Vector2(0.25f, 0.25f);
      ControlStickLocation = new Vector2(750, 300);
      ControlStickFaceLocation = ControlStickLocation - new Vector2(100, 100) * ControlStickScale;
      ControlStickBallLocation = ControlStickLocation - new Vector2(20, 20) * ControlStickScale;

      VerticalSpeedLocation = new Vector2(50, 300);
      AirspeedLocation = new Vector2(50, 320);
      HeightLocation = new Vector2(50, 360);
      ThrottleLocation = new Vector2(50, 280);
      CompassLocation = new Vector2(700, 80);
    }


    public override void LoadContent(ContentManager content)
    {
      HorizonTexture = content.Load<Texture2D>("Horizon");
      ControlStickBallTexture = content.Load<Texture2D>("DarkBall");
      ControlStickFaceTexture = content.Load<Texture2D>("ControlStickFace");
      CompassTexture = content.Load<Texture2D>("Compass");
      DialFont = content.Load<SpriteFont>("HUD");
      base.LoadContent(content);
    }


    public override void Draw(SpriteBatch spriteBatch)
    {
      DrawControlStick(spriteBatch);
      DrawHorizon(spriteBatch);
      DrawVerticalSpeed(spriteBatch);
      DrawAirspeed(spriteBatch);
      DrawHeight(spriteBatch);
      DrawThrottle(spriteBatch);
      DrawCompass(spriteBatch);
    }


    public void DrawControlStick(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(ControlStickFaceTexture, ControlStickFaceLocation, null, Color.White, 0f, new Vector2(0, 0), ControlStickScale, SpriteEffects.None, 0.9f);

      Vector2 stickPosition = ControlStickBallLocation + 100 * ControlStickScale.X * Aircraft.CurrentStickPosition;
      spriteBatch.Draw(ControlStickBallTexture, stickPosition, null, Color.White, 0f, new Vector2(0,0), ControlStickScale, SpriteEffects.None, 0.9f);
    }


    public void DrawHorizon(SpriteBatch spriteBatch)
    {
      float pitchAngle = AircraftBody.Rotation.Y;
      float pitchOffset = Math.Max(Math.Min(pitchAngle * 100, 200), -200);
      Vector2 position = new Vector2(400, 250 + pitchOffset);

      float rollAngle = -AircraftBody.Rotation.X;
      var origin = new Vector2(200, 22);

      var scale = new Vector2(1, 1);

      spriteBatch.Draw(HorizonTexture, position, null, Color.White, rollAngle, origin, scale, SpriteEffects.None, 0.9f);
    }


    public void DrawVerticalSpeed(SpriteBatch spriteBatch)
    {
      string text = $"Vertical speed: {AircraftPhysics.Velocity.Z:N1} m/s";
      spriteBatch.DrawString(DialFont, text, VerticalSpeedLocation, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
    }


    public void DrawAirspeed(SpriteBatch spriteBatch)
    {
      string text = $"Air speed: {Converters.MetersSecondToKilometersHour(Aircraft.RelativeAirspeed.X):####} km/h. AoA: {MathHelper.ToDegrees(Aircraft.AngleOfAttack.Y):N0}.";
      spriteBatch.DrawString(DialFont, text, AirspeedLocation, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
      
      text = $"Absolute speed: {Converters.MetersSecondToKilometersHour(Aircraft.AircraftPhysics.Velocity.Length()):N1} km/h ({Converters.MetersSecondToKilometersHour(AircraftPhysics.Velocity.X):N1}/{Converters.MetersSecondToKilometersHour(AircraftPhysics.Velocity.Y):N1}/{Converters.MetersSecondToKilometersHour(AircraftPhysics.Velocity.Z):N1}). Vrot = ({AircraftPhysics.RotationalVelocity.X:N3} / {AircraftPhysics.RotationalVelocity.Y:N3} / {AircraftPhysics.RotationalVelocity.Z:N3})";
      spriteBatch.DrawString(DialFont, text, AirspeedLocation+new Vector2(0,20), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
    }


    public void DrawHeight(SpriteBatch spriteBatch)
    {
      string text = $"Height: {AircraftBody.Position.Z:N0} m. Roll: {MathHelper.ToDegrees(AircraftBody.Rotation.X):N0}. Pitch: {MathHelper.ToDegrees(AircraftBody.Rotation.Y):N0}. Yaw: {MathHelper.ToDegrees(AircraftBody.Rotation.Z):N0}. ForwardVector: ({AircraftBody.ForwardUnitVector.X:N1} / {AircraftBody.ForwardUnitVector.Y:N1} / {AircraftBody.ForwardUnitVector.Z:N1})";
      spriteBatch.DrawString(DialFont, text, HeightLocation, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
    }


    public void DrawThrottle(SpriteBatch spriteBatch)
    {
      string text = $"Throttle: {Aircraft.CurrentThrottle*100:N0}%";
      spriteBatch.DrawString(DialFont, text, ThrottleLocation, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
    }


    public void DrawCompass(SpriteBatch spriteBatch)
    {
      float yawAngle = AircraftBody.Rotation.Z;

      var origin = new Vector2(100, 100);

      var scale = new Vector2(0.5f, 0.5f);

      spriteBatch.Draw(CompassTexture, CompassLocation, null, Color.White, yawAngle, origin, scale, SpriteEffects.None, 0);
    }
  }
}
