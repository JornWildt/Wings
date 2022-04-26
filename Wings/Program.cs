using System;
using Castle.Windsor;
using Elfisk.Commons;
using Elfisk.Commons.Castle;
using Elfisk.ECS.Core;
using Elfisk.ECS.Core.Implementation;
using Wings.Blueprint;
using Wings.Blueprint.Aircraft;
using Wings.Blueprint.Missiles;
using Wings.Blueprint.Physics;

namespace Wings
{
  public static class Program
  {
    [STAThread]
    static void Main()
    {
      IDependencyContainer container = new DependencyContainer(new WindsorContainer());

      container.AddComponent<IEntityRepository, InMemoryEntityRepository>();
      container.AddComponent<ISystem, PhysicsSystem>();
      container.AddComponent<ISystem, AircraftSystem>();
      container.AddComponent<ISystem, MissileSystem>();

      GameEnvironment environment = new GameEnvironment(container, TimeSpan.FromMilliseconds(100));

      WingsInitializer.Initialize(environment);

      using (var game = new WingsGame(environment))
        game.Run();
    }
  }
}
