using CelesteMap.Utility;
using System.Drawing;
using System.Xml;
namespace CelesteMap.Entities {
	public class CoverupWall : Entity {
		public char TileType;
		public int Width, Height;
		public CoverupWall(char tileType, int width, int height) {
			TileType = tileType;
			Width = width;
			Height = height;
			Depth = -13000;
		}
		public static CoverupWall FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			char tileType = node.AttrChar("tiletype", '3');
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);

			CoverupWall entity = new CoverupWall(tileType, width, height);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetTiler(false).GenerateOverlay(TileType, (int)Position.X / 8, (int)Position.Y / 8, Width / 8, Height / 8, solids).DisplayMap(null, Rectangle.Empty, false);
			if (img == null) { return; }

			map.DrawImage(img, (int)Position.X, (int)Position.Y);

			img.Dispose();
		}
	}
}