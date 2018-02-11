using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class FriendlyGhost : Entity {
		public FriendlyGhost() {
			Depth = 0;
		}
		public static FriendlyGhost FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			FriendlyGhost entity = new FriendlyGhost();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("characters/oshiro/boss20");
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height);
			img.Dispose();
		}
	}
}