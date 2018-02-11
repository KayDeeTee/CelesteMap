using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class TrapDoor : Entity {
		public TrapDoor() {
			Depth = 8999;
		}
		public static TrapDoor FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			TrapDoor entity = new TrapDoor();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/door/trap00");
			if (img == null) { return; }

			map.DrawImage(img, Position.X, Position.Y + 7);
			img.Dispose();
		}
	}
}