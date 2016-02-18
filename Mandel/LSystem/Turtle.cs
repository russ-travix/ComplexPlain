using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace Mandel.LSystem
{
	public class Turtle
	{
		private readonly WriteableBitmap canvas;
		private readonly Color penColor;

		private readonly bool fancy;
		private readonly int width;
		private readonly int height;

		private readonly double penSize;

		public Turtle(Size canvasSize) : this(canvasSize, Colors.Black, true, 1, 0, 0, 0) { }

		public Turtle(Size canvasSize, Color color, bool fancyColors, double size, double startX, double startY, double startAngle)
		{
			fancy = fancyColors;

			this.width = canvasSize.Width;
			this.height = canvasSize.Height;

			canvas = BitmapFactory.New(this.width, this.height);

			penColor = color;
			penSize = size;

			this.X = startX;
			this.Y = startY;

			this.Angle = startAngle;
		}

		//draws the turtle path, angles assumed to be in degrees
		public WriteableBitmap CreatePath(double distance, double pathAngle, double distanceChange, double angleChange, uint iterations)
		{
			canvas.Clear(Colors.White);

			for (uint i = 0; i != iterations; i++)
			{
				this.Move(distance);
				this.TurnDegrees(pathAngle);

				distance += distanceChange;
				pathAngle += angleChange;
			}

			return canvas;
		}

		private void Move(double distance)
		{
			double newX = this.X + distance * (double)Math.Cos(this.Angle);
			double newY = this.Y + distance * (double)Math.Sin(this.Angle);

			Color col = fancy
				? HSLtoRGB(this.ProperAngle(this.Angle), 1.0, 1.0)
				: penColor;

			var px = this.ToCanvasCoordinates(this.X, this.Y);
			var py = this.ToCanvasCoordinates(newX, newY);

			canvas.DrawLine(px.X, px.Y, py.X, py.Y, col);

			this.X = newX;
			this.Y = newY;
		}

		private void InvisibleMove(double distance)
		{
			this.X += distance * (double)Math.Cos(this.Angle);
			this.Y += distance * (double)Math.Sin(this.Angle);
		}

		private Point ToCanvasCoordinates(double ox, double oy)
		{
			return new Point((int)Math.Round(ox + this.width / 2), (int)Math.Round(this.height / 2 - oy));
		}

		private void TurnDegrees(double degrees)
		{
			this.Turn((double)Math.PI * degrees / 180f);
		}

		private void Turn(double radians)
		{
			this.Angle += radians;
		}

		private double NewRange(double val, double oMin, double oMax, double nMin, double nMax)
		{
			return (nMax - nMin) * (val - oMin) / (oMax - oMin) + nMin;
		}

		//puts reflex angle in 0 to 2pi range
		private double ProperAngle(double a)
		{
			a %= 2.0 * Math.PI;

			if (a < 0)
			{
				a += 2.0 * Math.PI;
			}

			return a;
		}

		private Color HSLtoRGB(double hue, double saturation, double val)
		{
			double r = 0;
			double g = 0;
			double b = 0;

			hue /= 2 * Math.PI;

			if (saturation == 0)
			{
				r = g = b = val;
			}
			else
			{
				if (hue == 1)
				{
					hue = 0;
				}
				double z = Math.Floor(hue * 6);
				int i = (int)(z);
				double f = hue * 6 - z;
				double p = val * (1 - saturation);
				double q = val * (1 - saturation * f);
				double t = val * (1 - saturation * (1 - f));

				switch (i)
				{
					case 0: r = val; g = t; b = p; break;
					case 1: r = q; g = val; b = p; break;
					case 2: r = p; g = val; b = t; break;
					case 3: r = p; g = q; b = val; break;
					case 4: r = t; g = p; b = val; break;
					case 5: r = val; g = p; b = q; break;
				}
			}

			byte R = (byte)(256 * r);
			byte G = (byte)(256 * g);
			byte B = (byte)(256 * b);

			if (R > 255)
				R = 255;
			if (G > 255)
				G = 255;
			if (B > 255)
				B = 255;
			
			return Color.FromArgb(255, R, B, G);
		}

		public double X { get; private set; }

		public double Y { get; private set; }

		public double Angle { get; private set; }
	}
}
