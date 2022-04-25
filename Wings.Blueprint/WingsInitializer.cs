using System;
using Elfisk.ECS.Core;
using Elfisk.ECS.Core.Components;
using Microsoft.Xna.Framework;
using Wings.Blueprint.Aircraft;
using Wings.Blueprint.Physics;
using Wings.Blueprint.Visuals;

namespace Wings.Blueprint
{
  public static class WingsInitializer
  {
    public static void Initialize(GameEnvironment environment)
    {
      IEntityRepository entities = environment.DependencyContainer.Resolve<IEntityRepository>();

      Entity aircraft = BuildAircraft(out AircraftComponent aircraftComponent);
      entities.AddEntity(aircraft);

      entities.AddEntity(BuildSkyItem("Cloud1", 20, 10, 0.5f, aircraftComponent.AircraftBody));
      entities.AddEntity(BuildSkyItem("Cloud1", -20, 40, 0.75f, aircraftComponent.AircraftBody));
      entities.AddEntity(BuildSkyItem("Sun", 50, 45, 1f, aircraftComponent.AircraftBody));
      
      entities.AddEntity(BuildSkyItem("GreenDot", 0, 0, 1f, aircraftComponent.AircraftBody));
      entities.AddEntity(BuildSkyItem("GreenDot", 45, 0, 1f, aircraftComponent.AircraftBody));
      entities.AddEntity(BuildSkyItem("GreenDot", 90, 0, 1f, aircraftComponent.AircraftBody));
      entities.AddEntity(BuildSkyItem("GreenDot", 135, 0, 1f, aircraftComponent.AircraftBody));
      entities.AddEntity(BuildSkyItem("GreenDot", 180, 0, 1f, aircraftComponent.AircraftBody));
      entities.AddEntity(BuildSkyItem("GreenDot", 225, 0, 1f, aircraftComponent.AircraftBody));
      entities.AddEntity(BuildSkyItem("GreenDot", 270, 0, 1f, aircraftComponent.AircraftBody));
      entities.AddEntity(BuildSkyItem("GreenDot", 315, 0, 1f, aircraftComponent.AircraftBody));
    }


    private static Entity BuildAircraft(out AircraftComponent aircraft)
    {
      EntityId id = EntityId.NewId();

      var aircraftBody = new BodyComponent(id, 
        position: new Vector3(0, 0, 1000),
        rotation: new Vector3(0f, 0f, 0));
        //rotation: new Vector3(-Angles.QuarterCircle, 0f, 0));
      var aircraftPhysics = new PhysicsComponent(id, 
        velocity: new Vector3(30, 0, 0), 
        acceleration: new Vector3(0, 0, 0), 
        rotationalVelocity: new Vector3(0,0,0));
      aircraft = new AircraftComponent(id, aircraftBody, aircraftPhysics);

      return new Entity(
        id,
        new IComponent[]
        {
          new NamedComponent(id, "Aircraft"),
          aircraftBody,
          aircraftPhysics,
          new HUDComponent(id, aircraft),
          aircraft
        });
    }


    private static Entity BuildSkyItem(string texture, float yaw, float pitch, float scale, BodyComponent centerBody)
    {
      EntityId id = EntityId.NewId();

      return new Entity(
        id,
        new Component[]
        {
          new SkyComponent(id, texture, yaw, pitch, scale, centerBody)
        });
    }
  }
}
