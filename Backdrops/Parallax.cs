using System.Drawing;
using System.Drawing.Imaging;
using CelesteMap.Utility;
namespace CelesteMap.Backdrops {
	public enum BlendState {
		Opaque,
		AlphaBlend,
		Additive,
		NonPremultiplied
	}
	public class Parallax : Backdrop {
		public Vector2 CameraOffset = Vector2.Zero;
		public BlendState BlendState = BlendState.AlphaBlend;
		public Bitmap Texture;

		public Parallax(Bitmap texture) {
			Texture = texture;
		}
		public override void Render(Rectangle bounds, Graphics map) {
			if (Texture == null) { return; }

			Bitmap drawImg = Texture;
			if (FlipX || FlipY) {
				drawImg = new Bitmap(Texture);
				if (FlipX && FlipY) {
					drawImg.RotateFlip(RotateFlipType.RotateNoneFlipXY);
				} else if (FlipX) {
					drawImg.RotateFlip(RotateFlipType.RotateNoneFlipX);
				} else {
					drawImg.RotateFlip(RotateFlipType.RotateNoneFlipY);
				}
			}
			int x = (int)Util.Lerp(Position.X, Position.X - bounds.X, Scroll.X);
			if (LoopX) {
				while (x > map.VisibleClipBounds.Width) {
					x -= drawImg.Width;
				}
				while (x < -drawImg.Width) {
					x += drawImg.Width;
				}
				if (x > 0) {
					x -= drawImg.Width;
				}
			}

			int y = (int)Util.Lerp(Position.Y, bounds.Height - bounds.Y + Position.Y - map.VisibleClipBounds.Height, Scroll.Y);
			if (LoopY) {
				while (y > drawImg.Height) {
					y -= drawImg.Height;
				}
				while (y < -map.VisibleClipBounds.Height) {
					y += drawImg.Height;
				}
				if (y < 0) {
					y += drawImg.Height;
				}
			}
			y += (int)(map.VisibleClipBounds.Height - drawImg.Height);
			do {
				do {
					ColorMatrix matrix = new ColorMatrix();
					matrix.Matrix33 = (float)Color.A / 255f;
					ImageAttributes attributes = new ImageAttributes();
					attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
					map.DrawImage(drawImg, new Rectangle((int)x, (int)y, drawImg.Width, drawImg.Height), 0, 0, drawImg.Width, drawImg.Height, GraphicsUnit.Pixel, attributes);

					//map.DrawImage(drawImg, x, y);
					x += drawImg.Width;
				} while (LoopX && x < map.VisibleClipBounds.Width);
				y -= drawImg.Height;
			} while (LoopY && y > -drawImg.Height);
		}
	}
}