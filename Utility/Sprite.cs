using System.Drawing;
namespace CelesteMap.Utility {
	public class Sprite {
		public Rectangle Bounds, Offset;
		public Sprite(int x, int y, int width, int height, int offsetX, int offsetY, int widthOffset, int heightOffset) {
			Bounds = new Rectangle(x, y, width, height);
			Offset = new Rectangle(-offsetX, -offsetY, widthOffset, heightOffset);
		}
		public override string ToString() {
			return Bounds.ToString() + " " + Offset.ToString();
		}
	}
}