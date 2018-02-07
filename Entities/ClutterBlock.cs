using CelesteMap.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace CelesteMap.Entities {
	public class ClutterBlock : Entity {
		public enum BlockColor {
			Red,
			Green,
			Yellow
		}
		public BlockColor Color;
		public int Width, Height;
		public ClutterBlock(BlockColor color, int width, int height) {
			Color = color;
			Width = width;
			Height = height;
			Depth = 0;
		}
		public static ClutterBlock FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);

			ClutterBlock entity = new ClutterBlock(node.Name.Equals("redBlocks") ? BlockColor.Red : node.Name.Equals("greenBlocks") ? BlockColor.Green : BlockColor.Yellow, width, height);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			List<Bitmap> images = Gameplay.GetAllImages("objects/resortclutter/" + Color.ToString());

			int widthTiles = Width / 8;
			int heightTiles = Height / 8;
			int y = (int)Position.Y;
			for (int j = 0; j < heightTiles; j++) {
				int x = (int)Position.X;
				for (int i = 0; i < widthTiles; i++) {
					Bitmap img = Util.Random.Choose(images);
					map.DrawImage(img, x, y);
					x += 8;
				}
				y += 8;
			}

			for (int i = 0; i < images.Count; i++) {
				images[i].Dispose();
			}
		}
	}
}