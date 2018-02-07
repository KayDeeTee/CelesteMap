using CelesteMap.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace CelesteMap.Entities {
	public class TriggerSpikes : Entity {
		public Direction SpikeDirection;
		public int Size;
		public enum Direction {
			Up,
			Down,
			Left,
			Right
		}
		public TriggerSpikes(Direction direction, int size) {
			SpikeDirection = direction;
			Size = size;
			Depth = -1;
		}
		public static TriggerSpikes FromElement(MapElement node) {
			string direction = node.Name.Substring(13);
			if (!Enum.TryParse<Direction>(direction, true, out Direction spikeDir)) { return null; }

			int size = spikeDir == Direction.Up || spikeDir == Direction.Down ? node.AttrInt("width", 0) : node.AttrInt("height", 0);
			if (size > 0) {
				TriggerSpikes entity = new TriggerSpikes(spikeDir, size);
				int x = node.AttrInt("x", 0);
				int y = node.AttrInt("y", 0);
				entity.Position = new Vector2(x, y);
				entity.ID = node.AttrInt("id", 0);
				return entity;
			}
			return null;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			List<Bitmap> images = Gameplay.GetAllImages(SpikeDirection == Direction.Down || SpikeDirection == Direction.Up ? "danger/triggertentacle/wiggle_v" : "danger/triggertentacle/wiggle_h");

			int width = images[0].Width;
			int height = images[0].Height;
			int x = (int)Position.X - (SpikeDirection == Direction.Left ? width : 0);
			int y = (int)Position.Y - (SpikeDirection == Direction.Up ? height : 0);
			int count = Size / (SpikeDirection == Direction.Up || SpikeDirection == Direction.Down ? width : height);
			Color[] colors = { Util.HexToColor("f25a10"), Util.HexToColor("ff0000"), Util.HexToColor("f21067") };

			for (int i = 0; i < count; i++) {
				Bitmap img = Util.Random.Choose<Bitmap>(images);
				if (SpikeDirection == Direction.Up) {
					img.RotateFlip(RotateFlipType.RotateNoneFlipY);
				} else if (SpikeDirection == Direction.Left) {
					img.RotateFlip(RotateFlipType.RotateNoneFlipX);
				}
				
				Color color = colors[i % 3];
				ColorMatrix matrix = new ColorMatrix();
				matrix.Matrix40 = color.R / 255f - 1f;
				matrix.Matrix41 = color.G / 255f - 1f;
				matrix.Matrix42 = color.B / 255f - 1f;
				ImageAttributes attributes = new ImageAttributes();
				attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				map.DrawImage(img, new Rectangle(x, y, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);

				switch (SpikeDirection) {
					case Direction.Up:
					case Direction.Down: x += width; break;
					case Direction.Left:
					case Direction.Right: y += height; break;
				}
			}

			for (int i = 0; i < images.Count; i++) {
				images[i].Dispose();
			}
		}
	}
}