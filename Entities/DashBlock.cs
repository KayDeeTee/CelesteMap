using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class DashBlock : Entity {
		public char TileType;
		public int Width, Height;
		public bool BlendIn;
		public DashBlock(char tileType, int width, int height, bool blend) {
			TileType = tileType;
			Width = width;
			Height = height;
			BlendIn = blend;
			Depth = blend ? -10501 : -12999;
		}
		public static DashBlock FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			char tileType = node.AttrChar("tiletype", '3');
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);
			bool blend = node.AttrBool("blendin", false);

			DashBlock entity = new DashBlock(tileType, width, height, blend);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = null;
			if (BlendIn) {
				img = Gameplay.GetTiler(false).GenerateOverlay(TileType, (int)Position.X / 8, (int)Position.Y / 8, Width / 8, Height / 8, solids).DisplayMap(null, null, Rectangle.Empty, false);
			} else {
				img = Gameplay.GetTiler(false).GenerateBox(TileType, Width / 8, Height / 8).DisplayMap(null, null, Rectangle.Empty, false);
			}
			if (img == null) { return; }

			map.DrawImage(img, (int)Position.X, (int)Position.Y);

			img.Dispose();
		}
	}
}