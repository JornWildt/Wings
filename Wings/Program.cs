using System;
using Castle.Windsor;
using Elfisk.Commons;
using Elfisk.Commons.Castle;
using Elfisk.ECS.Core;
using Elfisk.ECS.Core.Implementation;
using Wings.Blueprint;
using Wings.Core;
using Wings.Core.Aircraft;
using Wings.Core.Missiles;
using Wings.Core.Physics;

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

      using (var game = new WingsGame(environment))
        game.Run();
    }
  }
}
