using System;
using System.Drawing;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class Spikes : Entity {
		public Direction SpikeDirection;
		public string SpikeType;
		public int Size;
		public enum Direction {
			Up,
			Down,
			Left,
			Right
		}
		public Spikes(Direction direction, string spikeType, int size) {
			SpikeDirection = direction;
			SpikeType = spikeType;
			Size = size;
			Depth = -1;
		}
		public static Spikes FromElement(XmlNode node) {
			string direction = node.Name.Substring(6);
			if (!Enum.TryParse<Direction>(direction, true, out Direction spikeDir)) { return null; }

			string type = node.Attr("type", "default");
			int size = spikeDir == Direction.Up || spikeDir == Direction.Down ? node.AttrInt("width", 0) : node.AttrInt("height", 0);
			if (size > 0) {
				Spikes entity = new Spikes(spikeDir, type, size);
				int x = node.AttrInt("x", 0);
				int y = node.AttrInt("y", 0);
				entity.Position = new Vector2(x, y);
				entity.ID = node.AttrInt("id", 0);
				return entity;
			}
			return null;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetRandomImage($"danger/spikes/{SpikeType}_{SpikeDirection.ToString()}");
			if (img == null) { return; }

			int width = img.Width;
			int height = img.Height;
			int x = (int)Position.X - (SpikeDirection == Direction.Left ? width : 0);
			int y = (int)Position.Y - (SpikeDirection == Direction.Up ? height : 0);

			int count = Size / (SpikeDirection == Direction.Up || SpikeDirection == Direction.Down ? width : height);
			for (int i = 0; i < count; i++) {
				map.DrawImage(img, x, y);
				switch (SpikeDirection) {
					case Direction.Up:
					case Direction.Down: x += width; break;
					case Direction.Left:
					case Direction.Right: y += height; break;
				}
			}
			img.Dispose();
		}
	}
}