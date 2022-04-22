using System;
using Microsoft.Xna.Framework;

namespace Wings.Blueprint
{
  public static class Converters
  {
    public static float MetersSecondToKilometersHour(float s) => s * 3.6f;

    public static Vector3 RotationRadiansToUnitVector(Vector3 rotation) => new Vector3(
        MathF.Cos(rotation.Z) * MathF.Cos(rotation.Y),
        MathF.Sin(rotation.Z) * MathF.Cos(rotation.Y),
        MathF.Sin(rotation.Y));

    public static float AngleBetweenVectors(Vector3 a, Vector3 b) => MathF.Acos(Vector3.Dot(Vector3.Normalize(a), Vector3.Normalize(b)));
  }
}
