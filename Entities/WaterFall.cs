using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class WaterFall : Entity {
		public WaterFall() {
			Depth = -9999;
		}
		public static WaterFall FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			WaterFall entity = new WaterFall();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			int tx = (int)Position.X / 8;
			int ty = (int)Position.Y / 8;
			int height = 0;
			while (ty < solids.Rows && solids[tx, ty] == solids.EmptyValue) {
				height += 8;
				ty++;
			}
			using (SolidBrush brush = new SolidBrush(Color.FromArgb(76, Color.LightSkyBlue))) {
				map.FillRectangle(brush, Position.X + 1, Position.Y, 6, height);
			}
			using (SolidBrush brush = new SolidBrush(Color.FromArgb(204, Color.LightSkyBlue))) {
				map.FillRectangle(brush, Position.X - 1, Position.Y, 2, height);
				map.FillRectangle(brush, Position.X + 7, Position.Y, 2, height);
			}
		}
	}
}