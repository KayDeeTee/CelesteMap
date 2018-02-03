using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap {
	public partial class Maper : Form {
		private Gameplay gameplay;
		
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Maper());
		}
		public Maper() {
			InitializeComponent();
			gameplay = new Gameplay();
		}

		private void btnLoad_Click(object sender, EventArgs e) {
			try {
				DecodeMap();

				string fileName = Path.GetFileNameWithoutExtension(txtFilePath.Text);
				XmlDocument xml = MapCoder.ToXML(txtFilePath.Text);
				Bitmap chapter = gameplay.GenerateMap(xml);
				chapter.Save(fileName + ".png");

				if (map.Image != null) {
					map.Image.Dispose();
				}
				map.Image = chapter;
			} catch (Exception ex) {
				MessageBox.Show(this, ex.ToString());
			}
		}
		private void DecodeMap() {
			string fileName = Path.GetFileNameWithoutExtension(txtFilePath.Text);
			string xmlName = fileName + ".xml";
			MapCoder.ToXML(txtFilePath.Text, xmlName);
			//MapCoder.ToBinary(xmlName, fileName + ".bin");
		}
		private void DecodeGraphics() {
			Directory.CreateDirectory("Images");
			string path = @"D:\Steam\steamapps\common\Celeste\Content\Graphics\";
			string[] files = Directory.GetFiles(path, "*.data", SearchOption.AllDirectories);
			for (int i = files.Length - 1; i >= 0; i--) {
				string file = files[i];
				Image bmp = Texture.Read(file);
				string savePath = "Images\\" + Path.GetDirectoryName(file.Substring(path.Length)) + "\\" + Path.GetFileNameWithoutExtension(file) + ".png";
				Directory.CreateDirectory(Path.GetDirectoryName(savePath));
				bmp.Save(savePath);
			}
		}
	}
}
