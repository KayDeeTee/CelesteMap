using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class FloatingDebris : Entity {
		public FloatingDebris() {
			Depth = -5;
		}
		public static FloatingDebris FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			FloatingDebris entity = new FloatingDebris();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("scenery/debris");
			if (img == null) { return; }

			using (Bitmap sub = Util.GetSubImage(img, new Sprite(Util.Random.Next(img.Width / 8) * 8, 0, 8, 8, 0, 0, 8, 8))) {
				map.DrawImage(sub, Position.X - sub.Width / 2, Position.Y - sub.Height / 2);
			}
			img.Dispose();
		}
	}
}