using CelesteMap.Utility;
using System.Drawing;
using System.Xml;
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
			img.Save("light.png");
			if (img == null) { return; }

			float rotation = Rotation + 90;
			if (rotation == 90) {
				img.RotateFlip(RotateFlipType.Rotate90FlipNone);
			} else if (rotation == 180) {
				img.RotateFlip(RotateFlipType.Rotate180FlipNone);
			} else if (rotation == 270) {
				img.RotateFlip(RotateFlipType.Rotate270FlipNone);
			}

			map.DrawImage(img, Position.X - Width / 2, Position.Y, Width, Height);
			img.Dispose();
		}
	}
}