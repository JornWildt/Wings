using System;
using Microsoft.Xna.Framework;

namespace Wings.Blueprint
{
  public static class Converters
  {
    public static float MetersSecondToKilometersHour(float s) => s * 3.6f;

    public static Vector3 RotationRadiansToUnitVector(Vector3 rotation) => new Vector3(
        (float)(Math.Cos(rotation.Z) * Math.Cos(rotation.Y)),
        (float)(Math.Sin(rotation.Y)),
        (float)(Math.Sin(rotation.Z) * Math.Cos(rotation.Y)));
  }
}
