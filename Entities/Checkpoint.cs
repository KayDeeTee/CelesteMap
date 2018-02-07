using CelesteMap.Utility;
using System.Drawing;
using System.Drawing.Imaging;
namespace CelesteMap.Entities {
	public class Checkpoint :Entity{
		public Checkpoint() {
			Depth = 0;
		}
		public static Checkpoint FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			Checkpoint entity = new Checkpoint();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/checkpoint/flag15");
			if (img == null) { return; }

			ColorMatrix matrix = new ColorMatrix();
			matrix.Matrix33 = 0.5f;
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			map.DrawImage(img, new Rectangle((int)Position.X - img.Width / 2, (int)Position.Y - img.Height, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);

			img.Dispose();
		}
	}
}