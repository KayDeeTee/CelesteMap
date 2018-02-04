using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
namespace CelesteMap.Utility {
	public static class Util {
		[DllImport("Kernel32.dll")]
		private extern static void CopyMemory(IntPtr dest, IntPtr src, uint length);
		public static Rand Random = new Rand();
		public static Bitmap GetSubImage(Bitmap parent, Sprite bounds) {
			BitmapData parentData = parent.LockBits(bounds.Bounds, ImageLockMode.ReadOnly, parent.PixelFormat);
			try {
				Bitmap subImage = new Bitmap(bounds.Offset.Width, bounds.Offset.Height, parent.PixelFormat);
				BitmapData subData = subImage.LockBits(new Rectangle(bounds.Offset.X, bounds.Offset.Y, bounds.Bounds.Width, bounds.Bounds.Height), ImageLockMode.WriteOnly, parent.PixelFormat);
				try {
					int copyAmount = bounds.Bounds.Width * 4;
					for (int i = bounds.Bounds.Height - 1; i >= 0; i--) {
						CopyMemory(subData.Scan0 + (subData.Stride * i), parentData.Scan0 + (parentData.Stride * i), (uint)copyAmount);
					}
				} finally {
					subImage.UnlockBits(subData);
				}
				return subImage;
			} finally {
				parent.UnlockBits(parentData);
			}
		}
		public static void CopyTo(Bitmap dest, Bitmap src, Point pos) {
			if (src != null) {
				using (Graphics g = Graphics.FromImage(dest)) {
					g.DrawImage(src, pos);
				}
			}
		}
		public static XmlDocument LoadContentXML(string filename) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			return xmlDocument;
		}
		public static string ReadResource(string path) {
			Assembly current = Assembly.GetExecutingAssembly();
			using (Stream file = current.GetManifestResourceStream(current.GetName().Name + "." + path)) {
				using (StreamReader sr = new StreamReader(file)) {
					return sr.ReadToEnd();
				}
			}
		}
		public static Stream ReadResourceStream(string path) {
			Assembly current = Assembly.GetExecutingAssembly();
			return current.GetManifestResourceStream(current.GetName().Name + "." + path);
		}
		public static byte[] ReadResourceData(string path) {
			Assembly current = Assembly.GetExecutingAssembly();
			using (MemoryStream mem = new MemoryStream()) {
				using (Stream file = current.GetManifestResourceStream(current.GetName().Name + "." + path)) {
					file.CopyTo(mem);
				}
				return mem.ToArray();
			}
		}
		public static float ClampedMap(float val, float min, float max, float newMin = 0f, float newMax = 1f) {
			return Clamp((val - min) / (max - min), 0f, 1f) * (newMax - newMin) + newMin;
		}
		public static char AttrChar(this XmlElement xml, string attributeName) {
			return Convert.ToChar(xml.Attributes[attributeName]?.InnerText);
		}
		public static bool HasAttr(this XmlElement xml, string attributeName) {
			return xml.Attributes[attributeName] != null;
		}
		public static string Attr(this XmlElement xml, string attributeName, string defaultValue = "") {
			string attr = xml.Attributes[attributeName]?.InnerText;
			if (string.IsNullOrEmpty(attr)) {
				return defaultValue;
			}
			return attr;
		}
		public static string Attr(this XmlElement xml, string attributeName) {
			return xml.Attributes[attributeName]?.InnerText;
		}
		public static char AttrChar(this XmlNode xml, string attributeName) {
			return Convert.ToChar(xml.Attributes[attributeName]?.InnerText);
		}
		public static char AttrChar(this XmlNode xml, string attributeName,char defaultValue = '\0') {
			string attr = xml.Attributes[attributeName]?.InnerText;
			if (string.IsNullOrEmpty(attr)) {
				return defaultValue;
			}
			return Convert.ToChar(attr);
		}
		public static bool HasAttr(this XmlNode xml, string attributeName) {
			return xml.Attributes[attributeName] != null;
		}
		public static string Attr(this XmlNode xml, string attributeName, string defaultValue = "") {
			string attr = xml.Attributes[attributeName]?.InnerText;
			if (string.IsNullOrEmpty(attr)) {
				return defaultValue;
			}
			return attr;
		}
		public static float AttrFloat(this XmlNode xml, string attributeName, float defaultValue = 0f) {
			string attr = xml.Attributes[attributeName]?.InnerText;
			if (string.IsNullOrEmpty(attr)) {
				return defaultValue;
			}
			return float.Parse(attr, CultureInfo.InvariantCulture);
		}
		public static int AttrInt(this XmlNode xml, string attributeName, int defaultValue = 0) {
			string attr = xml.Attributes[attributeName]?.InnerText;
			if (string.IsNullOrEmpty(attr)) {
				return defaultValue;
			}
			return int.Parse(attr, CultureInfo.InvariantCulture);
		}
		public static bool AttrBool(this XmlNode xml, string attributeName, bool defaultValue = false) {
			string attr = xml.Attributes[attributeName]?.InnerText;
			if (string.IsNullOrEmpty(attr)) {
				return defaultValue;
			}
			return bool.Parse(attr);
		}
		public static string Attr(this XmlNode xml, string attributeName) {
			return xml.Attributes[attributeName]?.InnerText;
		}
		public static float Lerp(float value1, float value2, float amount) {
			return value1 + (value2 - value1) * amount;
		}
		public static int Clamp(int value, int min, int max) {
			return Math.Min(Math.Max(value, min), max);
		}
		public static float Clamp(float value, float min, float max) {
			return Math.Min(Math.Max(value, min), max);
		}
		public static T Choose<T>(this Rand random, T a, T b) {
			return GiveMe<T>(random.Next(2), a, b);
		}
		public static T Choose<T>(this Rand random, T a, T b, T c) {
			return GiveMe<T>(random.Next(3), a, b, c);
		}
		public static T Choose<T>(this Rand random, T a, T b, T c, T d) {
			return GiveMe<T>(random.Next(4), a, b, c, d);
		}
		public static T Choose<T>(this Rand random, T a, T b, T c, T d, T e) {
			return GiveMe<T>(random.Next(5), a, b, c, d, e);
		}
		public static T Choose<T>(this Rand random, T a, T b, T c, T d, T e, T f) {
			return GiveMe<T>(random.Next(6), a, b, c, d, e, f);
		}
		public static T Choose<T>(this Rand random, params T[] choices) {
			return choices[random.Next(choices.Length)];
		}
		public static T Choose<T>(this Rand random, List<T> choices) {
			return choices[random.Next(choices.Count)];
		}
		public static Color HexToColor(string hex) {
			if (hex.Length >= 6) {
				int r = HexToByte(hex[0]) * 16 + HexToByte(hex[1]);
				int g = HexToByte(hex[2]) * 16 + HexToByte(hex[3]);
				int b = HexToByte(hex[4]) * 16 + HexToByte(hex[5]);
				return Color.FromArgb(r, g, b);
			}
			return Color.White;
		}
		public static byte HexToByte(char c) {
			return (byte)"0123456789ABCDEF".IndexOf(char.ToUpper(c));
		}
		public static T GiveMe<T>(int index, T a, T b) {
			if (index == 0) {
				return a;
			}
			if (index != 1) {
				throw new Exception("Index was out of range!");
			}
			return b;
		}
		public static T GiveMe<T>(int index, T a, T b, T c) {
			switch (index) {
				case 0: return a;
				case 1: return b;
				case 2: return c;
				default: throw new Exception("Index was out of range!");
			}
		}
		public static T GiveMe<T>(int index, T a, T b, T c, T d) {
			switch (index) {
				case 0: return a;
				case 1: return b;
				case 2: return c;
				case 3: return d;
				default: throw new Exception("Index was out of range!");
			}
		}
		public static T GiveMe<T>(int index, T a, T b, T c, T d, T e) {
			switch (index) {
				case 0: return a;
				case 1: return b;
				case 2: return c;
				case 3: return d;
				case 4: return e;
				default: throw new Exception("Index was out of range!");
			}
		}
		public static T GiveMe<T>(int index, T a, T b, T c, T d, T e, T f) {
			switch (index) {
				case 0: return a;
				case 1: return b;
				case 2: return c;
				case 3: return d;
				case 4: return e;
				case 5: return f;
				default: throw new Exception("Index was out of range!");
			}
		}
	}
}