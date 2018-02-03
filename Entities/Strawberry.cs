using System;
using System.Drawing;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class Strawberry : Entity {
		public Berry Type;
		public enum Berry {
			Normal,
			Flying,
			Golden
		}
		public Strawberry(Berry type) {
			Type = type;
			Depth = -100;
		}
		public static Strawberry FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			Strawberry entity = null;
			if (node.Name.Equals("goldenBerry", StringComparison.OrdinalIgnoreCase)) {
				entity = new Strawberry(Berry.Golden);
			} else {
				bool winged = node.AttrBool("winged", false);
				entity = new Strawberry(winged ? Berry.Flying : Berry.Normal);
			}
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			string path = "collectables/strawberry/normal00";
			if (Type == Berry.Flying) {
				path = "collectables/strawberry/wings00";
			} else if (Type == Berry.Golden) {
				path = "collectables/goldberry/idle00";
			}
			Bitmap img = Gameplay.GetImage(path);
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height / 2);
			img.Dispose();
		}
	}
}