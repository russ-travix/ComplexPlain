using System;
using System.Windows.Media;

using Mandel.Bitmaps;

namespace Mandel.Particles
{
	public class Particle
	{
		private const float DAMPING = 0.999f;

		private double x;
		private double y;
		private double old_x;
		private double old_y;

		private int width;

		private int height;

		public Particle(int x, int y, int width, int height)
		{
			this.x = this.old_x = x;
			this.y = this.old_y = y;

			this.width = width;
			this.height = height;
		}

		public Particle Integrate()
		{
			var velocityX = ( this.x - this.old_x ) * DAMPING;
			var velocityY = ( this.y - this.old_y ) * DAMPING;

			this.old_x = x;
			this.old_y = y;

			this.x += velocityX;
			this.y += velocityY;

			return this;
		}

		public Particle Attract(int ax, int ay)
		{
			var dx = ax - x;
			var dy = ay - y;

			var distance = Math.Sqrt(dx * dx + dy * dy);

			this.x += dx / distance;
			this.y += dy / distance;

			return this;
		}

		private int ClampX(double clampX)
		{
			if (clampX < 0)
				return 0;
			if (clampX >= width)
				return width - 1;
			return (int) clampX;
		}

		private int ClampY(double clampY)
		{
			if (clampY < 0)
				return 0;
			if (clampY >= height)
				return height - 1;
			return (int) clampY;
		}

		public void Draw(WBitmap bmp)
		{
			int dx = ClampX(x);
			int dy = ClampY(y);
			int ox = ClampX(old_x);
			int oy = ClampY(old_y);

			bmp.DrawLineBresenham(ox, oy, dx, dy, Colors.White);
			
			//bmp.DrawPixel(dx, dy,);
		}
	}
}