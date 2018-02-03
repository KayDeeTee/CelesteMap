using System.Drawing;
namespace CelesteMap.Backdrops {
	public class Tentacles : Backdrop {
		public Side TentacleSide;
		public Color TentacleColor;
		public float Offset;
		public Tentacles(Side side, Color color, float offset) {
			TentacleSide = side;
			TentacleColor = color;
			Offset = offset;
		}
		public override void Render(Rectangle bounds, Graphics map) {
		}
		public enum Side {
			Right,
			Left,
			Top,
			Bottom
		}
	}
}