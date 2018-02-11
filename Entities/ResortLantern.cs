using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class ResortLantern : Entity {
		public ResortLantern() {
			Depth = 2000;
		}
		public static ResortLantern FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			ResortLantern entity = new ResortLantern();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap holder = Gameplay.GetImage("objects/resortLantern/holder");
			Bitmap lantern = Gameplay.GetImage("objects/resortLantern/lantern00");
			if (holder == null) { return; }

			if (solids[(int)Position.X / 8 + 1, (int)Position.Y / 8] != solids.EmptyValue) {
				holder.RotateFlip(RotateFlipType.RotateNoneFlipX);
				lantern.RotateFlip(RotateFlipType.RotateNoneFlipX);
			}
			map.DrawImage(lantern, Position.X - lantern.Width / 2, Position.Y - lantern.Height / 2);
			map.DrawImage(holder, Position.X - holder.Width / 2, Position.Y - holder.Height / 2);
			holder.Dispose();
		}
	}
}