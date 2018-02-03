using System.Drawing;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class Spring : Entity {
		public bool PlayerCanUse;
		public Spring(bool canUse) {
			PlayerCanUse = canUse;
			Depth = -51;
		}
		public static Spring FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			bool canUse = node.AttrBool("PlayerCanUse", true);

			Spring entity = new Spring(canUse);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/spring/00");
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height);
			img.Dispose();
		}
	}
}