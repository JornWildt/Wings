using System;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Wings.Blueprint.Physics;

namespace Wings.Blueprint.Aircraft
{
  public class AircraftComponent : Component
  {
    public BodyComponent AircraftBody { get; private set; }

    public PhysicsComponent AircraftPhysics { get; private set; }

    private Vector2? InitialMousePosition { get; set; }

    public Vector2 CurrentStickPosition { get; set; }

    public float CurrentThrottle { get; set; }

    // Current airspeed is "wind over wings" along fuselage
    public float CurrentAirspeed { get; set; }

    private const float MaxAirspeed = 60f;


    public AircraftComponent(EntityId entityId, BodyComponent aircraftBody, PhysicsComponent aircraftPhysics)
      : base(entityId)
    {
      AircraftBody = aircraftBody;
      AircraftPhysics = aircraftPhysics;
      CurrentThrottle = 0.6f;
    }

    const int CenterZero = 10;
    const float MaxControlMovement = 100;

    const float MaxPitch = Angles.QuarterCircle;

    const float MaxRollSpeed = Angles.FullCircle / 10;
    const float MaxPitchSpeed = Angles.FullCircle / 10;

    public void Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
      MouseState mouseState = Mouse.GetState();

      if (InitialMousePosition == null || mouseState.LeftButton == ButtonState.Pressed)
      {
        InitialMousePosition = new Vector2(mouseState.X, mouseState.Y);
      }

      Vector2 mouseTravel = new Vector2(mouseState.X, mouseState.Y) - InitialMousePosition.Value;

      // Make it easier to center by ignoring the innermost CenterZero pixel movements.
      mouseTravel = new Vector2(
        -CenterZero < mouseTravel.X && mouseTravel.X < CenterZero ? 0 : (mouseTravel.X < 0 ? mouseTravel.X + CenterZero : mouseTravel.X - CenterZero),
        -CenterZero < mouseTravel.Y && mouseTravel.Y < CenterZero ? 0 : (mouseTravel.Y < 0 ? mouseTravel.Y + CenterZero : mouseTravel.Y - CenterZero));

      // Let max travel be MaxControlMovement pixels
      mouseTravel = new Vector2(
        MathHelper.Clamp(mouseTravel.X, -MaxControlMovement, MaxControlMovement),
        MathHelper.Clamp(mouseTravel.Y, -MaxControlMovement, MaxControlMovement));

      CurrentStickPosition = new Vector2(
        mouseTravel.X / MaxControlMovement,
        mouseTravel.Y / MaxControlMovement);

      KeyboardState keyboard = Keyboard.GetState();

      if (keyboard.IsKeyDown(Keys.A))
        CurrentThrottle += 0.01f;
      else if (keyboard.IsKeyDown(Keys.Z))
        CurrentThrottle -= 0.01f;

      CurrentThrottle = MathHelper.Clamp(CurrentThrottle, 0, 1);

      AircraftPhysics.RotationalVelocity = new Vector3(
        MaxRollSpeed * CurrentStickPosition.X,
        MaxPitchSpeed * CurrentStickPosition.Y,
        0);

      CurrentAirspeed = Vector3.Dot(AircraftBody.DirectionalVector, AircraftPhysics.Velocity);

      float restrictedAirspeed = MathHelper.Clamp(CurrentAirspeed, 0, MaxAirspeed);

      float lift = (CurrentAirspeed / 30f) * 9.81f; // So far lift is in "acceleration" unit
      float forwardPull = CurrentThrottle * (MaxAirspeed - restrictedAirspeed) * 2; // pull in "acceleration" unit
      float drag = (restrictedAirspeed / MaxAirspeed) * -2; // drag in "acceleration" unit

      AircraftPhysics.Acceleration = new Vector3(0, 0, lift) + AircraftBody.DirectionalVector * forwardPull + AircraftPhysics.Velocity * drag;
    }
  }
}
