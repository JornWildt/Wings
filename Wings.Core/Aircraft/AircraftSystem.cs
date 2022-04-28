using System;
using System.Threading.Tasks;
using Elfisk.ECS.Core;

namespace Wings.Core.Aircraft
{
  public class AircraftSystem : ISystem
  {
    public Task Update(GameEnvironment environment, TimeSpan elapsedTime)
    {
      foreach (var aircraft in environment.Entities.GetComponents<AircraftComponent>())
      {
        aircraft.Update((WingsEnvironment)environment, elapsedTime);
      }

      return Task.CompletedTask;
    }
  }
}
