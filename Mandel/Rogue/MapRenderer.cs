using System.Windows.Media;

using Mandel.Bitmaps;
using Mandel.Mapbuilder;

namespace Mandel.Rogue
{
	public class MapRenderer : DrawableWindow
	{
		private MapBuilder builder;

		public MapRenderer()
		{
			builder = new MapBuilder(this.Width, this.Height);

			builder.BuildOneStartRoom();
		}

		public void Render()
		{
			//this.Clear(Colors.Black);

			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					this.SetPixel(i,j, builder.Map[i,j] == 1 ? Colors.Black : Colors.White);
				}
			}
		}
	}
}
