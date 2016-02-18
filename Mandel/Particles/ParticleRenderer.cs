using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Input;

using Mandel.Bitmaps;

using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Mandel.Particles
{
	public class ParticleRenderer
	{
		private const int MAX_PARTICLES = 100;

		private static WBitmap bitmap;

		private static Window window;

		private static Image image;

		private Particle[] particles;

		private Timer timer;

		private int width;

		private int height;

		private int mouseX;

		private int mouseY;

		public ParticleRenderer(int width = 800, int height = 600)
		{
			this.width = width;
			this.height = height;

			mouseX = width / 2;
			mouseY = height / 2;

			image = new Image();
			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
			RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

			window = new Window
				{
					Content = image, Width = width, Height = height
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
			image.MouseMove += image_MouseMove;
			
			initParticles();
		}

		public void Render()
		{
			renderFrame();

			timer = new Timer { Interval = 30 };

			timer.Tick += TimerOnTick;

			timer.Enabled = true;
		}

		private void initParticles()
		{
			Random random = new Random();

			particles = new Particle[MAX_PARTICLES];

			for (int i = 0; i < MAX_PARTICLES; i++)
			{
				particles[i] = new Particle(random.Next(width), random.Next(height), this.width, this.height);
			}
		}

		private void renderFrame()
		{
			bitmap.ClearSafe(Colors.Black);

			for (int i = 0; i < MAX_PARTICLES; i++)
			{
				particles[i]
					.Attract(mouseX, mouseY)
					.Integrate();

				particles[i].Draw(bitmap);
			}
		}

		private void TimerOnTick(object sender, EventArgs e)
		{
			timer.Stop();

			try
			{
				renderFrame();
			}
			finally 
			{
				timer.Start();
			}

		}

		private void image_MouseMove(object sender, MouseEventArgs e)
		{
			mouseX = (int) e.GetPosition(image).X;
			mouseY = (int) e.GetPosition(image).Y;

			window.Title = String.Format("X: {0} - Y: {1}", mouseX, mouseY);

			//renderFrame();

			//if (e.LeftButton == MouseButtonState.Pressed)
			//{
			//	bitmap.DrawPixel(x, y, Colors.White);
			//}
			//else if (e.RightButton == MouseButtonState.Pressed)
			//{
			//	bitmap.ErasePixel(x, y);
			//}
		}

		private void ImageOnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			Matrix matrix = image.RenderTransform.Value;

			if (e.Delta > 0)
			{
				matrix.ScaleAt(
				    1.5,
				    1.5,
				    e.GetPosition(window).X,
				    e.GetPosition(window).Y);
			}
			else
			{
				matrix.ScaleAt(
				    1.0 / 1.5,
				    1.0 / 1.5,
				    e.GetPosition(window).X,
				    e.GetPosition(window).Y);
			}

			image.RenderTransform = new MatrixTransform(matrix);
		}
	}

}