using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class FakeWall : Entity {
		public char TileType;
		public int Width, Height;
		public FakeWall(char tileType, int width, int height) {
			TileType = tileType;
			Width = width;
			Height = height;
			Depth = -13000;
		}
		public static FakeWall FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			char tileType = node.AttrChar("tiletype", '3');
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);

			FakeWall entity = new FakeWall(tileType, width, height);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetTiler(false).GenerateOverlay(TileType, (int)Position.X / 8, (int)Position.Y / 8, Width / 8, Height / 8, solids).DisplayMap(null, Rectangle.Empty, false);
			if (img == null) { return; }

			ColorMatrix matrix = new ColorMatrix();
			matrix.Matrix33 = 0.5f;
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			map.DrawImage(img, new Rectangle((int)Position.X, (int)Position.Y, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);

			img.Dispose();
		}
	}
}