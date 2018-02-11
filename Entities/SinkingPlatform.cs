using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class SinkingPlatform : Entity {
		public int Width;
		public SinkingPlatform(int width) {
			Width = width;
			Depth = 9001;
		}
		public static SinkingPlatform FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);

			SinkingPlatform entity = new SinkingPlatform(width);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/woodPlatform/default");
			if (img == null) { return; }

			int widthTiles = Width / 8;
			int x = (int)Position.X;
			int y = (int)Position.Y;

			using (Pen pen = new Pen(Util.HexToColor("2a1923"))) {
				map.DrawRectangle(pen, x + Width / 2 - 2, y, 3, map.VisibleClipBounds.Height - y);
			}
			using (Pen pen = new Pen(Util.HexToColor("160b12"))) {
				map.DrawRectangle(pen, x + Width / 2 - 1, y + 1, 1, map.VisibleClipBounds.Height - y);
			}

			for (int i = 0; i < widthTiles; i++) {
				int xOffset;
				if (i == 0) {
					xOffset = 0;
				} else if (i == widthTiles - 1) {
					xOffset = 3;
				} else if (i == widthTiles / 2) {
					xOffset = 2;
				} else {
					xOffset = 1;
				}

				using (Bitmap sub = Util.GetSubImage(img, new Sprite(xOffset * 8, 0, 8, 8, 0, 0, 8, 8))) {
					map.DrawImage(sub, x, y);
				}
				x += 8;
			}

			img.Dispose();
		}
	}
}