using System;
using System.IO;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Wings.Core.Missiles;
using Wings.Core.Physics;

namespace Wings.Core.Aircraft
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

    static TextWriter LogFile;


    public AircraftComponent(EntityId entityId, BodyComponent aircraftBody, PhysicsComponent aircraftPhysics)
      : base(entityId)
    {
      AircraftBody = aircraftBody;
      AircraftPhysics = aircraftPhysics;
      CurrentThrottle = 0.6f;

      LogFile = new StreamWriter(new FileStream("C:\\tmp\\wings-log.txt", FileMode.Create));
    }

    const int CenterZero = 16;
    const float MaxControlMovement = 100;

    const float MaxRollRate = Angles.FullCircle / 5;
    const float MaxPitchRate = Angles.FullCircle / 10;
    const float MaxYawRate = Angles.FullCircle / 20;


    public void Update(WingsEnvironment environment, TimeSpan elapsedTime)
    {
      UpdateControlStickState();
      ReadKeyboardState(environment);
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
      //CurrentStickPosition = new Vector2(0f, 0f);
      //CurrentRudder = -1f;

      // [pitch,yaw]
      // If pointing straight up then use aircraft roll as yaw in 2D space
      Vector2 velocityDirection = Converters.VectorToRotationRadians(AircraftPhysics.VelocityUnitVector, AircraftBody.Rotation.X);

      Vector2 relativeWindDirection_unrotated = AircraftBody.Direction - velocityDirection;
      
      relativeWindDirection_unrotated = new Vector2(
        MathHelper.WrapAngle(relativeWindDirection_unrotated.X),
        MathHelper.WrapAngle(relativeWindDirection_unrotated.Y));

      // Rotate relative wind around roll axis (X).
      float roll = AircraftBody.Rotation.X;
      Vector2 relativeWindDirection = new Vector2(
        MathF.Cos(roll) * relativeWindDirection_unrotated.X + MathF.Sin(roll) * relativeWindDirection_unrotated.Y,
        MathF.Cos(roll) * relativeWindDirection_unrotated.Y + MathF.Sin(roll) * relativeWindDirection_unrotated.X);

      relativeWindDirection = Converters.Round(relativeWindDirection);

      float speed = AircraftPhysics.Velocity.Length();
      Vector3 relativeAirspeed = new Vector3(
        MathF.Cos(relativeWindDirection.X) * MathF.Cos(relativeWindDirection.Y) * speed,
        MathF.Cos(relativeWindDirection.X) * MathF.Sin(relativeWindDirection.Y) * speed,
        MathF.Sin(relativeWindDirection.X) * speed);

      RelativeAirspeed = relativeAirspeed = Converters.Round(relativeAirspeed);

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
      AircraftPhysics.RotationalVelocity = AircraftPhysics.RotationalVelocity + new Vector3(
        0,
        -(aoaHorzStabDeg / 10.0f) * MaxPitchRate,
        -(aoaVertStabDeg / 2.0f) * MaxYawRate);

      // Max lift is at max air speed * factor of gravity (factor should be >1 to counter gravity at max speed)
      float lift = (relativeAirspeed.X / MaxAirspeed) * 2f * 9.81f; // So far lift is in "acceleration" unit

      // Scale max lift with wing's lift constant calculated from wing's angle of attack.
      if (aoaWingDeg > -15 && aoaWingDeg < 15)
        lift = lift * aoaWingDeg / 5;
      else if (aoaWingDeg > 0)
        lift = lift * 3;
      else
        lift = lift * -3;

      // Forward pull from engine. More throttle => more pull.
      // More speed yields lesser pull as the propeller's effect reduces with forward speed.
      float forwardPull = (CurrentThrottle * MathHelper.Clamp((MaxAirspeed - relativeAirspeed.X), 0, MaxAirspeed) / MaxAirspeed) * 20; // pull in "acceleration" unit

      // Drag increases with speed (in "acceleration" unit)
      // FIXME: Add different constants for all three dimensions
      Vector3 drag = new Vector3(
        (relativeAirspeed.X * relativeAirspeed.X / (MaxAirspeed * MaxAirspeed)) * -10,
        (relativeAirspeed.Y * relativeAirspeed.Y / (MaxAirspeed * MaxAirspeed)) * -50 * MathF.Sign(relativeAirspeed.Y),
        (relativeAirspeed.Z * relativeAirspeed.Z / (MaxAirspeed * MaxAirspeed)) * -50 * MathF.Sign(relativeAirspeed.Z));

      // Calculate X/Y/Z acceleration (relative to aircraft)
      Vector3 acceleration =
        new Vector3(forwardPull, 0, lift)
        + drag;

      // At last, rotate the forces back into the absolute coordinate system

      var unRolledAcc = RotateRoll(acceleration, AircraftBody.Rotation.X);
      var unPitchedAcc = RotatePitch(unRolledAcc, AircraftBody.Rotation.Y);
      var unYawedAcc = RotateYaw(unPitchedAcc, AircraftBody.Rotation.Z);
      AircraftPhysics.Acceleration = Converters.Round(unYawedAcc);

      // Rotate rotational velocity around roll axis
      var unRolledRotation = new Vector3(
        AircraftPhysics.RotationalVelocity.X,
        MathF.Cos(AircraftBody.Rotation.X) * AircraftPhysics.RotationalVelocity.Y + MathF.Sin(AircraftBody.Rotation.X) * AircraftPhysics.RotationalVelocity.Z,
        MathF.Cos(AircraftBody.Rotation.X) * AircraftPhysics.RotationalVelocity.Z + MathF.Sin(AircraftBody.Rotation.X) * AircraftPhysics.RotationalVelocity.Y);

      AircraftPhysics.RotationalVelocity = Converters.Round(unRolledRotation);

#if false
      LogFile.WriteLine($@"{DateTime.Now}
  Position                  : {AircraftBody.Position.X:###.##} / {AircraftBody.Position.Y:###.##} / {AircraftBody.Position.Z:###.##}
  Roll / pitch / yaw        : {MathHelper.ToDegrees(AircraftBody.Rotation.X):###.##} / {MathHelper.ToDegrees(AircraftBody.Rotation.Y):###.##} / {MathHelper.ToDegrees(AircraftBody.Rotation.Z):###.##}
  Ground speed              : {AircraftPhysics.Velocity.X:###.##} / {AircraftPhysics.Velocity.Y:###.##} / {AircraftPhysics.Velocity.Z:###.##}
  Ground speed dir (p/y)    : {MathHelper.ToDegrees(velocityDirection.X):###.##} / {MathHelper.ToDegrees(velocityDirection.Y):###.##}
  Direction (p/y)           : {MathHelper.ToDegrees(AircraftBody.Direction.X):###.##} / {MathHelper.ToDegrees(AircraftBody.Direction.Y):###.##}
  Relative speed (p/y)      : {MathHelper.ToDegrees(relativeWindDirection.X):###.##} / {MathHelper.ToDegrees(relativeWindDirection.Y):###.##}
  Relative speed            : {relativeAirspeed.X:###.##} / {relativeAirspeed.Y:###.##} / {relativeAirspeed.Z:###.##}
  Angle of attack           : {MathHelper.ToDegrees(AngleOfAttack.X):###.##} / {MathHelper.ToDegrees(AngleOfAttack.Y):###.##} / {MathHelper.ToDegrees(AngleOfAttack.Z):###.##}
  Forward pull              : {forwardPull:###.##}
  Drag                      : {drag.X:###.##} / {drag.Y:###.##} / {drag.Z:###.##}
  Relative acceleration     : {acceleration.X:###.##} / {acceleration.Y:###.##} / {acceleration.Z:###.##}
  Relative rotation velocity: {MathHelper.ToDegrees(AircraftPhysics.RotationalVelocity.X):###.##} / {MathHelper.ToDegrees(AircraftPhysics.RotationalVelocity.Y):###.##} / {MathHelper.ToDegrees(AircraftPhysics.RotationalVelocity.Z):###.##}
  Absolute acceleration     : {AircraftPhysics.Acceleration.X:###.##} / {AircraftPhysics.Acceleration.Y:###.##} / {AircraftPhysics.Acceleration.Z:###.##}
  Absolute rotation velocity: {MathHelper.ToDegrees(unRolledRotation.X):###.##} / {MathHelper.ToDegrees(unRolledRotation.Y):###.##} / {MathHelper.ToDegrees(unRolledRotation.Z):###.##}
");
#endif
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
        MathF.Cos(yaw) * v.X - MathF.Sin(yaw) * v.Y,
        MathF.Sin(yaw) * v.X + MathF.Cos(yaw) * v.Y,
        v.Z);
    }


    private Vector3 RotatePitch(Vector3 v, float pitch)
    {
      return new Vector3(
        MathF.Cos(pitch) * v.X - MathF.Sin(pitch) * v.Z,
        v.Y,
        MathF.Sin(pitch) * v.X + MathF.Cos(pitch) * v.Z);
    }


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


    DateTime LastShotTimestamp = DateTime.Now.AddSeconds(-10);

    private void ReadKeyboardState(WingsEnvironment environment)
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

      if (keyboard.IsKeyDown(Keys.Space) && DateTime.Now - LastShotTimestamp > TimeSpan.FromSeconds(0.25))
      {
        ShootMissile(environment);
        LastShotTimestamp = DateTime.Now;
      }

      if (keyboard.IsKeyDown(Keys.F1))
      {
        AircraftBody.Direction = Vector2.Zero;
        AircraftBody.ForwardUnitVector = new Vector3(1, 0, 0);
        AircraftBody.Position = Vector3.Zero;
        AircraftBody.Rotation = Vector3.Zero;
        AircraftPhysics.Acceleration = Vector3.Zero;
        AircraftPhysics.RotationalVelocity = Vector3.Zero;
        AircraftPhysics.Velocity = new Vector3(30, 0, 0);
        AircraftPhysics.VelocityUnitVector = new Vector3(1, 0, 0);
        Mouse.SetPosition(750, 300);
      }
      else if (keyboard.IsKeyDown(Keys.F2))
      {
        AircraftBody.Position = Vector3.Zero;
        AircraftBody.Rotation = new Vector3(0, 0, Angles.QuarterCircle);
        AircraftBody.Direction = new Vector2(0, Angles.QuarterCircle);
        AircraftBody.ForwardUnitVector = new Vector3(0, 1, 0);
        AircraftPhysics.Acceleration = Vector3.Zero;
        AircraftPhysics.RotationalVelocity = Vector3.Zero;
        AircraftPhysics.Velocity = new Vector3(0, 30, 0);
        AircraftPhysics.VelocityUnitVector = new Vector3(0, 1, 0);
        Mouse.SetPosition(750, 300);
      }
      else if (keyboard.IsKeyDown(Keys.F3))
      {
        AircraftBody.Position = Vector3.Zero;
        AircraftBody.Rotation = new Vector3(0, 0, -Angles.QuarterCircle);
        AircraftBody.Direction = new Vector2(0, -Angles.QuarterCircle);
        AircraftBody.ForwardUnitVector = new Vector3(0, -1, 0);
        AircraftPhysics.Acceleration = Vector3.Zero;
        AircraftPhysics.RotationalVelocity = Vector3.Zero;
        AircraftPhysics.Velocity = new Vector3(0, -30, 0);
        AircraftPhysics.VelocityUnitVector = new Vector3(0, -1, 0);
        Mouse.SetPosition(750, 300);
        System.Diagnostics.Debugger.Break();
      }
      else if (keyboard.IsKeyDown(Keys.F4))
      {
        AircraftBody.Position = Vector3.Zero;
        AircraftBody.Rotation = new Vector3(0, MathHelper.ToRadians(20), -Angles.QuarterCircle);
        AircraftBody.Direction = new Vector2(AircraftBody.Rotation.Y, AircraftBody.Rotation.Z);
        AircraftBody.ForwardUnitVector = Converters.RotationRadiansToUnitVector(AircraftBody.Direction);
        AircraftPhysics.Acceleration = Vector3.Zero;
        AircraftPhysics.RotationalVelocity = Vector3.Zero;
        AircraftPhysics.Velocity = AircraftBody.ForwardUnitVector * 30f;
        AircraftPhysics.VelocityUnitVector = AircraftBody.ForwardUnitVector;
        Mouse.SetPosition(750, 300);
        System.Diagnostics.Debugger.Break();
      }

      if (keyboard.IsKeyDown(Keys.F10))
      {
        System.Diagnostics.Debugger.Break();
      }
    }

    private void ShootMissile(WingsEnvironment environment)
    {
      Entity missile = MissileSystem.CreateMissile(environment.Content, AircraftBody.Position, AircraftBody.ForwardUnitVector * AircraftPhysics.Velocity.Length() * 4f);
      environment.Entities.AddEntity(missile);
    }
  }
}
