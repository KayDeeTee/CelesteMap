using CelesteMap.Utility;
using System.Drawing;
using System.Xml;
namespace CelesteMap.Entities {
	public class ForsakenCityGem : Entity {
		private HeartGem heart;
		public ForsakenCityGem(int gemX, int gemY) {
			heart = new HeartGem();
			heart.Position = new Vector2(gemX, gemY);
			Depth = 8999;
		}
		public static ForsakenCityGem FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int gemX = 0, gemY = 0;
			int i = 0;
			foreach (XmlNode child in node.ChildNodes) {
				if (i++ == 1) {
					gemX = child.AttrInt("x", 0);
					gemY = child.AttrInt("y", 0);
					break;
				}
			}

			ForsakenCityGem entity = new ForsakenCityGem(gemX, gemY);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap satelite = Gameplay.GetImage("objects/citysatellite/dish");
			map.DrawImage(satelite, Position.X - satelite.Width / 2, Position.Y - satelite.Height);
			satelite.Dispose();
			Bitmap computer = Gameplay.GetImage("objects/citysatellite/computer");
			map.DrawImage(computer, Position.X - computer.Width / 2 + 32, Position.Y - computer.Height + 40);
			computer.Dispose();
			Bitmap screen = Gameplay.GetImage("objects/citysatellite/computerscreen");
			map.DrawImage(screen, Position.X - screen.Width / 2 + 32, Position.Y - screen.Height + 40);
			screen.Dispose();
			heart.Render(map, solids);
		}
	}
}