using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Text;
using System.Threading.Tasks;

namespace Mandel.LSystem
{
	public class Dimension
	{
		public int MinX { get; set; }

		public int MinY { get; set; }

		public int MaxX { get; set; }

		public int MaxY { get; set; }
	}
	

	public class Location
	{
		public double X { get; set; }

		public double Y { get; set; }

		public double Heading { get; set; }

		public Color Colour { get; set; }

		public Location(double x, double y, double angle, Color color)
		{
			this.X = x;
			this.Y = y;
			this.Heading = angle;
			Colour = color;
		}
	}

	public class LSystem
	{
		private const char ANTICLOCK = '+';

		private const char CLOCKWISE = '-';

		private const char PUSH = '[';

		private const char POP = ']';

		private const char COLOUR = 'C';

		private const double RAD = Math.PI / 180;

		private int Width = 0;

		private int Height = 0;

		private int XOffset = 0;

		private int YOffset = 0;

		// Render distance units to move per forward turtle movement
		private int distance = 10;

		// Turning angle in degrees per turtle rotation
		private int angle = 30;

		// Minimum x reached in last phase
		private int minX = 0;

		// Ditto
		private int minY = 0;

		private int maxX = 0;

		private int maxY = 0;

		private int maxStackDepth = 0;

		private Stack<Location> stack;

		private List<Color> colorList;

		private Dictionary<char, bool> constants;

		private bool renderLineWidths = true;

		public void SetDistance(int d)
		{
			this.distance = d;
		}

		public void SetOffsets(int ox, int oy)
		{
			XOffset = ox;
			YOffset = oy;
		}

		public void SetConstants(string constant)
		{
			constants = new Dictionary<char, bool>();

			foreach (char c in constant)
			{
				if (c != ' ' && c != ',')
				{
					constants[c] = true;
				}
			}
		}

		public LSystem()
		{
			colorList = new List<Color>
						{
							Color.FromArgb(182, 140, 80, 60),
							Color.FromArgb(182, 24, 180, 25),
							Color.FromArgb(128, 48, 220, 48),
							Color.FromArgb(128, 64, 255, 64)
						};

		}

		public void Process(string cmds, bool draw)
		{
			stack = new Stack<Location>();

			double lastx, lasty, rad;

			var pos = new Location(0, 0, 90, Colors.White);

			for (int i = 0; i < cmds.Length; i++)
			{
				char c = cmds[i];

				switch (c)
				{
					case COLOUR:
						// Get the colour index and increment the counter so we don't parse it on the next pass.
						char colorIndex = cmds[++i];

						int cindex;
						int.TryParse(colorIndex.ToString(CultureInfo.InvariantCulture), out cindex);

						// Clamp colour index to the range of colours.
						if (cindex < 0)
						{
							cindex = 0;
						}
						if (cindex >= colorList.Count)
						{
							cindex = colorList.Count - 1;
						}

						pos.Colour = colorList[cindex];
						break;

					case ANTICLOCK:
						pos.Heading += angle;
						break;

					case CLOCKWISE:
						pos.Heading -= angle;
						break;

					case PUSH:
						stack.Push(new Location(pos.X, pos.Y, pos.Heading, pos.Colour));
						break;

					case POP:
						pos = stack.Pop();
						break;

					default:
						if (!constants[c])
						{
							lastx = pos.X;
							lasty = pos.Y;

							rad = pos.Heading * RAD;

							pos.X += distance * Math.Cos(rad);
							pos.Y += distance * Math.Sin(rad);

							if (draw)
							{
								
							}
							else
							{
								
							}
						}
						break;
				}
			}
		}
	}
}
