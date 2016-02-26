using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using Mandel.Colours;

namespace Mandel.Fractals
{
	/// <summary>
	/// Generates bitmap of Complex plane function and display it on the form.
	/// </summary>
	public sealed partial class Fractal : Form
	{
		private const int MaxJulia = 500;

		public ComplexFunction ComplexFunc { private get; set; }

		public PaletteMap Palette { private get; set; }

		private int width { get { return ClientSize.Width; } }

		private int height { get { return ClientSize.Height; } }

		private volatile bool stopRequested;

		private bool backgroundIsJulia;

		private bool resizing;

		//private Complex[,] complexMap;
		private Complex[,] zoomMap;

		private ComplexResult[,] resultMap;

		private Complex ZoomMin;
		private Complex ZoomMax;
		
		private DateTime start = DateTime.Now;
		private DateTime end = DateTime.Now;

		private ComplexPlane plane;

		Thread worker;

		bool dragging = false;
		bool zooming = false;

		Point startPoint = new Point(0, 0);
		Point complexStart = new Point(0, 0);

		Rectangle theRectangle = new Rectangle(new Point(0, 0), new Size(0, 0));

		public Fractal()
		{
			// form creation
			this.Text = @"Mandelbrot Set Drawing";
			this.BackColor = Color.Black;
			this.BackgroundImageLayout = ImageLayout.Stretch;
			this.MaximizeBox = false;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.FormBorderStyle = FormBorderStyle.Sizable;
			this.ClientSize = new Size(1280, 1024);
			//this.complexMap = new Complex[width,height];
			this.zoomMap = new Complex[width, height];
			this.resultMap = new ComplexResult[width, height];

			this.Load += (s, e) =>
					{
						this.plane = new ComplexPlane(ComplexFunc.Min, ComplexFunc.Max, this.width, this.height);
						worker = new Thread(thread_Proc) { IsBackground = true };
						worker.Start();
					};

			this.ClientSizeChanged += OnClientSizeChanged;

			this.MouseDown += (s, a) =>
				{
					if (a.Button == MouseButtons.Left)
					{
						dragging = true;
						Control control = (Control)s;

						// Calculate the startPoint by using the PointToScreen  
						// method.
						startPoint = control.PointToScreen(new Point(a.X, a.Y));
						complexStart = a.Location;
					}
				};

			this.ResizeBegin += (s, a) => resizing = true;
			this.ResizeEnd += (s, a) => resizing = false;

			this.MouseClick += (s, a) =>
				{
					if (ComplexFunc.AllowJulia)
					{
						Complex juliaSeed = resultMap[a.X, a.Y].Complex;

						var preview = GenerateJulia(64, 64, juliaSeed, 64);
						this.Cursor = ToCursor(preview);
					}
				};

			this.MouseDoubleClick += (s, a) =>
				{
					if (ComplexFunc.AllowJulia)
					{
						Complex juliaSeed = resultMap[a.X, a.Y].Complex;
						var julia = GenerateJulia(this.width, this.height, juliaSeed);

						this.BackgroundImage = julia;
						backgroundIsJulia = true;
					}
				};

			this.MouseMove += (s, a) =>
				{
					Complex seed = zoomMap[a.X, a.Y];
					Complex func = resultMap[a.X, a.Y] == null ? new Complex() : resultMap[a.X, a.Y].Complex;

					this.Text = String.Format("Re: {0}; Im: {1} - X:{2} Y:{3} - {4}. Mag: {5}", seed.Real, seed.Imaginary,a.X,a.Y, end-start, func.Magnitude);

					if (dragging)
					{
						ControlPaint.DrawReversibleFrame(theRectangle, this.BackColor, FrameStyle.Dashed);

						theRectangle = ResizeRectangle(((Control)s).PointToScreen(new Point(a.X, a.Y)));

						// Draw the new rectangle by calling DrawReversibleFrame again.  
						ControlPaint.DrawReversibleFrame(theRectangle, this.BackColor, FrameStyle.Dashed);
					}
				};

			this.MouseUp += (s, a) =>
				{
					// If the MouseUp event occurs, the user is not dragging.
					dragging = false;

					// Draw the rectangle to be evaluated. Set a dashed frame style  
					// using the FrameStyle enumeration.
					ControlPaint.DrawReversibleFrame(theRectangle, this.BackColor, FrameStyle.Dashed);

					// Reset the rectangle.
					theRectangle = new Rectangle(0, 0, 0, 0);

					ZoomMin = zoomMap[complexStart.X, complexStart.Y];
					ZoomMax = zoomMap[a.X, a.Y];

					zooming = true;

					/*this.ComplexFunc.RealMin = lft.Real;
					this.ComplexFunc.RealMax = rgt.Real;
					this.ComplexFunc.ImagMin = lft.Imaginary;
					this.ComplexFunc.ImagMax = rgt.Imaginary;
					*/
					//this.OnClientSizeChanged(s, a);
				};

			this.KeyUp += (s, a) =>
				{
					switch (a.KeyCode)
					{
						case Keys.Escape:
							if (backgroundIsJulia)
							{
								backgroundIsJulia = false;
								this.OnClientSizeChanged(s, a);
							}
							else
							{
								this.Cursor = Cursors.Default;	
							}

							if (zooming)
							{
								zooming = false;
								this.OnClientSizeChanged(s, a);
							}
							break;

						case Keys.L:
							using (OpenFileDialog fopen = new OpenFileDialog())
							{
								fopen.Multiselect = false;
								fopen.AutoUpgradeEnabled = true;
								//fopen.InitialDirectory = string.Format("{0}\\Maps", Environment.CurrentDirectory);

								if (fopen.ShowDialog() == DialogResult.OK)
								{
									Palette = new PaletteMap(fopen.FileName);
								}

								render_ResultMap();
								//OnClientSizeChanged(s, a);
							}
							break;

						case Keys.F:
							this.ClientSize = new Size(1920, 1080);
							OnClientSizeChanged(s, a);
							
							break;
						case Keys.B:
							SaveBackgroundImage(ImageFormat.Bmp);
							break;
						case Keys.S:
							SaveBackgroundImage(ImageFormat.Png);
							break;

						case Keys.Z:
							this.OnClientSizeChanged(s, a);
							break;
					}
				};
		}

