using System.Windows.Media;
using Mandel.Bitmaps;

namespace Mandel.Fractals
{
	public class Bifurcation : DrawableWindow 
	{
		public Bifurcation()
			: base(800, 800)
		{
			
		}

		public void RenderDiagram()
		{
			this.Clear(Colors.White);

			int ux = 0;
			var uy = 0.0;
			int N = Width - 295;

			for (int i = -295; i < N; i++)
			{
				int x = i;
				int u = x + (x / N);
				var y = 0.05;

				for (int j = 0; j < 500; j++)
				{
					y = u * y * (1 - y);
				}
				for (int j = 0; j < Height; j++)
				{
					y = u * y * (1 - y);
					uy = Height - y * Height;

					if (ux < Width && uy >= 0 && uy < Height)
					{
						this.SetPixel(ux, (int)uy, Colors.Black);
					}
				}
				ux++;
			}
		}
	}
}
