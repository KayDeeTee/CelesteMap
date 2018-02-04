using System.Drawing;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class InvisibleBarrier : Entity {
		public int Width, Height;
		public InvisibleBarrier(int width, int height) {
			Width = width;
			Height = height;
			Depth = 0;
		}
		public static InvisibleBarrier FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);

			InvisibleBarrier entity = new InvisibleBarrier(width, height);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			
		}
	}
}