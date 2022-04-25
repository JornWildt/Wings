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

    public Vector2 CurrentStickPosition { get; set; } /* -1 .. 1 */

    public float CurrentThrottle { get; set; } /* 0 .. 1 */

    public float CurrentRudder { get; set; } /* -1 .. 1 */

    public Vector3 RelativeAirspeed { get; set; }
    public Vector3 AngleOfAttack { get; set; }

    // Current airspeed is "wind over wings" along fuselage, not in direction of travel
    //public float CurrentAirspeed_1 { get; set; }
    //public float CurrentAngleOfAttack_1 { get; set; }

    private const float MaxAirspeed = 60f;


    public AircraftComponent(EntityId entityId, BodyComponent aircraftBody, PhysicsComponent aircraftPhysics)
      : base(entityId)
    {
      AircraftBody = aircraftBody;
      AircraftPhysics = aircraftPhysics;
      CurrentThrottle = 0.6f;
    }

    const int CenterZero = 16;
    const float MaxControlMovement = 100;

    const float MaxRollRate = Angles.FullCircle / 10;
    const float MaxPitchRate = Angles.FullCircle / 10;
    const float MaxYawRate = Angles.FullCircle / 20;


    public void Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
      UpdateControlStickState();
      ReadKeyboardState();
      ApplyAerodynamics();
    }


#if false
      Vector3 velocityDirection = Converters.UnitVectorToRotationRadians(AircraftPhysics.VelocityUnitVector);
      Vector3 relativeWindDirection = velocityDirection - AircraftBody.Rotation;
      Vector3 relativeWindUnitVector = Converters.RotationRadiansToUnitVector(relativeWindDirection);

      Vector3 relativeWindUnitVectorRotated = new Vector3(
        relativeWindUnitVector.X,
        MathF.Cos(AircraftBody.Rotation.X) * relativeWindUnitVector.Y - MathF.Sin(AircraftBody.Rotation.X) * relativeWindUnitVector.Z,
        MathF.Sin(AircraftBody.Rotation.X) * relativeWindUnitVector.Y + MathF.Cos(AircraftBody.Rotation.X) * relativeWindUnitVector.Z);

      CalculateRelativeAirspeedAndAngleOfAttack();
