using CelesteMap.Utility;
using System.Drawing;
using System.Xml;
namespace CelesteMap.Entities {
	public class FallingBlock : Entity {
		public char TileType;
		public int Width, Height;
		public FallingBlock(char tileType, int width, int height, bool behind) {
			TileType = tileType;
			Width = width;
			Height = height;
			Depth = behind ? 5000 : 0;
		}
		public static FallingBlock FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			char tileType = node.AttrChar("tiletype", '3');
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);
			bool behind = node.AttrBool("behind", false);

			FallingBlock entity = new FallingBlock(tileType, width, height, behind);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetTiler(false).GenerateBox(TileType, Width / 8, Height / 8).DisplayMap(null, Rectangle.Empty, false);
			if (img == null) { return; }

			map.DrawImage(img, (int)Position.X, (int)Position.Y);

			img.Dispose();
		}
	}
}