using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class HeartGem : Entity {
		public HeartGem() {
			Depth = -2000000;
		}
		public static HeartGem FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			HeartGem entity = new HeartGem();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("collectables/heartgem/0/00");
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height / 2);
			img.Dispose();
		}
	}
}