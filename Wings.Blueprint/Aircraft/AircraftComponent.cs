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

    public float CurrentAngleOfAttack { get; set; }

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

    const float MaxRollRate = Angles.FullCircle / 4;
    const float MaxPitchRate = Angles.FullCircle / 10;

    public void Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
      if (InitialMousePosition == null)
        Mouse.SetPosition(750, 300);

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

      float rudderYaw = 0f;
      if (keyboard.IsKeyDown(Keys.Q))
        rudderYaw = Angles.FullCircle / 10;
      else if (keyboard.IsKeyDown(Keys.W))
        rudderYaw = -Angles.FullCircle / 10;

      CurrentThrottle = MathHelper.Clamp(CurrentThrottle, 0, 1);
      CurrentAirspeed = Vector3.Dot(AircraftBody.ForwardUnitVector, AircraftPhysics.Velocity);
      CurrentAngleOfAttack = Converters.AngleBetweenVectors(AircraftBody.ForwardUnitVector, AircraftPhysics.Velocity);

      if (float.IsNaN(CurrentAirspeed) || float.IsNaN(CurrentAngleOfAttack))
        throw new InvalidOperationException("CurrentAirSpeed or CurrentAngleOfAttack is NaN");

      var crossProd = Vector3.Cross(AircraftBody.ForwardUnitVector, Vector3.Normalize(AircraftPhysics.Velocity));
      var dot = Vector3.Dot(crossProd, new Vector3(0, 1, 0)); // FIXME: must depend on rotation
      if (dot < 0)
        CurrentAngleOfAttack = -CurrentAngleOfAttack;

      float aoaWingDeg = MathHelper.ToDegrees(CurrentAngleOfAttack) + 4 /* wing incidence */;
      float aoaElevatorDeg = MathHelper.ToDegrees(CurrentAngleOfAttack);

      AircraftPhysics.RotationalVelocity = new Vector3(
        0,//MaxRollRate * CurrentStickPosition.X,
        MaxPitchRate * CurrentStickPosition.Y * MathF.Cos(AircraftBody.Rotation.X),
        0);// MaxPitchRate * CurrentStickPosition.Y * MathF.Sin(AircraftBody.Rotation.X) + rudderYaw);

      AircraftPhysics.RotationalVelocity = AircraftPhysics.RotationalVelocity + new Vector3(
        0,
        -(aoaElevatorDeg / 4.0f) * MaxPitchRate,
        0);

      float restrictedAirspeed = MathHelper.Clamp(CurrentAirspeed, 0, MaxAirspeed);

      // Max lift is at max air speed * factor of gravity (factor should be >1 to counter gravity at max speed)
      float lift = (CurrentAirspeed / MaxAirspeed) * 5f * 9.81f; // So far lift is in "acceleration" unit

      if (aoaWingDeg > -15 && aoaWingDeg < 15)
        lift = lift * aoaWingDeg / 10;
      else if (aoaWingDeg < -15)
        lift = lift / -10;
      else if (aoaWingDeg > 15)
        lift = lift / 10;

      float forwardPull = (CurrentThrottle * (MaxAirspeed - restrictedAirspeed) / MaxAirspeed) * 10; // pull in "acceleration" unit

      float drag = (restrictedAirspeed * restrictedAirspeed / (MaxAirspeed * MaxAirspeed)) * -10; // drag in "acceleration" unit

      var liftVector = new Vector3(
          0, //lift * (-1f * MathF.Sin(AircraftBody.Rotation.Y) * MathF.Cos(AircraftBody.Rotation.X)),
          0,//lift * (MathF.Sin(AircraftBody.Rotation.X) * MathF.Cos(AircraftBody.Rotation.Z)),
          lift * MathF.Cos(AircraftBody.Rotation.X) * MathF.Cos(AircraftBody.Rotation.Y));

      AircraftPhysics.Acceleration = 
       liftVector
        + AircraftBody.ForwardUnitVector * forwardPull 
        + AircraftPhysics.VelocityUnitVector * drag;
    }
  }
}