#endif

    private void ApplyAerodynamics()
    {
      //CurrentStickPosition = new Vector2(0, 0.5f);

      // [pitch,yaw]
      // If pointing straight up then use aircraft roll as yaw in 2D space
      Vector2 velocityDirection = AircraftPhysics.VelocityUnitVector.Z < 1
        ? Converters.UnitVectorToRotationRadians(AircraftPhysics.VelocityUnitVector)
        : new Vector2(
            MathF.PI/2,
            AircraftBody.Rotation.X);

      Vector2 relativeWindDirection_unrotated = AircraftBody.Direction - velocityDirection;

      // Rotate relative wind around roll axis (X).
      float roll = AircraftBody.Rotation.X;
      Vector2 relativeWindDirection = new Vector2(
        MathF.Cos(roll) * relativeWindDirection_unrotated.X + MathF.Sin(roll) * relativeWindDirection_unrotated.Y,
        MathF.Cos(roll) * relativeWindDirection_unrotated.Y + MathF.Sin(roll) * relativeWindDirection_unrotated.X);

      float speed = AircraftPhysics.Velocity.Length();
      Vector3 relativeWindspeed = new Vector3(
        MathF.Cos(relativeWindDirection.X) * MathF.Cos(relativeWindDirection.Y) * speed,
        -MathF.Cos(relativeWindDirection.X) * MathF.Sin(relativeWindDirection.Y) * speed,
        MathF.Sin(relativeWindDirection.X) * speed);

      RelativeAirspeed = relativeWindspeed;

      // AoA given as rotation around X, Y and Z axes
      AngleOfAttack = new Vector3(
        0, 
        relativeWindDirection.X,
        relativeWindDirection.Y);

      // Get angle of attack in degrees as it is easier to work with (since wing lift is normally given relative to AoA in degrees)
      float aoaWingDeg = MathHelper.ToDegrees(AngleOfAttack.Y) + 4 /* wing incidence */;
      float aoaHorzStabDeg = MathHelper.ToDegrees(AngleOfAttack.Y);
      float aoaVertStabDeg = MathHelper.ToDegrees(AngleOfAttack.Z);

      // Rotational velocity depends on stick position (relative to aircraft)
      AircraftPhysics.RotationalVelocity = new Vector3(
        MaxRollRate * CurrentStickPosition.X,
        MaxPitchRate * CurrentStickPosition.Y,
        MaxYawRate * CurrentRudder);

      // Apply "weather wane" effect of vertical and horizontal stabilizers.
      //AircraftPhysics.RotationalVelocity = AircraftPhysics.RotationalVelocity + new Vector3(
      //  0,
      //  -(aoaHorzStabDeg / 5.0f) * MaxPitchRate,
      //  -(aoaVertStabDeg / 1.0f) * MaxYawRate);

      //if (MathF.Abs(MathHelper.ToDegrees(AircraftPhysics.RotationalVelocity.Z)) > 20)
      //  throw new InvalidOperationException();

      // Clamp airspeed to reduce calculation issues when falling too fast. This is only a stop gap solution.
      Vector3 restrictedAirspeed = new Vector3(
        MathHelper.Clamp(relativeWindspeed.X, -MaxAirspeed, MaxAirspeed),
        MathHelper.Clamp(relativeWindspeed.Y, -MaxAirspeed, MaxAirspeed),
        MathHelper.Clamp(relativeWindspeed.Z, -MaxAirspeed, MaxAirspeed));

      // Max lift is at max air speed * factor of gravity (factor should be >1 to counter gravity at max speed)
      float lift = (restrictedAirspeed.X / MaxAirspeed) * 1.5f * 9.81f; // So far lift is in "acceleration" unit

      // Scale max lift with wing's lift constant calculated from wing's angle of attack.
      if (aoaWingDeg > -15 && aoaWingDeg < 15)
        lift = lift * aoaWingDeg / 5;
      else if (aoaWingDeg > 0)
        lift = lift * 3;
      else
        lift = lift * -3;

      // Forward pull from engine. More throttle => more pull.
      // More speed yields lesser pull as the propeller's effect reduces with forward speed.
      float forwardPull = (CurrentThrottle * (MaxAirspeed - restrictedAirspeed.X) / MaxAirspeed) * 20; // pull in "acceleration" unit

      // Drag increases with speed (in "acceleration" unit)
      // FIXME: Add different constants for all three dimensions
      Vector3 drag = new Vector3(
        (restrictedAirspeed.X * restrictedAirspeed.X / (MaxAirspeed * MaxAirspeed)) * -10,
        (restrictedAirspeed.Y * restrictedAirspeed.Y / (MaxAirspeed * MaxAirspeed)) * -10 * (float)MathF.Sign(restrictedAirspeed.Y),
        (restrictedAirspeed.Z * restrictedAirspeed.Z / (MaxAirspeed * MaxAirspeed)) * -10);

      // Calculate X/Y/Z acceleration (relative to aircraft)
      Vector3 acceleration =
        new Vector3(forwardPull, 0, lift)
        + drag;

      // At last, rotate the acceleration back into the absolute coordinate system

      var unRolledAcc = RotateRoll(acceleration, AircraftBody.Rotation.X);
      var unYawedAcc = RotateYaw(unRolledAcc, AircraftBody.Rotation.Z);
      AircraftPhysics.Acceleration = RotatePitch(unYawedAcc, AircraftBody.Rotation.Y);

      //var x = RotateRoll(AircraftPhysics.RotationalVelocity, AircraftBody.Rotation.X);
      var y = new Vector3(
        AircraftPhysics.RotationalVelocity.X,
        MathF.Cos(AircraftBody.Rotation.X) * AircraftPhysics.RotationalVelocity.Y + MathF.Sin(AircraftBody.Rotation.X) * AircraftPhysics.RotationalVelocity.Z,
        MathF.Cos(AircraftBody.Rotation.X) * AircraftPhysics.RotationalVelocity.Z + MathF.Sin(AircraftBody.Rotation.X) * AircraftPhysics.RotationalVelocity.Y);

      AircraftPhysics.RotationalVelocity = y;
    }


    private Vector3 RotateRoll(Vector3 v, float roll)
    {
      return new Vector3(
        v.X,
        MathF.Cos(roll) * v.Y - MathF.Sin(roll) * v.Z,
        MathF.Sin(roll) * v.Y + MathF.Cos(roll) * v.Z);
    }


    private Vector3 RotateYaw(Vector3 v, float yaw)
    {
      return new Vector3(
        MathF.Cos(yaw) * v.X + MathF.Sin(yaw) * v.Y,
        -MathF.Sin(yaw) * v.X + MathF.Cos(yaw) * v.Y,
        v.Z);
    }


    private Vector3 RotatePitch(Vector3 v, float pitch)
    {
      return new Vector3(
        MathF.Cos(pitch) * v.X - MathF.Sin(pitch) * v.Z,
        v.Y,
        MathF.Sin(pitch) * v.X + MathF.Cos(pitch) * v.Z);
    }

