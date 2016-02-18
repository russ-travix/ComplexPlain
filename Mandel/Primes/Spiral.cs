using System;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

using Mandel.Bitmaps;
using Mandel.Colours;

using Color = System.Windows.Media.Color;

namespace Mandel.Primes
{
	public class Spiral : DrawableWindow
	{
		public PaletteMap Palette = new PaletteMap();

		public bool RenderSpiralMatrix { get; set; }

		public Spiral(SpiralFunc func) : base(800, 800)
		{
			this.func = func;

			window.KeyUp += (s, a) =>
			{
				switch (a.Key)
				{
					case Key.L:
						using (OpenFileDialog fopen = new OpenFileDialog())
						{
							fopen.Multiselect = false;
							fopen.AutoUpgradeEnabled = true;

							if (fopen.ShowDialog() == DialogResult.OK)
							{
								this.Palette = new PaletteMap(fopen.FileName);
							}

							this.Render();
						}
						break;
				}
			};
		}

		public void Render()
		{
			if (RenderSpiralMatrix)
			{
				this.DoSpiralMatrix();
			}
			else
			{
				this.DoUlamSpiral();
			}
		}

		private readonly SpiralFunc func;

		private void Plot(int value, int xp, int yp)
		{
			int result = this.func.Function.Invoke(value, xp, yp);

			if (result >= this.Palette.Colors.Count)
			{
				result = result % this.Palette.Colors.Count;
			}

			var color = this.Palette.GetColor(result);

			this.SetPixel(xp, yp, Color.FromRgb(color.R, color.G, color.B));
		}

		public void DoSpiralMatrix()
		{
			this.Clear(func.BackgroundColor);

			int n = 800;
			int pos = 0;
			int count = 800;
			int val = -800;
			int sum = -1;

			do
			{
				val = -1 * val / n;
				for (int i = 0; i < count; i++)
				{
					sum += val;

					int x = sum / n;
					int y = sum % n;

					this.Plot(pos++, x, y);
				}

				val *= n;
				count--;
				for (int i = 0; i < count; i++)
				{
					sum += val;

					int x = sum / n;
					int y = sum % n;

					this.Plot(pos++, x, y);
				}
			}
			while (count > 0);
		}

		public void DoUlamSpiral(int i = 0, int maxI = 640000)
		{
			this.Clear(Colors.White);

			int w = this.Width;
			int h = this.Height;

			int x = 0;
			int y = 0;

			int orgX = this.Width / 2;
			int orgY = this.Height / 2;

			int dx = 0;
			int dy = -1;

			int t = Math.Max(w, h);

			int negW2 = -w / 2;
			int divW2 = w / 2;
			int negH2 = -h / 2;
			int divH2 = h / 2;

			for (; i < maxI; i++)
			{
				if ((negW2 <= x) && (x < divW2) && (negH2 <= y) && (y <= divH2))
				{
					int xp = orgX + x;
					int yp = orgY + y;

					if (xp < 0) xp = 0;
					if (xp >= w) xp = w - 1;
					if (yp < 0) yp = 0;
					if (yp >= h) yp = h - 1;

					this.Plot(i, xp, yp);
				}

				if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y)))
				{
					t = dx;
					dx = -dy;
					dy = t;
				}

				x += dx;
				y += dy;
			}

		}

		public void DoRealSpiral()
		{
			this.Clear(Colors.Black);

			int ox = this.Width / 2;
			int oy = this.Height / 2;
			int index = 0;

			var gap = 1.1;

			var steps = 1440;

			var increment = 2 * Math.PI / steps;

			var theta = increment;

			while (theta < 450 * Math.PI)
			{
				var nx = (int)(ox + theta * Math.Cos(theta) * gap);
				var ny = (int)(oy + theta * Math.Sin(theta) * gap);

				if ((nx >= 0 && nx < this.Width) && (ny >= 0 && ny < this.Height))
				{
					this.Plot(index++, nx, ny);
				}

				theta += increment;

				//gap += 0.001;
			}
		}

		public void DoZigzagSpiral()
		{
			int w = Width - 1;
			int h = Height - 1;

			this.Clear(Colors.Black);

			int i = 0;
			int j = 0;

			int d = -1;

			int start = 0;
			int end = w * h;

			do
			{
				this.Plot(start++, i, j);
				this.Plot(end--, w - i, h - j);

				i += d;
				j -= d;

				if (i < 0)
				{
					i++;
					d = -d;
				}
				else if (j < 0)
				{
					j++;
					d = -d;
				}
			}
			while (start < end);
			if (start == end)
			{
				this.Plot(start,i,j);
			}
		}
	}
}
