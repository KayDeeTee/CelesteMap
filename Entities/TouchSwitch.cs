using CelesteMap.Utility;
using System.Drawing;
using System.Drawing.Imaging;
namespace CelesteMap.Entities {
	public class TouchSwitch : Entity {
		public TouchSwitch() {
			Depth = 2000;
		}
		public static TouchSwitch FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			TouchSwitch entity = new TouchSwitch();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/touchswitch/container");
			Bitmap icon = Gameplay.GetImage("objects/touchswitch/icon00");
			if (img == null) { return; }

			Color color = Util.HexToColor("5fcde4");
			ColorMatrix matrix = new ColorMatrix();
			matrix.Matrix40 = color.R / 255f - 1f;
			matrix.Matrix41 = color.G / 255f - 1f;
			matrix.Matrix42 = color.B / 255f - 1f;
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

			map.DrawImage(img, new Rectangle((int)Position.X - img.Width / 2, (int)Position.Y - img.Height / 2, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);
			map.DrawImage(icon, new Rectangle((int)Position.X - icon.Width / 2, (int)Position.Y - icon.Height / 2, icon.Width, icon.Height), 0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel, attributes);
			icon.Dispose();
			img.Dispose();
		}
	}
}