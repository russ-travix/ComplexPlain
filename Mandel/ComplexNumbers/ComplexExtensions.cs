using System;
using System.Numerics;

namespace Mandel
{
	public static class ComplexExtensions
	{
		public static Complex Square(this Complex z)
		{
			double ar_temp = z.Real * z.Real - z.Imaginary * z.Imaginary;
			double ai = 2 * z.Real * z.Imaginary;
			double ar = ar_temp;

			return new Complex(ar, ai);
		}

		public static Complex AbsSquare(this Complex z)
		{
			double ar_temp = z.Real * z.Real - z.Imaginary * z.Imaginary;
			double ai = 2 * Math.Abs(z.Real * z.Imaginary);
			double ar = ar_temp;

			return new Complex(ar, ai);
		}

		public static void Test()
		{
			Complex a = new Complex(3, 4);
			
			var resultOne = a * a;
			
			double ar_temp = a.Real * a.Real - a.Imaginary * a.Imaginary;
			double ai = 2 * a.Real * a.Imaginary;
			double ar = ar_temp;

			Complex resultTwo = new Complex(ar, ai);

			bool yeah = resultOne == resultTwo;
		}

		public static Complex BurningShip(this Complex z, Complex c)
		{
			Complex o = new Complex(
					z.Real * z.Real - z.Imaginary * z.Imaginary - c.Real,
					2 * Math.Abs(z.Real * z.Imaginary) - c.Imaginary
				);

			return o;
		}
	}
}
