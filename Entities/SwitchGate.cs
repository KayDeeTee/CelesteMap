using CelesteMap.Utility;
using System.Drawing;
using System.Drawing.Imaging;
namespace CelesteMap.Entities {
	public class SwitchGate : Entity {
		public int Width, Height;
		public string Texture;
		public SwitchGate(int width, int height, string texture) {
			Width = width;
			Height = height;
			Texture = texture;
			Depth = 0;
		}
		public static SwitchGate FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			string texture = node.Attr("sprite", "block");
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);

			SwitchGate entity = new SwitchGate(width, height, texture);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("objects/switchgate/" + Texture);
			Bitmap icon = Gameplay.GetImage("objects/switchgate/icon00");
			if (img == null) { return; }

			using (SolidBrush brush = new SolidBrush(Color.Black)) {
				map.FillRectangle(brush, Position.X, Position.Y, Width, Height);
			}

			int edgeColumn = 0;
			while (edgeColumn < Width / 8f) {
				int edgeRow = 0;
				while (edgeRow < Height / 8f) {
					int edgeTileX = (edgeColumn == 0) ? 0 : ((edgeColumn == Width / 8f - 1f) ? 2 : 1);
					int edgeTileY = (edgeRow == 0) ? 0 : ((edgeRow == Height / 8f - 1f) ? 2 : 1);
					if (edgeTileX != 1 || edgeTileY != 1) {
						using (Bitmap sub = Util.GetSubImage(img, new Sprite(edgeTileX * 8, edgeTileY * 8, 8, 8, 0, 0, 8, 8))) {
							map.DrawImage(sub, Position.X + edgeColumn * 8, Position.Y + edgeRow * 8);
						}
					}
					edgeRow++;
				}
				edgeColumn++;
			}

			Color color = Util.HexToColor("5fcde4");
			ColorMatrix matrix = new ColorMatrix();
			matrix.Matrix40 = color.R / 255f - 1f;
			matrix.Matrix41 = color.G / 255f - 1f;
			matrix.Matrix42 = color.B / 255f - 1f;
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

			map.DrawImage(icon, new Rectangle((int)Position.X + Width / 2 - icon.Width / 2, (int)Position.Y + Height / 2 - icon.Height / 2, icon.Width, icon.Height), 0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel, attributes);

			img.Dispose();
			icon.Dispose();
		}
	}
}