using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Mandel.Colours
{
	public class PaletteMap
	{
		private readonly List<Color> colors;

		public List<Color> Colors
		{
			get { return colors; }
		}

		public Color GetColor(int index)
		{
			return colors[index];
		}

		public PaletteMap()
		{
			colors = new List<Color>();

			// Default to a grayscale palette
			for (int i = 0; i < 256; i++ )
			{
				colors.Add(Color.FromArgb(255, i, i, i));
			}
		}

		public PaletteMap(string file)
		{
			colors = new List<Color>();

			var sr = new StreamReader(file);

			string input = sr.ReadLine();
			while (!String.IsNullOrEmpty(input))
			{
				string[] line = input.Trim().Split(new [] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);

				byte red;
				byte grn;
				byte blu;

				if (!byte.TryParse(line[0], out red))
				{
					red = 0;
				}

				if (line.Length < 2 || !byte.TryParse(line[1], out grn))
				{
					grn = 0;
				}

				if (line.Length < 3 || !byte.TryParse(line[2], out blu))
				{
					blu = 0;
				}

				colors.Add(Color.FromArgb(255, red, grn, blu));

				input = sr.ReadLine();
			}
		}
	}
}
