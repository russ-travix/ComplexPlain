using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Input;

using Mandel.Bitmaps;

using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Mandel.Bitmaps
{

	public class DrawableWindow
	{
		private static WBitmap bitmap;

		private static Image image;

		protected static Window window;

		protected int Width;

		protected int Height;

		private int mouseX;

		private int mouseY;

		public DrawableWindow(int width = 800, int height = 600)
		{
			this.Width = width;
			this.Height = height;

			mouseX = width / 2;
			mouseY = height / 2;

			image = new Image();
			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
			RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

			window = new Window { Content = image, Width = width, Height = height };

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

		public void SetPixel(int x, int y, Color color)
		{
			bitmap.DrawPixelSafe(x, y, color);
		}

		public void DrawLine(int x1, int y1, int x2, int y2, Color colour)
		{
			bitmap.DrawLine(x1, y1, x2, y2, colour);
		}

		public void Clear(Color color)
		{
			bitmap.ClearSafe(color);
		}
	}
}