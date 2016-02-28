using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mandel.Colours;
using Mandel.ComplexNumbers;

namespace Mandel.Fractals
{
	public sealed class BuddhaBrot : Form
	{
		// Define the Complex plane we're processing
		private Complex topLeftCorner = new Complex(-2, -1.21);
		private Complex botRightCorner = new Complex(0.6, 1.2);

		// How many times do we want to process a specific complex
		// number? lower is faster but less detail.
		private readonly int dwell;

		// Helper booleans
		private bool HasHandle { get; set; }
		private bool paused;
		private bool dragging;

		// Zoom functions
		private Point startPoint = new Point(0, 0);
		private Point complexStart = new Point(0, 0);
		private Rectangle theRectangle = new Rectangle(new Point(0, 0), new Size(0, 0));
		private Complex zoomMin;
		private Complex zoomMax;

		// Scaling so we can know which index in hitArray
		// is referred to by whatever complex number is processed.
		private double scaleX;
		private double scaleY;

		// We track the highest hit made in the entire plane
		// so we can scale our colour palette accordingly.
		private int maxColourHit;

		// Hit counter so we can preview every RenderAfterNHits hits.
		private int hits;
		private int RenderAfterNHits { get; set; }

		// How many *total* hits do we want to process?
		private int OrbitMax { get; set; }

		// Width and Height of our BMP
		private int FractalWidth { get; set; }
		private int FractalHeight { get; set; }

		// Our backbuffer.
		private int[,] hitArray;

		public PaletteMap Palette { private get; set; }

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

			this.FractalWidth = 1000;
			this.FractalHeight = 1000;

			this.ClientSize = new Size(FractalWidth, FractalHeight);

			this.RenderAfterNHits = 200000;
			this.OrbitMax = 80000000;
			this.dwell = 500;

			// Default our zoom to the original scale
			this.zoomMin = topLeftCorner;
			this.zoomMax = botRightCorner;

			var workerThread = new Thread(Generate)
			{
				IsBackground = true,
				Priority = ThreadPriority.Highest
			};

			this.Load += (s, e) =>
			{
				workerThread.Start();
			};

			this.MouseDown += (s, a) =>
			{
				if (a.Button == MouseButtons.Left)
				{
					paused = true;
					dragging = true;
					var control = (Control) s;

					// Calculate the startPoint by using the PointToScreen  
					// method.
					startPoint = control.PointToScreen(new Point(a.X, a.Y));
					complexStart = a.Location;
					paused = false;
				}
			};

			this.MouseMove += (s, a) =>
			{
				if (dragging)
				{
					paused = true;
					ControlPaint.DrawReversibleFrame(theRectangle, this.BackColor, FrameStyle.Dashed);

					theRectangle = ResizeRectangle(((Control) s).PointToScreen(new Point(a.X, a.Y)));

					// Draw the new rectangle by calling DrawReversibleFrame again.  
					ControlPaint.DrawReversibleFrame(theRectangle, this.BackColor, FrameStyle.Dashed);
					paused = false;
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

				zoomMin = topLeftCorner.ComplexFromCartesian(complexStart.X, complexStart.Y, scaleX, scaleY);
				zoomMax = botRightCorner.ComplexFromCartesian(a.X, a.Y, scaleX, scaleY);
			};

			this.Closing += (s, e) => 
			{ 
				this.HasHandle = false;
				paused = true;
				workerThread.Abort();
			};

			this.Closed += (s, a) =>
			{
				this.HasHandle = false;
				paused = true;
			};
		
			this.KeyUp += (s, e) =>
			{
				switch (e.KeyCode)
				{
					case Keys.L:
						{
							this.paused = true;
							using (var fopen = new OpenFileDialog())
							{
								fopen.Multiselect = false;
								fopen.AutoUpgradeEnabled = true;

								if (fopen.ShowDialog() == DialogResult.OK)
								{
									Palette = new PaletteMap(fopen.FileName);
								}
								// Instantly render with the new colour palette.
								this.BeginInvoke(new SetNewBitmapDelegate(SetNewBitmap), DisplayBrot());
							}
							paused = false;
							break;
						}

					case Keys.Z:
						{
							this.paused = true;
							workerThread.Abort();
							while (workerThread.IsAlive)
							{
								Thread.Sleep(500);
							}

							topLeftCorner = zoomMin;
							botRightCorner = zoomMax;

							workerThread = new Thread(Generate)
							{
								IsBackground = true,
								Priority = ThreadPriority.AboveNormal
							};
							this.paused = false;
							workerThread.Start();
							break;
						}
				}

			};
		}

