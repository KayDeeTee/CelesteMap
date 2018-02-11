using CelesteMap.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace CelesteMap.Entities {
	public class Bridge : Entity {
		public List<Rectangle> TileSizes;
		public int Width, GapStart, GapEnd;
		public Bridge(int width, int gapStart, int gapEnd) {
			TileSizes = new List<Rectangle>();
			TileSizes.Add(new Rectangle(0, 0, 16, 52));
			TileSizes.Add(new Rectangle(16, 0, 8, 52));
			TileSizes.Add(new Rectangle(24, 0, 8, 52));
			TileSizes.Add(new Rectangle(32, 0, 8, 52));
			TileSizes.Add(new Rectangle(40, 0, 8, 52));
			TileSizes.Add(new Rectangle(48, 0, 8, 52));
			TileSizes.Add(new Rectangle(56, 0, 8, 52));
			TileSizes.Add(new Rectangle(64, 0, 8, 52));
			TileSizes.Add(new Rectangle(72, 0, 8, 52));
			TileSizes.Add(new Rectangle(80, 0, 16, 52));
			TileSizes.Add(new Rectangle(96, 0, 8, 52));
			Width = width;
			GapStart = gapStart;
			GapEnd = gapEnd;
			Depth = -60;
		}
		public static Bridge FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);
			List<MapElement> nodes = node.SelectChildren("node");
			Bridge entity = new Bridge(width, nodes[0].AttrInt("x"), nodes[1].AttrInt("x"));
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("scenery/bridge");
			if (img == null) { return; }

			Util.Random.Seed = 1;
			Vector2 position = Position;
			int num = 0;
			while (position.X < Position.X + Width) {
				Rectangle rectangle;
				if (num >= 2 && num <= 7) {
					rectangle = TileSizes[2 + Util.Random.Next(6)];
				} else {
					rectangle = TileSizes[num];
				}
				if (position.X < GapStart || position.X >= GapEnd) {
					if (rectangle.Width == 16) {
						int height = 24;
						int i = 0;
						while (i < rectangle.Height) {
							using (Bitmap sub = Util.GetSubImage(img, new Sprite(rectangle.X, i, rectangle.Width, height, 0, 0, rectangle.Width, height))) {
								map.DrawImage(sub, position.X, position.Y + i - 8);
							}
							i += height;
							height = 12;
						}
					} else {
						using (Bitmap sub = Util.GetSubImage(img, new Sprite(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 0, 0, rectangle.Width, rectangle.Height))) {
							map.DrawImage(sub, position.X, position.Y - 8);
						}
					}
				}
				position.X += rectangle.Width;
				num = (num + 1) % TileSizes.Count;
			}

			img.Dispose();
		}
	}
}