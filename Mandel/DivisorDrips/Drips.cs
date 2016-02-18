using System.Windows.Media;

using Mandel.Bitmaps;
using Mandel.Primes;

namespace Mandel.DivisorDrips
{
	public class Drips : DrawableWindow
	{
		public Drips()
			: base(1024, 1024)
		{
			this.Clear(Colors.Black);
		}


		public void RenderDrips()
		{
			int root = 1;
			int step = 1;

			for (int x = 1; x < this.Width; x += step)
			{
				//this.SetPixel(x - 1, 0, Colors.White);

				for (int y = 1; y <= root && y < this.Height; y += step)
				{

					if (x % y == 0)
					{
						this.SetPixel(x-1, y, Colors.White);
					}
				}

				root++;
			}
		}

		public void RenderPrimes()
		{
			int root = 1;
			int step = 1;

			int counter = 0;

			for (int x = 1; x < this.Width; x += step)
			{
				//this.SetPixel(x - 1, 0, Colors.White);

				for (int y = 1; y < this.Height; y += step)
				{

					if (MathFunctions.IsPrime(counter++))
					{
						this.SetPixel(x-1,y-1, Colors.White);
					}
				}

				root++;
			}
		}
	}
}
