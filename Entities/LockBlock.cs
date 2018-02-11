using CelesteMap.Utility;
using System;
using System.Drawing;
namespace CelesteMap.Entities {
	public class LockBlock : Entity {
		public string Texture;
		public LockBlock(string texture) {
			Texture = texture;
			Depth = 0;
		}
		public static LockBlock FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			LockBlock entity = new LockBlock(node.Attr("sprite", "wood"));
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage(Texture.Equals("wood", StringComparison.OrdinalIgnoreCase) ? "objects/door/lockdoor00" : "objects/door/lockdoor" + Texture + "00");
			if (img == null) { return; }

			map.DrawImage(img, Position.X - 32, Position.Y - 16);
			img.Dispose();
		}
	}
}