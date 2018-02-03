using System.Drawing;
namespace CelesteMap.Backdrops {
	public class Snow : Backdrop {
		public bool Foreground;

		public Snow(bool foreground) {
			Foreground = foreground;
		}
		public override void Render(Rectangle bounds, Graphics map) {
		}
	}
}