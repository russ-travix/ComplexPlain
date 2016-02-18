using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Mandel.Bitmaps;

using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Mandel.Raycaster
{
	public class Vector
	{
		float x;
		float y;
		float z;

		public Vector(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector Add(Vector r)
		{
			return new Vector(x + r.x, y + r.y, z + r.z);
		}

		public Vector Mul(float r)
		{
			return new Vector(x * r, y * r, z * r);
		}

		public float Dot(Vector r)
		{
			return x * r.x + y * r.y + z * r.z;
		}

		public Vector Crs(Vector r)
		{
			return new Vector(y * r.z - z * r.y, z * r.x - x * r.z, x * r.y - y * r.x);
		}

		public Vector Nrm()
		{
			double dot = this.Dot(this);
			float sqrt = (float)System.Math.Sqrt(dot);

			return this.Mul(1 / sqrt);
		}
	}

	public class Raycaster
	{
		Random r = new Random();

		private int[] G = { 247570, 280596, 280600, 249748, 18578, 18577, 231184, 16, 16 };

		private Vector g = new Vector(-6, 16, 0).Nrm();
		private Vector a;
		private Vector bb;

		private Vector b;
		private Vector c;
		

		private Window window;

		private WBitmap surface;

		private Image image;

		private int width;

		private int height;

		private double R()
		{
			return r.NextDouble();
		}

		public Raycaster(int width = 800, int height = 600)
		{
			a = ((new Vector(0, 0, 1).Crs(g)).Mul(0.002f)).Nrm();
			bb = g.Crs(a);
			b = ((g.Crs(a)).Mul(0.002f)).Nrm();

			var ab = a.Add(b);
			var abmg = g.Mul(-256);

			c = ab.Mul(abmg.Dot(abmg));

			this.width = width;
			this.height = height;

			image = new Image();
			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
			RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

			window = new Window
				{
					Content = image, Width = width, Height = height
				};

			window.Show();

			surface = new WBitmap(
				(int)window.ActualWidth,
				(int)window.ActualHeight,
				PixelFormats.Bgr32);

			image.Source = surface.Bitmap;
			image.Stretch = Stretch.None;
			image.HorizontalAlignment = HorizontalAlignment.Left;
			image.VerticalAlignment = VerticalAlignment.Top;
			//image.MouseMove += ImageMouseMove;
			//image.KeyDown += ImageOnKeyDown;
		}

		
	}
}
