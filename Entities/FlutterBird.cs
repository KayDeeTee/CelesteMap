using CelesteMap.Utility;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
namespace CelesteMap.Entities {
	public class FlutterBird : Entity {
		private static readonly Color[] colors = new Color[]
		{
			Util.HexToColor("89fbff"),
			Util.HexToColor("f0fc6c"),
			Util.HexToColor("f493ff"),
			Util.HexToColor("93baff")
		};
		public FlutterBird() {
			Depth = -9999;
		}
		public static FlutterBird FromElement(XmlNode node) {
			int x = (int)node.AttrFloat("x", 0);
			int y = (int)node.AttrFloat("y", 0);

			FlutterBird entity = new FlutterBird();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("scenery/flutterbird/idle00");
			if (img == null) { return; }

			Color color = Util.Random.Choose(FlutterBird.colors);
			ColorMatrix matrix = new ColorMatrix();
			matrix.Matrix40 = color.R / 255f - 1f;
			matrix.Matrix41 = color.G / 255f - 1f;
			matrix.Matrix42 = color.B / 255f - 1f;
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			map.DrawImage(img, new Rectangle((int)Position.X - img.Width / 2, (int)Position.Y - img.Height, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);

			img.Dispose();
		}
	}
}