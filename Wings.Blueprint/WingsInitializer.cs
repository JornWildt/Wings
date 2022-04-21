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

      Entity aircraft = BuildAircraft();
      entities.AddEntity(aircraft);
    }


    private static Entity BuildAircraft()
    {
      EntityId id = EntityId.NewId();

      var aircraftBody = new BodyComponent(id, new Vector3(0, 0, 1000), new Vector3(0, 0, 0));
      var aircraftPhysics = new PhysicsComponent(id, 
        velocity: new Vector3(30, 0, 0), 
        acceleration: new Vector3(0, 0, 0), 
        rotationalVelocity: new Vector3(0,0,0));
      var aircraft = new AircraftComponent(id, aircraftBody, aircraftPhysics);

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
  }
}
