using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Mandel.DiamondSquare
{
	public class TPoint
	{
		public float X { get; set; }
		public float Y { get; set; }

		public TPoint(float x, float y)
		{
			X = x;
			Y = y;
		}

		public override string ToString()
		{
			return String.Format("x: {0}, y: {1}", X, Y);
		}
	}

	public class Terrain
	{
		private readonly int Size;

		private readonly int Max;

		private readonly float[,] Map;

		public Image BackgroundImage { get; set; }

		public Terrain(Image backgroundImage, float detail = 9)
		{
			Size = (int)Math.Pow(2, detail) + 1;
			Max = Size - 1;
			Map = new float[Size,Size];
			BackgroundImage = backgroundImage;
		}

		public float GetMap(int x, int y)
		{
			if (x < 0 || x > Max || y < 0 || y > Max)
			{
				return -1;
			}

			return Map[x,y];
		}

		public void SetMap(int x, int y, float val)
		{
			Map[x,y] = val;
		}

		public void Generate(float roughness)
		{
			SetMap(0, 0, Max);
			SetMap(Max, 0, (float)Max / 2);
			SetMap(Max, Max, 0);
			SetMap(0, Max, (float)Max / 2);

			Divide(roughness, Max);
		}

		private void Divide(float roughness, int size)
		{
			int x, y, half = size / 2;

			var scale = roughness * size;
			if (half < 1)
			{
				return;
			}

			Random rnd = new Random();

			for (y = half; y < Max; y += size)
			{
				for (x = half; x < Max; x += size)
				{
					Square(x, y, half, (float)rnd.NextDouble() * scale * 2 - scale);
				}
			}

			for (y = 0; y <= Max; y += half)
			{
				for (x = ( y + half ) % size; x <= Max; x += size)
				{
					Diamond(x, y, half, (float)rnd.NextDouble() * scale * 2 - scale);
				}
			}

			Divide(roughness, size / 2);
		}

		private static float Average(params float[] values)
		{
			var valid = values.Where(v => v != -1).ToArray();
			var total = valid.Sum();

			return total / valid.Length;
		}

		private void Square(int x, int y, int size, float d)
		{
			var ul = GetMap(x - size, y - size);
			var ur = GetMap(x + size, y - size);
			var lr = GetMap(x + size, y + size);
			var ll = GetMap(x - size, y + size);

			SetMap(x, y, Average(ul, ur, lr, ll) + d);
		}

		private void Diamond(int x, int y, int size, float d)
		{
			var t = GetMap(x, y - size);
			var r = GetMap(x + size, y);
			var b = GetMap(x, y + size);
			var l = GetMap(x - size, y);

			SetMap(x, y, Average(t, r, b, l) + d);
		}

		public void Draw()
		{
			int width = BackgroundImage.Width;
			int height = BackgroundImage.Height;

			Bitmap finalBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
			
			Graphics gr = Graphics.FromImage(finalBitmap);
			gr.Clear(Color.Black);

			Action<TPoint, TPoint, Color> rect = (top, bot, color) =>
				{
					Rectangle r = new Rectangle((int)top.X, (int)top.Y, (int)(bot.X - top.X)+1, 2);
					gr.DrawRectangle(new Pen(color), r);
				};

			Func<int, int, TPoint> iso = (x, y) => new TPoint(0.5f * (Size + x - y), 0.5f * (x + y));

			Func<int, int, float, Color> brightness = (x, y, slope) =>
				{
					if (x == Max || y == Max)
					{
						return Color.Black;
					}

					var b = (int)Math.Truncate(slope * 50) + 128;
					
					if (b < 0)
						b = 0;

					if (b > 255)
						b = 255;

					return Color.FromArgb(b, b, b);

				};

			Func<int, int, float, TPoint> project = (fx, fy, fz) =>
				{
					var Iso = iso(fx, fy);

					var x0 = width * 0.5f;
					var y0 = height * 0.2f;

					var z = Size * 0.5f - fz + Iso.Y * 0.75f;
					var x = ( Iso.X - Size * 0.5f ) * 6;
					var y = ( Size - Iso.Y ) * 0.005f + 1;

					var xx = (int)Math.Abs(x0 + x / y);
					var yy = (int)Math.Abs(y0 + z / y);

					if (xx >= width)
						xx = width - 1;
					if (yy >= height)
						yy = height - 1;

					return new TPoint(xx, yy);
				};


			var water = Size * 0.1f;

			for (int x = 0; x < Size; x++)
			{
				for (int y = 0; y < Size; y++)
				{
					var val = GetMap(x, y);
					var top = project(x, y, val);
					var bot = project(x + 1, y, 0);
					var wat = project(x, y, water);
					var sty = brightness(x, y, GetMap(x + 1, y) - val);

					//finalBitmap.SetPixel(x, y, sty);
					//gr.DrawRectangle(new Pen(sty),x,y,1,1 );
					rect(top, bot, sty);
					rect(wat, bot, Color.FromArgb(50, 150, 200));
				}
			}

			if (this.BackgroundImage != null)
			{
				this.BackgroundImage.Dispose();
			}

			this.BackgroundImage = finalBitmap;
		}
	}
}