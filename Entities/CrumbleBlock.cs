using System.Drawing;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class CrumbleBlock : Entity {
		string Texture;
		int Width;
		public CrumbleBlock(string texture, int width) {
			Texture = texture;
			Width = width;
			Depth = 0;
		}
		public static CrumbleBlock FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);
			string texture = node.Attr("texture", "default");
			CrumbleBlock entity = new CrumbleBlock(texture, width);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/crumbleBlock/" + Texture);
			if (img == null) { return; }

			int xOffset = 0;
			while (xOffset < Width) {
				int tile = (xOffset / 8) % 4;
				using (Bitmap sub = Util.GetSubImage(img, new Sprite(tile * 8, 0, 8, 8, 0, 0, 8, 8))) {
					map.DrawImage(sub, Position.X + xOffset, Position.Y);
				}
				xOffset += 8;
			}

			img.Dispose();
		}
	}
}