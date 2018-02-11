using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class ClutterCabinet : Entity {
		public ClutterCabinet() {
			Depth = 0;
		}
		public static ClutterCabinet FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			ClutterCabinet entity = new ClutterCabinet();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/resortclutter/cabinet00");
			if (img == null) { return; }

			map.DrawImage(img, Position.X - 16, Position.Y - 16);
			img.Dispose();
		}
	}
}