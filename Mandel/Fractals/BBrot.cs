using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Input;

using Mandel.Bitmaps;

using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace Mandel.Fractals
{
	public class BBrot
	{
		private static WBitmap bitmap;

		private static Window window;

		private static Image image;

		private int width = 1024;

		private int height = 768;

		private int[] counters;

		private int maxCount = 0;

		private int numpoints = 8000000;

		private int maxIter = 256;

		public BBrot()
		{
			counters = new int[width * height];

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
				PixelFormats.Pbgra32);

			image.Source = bitmap.Bitmap;
			image.Stretch = Stretch.None;
			image.HorizontalAlignment = HorizontalAlignment.Left;
			image.VerticalAlignment = VerticalAlignment.Top;
		}

		private bool inset(Complex c)
		{
			Complex z = new Complex();

			if (c.Mag() > 16)
			{
				return false;
			}

			for (int i = 0; i < maxIter; i++)
			{
				z = z * z + c;
				if (z.Mag() >= 4)
				{
					return false;
				}
			}

			return true;
		}

		public void Render()
		{
			Random rnd = new Random();
			int pindex = 0;

			for (int i = 0; i < numpoints; i++)
			{
				Complex random = new Complex(((rnd.NextDouble() * 16) - 8.0), ((rnd.NextDouble() * 16) - 8.0));

				if (!this.inset(random))
				{
					Complex org = new Complex(random.Real, random.Imaginary);

					for (int j = 0; j < maxIter; j++)
					{
						int pix = (int)(width * (random.Real + 2.0) / 4.0);
						int piy = (int)(height * (random.Imaginary + 2.0) / 4.0);

						if ((pix > 0 && pix < width) && (piy > 0 && piy < height))
						{
							pindex = height * piy + pix;
							counters[pindex]++;

							if (counters[pindex] > maxCount)
							{
								maxCount = counters[pindex];
							}

							random = random * random + org;
						}
						else
						{
							j = maxIter;
						}
					}
				}
			}

			pindex = 0;

			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					int temp_colour = counters[pindex];
					byte c = 0;

					if (temp_colour > 0)
					{
						c = (byte)((Math.Log(temp_colour) / Math.Log(maxCount + 1)) * 255);
					}

					bitmap.DrawPixelSafe(j, i, Color.FromRgb(c,c,c));

					pindex++;
				}
			}
		}
	}
}
