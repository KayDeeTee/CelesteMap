using System.Drawing;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class DreamMirror : Entity {
		public DreamMirror() {
			Depth = 9500;
		}
		public static DreamMirror FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			DreamMirror entity = new DreamMirror();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap bg = Gameplay.GetImage("objects/mirror/glassbg");
			Bitmap frame = Gameplay.GetImage("objects/mirror/frame");
			Bitmap fg = Gameplay.GetImage("objects/mirror/glassfg");
			if (bg == null) { return; }

			map.DrawImage(bg, Position.X - bg.Width / 2, Position.Y - bg.Height);
			map.DrawImage(fg, Position.X - bg.Width / 2, Position.Y - bg.Height);
			map.DrawImage(frame, Position.X - frame.Width / 2, Position.Y - frame.Height);
			fg.Dispose();
			frame.Dispose();
			bg.Dispose();
		}
	}
}