using System;
using System.Threading.Tasks;
using Elfisk.ECS.Core;

namespace Wings.Blueprint.Aircraft
{
  public class AircraftSystem : ISystem
  {
    public Task Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
      foreach (var aircraft in environment.Entities.GetComponents<AircraftComponent>())
      {
        aircraft.Update(environment, elapsedTime);
      }

      return Task.CompletedTask;
    }
  }
}
