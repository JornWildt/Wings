using System;
using Microsoft.Xna.Framework;

namespace Test
{
  class Program
  {
    static void Main(string[] args)
    {
      Matrix m = new Matrix();
      Console.WriteLine(m);

      m = Matrix.Identity;
      Console.WriteLine(m);

      m = Matrix.CreateRotationX(0) * Matrix.CreateRotationY(0) * Matrix.CreateRotationZ(0);
      Console.WriteLine(m);
    }
  }
}
