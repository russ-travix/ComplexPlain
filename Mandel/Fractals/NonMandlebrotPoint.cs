using System;
using System.Numerics;

namespace Mandel.Fractals
{

	public class NonMandlebrotPoint
	{
		public int X { get; set; }

		public int Y { get; set; }

		public Complex Z { get; set; }

		public NonMandlebrotPoint(int X, int Y, Complex Z)
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}

		public void Peturb()
		{
			Random rng = new Random();

			this.X += rng.Next(-1, 1);
			this.Y += rng.Next(-1, 1);
		}
	}
}