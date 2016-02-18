using System.Drawing;
using System.Windows.Forms;

namespace Mandel.DiamondSquare
{
	public sealed partial class TerrainForm : Form
	{
		public TerrainForm()
		{
			InitializeComponent();

			this.BackColor = Color.Black;
			this.BackgroundImageLayout = ImageLayout.Stretch;
			this.MaximizeBox = false;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.FormBorderStyle = FormBorderStyle.Sizable;
			this.ClientSize = new Size(800, 600);
		}

		public void Render()
		{
			if (this.BackgroundImage == null)
			{
				this.BackgroundImage = new Bitmap(this.Width, this.Height);
			}

			Terrain t = new Terrain(this.BackgroundImage);
			t.Generate(0.7f);
			t.Draw();

			this.BackgroundImage = t.BackgroundImage;
		}
	}
}
