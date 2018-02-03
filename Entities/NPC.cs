using System;
using System.Drawing;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class NPC : Entity {
		public string Name;
		public NPC(string name) {
			Name = name;
			Depth = 1000;
		}
		public static NPC FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			string name = node.Attr("npc");

			NPC entity = new NPC(name);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = null;
			if (Name.IndexOf("granny", StringComparison.OrdinalIgnoreCase) >= 0) {
				img = Gameplay.GetImage("characters/oldlady/idle00");
			} else if (Name.IndexOf("crystal", StringComparison.OrdinalIgnoreCase) >= 0) {
				img = Gameplay.GetImage("characters/theoCrystal/idle00");
			} else if (Name.IndexOf("theo", StringComparison.OrdinalIgnoreCase) >= 0) {
				img = Gameplay.GetImage("characters/theo/theo00");
			} else if (Name.IndexOf("oshiro", StringComparison.OrdinalIgnoreCase) >= 0) {
				img = Gameplay.GetImage("characters/oshiro/oshiro00");
			}
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height);
			img.Dispose();
		}
	}
}