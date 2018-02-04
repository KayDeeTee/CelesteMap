using CelesteMap.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
namespace CelesteMap.Entities {
	public class DreamBlock : Entity {
		public int Width, Height;
		public bool FastMoving;
		public Vector2 Target;
		public DreamBlock(int width, int height, bool fastMoving, Vector2 target) {
			Width = width;
			Height = height;
			FastMoving = fastMoving;
			Target = target;
			Depth = -11000;
		}
		public static DreamBlock FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);
			bool fast = node.AttrBool("fastMoving", false);
			Vector2 target = new Vector2(x, y);
			XmlNode child = node.SelectSingleNode(".//node");
			if (child != null) {
				target = new Vector2(node.AttrInt("x", 0), node.AttrInt("y", 0));
			}

			DreamBlock entity = new DreamBlock(width, height, fast, target);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap particles = Gameplay.GetImage("objects/dreamblock/particles");
			if (particles == null) { return; }

			List<Bitmap> parts = new List<Bitmap>();
			parts.Add(Util.GetSubImage(particles, new Sprite(14, 0, 7, 7, 0, 0, 7, 7)));
			parts.Add(Util.GetSubImage(particles, new Sprite(7, 0, 7, 7, 0, 0, 7, 7)));
			parts.Add(Util.GetSubImage(particles, new Sprite(0, 0, 7, 7, 0, 0, 7, 7)));

			using (SolidBrush brush = new SolidBrush(Color.Black)) {
				map.FillRectangle(brush, Position.X, Position.Y, Width, Height);
			}

			Bitmap img = null;
			Color color = Color.Red;
			int count = (int)((float)Width * (float)Height / 64f * 0.7f);
			for (int i = 0; i < count; i++) {
				int x = Util.Random.Next(Width);
				int y = Util.Random.Next(Height);
				int layer = Util.Random.Choose(0, 1, 1, 1, 2, 2, 2, 2, 2);
				switch (layer) {
					case 0:
						color = Util.Random.Choose(Util.HexToColor("FFEF11"), Util.HexToColor("FF00D0"), Util.HexToColor("08a310"));
						img = parts[0];
						break;
					case 1:
						color = Util.Random.Choose(Util.HexToColor("5fcde4"), Util.HexToColor("7fb25e"), Util.HexToColor("E0564C"));
						img = parts[1];
						break;
					case 2:
						color = Util.Random.Choose(Util.HexToColor("5b6ee1"), Util.HexToColor("CC3B3B"), Util.HexToColor("7daa64"));
						img = parts[2];
						break;
				}

				if (x + img.Width > Width) { x -= img.Width; }
				if (y + img.Height > Height) { y -= img.Height; }

				ColorMatrix matrix = new ColorMatrix();
				matrix.Matrix40 = color.R / 255f - 1f;
				matrix.Matrix41 = color.G / 255f - 1f;
				matrix.Matrix42 = color.B / 255f - 1f;
				ImageAttributes attributes = new ImageAttributes();
				attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

				map.DrawImage(img, new Rectangle((int)Position.X + x, (int)Position.Y + y, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);
			}

			using (Pen pen = new Pen(Color.White)) {
				map.DrawRectangle(pen, Position.X, Position.Y, Width - 1, Height - 1);
			}

			particles.Dispose();
			for (int i = 0; i < parts.Count; i++) {
				parts[i].Dispose();
			}
		}
	}
}