		private Rectangle ResizeRectangle(Point endPoint)
		{
			// Calculate the endpoint and dimensions for the new  
			// rectangle, again using the PointToScreen method.

			var cw = endPoint.X - startPoint.X;
			var ch = endPoint.Y - startPoint.Y;

			var newRect = new Rectangle(startPoint.X, startPoint.Y, cw, ch);

			return newRect;
		}

		private void Generate(object args)
		{
			try
			{
				// This is our back buffer
				hitArray = new int[FractalWidth, FractalHeight];

				// Calculate the scaling factor for our fractal.
				scaleX = (botRightCorner.Real - topLeftCorner.Real) / FractalHeight;
				scaleY = (botRightCorner.Imaginary - topLeftCorner.Imaginary) / FractalWidth;

				var totalHits = 0;
				var rand = new Random();

				while (totalHits < OrbitMax)
				{
					if (paused)
					{
						continue;
					}

					List<Complex> complexList = new List<Complex>(Environment.ProcessorCount);

					for (int i = 0; i < Environment.ProcessorCount; i++)
					{
						var c = new Complex(
							topLeftCorner.Real + rand.NextDouble() * (botRightCorner.Real - topLeftCorner.Real),
							topLeftCorner.Imaginary + rand.NextDouble() * (botRightCorner.Imaginary - topLeftCorner.Imaginary));
	
						complexList.Add(c);
					}

						// Find a random complex number in the plane

					Parallel.ForEach(complexList, ProcessPoint);
					
					// We want to see progress as we calculate, so periodically update the bitmap.
					if (hits > RenderAfterNHits)
					{
						hits = 0;

						if (HasHandle && !paused)
						{
							this.Invoke(new SetNewBitmapDelegate(SetNewBitmap), DisplayBrot());
							this.Invoke(new UpdateFormTextDelegate(UpdateFormText), string.Format("Buddhabrot Set Drawing - Total hits: {0}", totalHits));
						}
					}

					totalHits++;
				}

				this.Invoke(new SetNewBitmapDelegate(SetNewBitmap), DisplayBrot());
				this.Invoke(new UpdateFormTextDelegate(UpdateFormText), string.Format("Buddhabrot Set Finished - Total hits: {0}", totalHits));
			}
			catch (ThreadAbortException)
			{
				// we're probably zooming, just close.
			}
		}

		private void ProcessPoint(Complex c)
		{
			if (paused)
			{
				return;
			}

			var z = new Complex();

			// Iterate as per a standard mandlebrot
			for (var j = 0; j < dwell; j++)
			{
				z = z.Iterate(c);

				// If the Magnitude never exceeds 4 before j exceeds dwell, we're presuming that
				// this point is in the Mandlebrot set and can't be calculated
				// as it will continue to infinity.
				if (z.Magnitude < 4)
				{
					continue;
				}

				// We now want to 'orbit' our complex number, so base z on the random c we passed in.
				z = c;

				// This is where Buddhabrot comes from.
				for (var i = 0; i < dwell; i++)
				{
					// Find a cartesian point that corresponds to the last Complex we generated
					var x = (int) ((z.Real - topLeftCorner.Real)/scaleX);
					var y = (int) ((z.Imaginary - topLeftCorner.Imaginary)/scaleY);

					// Is this point within the scope of our image?
					if ((x >= 0) && (x < FractalHeight) && (y >= 0) && (y < FractalWidth))
					{
						// Update the hit count for that pixel.
						hitArray[y, x]++;

						// If this is the new highest hit, save it to maxHits
						// using (ref so we're updating the right color
						// This is used for scaling the palette later.
						if (maxColourHit < hitArray[y, x])
						{
							maxColourHit = hitArray[y, x];
						}
					}

					// Continue to orbit the origin 'c'
					z = z.Iterate(c);

					// if we exceed 4, we're not in the Mandlebrot set, so no point to continue.
					if (z.Magnitude >= 4)
					{
						break;
					}
				}
				hits++;
				break;
			}
		}

		private Color GetColor(int index)
		{
			if (index >= Palette.Colors.Count)
			{
				index = index % Palette.Colors.Count;
			}

			return Palette.Colors[index];
		}

		private Bitmap DisplayBrot()
		{
			var bmp = new Bitmap(FractalWidth, FractalHeight);

			var scale = 255 * 2.5 / maxColourHit;

			for (var y = 0; y < FractalHeight; y++)
			{
				for (var x = 0; x < FractalWidth; x++)
				{
					var brightness = (int)(hitArray[x, y] * scale);
					if (brightness > 255) brightness = 255;

					bmp.SetPixel(x, y, GetColor(brightness));
				}
			}

			return bmp;
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
