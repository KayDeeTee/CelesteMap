using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class BlockField : Entity {
		public BlockField() {
			Depth = 0;
		}
		public static BlockField FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			BlockField entity = new BlockField();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			
		}
	}
}