#if false
    private void ApplyAerodynamics_1()
    {
      CalculateRelativeAirspeedAndAngleOfAttack_1();

      // Get angle of attack in degrees as it is easier to work with (since wing lift is relative to AoA in degrees)
      float aoaWingDeg = MathHelper.ToDegrees(CurrentAngleOfAttack_1) + 4 /* wing incidence */;
      float aoaElevatorDeg = MathHelper.ToDegrees(CurrentAngleOfAttack_1);

      // Rotational velocity depends on stick position
      AircraftPhysics.RotationalVelocity = new Vector3(
        -MaxRollRate * CurrentStickPosition.X,
        MaxPitchRate * CurrentStickPosition.Y * MathF.Cos(AircraftBody.Rotation.X),
        0);// MaxPitchRate * CurrentStickPosition.Y * MathF.Sin(AircraftBody.Rotation.X) + rudderYaw);

      // Apply "weather wane" effect of vertical and horizontal stabilizers.
      AircraftPhysics.RotationalVelocity = AircraftPhysics.RotationalVelocity + new Vector3(
        0,
        -(aoaElevatorDeg / 4.0f) * MaxPitchRate,
        0);

      // Clamp airspeed to reduce calculation issues when falling too fast. This is only a stop gap solution.
      float restrictedAirspeed = MathHelper.Clamp(CurrentAirspeed_1, 0, MaxAirspeed);

      // Max lift is at max air speed * factor of gravity (factor should be >1 to counter gravity at max speed)
      float lift = (CurrentAirspeed_1 / MaxAirspeed) * 5f * 9.81f; // So far lift is in "acceleration" unit

      // Scale max lift with wing's lift constant calculated from wing's angle of attack.
      if (aoaWingDeg > -15 && aoaWingDeg < 15)
        lift = lift * aoaWingDeg / 10;
      else if (aoaWingDeg < -15)
        lift = lift / -10;
      else if (aoaWingDeg > 15)
        lift = lift / 10;

      // Forward pull from engine. More throttle => more pull.
      // More speed yields lesser pull as the propeller's effect reduces with forward speed.
      float forwardPull = (CurrentThrottle * (MaxAirspeed - restrictedAirspeed) / MaxAirspeed) * 10; // pull in "acceleration" unit

      // Drag increases with speed.
      float drag = (restrictedAirspeed * restrictedAirspeed / (MaxAirspeed * MaxAirspeed)) * -10; // drag in "acceleration" unit

      var liftVector = new Vector3(
          0, //lift * (-1f * MathF.Sin(AircraftBody.Rotation.Y) * MathF.Cos(AircraftBody.Rotation.X)),
          0,//lift * (MathF.Sin(AircraftBody.Rotation.X) * MathF.Cos(AircraftBody.Rotation.Z)),
          lift * MathF.Cos(AircraftBody.Rotation.X) * MathF.Cos(AircraftBody.Rotation.Y));

      // Calculate X/Y/Z calculation based on all the values.
      AircraftPhysics.Acceleration =
       liftVector
        + AircraftBody.ForwardUnitVector * forwardPull
        + AircraftPhysics.VelocityUnitVector * drag;
    }


    private void CalculateRelativeAirspeedAndAngleOfAttack_1()
    {
      // Current airspeed depends on angle between fuselage forward direction and actual directional velocity.
      // - it could in principle be falling vertically with the plane level, yielding zero airspeed over the wing.
      CurrentAirspeed_1 = Vector3.Dot(AircraftBody.ForwardUnitVector, AircraftPhysics.Velocity);

      // Angle of attack is the angle between the fuselage direction and the directional velocity.
      // - This does although not handle side slipping, but then airspeed would be zero anyway.
      CurrentAngleOfAttack_1 = Converters.AngleBetweenVectors(AircraftBody.ForwardUnitVector, AircraftPhysics.Velocity);

      // Figure out which way the angle of attack is. This depends on the roll.
      // Cross product yields vector normal to fuselage forward and directional velocity
      var crossProd = Vector3.Cross(AircraftBody.ForwardUnitVector, Vector3.Normalize(AircraftPhysics.Velocity));

      // Dot with a normal along the aircrafts Y axis (yaw axis - a vector along the wing).
      // The sign indicates direction of angle of attack
      var dot = Vector3.Dot(crossProd, new Vector3(0, 1, 0));
      if (dot < 0)
        CurrentAngleOfAttack_1 = -CurrentAngleOfAttack_1;
    }
#endif


    private void UpdateControlStickState()
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
    }


    private void ReadKeyboardState()
    {
      KeyboardState keyboard = Keyboard.GetState();

      if (keyboard.IsKeyDown(Keys.A))
        CurrentThrottle += 0.01f;
      else if (keyboard.IsKeyDown(Keys.Z))
        CurrentThrottle -= 0.01f;

      CurrentThrottle = MathHelper.Clamp(CurrentThrottle, 0, 1);

      if (keyboard.IsKeyDown(Keys.Q))
        CurrentRudder = 1f;
      else if (keyboard.IsKeyDown(Keys.W))
        CurrentRudder = -1f;
      else
        CurrentRudder = 0f;
    }
  }
}
