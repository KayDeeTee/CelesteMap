using CelesteMap.Utility;
using System;
using System.Drawing;
namespace CelesteMap.Entities {
	public class ColorSwitch : Entity {
		public enum BlockColor {
			Red,
			Green,
			Yellow
		}
		public BlockColor Color;
		public ColorSwitch(BlockColor color) {
			Color = color;
			Depth = 0;
		}
		public static ColorSwitch FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			string type = node.Attr("type", "green");

			ColorSwitch entity = new ColorSwitch(type.Equals("green", StringComparison.OrdinalIgnoreCase) ? BlockColor.Green : type.Equals("red", StringComparison.OrdinalIgnoreCase) ? BlockColor.Red : BlockColor.Yellow);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/resortclutter/clutter_button00");
			Bitmap icon = Gameplay.GetImage("objects/resortclutter/icon_" + Color.ToString());
			if (img == null) { return; }

			map.DrawImage(img, Position.X - 32, Position.Y - 32);
			map.DrawImage(icon, Position.X + icon.Width / 2, Position.Y);
			img.Dispose();
		}
	}
}