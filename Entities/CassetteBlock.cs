using CelesteMap.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
namespace CelesteMap.Entities {
	public class CassetteBlock : Entity {
		public static List<CassetteBlock> Blocks = new List<CassetteBlock>();
		public enum Type {
			Blue,
			Pink = 1,
			Purple = 2,
			Orange = 3
		}
		public Type Color;
		public List<Rectangle> Bounds;
		public CassetteBlock(Rectangle bounds, Type color) {
			Depth = -9990;
			Bounds = new List<Rectangle>();
			Bounds.Add(bounds);
			Color = color;
		}
		public static CassetteBlock FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);
			int index = node.AttrInt("index", 0);

			Rectangle bounds = new Rectangle(x, y, width, height);
			for (int i = 0; i < Blocks.Count; i++) {
				CassetteBlock block = Blocks[i];
				if (block.Color == (Type)index && block.CheckSame(bounds)) {
					block.Bounds.Add(bounds);
					return null;
				}
			}

			CassetteBlock entity = new CassetteBlock(bounds, (Type)index);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			Blocks.Add(entity);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/cassetteblock/solid");
			if (img == null) { return; }

			List<Bitmap> sub = new List<Bitmap>();
			for (int j = 0; j < 4; j++) {
				for (int i = 0; i < 4; i++) {
					sub.Add(Util.GetSubImage(img, new Sprite(i * 8, j * 8, 8, 8, 0, 0, 8, 8)));
				}
			}

			int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
			for (int i = 0; i < Bounds.Count; i++) {
				Rectangle rect = Bounds[i];
				if (rect.X < minX) {
					minX = rect.X;
				}
				if (rect.X + rect.Width > maxX) {
					maxX = rect.X + rect.Width;
				}
				if (rect.Y < minY) {
					minY = rect.Y;
				}
				if (rect.Y + rect.Height > maxY) {
					maxY = rect.Y + rect.Height;
				}
			}

			Color color;
			switch (Color) {
				case Type.Pink: color = Util.HexToColor("f049be"); break;
				case Type.Purple: color = Util.HexToColor("c547cb"); break;
				case Type.Orange: color = Util.HexToColor("b68026"); break;
				default: color = Util.HexToColor("49aaf0"); break;
			}

			ColorMatrix matrix = new ColorMatrix();
			matrix.Matrix40 = color.R / 255f - 1f;
			matrix.Matrix41 = color.G / 255f - 1f;
			matrix.Matrix42 = color.B / 255f - 1f;
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

			for (int j = minY; j < maxY; j += 8) {
				for (int i = minX; i < maxX; i += 8) {
					bool exists = Contains(i, j);
					if (!exists) { continue; }

					bool left = Contains(i - 8, j);
					bool right = Contains(i + 8, j);
					bool up = Contains(i, j - 8);
					bool down = Contains(i, j + 8);
					Bitmap tile = null;
					if (left && right && up && down) {
						if (!Contains(i + 8, i - 8)) {
							tile = sub[3];
						} else if (!Contains(i + 8, i + 8)) {
							tile = sub[11];
						} else if (!Contains(i - 8, i - 8)) {
							tile = sub[7];
						} else if (!Contains(i - 8, i + 8)) {
							tile = sub[15];
						} else {
							tile = sub[5];
						}
					} else if (left && up && down) {
						tile = sub[6];
					} else if (right && up && down) {
						tile = sub[4];
					} else if (left && right && up) {
						tile = sub[9];
					} else if (left && right && down) {
						tile = sub[1];
					} else if (left && down) {
						tile = sub[2];
					} else if (right && down) {
						tile = sub[0];
					} else if (right && up) {
						tile = sub[8];
					} else {
						tile = sub[10];
					}

					map.DrawImage(tile, new Rectangle(i, j, 8, 8), 0, 0, 8, 8, GraphicsUnit.Pixel, attributes);
				}
			}

			for (int i = 0; i < sub.Count; i++) {
				sub[i].Dispose();
			}
			img.Dispose();
		}
		public bool CheckSame(Rectangle bounds) {
			for (int i = 0; i < Bounds.Count; i++) {
				Rectangle rect = Bounds[i];
				if (rect.IntersectsWith(new Rectangle(bounds.X - 1, bounds.Y, bounds.Width + 2, bounds.Height)) || rect.IntersectsWith(new Rectangle(bounds.X, bounds.Y - 1, bounds.Width, bounds.Height + 2))) {
					return true;
				}
			}
			return false;
		}
		public bool Contains(int x, int y) {
			Point point = new Point(x, y);
			for (int i = 0; i < Bounds.Count; i++) {
				Rectangle rect = Bounds[i];
				if (rect.Contains(point)) {
					return true;
				}
			}
			return false;
		}
	}
}