using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class Haha : Entity {
		public Haha() {
			Depth = -10001;
		}
		public static Haha FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			Haha entity = new Haha();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("characters/oldlady/ha", 8);
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height);
			img.Dispose();
		}
	}
}