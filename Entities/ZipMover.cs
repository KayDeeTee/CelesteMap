using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public class ZipMover : Entity {
		private static readonly Color ropeColor = Util.HexToColor("663931");
		private static readonly Color ropeLightColor = Util.HexToColor("9b6157");
		public int Width, Height;
		public Vector2 Target;
		public ZipMover(int width, int height, int targetX, int targetY) {
			Width = width;
			Height = height;
			Depth = -9999;
			Target = new Vector2(targetX, targetY);
		}
		public static ZipMover FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);
			int height = node.AttrInt("height", 0);
			XmlNode target = node.SelectSingleNode(".//node");
			int targetX = target.AttrInt("x", 0);
			int targetY = target.AttrInt("y", 0);

			ZipMover entity = new ZipMover(width, height, targetX, targetY);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap block = Gameplay.GetImage("objects/zipmover/block");
			Bitmap light = Gameplay.GetImage("objects/zipmover/light00");
			List<Bitmap> cogs = Gameplay.GetAllImages("objects/zipmover/innercog");
			if (block == null) { return; }

			DrawCogs(map, new Vector2(Width / 2, Height / 2 + 1), Color.Black);
			DrawCogs(map, new Vector2(Width / 2, Height / 2), null);

			//Outline
			Vector2 position = Position;
			using (SolidBrush brush = new SolidBrush(Color.Black)) {
				map.FillRectangle(brush, Position.X - 1, Position.Y - 1, Width + 2, Height + 2);
			}

			int val = 1;
			float num2 = 0f;
			int count = cogs.Count;
			int yOffset = 4;
			while (yOffset <= Height - 4f) {
				int valTemp = val;
				int xOffset = 4;
				while (xOffset <= Width - 4f) {
					int index = (int)(mod((num2 + val * 3.14159274f * 4f) / 1.57079637f, 1f) * count);
					Bitmap currentCog = cogs[index];
					Rectangle cogBounds = new Rectangle(0, 0, currentCog.Width, currentCog.Height);
					Vector2 zero = Vector2.Zero;
					if (xOffset <= 4) {
						zero.X = 2f;
						cogBounds.X = 2;
						cogBounds.Width -= 2;
					} else if (xOffset >= Width - 4f) {
						zero.X = -2f;
						cogBounds.Width -= 2;
					}
					if (yOffset <= 4) {
						zero.Y = 2f;
						cogBounds.Y = 2;
						cogBounds.Height -= 2;
					} else if (yOffset >= Height - 4f) {
						zero.Y = -2f;
						cogBounds.Height -= 2;
					}

					using (Bitmap sub = Util.GetSubImage(currentCog, new Sprite(cogBounds.X, cogBounds.Y, cogBounds.Width, cogBounds.Height, 0, 0, cogBounds.Width, cogBounds.Height))) {
						if (val < 0) {
							//Darken
							ColorMatrix matrix = new ColorMatrix();
							matrix.Matrix00 = 0.5f;
							matrix.Matrix11 = 0.5f;
							matrix.Matrix22 = 0.5f;

							ImageAttributes attributes = new ImageAttributes();
							attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

							map.DrawImage(sub, new Rectangle((int)(Position.X + xOffset + zero.X - 5), (int)(Position.Y + yOffset + zero.Y - 5), sub.Width, sub.Height), 0, 0, sub.Width, sub.Height, GraphicsUnit.Pixel, attributes);
						} else {
							map.DrawImage(sub, Position.X + xOffset + zero.X - 5, Position.Y + yOffset + zero.Y - 5);
						}
					}
					val = -val;
					num2 += 1.04719758f;
					xOffset += 8;
				}
				if (valTemp == val) {
					val = -val;
				}
				yOffset += 8;
			}

			int edgeColumn = 0;
			while (edgeColumn < Width / 8f) {
				int edgeRow = 0;
				while (edgeRow < Height / 8f) {
					int edgeTileX = (edgeColumn == 0) ? 0 : ((edgeColumn == Width / 8f - 1f) ? 2 : 1);
					int edgeTileY = (edgeRow == 0) ? 0 : ((edgeRow == Height / 8f - 1f) ? 2 : 1);
					if (edgeTileX != 1 || edgeTileY != 1) {
						using (Bitmap sub = Util.GetSubImage(block, new Sprite(edgeTileX * 8, edgeTileY * 8, 8, 8, 0, 0, 8, 8))) {
							map.DrawImage(sub, Position.X + edgeColumn * 8, Position.Y + edgeRow * 8);
						}
					}
					edgeRow++;
				}
				edgeColumn++;
			}

			map.DrawImage(light, Position.X + Width / 2 - light.Width / 2, Position.Y);

			block.Dispose();
			light.Dispose();
			for (int i = 0; i < cogs.Count; i++) {
				cogs[i].Dispose();
			}
		}
		private void DrawCogs(Graphics g, Vector2 offset, Color? colorOverride = null) {
			Vector2 normal = (Position - Target).Normalize();
			Vector2 perp3 = normal.Perpendicular() * 4f;
			Vector2 perp4 = -normal.Perpendicular() * 3f;
			Vector2 perp5 = -normal.Perpendicular() * 4f;
			Vector2 drawFrom = Position + perp3 + offset;
			Vector2 drawTo = Target + perp3 + offset;
			using (Pen rope = new Pen(colorOverride != null ? colorOverride.Value : ropeColor)) {
				g.DrawLine(rope, (int)drawFrom.X, (int)drawFrom.Y, (int)drawTo.X, (int)drawTo.Y);
				drawFrom = Position + perp4 + offset;
				drawTo = Target + perp4 + offset;
				g.DrawLine(rope, (int)drawFrom.X, (int)drawFrom.Y, (int)drawTo.X, (int)drawTo.Y);
			}

			float length = (Target - Position).Length() - 3;
			using (Pen rope = new Pen(colorOverride != null ? colorOverride.Value : ropeLightColor)) {
				for (float num = 0; num < length; num += 4f) {
					Vector2 value3 = Position + perp3 + normal.Perpendicular() - normal * num;
					Vector2 value4 = Target + perp5 + normal * num;
					drawFrom = value3 + offset;
					drawTo = value3 + normal + offset;
					g.DrawLine(rope, (int)drawFrom.X, (int)drawFrom.Y, (int)drawTo.X, (int)drawTo.Y);
					drawFrom = value4 + offset;
					drawTo = value4 - normal + offset;
					g.DrawLine(rope, (int)drawFrom.X, (int)drawFrom.Y, (int)drawTo.X, (int)drawTo.Y);
				}
			}

			Bitmap cog = Gameplay.GetImage("objects/zipmover/cog");
			normal = new Vector2(cog.Width / 2, cog.Height / 2);
			drawFrom = Position + offset - normal;
			drawTo = Target + offset - normal;
			if (colorOverride != null) {
				ColorMatrix matrix = new ColorMatrix();
				matrix.Matrix00 = 0.5f;
				matrix.Matrix11 = 0.5f;
				matrix.Matrix22 = 0.5f;
				ImageAttributes attributes = new ImageAttributes();
				attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				g.DrawImage(cog, new Rectangle((int)drawFrom.X, (int)drawFrom.Y, cog.Width, cog.Height), 0, 0, cog.Width, cog.Height, GraphicsUnit.Pixel, attributes);
				g.DrawImage(cog, new Rectangle((int)drawTo.X, (int)drawTo.Y, cog.Width, cog.Height), 0, 0, cog.Width, cog.Height, GraphicsUnit.Pixel, attributes);
			} else {
				g.DrawImage(cog, (int)drawFrom.X, (int)drawFrom.Y);
				g.DrawImage(cog, (int)drawTo.X, (int)drawTo.Y);
			}
			cog.Dispose();
		}
		private float mod(float x, float m) {
			return (x % m + m) % m;
		}
	}
}