using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Data;
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
			string paths = @"D:\Steam\steamapps\common\Celeste\Content\Maps\";
			string[] files = Directory.GetFiles(paths, "*.bin", SearchOption.AllDirectories);
			DataTable dt = new DataTable();
			dt.Columns.Add("Name");
			dt.Columns.Add("Path");
			for (int i = 0; i < files.Length; i++) {
				string path = files[i];
				dt.Rows.Add(Path.GetFileName(path), path);
			}
			cboMaps.DisplayMember = "Name";
			cboMaps.ValueMember = "Path";
			dt.DefaultView.Sort = "Name";
			cboMaps.DataSource = dt;
			cboMaps.SelectedIndex = 0;
		}

		private void btnLoad_Click(object sender, EventArgs e) {
			try {
				DecodeMap();

				string fileName = Path.GetFileNameWithoutExtension(cboMaps.SelectedValue.ToString());
				MapElement element = MapCoder.FromBinary(cboMaps.SelectedValue.ToString());
				Bitmap chapter = gameplay.GenerateMap(element);
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

			//string paths = @"D:\Steam\steamapps\common\Celeste\Content\Maps\";
			//string[] files = Directory.GetFiles(paths, "*.bin", SearchOption.AllDirectories);
			//Stopwatch sw = new Stopwatch();
			//sw.Start();
			//for (int i = files.Length - 1; i >= 0; i--) {
			//	string file = files[i];
			//	string fileName = Path.GetFileNameWithoutExtension(file);
			//	string xmlName = fileName + ".xml";
			//	//MapElement element = MapCoder.FromBinary(file);
			//	MapCoder.ToXML(file, xmlName);
			//}
			//sw.Stop();
			//Console.WriteLine(sw.Elapsed.TotalSeconds);

			string fileName = Path.GetFileNameWithoutExtension(cboMaps.SelectedValue.ToString());
			string xmlName = fileName + ".xml";
			MapCoder.ToXML(cboMaps.SelectedValue.ToString(), xmlName);
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
