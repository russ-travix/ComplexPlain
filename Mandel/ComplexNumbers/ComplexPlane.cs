using System;
using System.Numerics;

using Mandel.Fractals;

namespace Mandel
{
	/// <summary>
	/// Represents the complex plain and its dimensions.
	/// This class provides helpers to convert a Pixel to it's Complex point and vice versa.
	/// </summary>
	public class ComplexPlane
	{
		private double realScale;
		private double imagScale;

		public ComplexPlane(Complex min, Complex max, int screenWidth, int screenHeight)
		{
			Min = min;
			Max = max;
			ScreenWidth = screenWidth;
			ScreenHeight = screenHeight;

			realScale = ( Max.Real - Min.Real) / ScreenWidth;
			imagScale = ( Max.Imaginary - Min.Imaginary) / ScreenHeight;
		}

		public Complex Min { get; set; }

		public Complex Max { get; set; }


		public int ScreenWidth { get; set; }

		public int ScreenHeight { get; set; }

		public double Real(int x)
		{
			return Min.Real + x * realScale;
		}

		public double Imaginary(int y)
		{
			return Min.Imaginary + y * imagScale;
		}

		public Complex ComplexFromCartesian(int x, int y)
		{
			return new Complex(Real(x), Imaginary(y));
		}

		public MandlebrotPoint CartesianFromComplex(Complex c)
		{
			var result = new MandlebrotPoint(
					(int)Math.Round((c.Real - Min.Real) * ScreenWidth / (Max.Real - Min.Real)),
					(int)Math.Round((c.Imaginary - Min.Imaginary) * ScreenHeight / (Max.Imaginary - Min.Imaginary))
				);

			return result;
		}

		public bool InBounds(MandlebrotPoint p)
		{
			return ( ( p.X > -1 ) && ( p.X < ScreenWidth ) && ( p.Y > -1 ) && ( p.Y < ScreenHeight ) );
		}
	}
}
