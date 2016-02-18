using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Mandel
{

	/// <summary>
	/// Generates bitmap of Mandelbrot Set and display it on the form.
	/// </summary>
	public sealed partial class MandelbrotSetForm : Form
	{
		public ComplexFunction ComplexFunc { private get; set; }

		private int width { get { return ClientSize.Width; } }
		private int height { get { return ClientSize.Height; } }

		private volatile bool stopRequested = false;
		private volatile bool resizing = false;

		private Complex[,] complexMap;

		Thread worker;

		public MandelbrotSetForm()
		{
			// form creation
			this.Text = @"Mandelbrot Set Drawing";
			this.BackColor = Color.Black;
			this.BackgroundImageLayout = ImageLayout.Stretch;
			this.MaximizeBox = false;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.FormBorderStyle = FormBorderStyle.Sizable;
			this.ClientSize = new Size(800, 600);
			this.complexMap = new Complex[width,height];
			this.Load += (s, e) =>
					{
						worker = new Thread(thread_Proc) { IsBackground = true };
						worker.Start();
					};

			//this.Resize += (sender, args) =>
			this.ClientSizeChanged += (s, a) => 
				{
					stopRequested = true;

					while (worker.IsAlive)
					{
						Thread.Sleep(1);
					}

					Graphics gr = Graphics.FromImage(BackgroundImage);
					gr.Clear(Color.Black);

					complexMap = new Complex[width, height];

					stopRequested = false;
					worker = new Thread(thread_Proc) { IsBackground = true }; 
					worker.Start();
				};

			this.MouseClick += (s, a) =>
				{
					Complex juliaSeed = complexMap[a.X, a.Y];

					var preview = GenerateJulia(64, 64, juliaSeed);
					this.Cursor = ToCursor(preview);
				};

			this.MouseMove += (sender, args) =>
						{
							Complex seed = complexMap[args.X, args.Y];
							this.Text = String.Format("Re: {0}; Im: {1}", seed.Real, seed.Imaginary);
						};

			this.KeyUp += (sender, args) =>
					{
						if (args.KeyCode == Keys.Escape)
						{
							this.Cursor = Cursors.Default;
						}
					};
		}

		private static Cursor ToCursor(Bitmap bmp)
		{
			var hicon = bmp.GetHicon();
			var cursr = new Cursor(hicon);
			var fi = typeof (Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
			fi.SetValue(cursr, true);

			return cursr;
		}

		/// <summary>
		/// Creates a Julia set of the chosen seed Complex number
		/// It takes width and height as this is also used to create a thumbnail preview.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		private Bitmap GenerateJulia(int w, int h, Complex seed)
		{
			Bitmap bmp = new Bitmap(w, h, PixelFormat.Format24bppRgb);

			for (int x = 0; x < w; x++)
			{
				double real = ComplexFunc.RealMin + x * (ComplexFunc.RealMax - ComplexFunc.RealMin) / w;

				for (int y = 0; y < h; y++)
				{
					double imag = ComplexFunc.ImagMin + y * (ComplexFunc.ImagMax - ComplexFunc.ImagMin) / h;

					int iteration;
					for (iteration = 0; iteration < 1000; iteration++)
					{
						Complex o = new Complex(real, imag);

						Complex n = o * o + seed;

						if (n.Magnitude >= 4.0)
						{
							break;
						}
					}

					bmp.SetPixel(x, y, iteration < 1000 ? GetColor(iteration) : Color.Black);
				}
			}

			return bmp;
		}


		private void thread_Proc(object args)
		{
			try
			{
				this.UseWaitCursor = true;

				this.BeginInvoke(new UpdateFormTextDelegate(UpdateFormText), String.Format("{0} in progress...", ComplexFunc.Label));

				Bitmap finalBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

				for (int x = 0; x < width; x++)
				{
					double real = ComplexFunc.RealMin + x * ( ComplexFunc.RealMax - ComplexFunc.RealMin ) / width;

					for (int y = 0; y < height; y++)
					{
						double imag = ComplexFunc.ImagMin + y * ( ComplexFunc.ImagMax - ComplexFunc.ImagMin ) / height;

						Complex result = ComplexFunc.Function.Invoke(new Complex(real, imag));
						complexMap[x, y] = result;

						if (stopRequested)
						{
							return;
						}
						
						finalBitmap.SetPixel(
							x,
							y, 
							ComplexFunc.RealIsIteration 
								? GetColor(result.Real) 
								: ColourMap.ComplexToColour(result)
							);
					}
				}

				this.BeginInvoke(new SetNewBitmapDelegate(SetNewBitmap), finalBitmap);
				this.BeginInvoke(new UpdateFormTextDelegate(UpdateFormText), String.Format("{0} done", ComplexFunc.Label));

				this.UseWaitCursor = false;
			}
			catch (ThreadAbortException)
			{
				// This is fine; we're resizing or closing.
			}
		}

		private Color GetColor(double value)
		{
			const double MaxColor = 256;
			const double ContrastValue = 0.2;
			return Color.FromArgb(0, 0, (int)((MaxColor * Math.Pow(value, ContrastValue)) % 255));
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