using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using Color = System.Windows.Media.Color;
	//using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace Mandel
{
	public class WPFFractal
	{
		private Window window;

		private Image image;

		private WBitmap bitmap;

		private const int MAX_JULIA = 500;

		public ComplexFunction ComplexFunc { private get; set; }

		public PaletteMap Palette { private get; set; }

		private int Width { get; set; }

		private int Height { get; set; }

		private volatile bool stopRequested;

		private bool resizing;

		private Complex[,] zoomMap;

		private ComplexResult[,] resultMap;

		private Complex zoomMin;

		private Complex zoomMax;

		private ComplexPlane plane;

		private Thread worker;

		private bool dragging = false;

		private bool zooming = false;

		private Point startPoint = new Point(0, 0);
		private Point endPoint = new Point(0, 0);

		private Rectangle theRect = new Rectangle(0, 0, 0, 0);

		public WPFFractal(int width = 800, int height = 600)
		{
			this.Width = width;
			this.Height = height;

			image = new Image();
			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
			RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

			window = new Window
			{
				Content = image,
				Width = width,
				Height = height
			};

			window.Show();

			bitmap = new WBitmap(
				(int)window.ActualWidth,
				(int)window.ActualHeight,
				PixelFormats.Bgr32);

			image.Source = bitmap.Bitmap;
			image.Stretch = Stretch.None;
			image.HorizontalAlignment = HorizontalAlignment.Left;
			image.VerticalAlignment = VerticalAlignment.Top;

			this.plane = new ComplexPlane(ComplexFunc.Min, ComplexFunc.Max, this.Width, this.Height );
		}

		private void Render()
		{
			window.Title = String.Format("{0} in progress", ComplexFunc.Label);

			if (ComplexFunc.UseDwell)
			{
				for (int x = 0; x < Width; x++)
				{
					double real = plane.Real(x);

					for (int y = 0; y < Height; y++)
					{
						double imag = plane.Imaginary(y);

						Complex c = new Complex(real, imag);

						var result = ComplexFunc.Function.Invoke(c);

						bitmap.DrawPixel(x, y, ComplexFunc.Smooth ? GetColor(result.Dwell, result.Complex) : GetColor(result.Dwell));

					}
				}
			}
		}

		private Color GetColor(int dwell, Complex complex)
		{
			double di = (double)dwell;
			double r2 = complex.Real * complex.Real;
			double i2 = complex.Imaginary * complex.Imaginary;

			double zn = Math.Sqrt(r2 + i2);
			double hue = di + 1.0 - Math.Log(Math.Log(Math.Abs(zn))) / Math.Log(2.0);
			hue = 0.95 + 20.0 * hue;

			if (double.IsNaN(hue))
			{
				return Colors.Black;
			}

			while (hue > 360)
			{
				hue -= 360.0;
			}

			while (hue < 0.0)
			{
				hue += 360.0;
			}

			return ColourMap.ColorFromHSV(hue, 0.8, 1.0);
		}

		private Color GetColor(int index)
		{
			if (index >= Palette.Colors.Count)
			{
				index = index % Palette.Colors.Count;
			}

			return Palette.Colors[index];
			//const double MaxColor = 256;
			//const double ContrastValue = 0.2;
			//return Color.FromArgb(0, 0, (int)((MaxColor * Math.Pow(value, ContrastValue)) % 255));
		}
	}
}