		private void SaveBackgroundImage(ImageFormat format)
		{
			using (SaveFileDialog fsave = new SaveFileDialog())
			{
				fsave.FileName = string.Format("screenshot_{0}.{1}", DateTime.Now.Ticks, format.ToString());
				fsave.AutoUpgradeEnabled = true;
				fsave.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

				if (fsave.ShowDialog() == DialogResult.OK)
				{
					using (StreamWriter sw = new StreamWriter(fsave.FileName))
					{
						this.BackgroundImage.Save(sw.BaseStream, format);
					}
				}
			}

		}

		private Rectangle ResizeRectangle(Point endPoint)
		{
			// Calculate the endpoint and dimensions for the new  
			// rectangle, again using the PointToScreen method.
						
			int cw = endPoint.X - startPoint.X;
			int ch = endPoint.Y - startPoint.Y;

			var newRect = new Rectangle(startPoint.X, startPoint.Y, cw, ch);

			return newRect;
		}
/*
		private Rectangle ResizeAspectRectangle(Point endPoint)
		{
			Size ratio = new Size(width / height, 1);

			float cw = endPoint.X - startPoint.X;
			float ch = endPoint.Y - startPoint.Y;

			if (cw == 0) cw = 1;
			if (ch == 0) ch = 1;

			float newH = cw * (ch / cw);
			float newW = ch * (cw / ch);

			var newRect = new Rectangle(startPoint.X, startPoint.Y, (int)newW, (int)newH);

			return newRect;
		}*/

