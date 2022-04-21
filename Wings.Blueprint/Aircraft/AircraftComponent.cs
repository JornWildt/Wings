using System;
using Elfisk.ECS.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Wings.Blueprint.Physics;

namespace Wings.Blueprint.Aircraft
{
  public class AircraftComponent : Component
  {
    private BodyComponent AircraftBody { get; set; }
    private PhysicsComponent AircraftPhysics { get; set; }

    private Vector2? InitialMousePosition { get; set; }

    public Vector2 CurrentStickPosition { get; set; }


    public AircraftComponent(EntityId entityId, BodyComponent aircraftBody, PhysicsComponent aircraftPhysics)
      : base(entityId)
    {
      AircraftBody = aircraftBody;
      AircraftPhysics = aircraftPhysics;
    }

    const int CenterZero = 10;
    const float MaxControlMovement = 50;

    const float MaxPitch = Angles.QuarterCircle;

    const float MaxRollSpeed = Angles.FullCircle / 5;
    const float MaxPitchSpeed = Angles.FullCircle / 8;

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

      // Let max travel be "max" pixels
      mouseTravel = new Vector2(
        MathHelper.Clamp(mouseTravel.X, -MaxControlMovement, MaxControlMovement),
        MathHelper.Clamp(mouseTravel.Y, -MaxControlMovement, MaxControlMovement));

      CurrentStickPosition = new Vector2(
        mouseTravel.X / MaxControlMovement,
        mouseTravel.Y / MaxControlMovement);

      AircraftPhysics.RotationalVelocity = new Vector3(
        MaxRollSpeed * CurrentStickPosition.X,
        MaxPitchSpeed * CurrentStickPosition.Y,
        0);
    }
  }
}
