using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class Payphone : Entity {
		public Payphone() {
			Depth = 1;
		}
		public static Payphone FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			Payphone entity = new Payphone();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("cutscenes/payphone/phone00");
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height);
			img.Dispose();
		}
	}
}