using System;
using System.Windows.Media;

namespace Mandel.Primes
{
	public class SpiralFunc
	{
		public string Label { get; set; }

		public Func<int, int, int, int> Function { get; set; }

		public Color BackgroundColor { get; set; }

		public SpiralFunc()
		{
			BackgroundColor = Colors.Black;
		}
	}

}
