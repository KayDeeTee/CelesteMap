using CelesteMap.Utility;
using System;
using System.Drawing;
using System.Drawing.Imaging;
namespace CelesteMap.Entities {
	public class RotateSpinner : Entity {
		public enum Type {
			Crystal,
			Dust
		}
		public Type SpinnerType;
		public RotateSpinner(Type type) {
			SpinnerType = type;
			Depth = 0;
		}
		public static RotateSpinner FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			string levelName = node.SelectParent("level").Attr("Name");
			string map = node.SelectParent("map").Attr("_package");
			bool isDust = map.IndexOf("-CelestialResort", StringComparison.OrdinalIgnoreCase) > 0;
			isDust |= map.IndexOf("-Summit", StringComparison.OrdinalIgnoreCase) > 0 && levelName.StartsWith("d-", StringComparison.OrdinalIgnoreCase);

			RotateSpinner entity = new RotateSpinner(isDust ? Type.Dust : Type.Crystal);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap center = Gameplay.GetRandomImage(SpinnerType == Type.Crystal ? "danger/crystal/fg_purple" : "danger/dustcreature/center");
			if (center == null) { return; }

			if (SpinnerType == Type.Dust) {
				Color color = Color.Red;
				ColorMatrix matrix = new ColorMatrix();
				matrix.Matrix40 = color.R / 255f;
				matrix.Matrix41 = color.G / 255f;
				matrix.Matrix42 = color.B / 255f;
				ImageAttributes attributes = new ImageAttributes();
				attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				map.DrawImage(center, new Rectangle((int)Position.X - center.Width / 2 + 1, (int)Position.Y - center.Height / 2, center.Width, center.Height), 0, 0, center.Width, center.Height, GraphicsUnit.Pixel, attributes);
				map.DrawImage(center, new Rectangle((int)Position.X - center.Width / 2 - 1, (int)Position.Y - center.Height / 2, center.Width, center.Height), 0, 0, center.Width, center.Height, GraphicsUnit.Pixel, attributes);
				map.DrawImage(center, new Rectangle((int)Position.X - center.Width / 2, (int)Position.Y - center.Height / 2 + 1, center.Width, center.Height), 0, 0, center.Width, center.Height, GraphicsUnit.Pixel, attributes);
				map.DrawImage(center, new Rectangle((int)Position.X - center.Width / 2, (int)Position.Y - center.Height / 2 - 1, center.Width, center.Height), 0, 0, center.Width, center.Height, GraphicsUnit.Pixel, attributes);
			}
			map.DrawImage(center, Position.X - center.Width / 2, Position.Y - center.Height / 2);

			center.Dispose();
		}
	}
}