using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace Mandel.Fractals
{
	public sealed class BuddhaBrot : Form
	{
		Thread workerThread;

		private const int MaxIterations = 256;

		private Complex Min = new Complex(-2, -1.5);
		private Complex Max = new Complex(1, 1.5);

		private bool HasHandle { get; set; }

		private int OrbitSkip { get; set; }
		private int OrbitMax{ get; set; }
		private int width { get; set; }
		private int height { get; set; }

		private int hitcount = 0;
		private int hits = 0;

		private ComplexPlane plane;

		public BuddhaBrot()
		{
			// form creation
			this.HasHandle = true;
			this.Text = @"Buddhabrot Set Drawing";
			this.BackColor = Color.Black;
			this.BackgroundImageLayout = ImageLayout.Stretch;
			this.MaximizeBox = false;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.ClientSize = new Size(1024, 768);
			this.width = 1024;
			this.height = 768;
			this.OrbitSkip = 8000000;
			this.OrbitMax = Int32.MaxValue;

			plane = new ComplexPlane(Min, Max, width, height);

			this.Load += (s, e) =>
			{
				workerThread = new Thread(Generate)
				{
					IsBackground = true,
					Priority = ThreadPriority.BelowNormal
				};

				workerThread.Start();
			};
		}

		private void Generate(object args)
		{
			var nonPoints = Pregenerate();

			Random rnd = new Random();

			// Make hit count arrays.
			int[,] counts = new int[width, height];

			while (hitcount < OrbitMax)
			{
				var nonPoint = nonPoints[rnd.Next(nonPoints.Count)];
				nonPoint.Peturb();

				if (PointFails(nonPoint))
					continue;
				
				DrawPoint(nonPoint.Z, ref counts);
				
				if (hits > OrbitSkip)
				{
					
					hits = 0;
					var bmp = DisplayBrot(counts);

					if (HasHandle)
					{
						this.BeginInvoke(new SetNewBitmapDelegate(SetNewBitmap), bmp);
						this.BeginInvoke(new UpdateFormTextDelegate(UpdateFormText), String.Format("Total hits: {0}", hitcount));
					}
				}
			}

			var final = DisplayBrot(counts);
			this.BeginInvoke(new SetNewBitmapDelegate(SetNewBitmap), final);
			this.BeginInvoke(new UpdateFormTextDelegate(UpdateFormText), "Bhuddabrot complete");
		}

		private bool PointFails(NonMandlebrotPoint p)
		{
			p.Z = plane.ComplexFromCartesian(p.X, p.Y);
			Complex z = new Complex();

			for (int i = 0; i < MaxIterations; i++)
			{
				z = z * z + p.Z;
				if (z.Magnitude >= 4.0)
				{
					return false;
				}
			}

			return true;
		}

		private void DrawPoint(Complex c, ref int[,] counts)
		{
			Complex z = new Complex();
			//int i = 0;
			while (z.Magnitude < 2)
			{
				z = z * z + c;

				var cp = plane.CartesianFromComplex(z);

				if (plane.InBounds(cp))
				{
					counts[cp.X, cp.Y]++;
					hitcount++;
					hits++;
				}
			}
		}

		private List<NonMandlebrotPoint> Pregenerate()
		{
			List<NonMandlebrotPoint> nonPoints = new List<NonMandlebrotPoint>();

			for (int x = 0; x < width; x++)
			{
				double re = plane.Real(x);

				for (int y = 0; y < height; y++)
				{
					double im = plane.Imaginary(y);

					Complex c = new Complex(re, im);
					Complex z = new Complex();

					for (int iter = 0; iter < MaxIterations; iter++)
					{
						if (z.Magnitude >= 4)
						{
							nonPoints.Add(new NonMandlebrotPoint(x, y, z));
							break;
						}

						z = z * z + c;
					}
				}
			}

			return nonPoints;
		}

		private Bitmap DisplayBrot(int[,] counts)
		{
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

			int maxi = 0;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (maxi < counts[x, y])
					{
						maxi = counts[x, y];
					}
				}
			}
			
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var c = GetColour(counts[x, y], maxi);

					//int r = (int)Math.Round(hits_r[x, y] * scale_r);
					//int g = (int)Math.Round(hits_g[x, y] * scale_g);
					//int b = (int)Math.Round(hits_b[x, y] * scale_b);
					//r = r % 255;
					//g = g % 255;
					//b = b % 255;

					bitmap.SetPixel(x, y, Color.FromArgb(c, c, c));
				}
			}

			return bitmap;
		}

		private byte GetColour(int count, int maxi)
		{
			return Convert.ToByte(255 * Math.Log10(1 + 9 * (count / (double)maxi)));
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			HasHandle = false;
			base.OnHandleDestroyed(e);
		}

		void SetNewBitmap(Bitmap image)
		{
			if (this.BackgroundImage != null)
			{
				this.BackgroundImage.Dispose();
			}

			this.BackgroundImage = image;
		}

		void UpdateFormText(string text)
		{
			this.Text = text;
		}

		delegate void SetNewBitmapDelegate(Bitmap image);

		delegate void UpdateFormTextDelegate(string text);
	}
}
