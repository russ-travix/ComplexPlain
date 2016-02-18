using System;
using System.Windows.Forms;

using Mandel.Colours;
using Mandel.DX;
using Mandel.DiamondSquare;
using Mandel.DivisorDrips;
using Mandel.Fractals;
using Mandel.Particles;
using Mandel.Primes;
using Mandel.Rogue;

namespace Mandel
{
	public partial class ConfigForm : Form
	{
		private PaletteMap palette = new PaletteMap();

		public ConfigForm()
		{
			InitializeComponent();
		}

		private void Config_Load(object sender, EventArgs e)
		{
			foreach (var func in ComplexFunctions.Functions)
			{
				this.cmbFunctions.Items.Add(func.Label);
			}

			foreach (var func in SpiralFunctions.Functions)
			{
				this.cmbSpiralFuncs.Items.Add(func.Label);
			}

			this.cmbFunctions.SelectedIndex = 0;
			this.cmbSpiralFuncs.SelectedIndex = 0;
		}

		private void btnDrawCmplx_Click(object sender, EventArgs e)
		{
			ComplexFunction func = ComplexFunctions.Functions[cmbFunctions.SelectedIndex];

			Fractal fractalForm = new Fractal
			{
				ComplexFunc = func,
				Palette = palette
			};

			fractalForm.Show();
		}

		private void btnBuddha_Click(object sender, EventArgs e)
		{
			//BuddhaBrot brot = new BuddhaBrot();
			//brot.Show();

			BBrot bb = new BBrot();
			bb.Render();
		}

		private void btnPalette_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fopen = new OpenFileDialog())
			{
				fopen.Multiselect = false;
				fopen.AutoUpgradeEnabled = true;
				fopen.InitialDirectory = string.Format("{0}\\Maps", Environment.CurrentDirectory);

				if (fopen.ShowDialog() == DialogResult.OK)
				{
					palette = new PaletteMap(fopen.FileName);
				}
			}
		}

		private void btnTerrain_Click(object sender, EventArgs e)
		{
			TerrainForm terrainForm = new TerrainForm();
			terrainForm.Show();

			terrainForm.Render();
		}

		private void btnParticle_Click(object sender, EventArgs e)
		{
			ParticleRenderer renderer = new ParticleRenderer();
			renderer.Render();
		}

		private void btnDirect2D_Click(object sender, EventArgs e)
		{
			//Direct2D direct2D = new Direct2D();
			//direct2D.StartRender();
		}

		private void btnDungeon_Click(object sender, EventArgs e)
		{
			MapRenderer mapRenderer = new MapRenderer();

			mapRenderer.Render();
		}

		private void btnSpiral_Click(object sender, EventArgs e)
		{
			SpiralFunc func = SpiralFunctions.Functions[cmbSpiralFuncs.SelectedIndex];
			Spiral spiral = new Spiral(func);
			spiral.RenderSpiralMatrix = false;

			spiral.Render();
		}

		private void btnMatrix_Click(object sender, EventArgs e)
		{
			SpiralFunc func = SpiralFunctions.Functions[cmbSpiralFuncs.SelectedIndex];
			Spiral spiral = new Spiral(func);
			spiral.RenderSpiralMatrix = true;

			spiral.Render();
		}

		private void btnRealSpiral_Click(object sender, EventArgs e)
		{
			SpiralFunc func = SpiralFunctions.Functions[cmbSpiralFuncs.SelectedIndex];
			Spiral spiral = new Spiral(func);
			spiral.RenderSpiralMatrix = true;

			spiral.DoRealSpiral();
		}

		private void btnDivDrips_Click(object sender, EventArgs e)
		{
			Drips drips = new Drips();

			drips.RenderDrips();
		}

		private void btnDivPrimes_Click(object sender, EventArgs e)
		{
			Drips drips = new Drips();
			
			drips.RenderPrimes();
		}

		private void btnBifurcate_Click(object sender, EventArgs e)
		{
			Bifurcation b = new Bifurcation();
			b.RenderDiagram();
		}

		private void btn_ZigZag_Click(object sender, EventArgs e)
		{
			SpiralFunc func = SpiralFunctions.Functions[cmbSpiralFuncs.SelectedIndex];
			Spiral spiral = new Spiral(func);

			spiral.DoZigzagSpiral();
		}

		
	}
}