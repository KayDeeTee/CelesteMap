using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class ClutterDoor : Entity {
		public ClutterDoor() {
			Depth = 8999;
		}
		public static ClutterDoor FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			ClutterDoor entity = new ClutterDoor();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/door/ghost_door00");
			if (img == null) { return; }

			map.DrawImage(img, Position.X - 16, Position.Y - 16);
			img.Dispose();
		}
	}
}