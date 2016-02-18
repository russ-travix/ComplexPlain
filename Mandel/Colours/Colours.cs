using System;
using System.Drawing;
using System.Numerics;

namespace Mandel.Colours
{
	public struct ColourTriplet
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }
	}

	public class ColourMap
	{
		private const double TAU = 2.0 * Math.PI;

		public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
		{
			int max = Math.Max(color.R, Math.Max(color.G, color.B));
			int min = Math.Min(color.R, Math.Min(color.G, color.B));

			hue = color.GetHue();
			saturation = (max == 0) ? 0 : 1d - (1d * min / max);
			value = max / 255d;
		}

		public static Color ColorFromHSV(double hue, double saturation, double val)
		{
			int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
			double f = hue / 60 - Math.Floor(hue / 60);

			val = val * 255;
			int v = Convert.ToInt32(val);
			int p = Convert.ToInt32(val * (1 - saturation));
			int q = Convert.ToInt32(val * (1 - f * saturation));
			int t = Convert.ToInt32(val * (1 - (1 - f) * saturation));

			if (hi == 0)
				return Color.FromArgb(255, v, t, p);
			if (hi == 1)
				return Color.FromArgb(255, q, v, p);
			if (hi == 2)
				return Color.FromArgb(255, p, v, t);
			if (hi == 3)
				return Color.FromArgb(255, p, q, v);
			return hi == 4 ? Color.FromArgb(255, t, p, v) : Color.FromArgb(255, v, p, q);
		}

		public static Color ComplexToColour(Complex c)
		{
			if (c == Complex.ImaginaryOne)
			{
				return Color.Black;
			}

			var ct = ComplexToHSV(c);
			var rgb = HsvToRGB(ct);

			int r = (int)Math.Truncate(255.0 * rgb.X);
			int g = (int)Math.Truncate(255.0 * rgb.Y);
			int b = (int)Math.Truncate(255.0 * rgb.Z);

			return Color.FromArgb(r, g, b);
		}

		public static ColourTriplet ComplexToHSV(Complex c)
		{
			double t = c.Phase;

			while (t < 0.0)
			{
				t += TAU;
			}
			
			while (t >= TAU)
			{
				t -= TAU;
			}

			double hue = t / TAU;
			double mag = c.Magnitude;

			double r0 = 0.0;
			double r1 = 1.0;
			while (mag > r1)
			{
				r0 = r1;
				r1 = r1 * Math.E;
			}

			double r = ( mag - r0 ) / ( r1 - r0 );

			double p = r < 0.5 ? 2.0 * r : 2.0 * ( 1.0 - r );
			double q = 1.0 - p;
			double p1 = 1 - q * q * q;
			double q1 = 1 - p * p * p;

			double sat = 0.4 + 0.6 * p1;
			double val = 0.6 + 0.4 * q1;

			return new ColourTriplet() { X = hue, Y = sat, Z = val};
		}

		public static ColourTriplet HsvToRGB(ColourTriplet hsv)
		{
			double h = hsv.X;
			double s = hsv.Y;
			double v = hsv.Z;

			double r, g, b;
			if (Math.Abs(s - 0.0) < double.Epsilon)
			{
				r = g = b = v;
			}
			else
			{
				if (Math.Abs(h - 1.0) < double.Epsilon)
				{
					h = 0.0;
				}

				double z = Math.Truncate(6 * h);
				int i = (int)z;
				double f = h * 6 - z;
				double p = v * (1 - s);
				double q = v * (1 - s * f);
				double t = v * (1 - s * (1 - f));

				if (double.IsNaN(p)) p = 0.0;
				if (double.IsNaN(q)) q = 0.0;
				if (double.IsNaN(v)) v = 0.0;
				if (double.IsNaN(t)) t = 0.0;
				
				switch (i)
				{
					case 0: r = v; g = t; b = p; break;
					case 1: r = q; g = v; b = p; break;
					case 2: r = p; g = v; b = t; break;
					case 3: r = p; g = q; b = v; break;
					case 4: r = t; g = p; b = v; break;
					case 5: r = v; g = p; b = q; break;
					default:r = 0; g = 0; b = 0; break;
				}
			}

			return (new ColourTriplet() { X = r, Y = g, Z = b });
		}
	}
}
