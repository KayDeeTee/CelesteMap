using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class DarkChaser : Entity {
		public DarkChaser() {
			Depth = -12500;
		}
		public static DarkChaser FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			DarkChaser entity = new DarkChaser();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("characters/badeline/sleep02");
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height);

			img.Dispose();
		}
	}
}