using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class Cobweb : Entity {
		public Vector2 Target;
		private SimpleCurve curve;
		private Color color;
		public Cobweb(int x, int y, int targetX, int targetY) {
			color = Util.HexToColor("696a6a");
			Position = new Vector2(x, y);
			Target = new Vector2(targetX, targetY);
			curve = new SimpleCurve(Position, Target, Vector2.Zero);
			curve.Control = (curve.Begin + curve.End) / 2f + new Vector2(0f, 24f);
			Depth = -2000;
		}
		public static Cobweb FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			MapElement target = node.SelectFirst("node");
			int targetX = target.AttrInt("x", 0);
			int targetY = target.AttrInt("y", 0);

			Cobweb entity = new Cobweb(x, y, targetX, targetY);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			curve.Render(color, 12, map);
		}
	}
}