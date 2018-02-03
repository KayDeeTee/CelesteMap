using System;
using System.Drawing;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class LightBeam : Entity {
		public int Width, Height;
		public float Rotation;
		public LightBeam(int width, int height, float rotation) {
			Width = width;
			Height = height;
			Rotation = rotation;
			Depth = -9998;
		}
		public static LightBeam FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);
			float rotation = node.AttrFloat("rotation", 0f);

			LightBeam entity = new LightBeam(width, height, rotation);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("util/lightbeam");
			if (img == null) { return; }

			float rotation = Rotation + 180;
			Vector2 move = new Vector2(-Height / 2, 0);
			if (rotation == 90) {
				img.RotateFlip(RotateFlipType.Rotate90FlipNone);
				move = new Vector2(0, -Height / 2);
			} else if (rotation == 180) {
				img.RotateFlip(RotateFlipType.Rotate180FlipNone);
				move = new Vector2(Height / 2, 0);
			} else if (rotation == 270) {
				img.RotateFlip(RotateFlipType.Rotate270FlipNone);
				move = new Vector2(0, Height / 2);
			}

			move += Position;
			map.DrawImage(img, move.X, move.Y, 1f / img.Width * Height, Width);
			map.DrawImage(img, Position.X - Width / 2, Position.Y - Height / 2, img.Width * Height, Width);
		}
	}
}