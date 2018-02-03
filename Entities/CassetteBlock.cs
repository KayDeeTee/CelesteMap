using CelesteMap.Utility;
using System.Drawing;
using System.Xml;
namespace CelesteMap.Entities {
	public class CassetteBlock : Entity {
		public enum Type {
			Blue,
			Pink = 1,
			Purple = 2,
			Orange = 3
		}
		public Type Color;
		public int Width, Height;
		public CassetteBlock(int width, int height, Type color) {
			Depth = -9990;
			Width = width;
			Height = height;
			Color = color;
		}
		public static CassetteBlock FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);
			int index = node.AttrInt("index", 0);

			CassetteBlock entity = new CassetteBlock(width, height, (Type)index);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/cassetteblock/solid");
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height / 2);
			img.Dispose();
		}
	}
}