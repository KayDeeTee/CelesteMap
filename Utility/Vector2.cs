using System;
namespace CelesteMap.Utility {
	public struct Vector2 {
		public static Vector2 Zero = new Vector2(0, 0);
		public static Vector2 One = new Vector2(1, 1);
		public float X, Y;
		public Vector2(float x, float y) {
			X = x;
			Y = y;
		}
		public float Length() {
			return (float)Math.Sqrt(X * X + Y * Y);
		}
		public Vector2 Perpendicular() {
			return new Vector2(-Y, X);
		}
		public Vector2 Normalize() {
			float distance = (float)Math.Sqrt(X * X + Y * Y);
			if (Math.Abs(distance) < 0.001) {
				return new Vector2(1, 0);
			}
			return new Vector2(X / distance, Y / distance);
		}
		public static Vector2 operator +(Vector2 one, Vector2 two) {
			return new Vector2(one.X + two.X, one.Y + two.Y);
		}
		public static Vector2 operator +(Vector2 one, float two) {
			return new Vector2(one.X + two, one.Y + two);
		}
		public static Vector2 operator +(float one, Vector2 two) {
			return new Vector2(one + two.X, one + two.Y);
		}
		public static Vector2 operator -(Vector2 one, Vector2 two) {
			return new Vector2(one.X - two.X, one.Y - two.Y);
		}
		public static Vector2 operator -(Vector2 one, float two) {
			return new Vector2(one.X - two, one.Y - two);
		}
		public static Vector2 operator -(float one, Vector2 two) {
			return new Vector2(one - two.X, one - two.Y);
		}
		public static Vector2 operator -(Vector2 one) {
			return new Vector2(-one.X, -one.Y);
		}
		public static Vector2 operator *(Vector2 one, Vector2 two) {
			return new Vector2(one.X * two.X, one.Y * two.Y);
		}
		public static Vector2 operator *(Vector2 one, float two) {
			return new Vector2(one.X * two, one.Y * two);
		}
		public static Vector2 operator *(float one, Vector2 two) {
			return new Vector2(one * two.X, one * two.Y);
		}
		public static Vector2 operator /(Vector2 one, float two) {
			return new Vector2(one.X / two, one.Y / two);
		}
		public static Vector2 operator /(float one, Vector2 two) {
			return new Vector2(one / two.X, one / two.Y);
		}
		public static bool operator ==(Vector2 one, Vector2 two) {
			return one.X == two.X && one.Y == two.Y;
		}
		public static bool operator !=(Vector2 one, Vector2 two) {
			return one.X != two.X || one.Y != two.Y;
		}
		public override bool Equals(object obj) {
			return (obj is Vector2) && (Vector2)obj == this;
		}
		public override int GetHashCode() {
			return (int)X ^ (int)Y;
		}
		public override string ToString() {
			return $"[{X.ToString("0.00")},{Y.ToString("0.00")}]";
		}
	}
}