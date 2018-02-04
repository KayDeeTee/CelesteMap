using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class ForegroundDebris : Entity {
		public ForegroundDebris() {
			Depth = -999900;
		}
		public static ForegroundDebris FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			ForegroundDebris entity = new ForegroundDebris();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			List<Bitmap> imgs = Gameplay.GetAllImages("scenery/fgdebris/" + Util.Random.Choose("rock_a", "rock_b"));
			if (imgs == null) { return; }

			for (int i = imgs.Count - 1; i >= 0; i--) {
				Bitmap img = imgs[i];
				img.Save(i + ".png");
				map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height / 2);
				img.Dispose();
			}
		}
	}
}