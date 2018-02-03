using System;
using System.Drawing;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class JumpThru : Entity {
		public static string DefaultTexture = "wood";
		public string Texture;
		public int Width;
		public JumpThru(string texture, int width) {
			if (texture.Equals("default", StringComparison.OrdinalIgnoreCase)) {
				texture = DefaultTexture;
			}
			Texture = texture;
			Width = width;
			Depth = -60;
		}
		public static JumpThru FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);

			JumpThru entity = new JumpThru(node.Attr("texture", "default"), width);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/jumpthru/" + Texture);
			if (img == null) { return; }

			int xTile = (int)Position.X / 8;
			int yTile = (int)Position.Y / 8;
			int widthTiles = Width / 8;
			int imgTiles = img.Width / 8;
			int x = (int)Position.X;
			int y = (int)Position.Y;
			for (int i = 0; i < widthTiles; i++) {
				int xOffset;
				int yOffset;
				if (i == 0) {
					xOffset = 0;
					yOffset = (solids[xTile - 1, yTile] != '0' ? 0 : 1);
				} else if (i == widthTiles - 1) {
					xOffset = imgTiles - 1;
					yOffset = (solids[xTile + 1 + i, yTile] != '0' ? 0 : 1);
				} else {
					xOffset = 1 + Util.Random.Next(imgTiles - 2);
					yOffset = Util.Random.Choose(0, 1);
				}

				using (Bitmap sub = Util.GetSubImage(img, new Sprite(xOffset * 8, yOffset * 8, 8, 8, 0, 0, 8, 8))) {
					map.DrawImage(sub, x, y);
				}
				x += 8;
			}

			img.Dispose();
		}
	}
}