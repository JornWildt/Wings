using System;
using Microsoft.Xna.Framework;

namespace Wings.Blueprint
{
  public static class Converters
  {
    public static float MetersSecondToKilometersHour(float s) => s * 3.6f;

    // Input X:pitch, y:yaw
    public static Vector3 RotationRadiansToUnitVector(Vector2 rotation) => new Vector3(
        MathF.Cos(rotation.Y) * MathF.Cos(rotation.X),
        MathF.Sin(rotation.Y) * MathF.Cos(rotation.X),
        MathF.Sin(rotation.X));


    // Output x:pitch, y:yaw
    public static Vector2 UnitVectorToRotationRadians(Vector3 direction, float rotationForVertical)
    {
      if (direction.Z < 1)
      {
        if (direction.X > 0)
        {
          if (direction.Y >= 0)
          {
            return new Vector2(
                MathF.Atan(direction.Z / MathF.Sqrt(direction.X * direction.X + direction.Y * direction.Y)),
                MathF.Atan(direction.Y / direction.X));
          }
          else
          {
            return new Vector2(
                MathF.Atan(direction.Z / MathF.Sqrt(direction.X * direction.X + direction.Y * direction.Y)),
                MathF.Atan(direction.Y / direction.X));
          }
        }
        else if (direction.X < 0)
        {
          if (direction.Y >= 0)
          {
            return new Vector2(
              MathF.Atan(direction.Z / MathF.Sqrt(direction.X * direction.X + direction.Y * direction.Y)),
              Angles.HalfCircle + MathF.Atan(direction.Y / direction.X));
          }
          else
          {
            return new Vector2(
              MathF.Atan(direction.Z / MathF.Sqrt(direction.X * direction.X + direction.Y * direction.Y)),
              -(Angles.HalfCircle - MathF.Atan(direction.Y / direction.X)));
          }
        }
        else
        {
          return new Vector2(
              MathF.Atan(direction.Z / MathF.Sqrt(direction.X * direction.X + direction.Y * direction.Y)),
              Angles.QuarterCircle * MathF.Sign(direction.Y));
        }
      }
      else
      {
        // Vertical vector
        return new Vector2(Angles.QuarterCircle, rotationForVertical);
      }
    }


    // Round() since Dot() has been seen returning 1.0000002 even on two unit vectors.
    public static float AngleBetweenVectors(Vector3 a, Vector3 b) => MathF.Acos(MathF.Round(Vector3.Dot(Vector3.Normalize(a), Vector3.Normalize(b)), 6));
  }
}
