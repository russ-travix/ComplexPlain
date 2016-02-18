namespace Mandel
{
	partial class ConfigForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblRenderMethod = new System.Windows.Forms.Label();
			this.cmbFunctions = new System.Windows.Forms.ComboBox();
			this.btnDrawCmplx = new System.Windows.Forms.Button();
			this.btnPalette = new System.Windows.Forms.Button();
			this.btnTerrain = new System.Windows.Forms.Button();
			this.btnBuddha = new System.Windows.Forms.Button();
			this.btnParticle = new System.Windows.Forms.Button();
			this.btnDirect2D = new System.Windows.Forms.Button();
			this.btnDungeon = new System.Windows.Forms.Button();
			this.btnSpiral = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.cmbSpiralFuncs = new System.Windows.Forms.ComboBox();
			this.btnMatrix = new System.Windows.Forms.Button();
			this.btnDivPrimes = new System.Windows.Forms.Button();
			this.btnRealSpiral = new System.Windows.Forms.Button();
			this.btnBifurcate = new System.Windows.Forms.Button();
			this.btn_ZigZag = new System.Windows.Forms.Button();
			this.btn_divDrips = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblRenderMethod
			// 
			this.lblRenderMethod.AutoSize = true;
			this.lblRenderMethod.Location = new System.Drawing.Point(12, 8);
			this.lblRenderMethod.Name = "lblRenderMethod";
			this.lblRenderMethod.Size = new System.Drawing.Size(62, 13);
			this.lblRenderMethod.TabIndex = 2;
			this.lblRenderMethod.Text = "Fractal type";
			// 
			// cmbFunctions
			// 
			this.cmbFunctions.FormattingEnabled = true;
			this.cmbFunctions.Location = new System.Drawing.Point(11, 25);
			this.cmbFunctions.Name = "cmbFunctions";
			this.cmbFunctions.Size = new System.Drawing.Size(260, 21);
			this.cmbFunctions.TabIndex = 0;
			// 
			// btnDrawCmplx
			// 
			this.btnDrawCmplx.Location = new System.Drawing.Point(196, 52);
			this.btnDrawCmplx.Name = "btnDrawCmplx";
			this.btnDrawCmplx.Size = new System.Drawing.Size(75, 23);
			this.btnDrawCmplx.TabIndex = 1;
			this.btnDrawCmplx.Text = "Complex";
			this.btnDrawCmplx.UseVisualStyleBackColor = true;
			this.btnDrawCmplx.Click += new System.EventHandler(this.btnDrawCmplx_Click);
			// 
			// btnPalette
			// 
			this.btnPalette.Location = new System.Drawing.Point(11, 256);
			this.btnPalette.Name = "btnPalette";
			this.btnPalette.Size = new System.Drawing.Size(75, 23);
			this.btnPalette.TabIndex = 4;
			this.btnPalette.Text = "Palette";
			this.btnPalette.UseVisualStyleBackColor = true;
			this.btnPalette.Click += new System.EventHandler(this.btnPalette_Click);
			// 
			// btnTerrain
			// 
			this.btnTerrain.Location = new System.Drawing.Point(104, 256);
			this.btnTerrain.Name = "btnTerrain";
			this.btnTerrain.Size = new System.Drawing.Size(75, 23);
			this.btnTerrain.TabIndex = 5;
			this.btnTerrain.Text = "Terrain";
			this.btnTerrain.UseVisualStyleBackColor = true;
			this.btnTerrain.Click += new System.EventHandler(this.btnTerrain_Click);
			// 
			// btnBuddha
			// 
			this.btnBuddha.Location = new System.Drawing.Point(196, 256);
			this.btnBuddha.Name = "btnBuddha";
			this.btnBuddha.Size = new System.Drawing.Size(75, 23);
			this.btnBuddha.TabIndex = 3;
			this.btnBuddha.Text = "Buddha";
			this.btnBuddha.UseVisualStyleBackColor = true;
			this.btnBuddha.Click += new System.EventHandler(this.btnBuddha_Click);
			// 
			// btnParticle
			// 
			this.btnParticle.Location = new System.Drawing.Point(104, 227);
			this.btnParticle.Name = "btnParticle";
			this.btnParticle.Size = new System.Drawing.Size(75, 23);
			this.btnParticle.TabIndex = 6;
			this.btnParticle.Text = "Particles";
			this.btnParticle.UseVisualStyleBackColor = true;
			this.btnParticle.Click += new System.EventHandler(this.btnParticle_Click);
			// 
			// btnDirect2D
			// 
			this.btnDirect2D.Location = new System.Drawing.Point(197, 227);
			this.btnDirect2D.Name = "btnDirect2D";
			this.btnDirect2D.Size = new System.Drawing.Size(75, 23);
			this.btnDirect2D.TabIndex = 7;
			this.btnDirect2D.Text = "Direct2D";
			this.btnDirect2D.UseVisualStyleBackColor = true;
			this.btnDirect2D.Click += new System.EventHandler(this.btnDirect2D_Click);
			// 
			// btnDungeon
			// 
			this.btnDungeon.Location = new System.Drawing.Point(11, 227);
			this.btnDungeon.Name = "btnDungeon";
			this.btnDungeon.Size = new System.Drawing.Size(75, 23);
			this.btnDungeon.TabIndex = 8;
			this.btnDungeon.Text = "Dungeon";
			this.btnDungeon.UseVisualStyleBackColor = true;
			this.btnDungeon.Click += new System.EventHandler(this.btnDungeon_Click);
			// 
			// btnSpiral
			// 
			this.btnSpiral.Location = new System.Drawing.Point(228, 125);
			this.btnSpiral.Name = "btnSpiral";
			this.btnSpiral.Size = new System.Drawing.Size(43, 23);
			this.btnSpiral.TabIndex = 9;
			this.btnSpiral.Text = "Ulam";
			this.btnSpiral.UseVisualStyleBackColor = true;
			this.btnSpiral.Click += new System.EventHandler(this.btnSpiral_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 82);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Spiral Method";
			// 
			// cmbSpiralFuncs
			// 
			this.cmbSpiralFuncs.FormattingEnabled = true;
			this.cmbSpiralFuncs.Location = new System.Drawing.Point(11, 98);
			this.cmbSpiralFuncs.Name = "cmbSpiralFuncs";
			this.cmbSpiralFuncs.Size = new System.Drawing.Size(260, 21);
			this.cmbSpiralFuncs.TabIndex = 11;
			// 
			// btnMatrix
			// 
			this.btnMatrix.Location = new System.Drawing.Point(179, 125);
			this.btnMatrix.Name = "btnMatrix";
			this.btnMatrix.Size = new System.Drawing.Size(43, 23);
			this.btnMatrix.TabIndex = 12;
			this.btnMatrix.Text = "Matrix";
			this.btnMatrix.UseVisualStyleBackColor = true;
			this.btnMatrix.Click += new System.EventHandler(this.btnMatrix_Click);
			// 
			// btnDivPrimes
			// 
			this.btnDivPrimes.Location = new System.Drawing.Point(11, 198);
			this.btnDivPrimes.Name = "btnDivPrimes";
			this.btnDivPrimes.Size = new System.Drawing.Size(75, 23);
			this.btnDivPrimes.TabIndex = 13;
			this.btnDivPrimes.Text = "Div Primes";
			this.btnDivPrimes.UseVisualStyleBackColor = true;
			this.btnDivPrimes.Click += new System.EventHandler(this.btnDivPrimes_Click);
			// 
			// btnRealSpiral
			// 
			this.btnRealSpiral.Location = new System.Drawing.Point(129, 125);
			this.btnRealSpiral.Name = "btnRealSpiral";
			this.btnRealSpiral.Size = new System.Drawing.Size(44, 23);
			this.btnRealSpiral.TabIndex = 14;
			this.btnRealSpiral.Text = "Spiral";
			this.btnRealSpiral.UseVisualStyleBackColor = true;
			this.btnRealSpiral.Click += new System.EventHandler(this.btnRealSpiral_Click);
			// 
			// btnBifurcate
			// 
			this.btnBifurcate.Location = new System.Drawing.Point(196, 198);
			this.btnBifurcate.Name = "btnBifurcate";
			this.btnBifurcate.Size = new System.Drawing.Size(75, 23);
			this.btnBifurcate.TabIndex = 15;
			this.btnBifurcate.Text = "Bifurcate";
			this.btnBifurcate.UseVisualStyleBackColor = true;
			this.btnBifurcate.Click += new System.EventHandler(this.btnBifurcate_Click);
			// 
			// btn_ZigZag
			// 
			this.btn_ZigZag.Location = new System.Drawing.Point(79, 125);
			this.btn_ZigZag.Name = "btn_ZigZag";
			this.btn_ZigZag.Size = new System.Drawing.Size(44, 23);
			this.btn_ZigZag.TabIndex = 16;
			this.btn_ZigZag.Text = "ZigZag";
			this.btn_ZigZag.UseVisualStyleBackColor = true;
			this.btn_ZigZag.Click += new System.EventHandler(this.btn_ZigZag_Click);
			// 
			// btn_divDrips
			// 
			this.btn_divDrips.Location = new System.Drawing.Point(104, 198);
			this.btn_divDrips.Name = "btn_divDrips";
			this.btn_divDrips.Size = new System.Drawing.Size(75, 23);
			this.btn_divDrips.TabIndex = 17;
			this.btn_divDrips.Text = "Div Drips";
			this.btn_divDrips.UseVisualStyleBackColor = true;
			this.btn_divDrips.Click += new System.EventHandler(this.btnDivDrips_Click);
			// 
			// ConfigForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 285);
			this.Controls.Add(this.btn_divDrips);
			this.Controls.Add(this.btn_ZigZag);
			this.Controls.Add(this.btnBifurcate);
			this.Controls.Add(this.btnRealSpiral);
			this.Controls.Add(this.btnDivPrimes);
			this.Controls.Add(this.btnMatrix);
			this.Controls.Add(this.cmbSpiralFuncs);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnSpiral);
			this.Controls.Add(this.btnDungeon);
			this.Controls.Add(this.btnDirect2D);
			this.Controls.Add(this.btnParticle);
			this.Controls.Add(this.btnTerrain);
			this.Controls.Add(this.btnPalette);
			this.Controls.Add(this.btnBuddha);
			this.Controls.Add(this.lblRenderMethod);
			this.Controls.Add(this.btnDrawCmplx);
			this.Controls.Add(this.cmbFunctions);
			this.Name = "ConfigForm";
			this.Text = "Complex visualizer";
			this.Load += new System.EventHandler(this.Config_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cmbFunctions;
		private System.Windows.Forms.Button btnDrawCmplx;
		private System.Windows.Forms.Label lblRenderMethod;
		private System.Windows.Forms.Button btnBuddha;
		private System.Windows.Forms.Button btnPalette;
		private System.Windows.Forms.Button btnTerrain;
		private System.Windows.Forms.Button btnParticle;
		private System.Windows.Forms.Button btnDirect2D;
		private System.Windows.Forms.Button btnDungeon;
		private System.Windows.Forms.Button btnSpiral;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cmbSpiralFuncs;
		private System.Windows.Forms.Button btnMatrix;
		private System.Windows.Forms.Button btnDivPrimes;
		private System.Windows.Forms.Button btnRealSpiral;
		private System.Windows.Forms.Button btnBifurcate;
		private System.Windows.Forms.Button btn_ZigZag;
		private System.Windows.Forms.Button btn_divDrips;
	}
}