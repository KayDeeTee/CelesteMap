using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class Water : Entity {
		public int Width, Height;
		public Water(int width, int height) {
			Width = width;
			Height = height;
			Depth = -9999;
		}
		public static Water FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);

			Water entity = new Water(width, height);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			using (SolidBrush brush = new SolidBrush(Color.FromArgb(76, Color.LightSkyBlue))) {
				map.FillRectangle(brush, Position.X + 2, Position.Y + 2, Width - 4, Height - 4);
			}
			using (SolidBrush brush = new SolidBrush(Color.FromArgb(204, Color.LightSkyBlue))) {
				map.FillRectangle(brush, Position.X, Position.Y, Width, 2);
				map.FillRectangle(brush, Position.X, Position.Y + 2, 2, Height - 4);
				map.FillRectangle(brush, Position.X + Width - 2, Position.Y + 2, 2, Height - 4);
				map.FillRectangle(brush, Position.X, Position.Y + Height - 2, Width, 2);
			}
		}
	}
}