		private void OnClientSizeChanged(object s, EventArgs a)
		{
			stopRequested = true;

			while (worker.IsAlive)
			{
				Thread.Sleep(1);
			}

			Graphics gr = Graphics.FromImage(BackgroundImage);
			gr.Clear(Color.Black);

			plane = zooming 
				? new ComplexPlane(ZoomMin, ZoomMax, width, height)
				: new ComplexPlane(ComplexFunc.Min, ComplexFunc.Max, width, height);

			//complexMap = new Complex[width,height];
			zoomMap = new Complex[width, height];
			resultMap = new ComplexResult[width, height];

			stopRequested = false;
			worker = new Thread(thread_Proc) { IsBackground = true };
			worker.Start();
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
		/// <param name="w">Width</param>
		/// <param name="h">Height</param>
		/// <param name="c">Seed Complex number</param>
		/// <param name="its">Optional max iterations; so thumbnails are rapid</param>
		/// <returns></returns>
		private Bitmap GenerateJulia(int w, int h, Complex c, int its = MaxJulia)
		{
			Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);

			for (int x = 0; x < w; x++)
			{
				double real = ComplexFunc.Min.Real + x * (ComplexFunc.Max.Real - ComplexFunc.Min.Real) / w;

				for (int y = 0; y < h; y++)
				{
					double imag = ComplexFunc.Min.Imaginary + y * (ComplexFunc.Max.Imaginary - ComplexFunc.Min.Imaginary) / h;
					
					Complex z = new Complex(real, imag);
					
					int iteration;
					for (iteration = 0; iteration < its; iteration++)
					{
						z = z * z + c;

						if (z.Magnitude >= 4.0)
						{
							break;
						}
					}

					bmp.SetPixel(x, y, iteration < its ? GetColor(iteration) : Color.Black);
				}
			}

			return bmp;
		}

		private void render_ResultMap()
		{
			Bitmap finalBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var result = resultMap[x, y];

					finalBitmap.SetPixel(
							x,
							y,
							ComplexFunc.UseDwell
								? GetColor(result.Dwell)
								: ColourMap.ComplexToColour(result.Complex)
							);
				}
			}

			this.BeginInvoke(new SetNewBitmapDelegate(SetNewBitmap), finalBitmap);
		}

		private void thread_Proc(object args)
		{
			try
			{
				start = DateTime.Now;
				this.UseWaitCursor = true;

				this.BeginInvoke(new UpdateFormTextDelegate(UpdateFormText), String.Format("{0} in progress...", ComplexFunc.Label));

				int pixelCounter = 0;

				Bitmap finalBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

				if (ComplexFunc.UseDwell)
				{
					for (int x = 0; x < width; x++)
					{
						double real = plane.Real(x);

						for (int y = 0; y < height; y++)
						{
							double imag = plane.Imaginary(y);

							if (resizing)
							{
								return;
							}

							Complex c = new Complex(real, imag);

							zoomMap[x, y] = c;

							var result = ComplexFunc.Function.Invoke(c, x, y, pixelCounter++);

							//if (x >= complexMap.GetLength(0) || y >= complexMap.GetLength(1))
							//{
							//	return;
							//}

							//complexMap[x, y] = result.Complex;

							if (stopRequested)
							{
								return;
							}

							finalBitmap.SetPixel(x, y, ComplexFunc.Smooth ? GetColor(result.Dwell, result.Complex) : GetColor(result.Dwell));

							resultMap[x, y] = result;
						}
					}
				}
				else
				{
					for (int x = 0; x < width; x++)
					{
						double real = plane.Real(x);

						for (int y = 0; y < height; y++)
						{
							if (stopRequested)
							{
								return;
							}

							double imag = plane.Imaginary(y);

							Complex c = new Complex(real, imag);

							zoomMap[x, y] = c;

							var result = ComplexFunc.Function.Invoke(c, x, y, pixelCounter++);

							//if (x >= complexMap.GetLength(0) || y >= complexMap.GetLength(1))
							//{
							//	return;
							//}

							//complexMap[x, y] = result.Complex;

							if (stopRequested)
							{
								return;
							}

							finalBitmap.SetPixel(x, y, ColourMap.ComplexToColour(result.Complex));

							resultMap[x, y] = result;
						}
					}
				}

				end = DateTime.Now;

				this.BeginInvoke(new SetNewBitmapDelegate(SetNewBitmap), finalBitmap);
				this.BeginInvoke(new UpdateFormTextDelegate(UpdateFormText), String.Format("{0} done - {1}", ComplexFunc.Label, end - start));

				this.UseWaitCursor = false;

			}
			catch (ThreadAbortException)
			{
				// This is fine; we're resizing or closing.
			}
		}

		private Color GetColor(int dwell, Complex complex)
		{
			double di = (double) dwell;
			double r2 = complex.Real * complex.Real;
			double i2 = complex.Imaginary * complex.Imaginary;

			double zn = Math.Sqrt(r2 + i2);
			double hue = di + 1.0 - Math.Log(Math.Log(Math.Abs(zn))) / Math.Log(2.0);
			hue = 0.95 + 20.0 * hue;

			if (double.IsNaN(hue))
			{
				return Color.Black;
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