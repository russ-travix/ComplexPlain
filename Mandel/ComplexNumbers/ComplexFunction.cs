using System;
using System.Numerics;

namespace Mandel
{
	public class ComplexResult
	{
		public Complex Complex { get; set; }

		public int Dwell { get; set; }

		public ComplexResult(Complex c)
		{
			Complex = c;
		}

		public static implicit operator ComplexResult(Complex c)
		{
			return new ComplexResult(c)
			{
				Dwell = -1
			};
		}
	}

	public class ComplexFunction
	{
		public string Label { get; set; }

		public Func<Complex, int, int, int, ComplexResult> Function { get; set; }

		public bool UseDwell { get; set; }
		public bool AllowJulia { get; set; }
		public bool Smooth { get; set; }

		public Complex Min = new Complex(-2.2, -1.4);
		public Complex Max = new Complex(1.0, 1.4);
	}
}