using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class Lamp : Entity {
		public bool Broken;
		public Lamp(bool broken) {
			Broken = broken;
			Depth = 5;
		}
		public static Lamp FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			bool broken = node.AttrBool("broken", false);

			Lamp entity = new Lamp(broken);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("scenery/lamp");
			if (img == null) { return; }

			Bitmap sub = Util.GetSubImage(img, new Sprite(Broken ? 16 : 0, 0, 16, 80, 0, 0, 16, 80));
			map.DrawImage(sub, Position.X - sub.Width / 2, Position.Y - sub.Height);
			sub.Dispose();
			img.Dispose();
		}
	}
}