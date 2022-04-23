using System;
using Microsoft.Xna.Framework;

namespace Wings.Blueprint
{
  public static class Converters
  {
    public static float MetersSecondToKilometersHour(float s) => s * 3.6f;

    // Input X:roll, y: pitch, z: yaw. Roll is ignored.
    public static Vector3 RotationRadiansToUnitVector(Vector2 rotation) => new Vector3(
        MathF.Cos(rotation.X) * MathF.Cos(rotation.Y),
        MathF.Sin(rotation.X) * MathF.Cos(rotation.Y),
        MathF.Sin(rotation.Y));

    
    public static Vector2 UnitVectorToRotationRadians(Vector3 direction) => new Vector2(
      MathF.Acos(MathF.Round(direction.X / (MathF.Cos(MathF.Asin(direction.Z))))),
      MathF.Asin(direction.Z));


    // Round() since Dot() has been seen returning 1.0000002 even on two unit vectors.
    public static float AngleBetweenVectors(Vector3 a, Vector3 b) => MathF.Acos(MathF.Round(Vector3.Dot(Vector3.Normalize(a), Vector3.Normalize(b)), 6));
  }
}
