namespace CelesteMap {
	partial class Maper {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.btnLoad = new System.Windows.Forms.Button();
			this.txtFilePath = new System.Windows.Forms.TextBox();
			this.map = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.map)).BeginInit();
			this.SuspendLayout();
			// 
			// btnLoad
			// 
			this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLoad.Location = new System.Drawing.Point(486, -1);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(75, 22);
			this.btnLoad.TabIndex = 0;
			this.btnLoad.Text = "Load";
			this.btnLoad.UseVisualStyleBackColor = true;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// txtFilePath
			// 
			this.txtFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtFilePath.Location = new System.Drawing.Point(0, 0);
			this.txtFilePath.Name = "txtFilePath";
			this.txtFilePath.Size = new System.Drawing.Size(487, 20);
			this.txtFilePath.TabIndex = 1;
			this.txtFilePath.Text = "D:\\Steam\\steamapps\\common\\Celeste\\Content\\Maps\\2-OldSite.bin";
			// 
			// map
			// 
			this.map.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.map.Location = new System.Drawing.Point(0, 20);
			this.map.Name = "map";
			this.map.Size = new System.Drawing.Size(560, 199);
			this.map.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.map.TabIndex = 2;
			this.map.TabStop = false;
			// 
			// Maper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(560, 219);
			this.Controls.Add(this.map);
			this.Controls.Add(this.txtFilePath);
			this.Controls.Add(this.btnLoad);
			this.Name = "Maper";
			this.ShowIcon = false;
			this.Text = "Celeste Map Maker";
			((System.ComponentModel.ISupportInitialize)(this.map)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.TextBox txtFilePath;
		private System.Windows.Forms.PictureBox map;
	}
}

