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

    private const float zeroBound = 0.0001f;

    public static Vector2 Round(Vector2 v)
    {
      Vector2 v2 = RoundX(v);
      return new Vector2(
        v2.X > -zeroBound && v2.X < zeroBound ? 0f : v2.X,
        v2.Y > -zeroBound && v2.Y < zeroBound ? 0f : v2.Y);
    }

    private static Vector2 RoundX(Vector2 v) => new Vector2(
      MathF.Round(v.X, 2),
      MathF.Round(v.Y, 2));

    public static Vector3 Round(Vector3 v)
    {
      Vector3 v2 = RoundX(v);
      return new Vector3(
        v2.X > -zeroBound && v2.X < zeroBound ? 0f : v2.X,
        v2.Y > -zeroBound && v2.Y < zeroBound ? 0f : v2.Y,
        v2.Z > -zeroBound && v2.Z < zeroBound ? 0f : v2.Z);
    }

    private static Vector3 RoundX(Vector3 v) => new Vector3(
      MathF.Round(v.X, 2),
      MathF.Round(v.Y, 2),
      MathF.Round(v.Z, 2));
  }
}
