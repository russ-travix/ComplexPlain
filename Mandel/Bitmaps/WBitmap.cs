using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mandel.Bitmaps
{
	public class WBitmap
	{
		public WriteableBitmap Bitmap { get; private set; }

		private readonly int width;

		private readonly int height;

		public WBitmap(int width, int height, PixelFormat format, float dpi = 96, BitmapPalette palette = null)
		{
			this.width = width;
			this.height = height;

			Bitmap = new WriteableBitmap(
				width, 
				height,
				dpi,
				dpi,
				format,
				palette);
		}

		~WBitmap()
		{
			Bitmap = null;
		}

		public void Pixels(int index, int color)
		{
			Bitmap.Lock();

			unsafe
			{
				int pbb = (int)Bitmap.BackBuffer;

				pbb += index * 4;

				*((int*)pbb) = color;
			}

			int y = index / (Bitmap.BackBufferStride / 4);
			int x = index % (Bitmap.BackBufferStride / 4);

			Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
			Bitmap.Unlock();
		}

		public void DrawPixel(int x, int y, Color c)
		{
			Bitmap.Lock();

			lock (Bitmap)
			{
				unsafe
				{
					int pbb = (int) Bitmap.BackBuffer;

					pbb += y * Bitmap.BackBufferStride;
					pbb += x * 4;

					int colorData = ( c.B << 24 ) | ( c.G << 16 ) | ( c.R << 8 ) | c.A;
					
					*( (int*) pbb ) = colorData;
				}

				Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
			}

			Bitmap.Unlock();
		}

		public void DrawPixel(int x, int y, int colorData)
		{
			Bitmap.Lock();

			unsafe
			{
				int pbb = (int)Bitmap.BackBuffer;

				pbb += y * Bitmap.BackBufferStride;
				pbb += x * 4;

				*((int*)pbb) = colorData;
			}

			Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
			Bitmap.Unlock();
		}

		public void DrawPixelSafe(int x, int y, Color c)
		{
			//byte[] colorData = {c.B, c.G, c.R, c.A};
			//var rect = new Int32Rect(x, y, 1, 1);

			//Bitmap.WritePixels(rect, colorData, 4, 0);
			Bitmap.DrawRectangle(x, y, x + 1, y + 1, c);
		}

		public void ErasePixel(int x, int y)
		{
			//byte[] colorData = { 0, 0, 0, 0 };
			//var rect = new Int32Rect(x, y, 1, 1);

			//Bitmap.WritePixels(rect, colorData, 4, 0);

			Bitmap.DrawRectangle(x, y, x + 1, y + 1, Colors.Black);
		}

		public void ClearSafe(Color c)
		{
			//byte[] colorData = { c.B, c.G, c.R, c.A };
			//var rect = new Int32Rect(0, 0, this.width, this.height);

			//Bitmap.WritePixels(rect, colorData, 4, 0);
			Bitmap.Clear(c);
		}

		public void Clear(Color c)
		{
			int colorData = ( c.B << 24 ) | ( c.G << 16 ) | ( c.R << 8 ) | c.A;

			Bitmap.Lock();

			unsafe
			{
				int pbb = (int) Bitmap.BackBuffer;

				for (int x = 0; x < Bitmap.Width; x++)
				{
					for (int y = 0; y < Bitmap.Height; y++)
					{
						*( (int*) pbb ) = colorData;

						pbb += 4;
					}
				}
			}

			Bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));

			Bitmap.Unlock();
		}
	}
}