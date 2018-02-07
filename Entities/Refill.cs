using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class Refill : Entity {
		public Refill() {
			Depth = -100;
		}
		public static Refill FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			Refill entity = new Refill();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/refill/idle00");
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height / 2);
			img.Dispose();
		}
	}
}