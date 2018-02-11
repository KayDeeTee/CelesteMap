using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class MovingPlatform : Entity {
		public int Width;
		public Vector2 Target;
		public MovingPlatform(int width, Vector2 target) {
			Width = width;
			Target = target;
			Depth = 9001;
		}
		public static MovingPlatform FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);
			MapElement child = node.SelectFirst("node");
			Vector2 target = new Vector2(child.AttrInt("x"), child.AttrInt("y"));

			MovingPlatform entity = new MovingPlatform(width, target);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/woodPlatform/default");
			if (img == null) { return; }

			int widthTiles = Width / 8;
			int x = (int)Position.X;
			int y = (int)Position.Y;

			Vector2 pos = new Vector2(Position.X + Width / 2, Position.Y + img.Height / 2);
			Vector2 tar = new Vector2(Target.X + Width / 2, Target.Y + img.Height / 2);
			Vector2 vector = (tar - pos).Normalize();
			Vector2 value = new Vector2(-vector.Y, vector.X);

			using (Pen pen = new Pen(Util.HexToColor("2a1923"))) {
				Vector2 temp1 = pos - vector - value;
				Vector2 temp2 = tar + vector - value;
				map.DrawLine(pen, temp1.X, temp1.Y, temp2.X, temp2.Y);
				temp1 = pos - vector;
				temp2 = tar + vector;
				map.DrawLine(pen, temp1.X, temp1.Y, temp2.X, temp2.Y);
				temp1 = pos - vector + value;
				temp2 = tar + vector + value;
				map.DrawLine(pen, temp1.X, temp1.Y, temp2.X, temp2.Y);
			}
			using (Pen pen = new Pen(Util.HexToColor("160b12"))) {
				map.DrawLine(pen, pos.X, pos.Y, tar.X, tar.Y);
			}

			for (int i = 0; i < widthTiles; i++) {
				int xOffset;
				if (i == 0) {
					xOffset = 0;
				} else if (i == widthTiles - 1) {
					xOffset = 3;
				} else if (i == widthTiles / 2) {
					xOffset = 2;
				} else {
					xOffset = 1;
				}

				using (Bitmap sub = Util.GetSubImage(img, new Sprite(xOffset * 8, 0, 8, 8, 0, 0, 8, 8))) {
					map.DrawImage(sub, x, y);
				}
				x += 8;
			}

			img.Dispose();
		}
	}
}