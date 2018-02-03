using System.Drawing;
using System.Xml;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public struct SimpleCurve {
		public Vector2 Begin, End, Control;

		public SimpleCurve(Vector2 begin, Vector2 end, Vector2 control) {
			Begin = begin;
			End = end;
			if (End.Y > Begin.Y) {
				Begin = end;
				End = begin;
			}
			Control = control;
		}

		public void DoubleControl() {
			Vector2 value = End - Begin;
			Vector2 value2 = Begin + value / 2f;
			Vector2 value3 = Control - value2;
			Control += value3;
		}
		public Vector2 GetPoint(float percent) {
			float num = 1f - percent;
			return num * num * Begin + 2f * num * percent * Control + percent * percent * End;
		}
		public float GetLengthParametric(int resolution) {
			Vector2 value = Begin;
			float num = 0f;
			for (int i = 1; i <= resolution; i++) {
				Vector2 point = GetPoint((float)i / (float)resolution);
				num += (point - value).Length();
				value = point;
			}
			return num;
		}
		public void Render(Vector2 offset, Color color, int resolution, Graphics g) {
			Vector2 start = offset + Begin;
			for (int i = 1; i <= resolution; i++) {
				Vector2 vector = offset + GetPoint((float)i / (float)resolution);
				using (Pen line = new Pen(color)) {
					g.DrawLine(line, start.X, start.Y, vector.X, vector.Y);
				}
				start = vector;
			}
		}
		public void Render(Vector2 offset, Color color, int resolution, float thickness, Graphics g) {
			Vector2 start = offset + Begin;
			for (int i = 1; i <= resolution; i++) {
				Vector2 vector = offset + GetPoint((float)i / (float)resolution);
				using (Pen line = new Pen(color, thickness)) {
					g.DrawLine(line, start.X, start.Y, vector.X, vector.Y);
				}
				start = vector;
			}
		}
		public void Render(Color color, int resolution, Graphics g) {
			Render(Vector2.Zero, color, resolution, g);
		}
		public void Render(Color color, int resolution, float thickness, Graphics g) {
			Render(Vector2.Zero, color, resolution, thickness, g);
		}
	}
	public class Wire : Entity {
		public Vector2 Target;
		private SimpleCurve curve;
		private Color color;
		public Wire(int x, int y, int targetX, int targetY, bool above) {
			color = Util.HexToColor("595866");
			Position = new Vector2(x, y);
			Target = new Vector2(targetX, targetY);
			curve = new SimpleCurve(Position, Target, Vector2.Zero);
			curve.Control = (curve.Begin + curve.End) / 2f + new Vector2(0f, 24f);
			Depth = (above ? -8500 : 2000);
		}
		public static Wire FromElement(XmlNode node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			bool above = node.AttrBool("above", false);
			XmlNode target = node.SelectSingleNode(".//node");
			int targetX = target.AttrInt("x", 0);
			int targetY = target.AttrInt("y", 0);

			Wire entity = new Wire(x, y, targetX, targetY, above);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			curve.Render(color, 16, map);
		}
	}
}