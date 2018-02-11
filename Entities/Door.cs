using System;
using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class Door : Entity {
		public string Texture;
		public Door(string texture) {
			Texture = texture;
			Depth = 8999;
		}
		public static Door FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			Door entity = new Door(node.Attr("type", "wood"));
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage(Texture.Equals("wood", StringComparison.OrdinalIgnoreCase) ? "objects/door/door00" : "objects/door/" + Texture + "door00");
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height);
			img.Dispose();
		}
	}
}