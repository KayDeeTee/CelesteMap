using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class HangingLamp : Entity {
		int Height;
		public HangingLamp(int height) {
			Height = height < 16 ? 16 : height;
			Depth = 2000;
		}
		public static HangingLamp FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int height = node.AttrInt("height", 0);

			HangingLamp entity = new HangingLamp(height);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/hanginglamp");
			if (img == null) { return; }

			using (Bitmap sub = Util.GetSubImage(img, new Sprite(0, 8, 8, 8, 0, 0, 8, 8))) {
				for (int i = 0; i < Height - 8; i += 8) {
					map.DrawImage(sub, Position.X, Position.Y + i);
				}
			}
			using (Bitmap sub = Util.GetSubImage(img, new Sprite(0, 0, 8, 8, 0, 0, 8, 8))) {
				map.DrawImage(sub, Position.X, Position.Y);
			}
			using (Bitmap sub = Util.GetSubImage(img, new Sprite(0, 16, 8, 8, 0, 0, 8, 8))) {
				map.DrawImage(sub, Position.X, Position.Y + Height - 8);
			}

			img.Dispose();
		}
	}
}