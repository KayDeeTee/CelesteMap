using CelesteMap.Utility;
using System.Collections.Generic;
using System.Drawing;
namespace CelesteMap.Backdrops {
	public class StarsBG : Backdrop {
		public StarsBG() {
		}
		public override void Render(Rectangle bounds, Graphics map) {
			List<Bitmap> imgs = Gameplay.GetAllImages("bgs/02/stars/a");
			imgs.AddRange(Gameplay.GetAllImages("bgs/02/stars/b"));
			imgs.AddRange(Gameplay.GetAllImages("bgs/02/stars/c"));

			for (int i = 0; i < 100; i++) {
				Bitmap img = Util.Random.Choose(imgs);

				map.DrawImage(img, Util.Random.Next((int)map.VisibleClipBounds.Width), Util.Random.Next((int)map.VisibleClipBounds.Height));
			}
			for (int i = 0; i < imgs.Count; i++) {
				imgs[i].Dispose();
			}
		}
	}
}