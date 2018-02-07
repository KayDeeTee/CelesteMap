using CelesteMap.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
namespace CelesteMap.Backdrops {
	public abstract class Backdrop {
		public const float DeltaTime = 0.0166666f;
		public bool UseSpritebatch = true;
		public string Name;
		public Vector2 Position;
		public Vector2 Scroll = Vector2.One;
		public Vector2 Speed;
		public Color Color = Color.White;
		public bool LoopX = true;
		public bool LoopY = true;
		public bool FlipX;
		public bool FlipY;
		public Fader FadeX;
		public Fader FadeY;
		public HashSet<string> ExcludeFrom;
		public HashSet<string> OnlyIn;
		public string OnlyIfFlag;
		public string OnlyIfNotFlag;
		public string AlsoIfFlag;
		public bool? Dreaming;
		public bool Visible;
		public bool InstantIn = true;
		public bool InstantOut;

		public Backdrop() {
			Visible = true;
		}
		public override string ToString() {
			return string.IsNullOrEmpty(Name) ? GetType().Name : Name;
		}
		public bool IsVisible(string levelName) {
			return string.IsNullOrEmpty(OnlyIfNotFlag) && (!string.IsNullOrEmpty(AlsoIfFlag) || (Dreaming == null && string.IsNullOrEmpty(OnlyIfFlag) && (ExcludeFrom == null || !ExcludeFrom.Contains(levelName)) && (OnlyIn == null || OnlyIn.Contains(levelName))));
		}
		public virtual void Render(Rectangle bounds, Graphics map) {
		}

		public static List<Backdrop> CreateBackdrops(MapElement level) {
			List<Backdrop> list = new List<Backdrop>();
			if (level != null) {
				Backdrop bd = null;
				for (int i = 0; i < level.Children.Count; i++) {
					MapElement element = level.Children[i];

					if (element.Name.Equals("apply", StringComparison.OrdinalIgnoreCase)) {
						for (int j = 0; j < element.Children.Count; j++) {
							MapElement child = element.Children[j];

							bd = ParseBackdrop(child, element);
							if (bd != null) {
								list.Add(bd);
							}
						}
					}
					bd = ParseBackdrop(element, null);
					if (bd != null) {
						list.Add(bd);
					}
				}
			}
			return list;
		}

		private static Backdrop ParseBackdrop(MapElement child, MapElement above) {
			Backdrop backdrop;
			if (child.Name.Equals("parallax", StringComparison.OrdinalIgnoreCase)) {
				string id = child.Attr("texture", "");
				string a = child.Attr("atlas", "game");
				Bitmap texture = null;
				if (a == "game") {
					texture = Gameplay.GetImage(id);
				}
				Parallax parallax = new Parallax(texture);
				backdrop = parallax;
				backdrop.Name = id;
				string text = "";
				if (child.HasAttr("blendmode")) {
					text = child.Attr("blendmode", "alphablend").ToLower();
				} else if (above != null && above.HasAttr("blendmode")) {
					text = above.Attr("blendmode", "alphablend").ToLower();
				}
				if (text.Equals("additive")) {
					parallax.BlendState = BlendState.Additive;
				}
			} else if (child.Name.Equals("snowfg", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new Snow(true);
			} else if (child.Name.Equals("snowbg", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new Snow(false);
			} else if (child.Name.Equals("windsnow", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new WindSnowFG();
			} else if (child.Name.Equals("dreamstars", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new DreamStars();
			} else if (child.Name.Equals("stars", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new StarsBG();
			} else if (child.Name.Equals("mirrorfg", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new MirrorFG();
			} else if (child.Name.Equals("reflectionfg", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new ReflectionFG();
			} else if (child.Name.Equals("godrays", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new Godrays();
			} else if (child.Name.Equals("tentacles", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new Tentacles((Tentacles.Side)Enum.Parse(typeof(Tentacles.Side), child.Attr("side", "Right")), Util.HexToColor(child.Attr("color", "")), child.AttrFloat("offset", 0f));
			} else if (child.Name.Equals("northernlights", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new NorthernLights();
			} else if (child.Name.Equals("bossStarField", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new FinalBossStarfield();
			} else if (child.Name.Equals("petals", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new Petals();
			} else if (child.Name.Equals("heatwave", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new HeatWave();
			} else if (child.Name.Equals("corestarsfg", StringComparison.OrdinalIgnoreCase)) {
				backdrop = new CoreStarsFG();
			} else {
				return null;
			}
			if (child.HasAttr("x")) {
				backdrop.Position.X = child.AttrFloat("x", 0f);
			} else if (above != null && above.HasAttr("x")) {
				backdrop.Position.X = above.AttrFloat("x", 0f);
			}
			if (child.HasAttr("y")) {
				backdrop.Position.Y = child.AttrFloat("y", 0f);
			} else if (above != null && above.HasAttr("y")) {
				backdrop.Position.Y = above.AttrFloat("y", 0f);
			}
			if (child.HasAttr("scrollx")) {
				backdrop.Scroll.X = child.AttrFloat("scrollx", 0f);
			} else if (above != null && above.HasAttr("scrollx")) {
				backdrop.Scroll.X = above.AttrFloat("scrollx", 0f);
			}
			if (child.HasAttr("scrolly")) {
				backdrop.Scroll.Y = child.AttrFloat("scrolly", 0f);
			} else if (above != null && above.HasAttr("scrolly")) {
				backdrop.Scroll.Y = above.AttrFloat("scrolly", 0f);
			}
			if (child.HasAttr("speedx")) {
				backdrop.Speed.X = child.AttrFloat("speedx", 0f);
			} else if (above != null && above.HasAttr("speedx")) {
				backdrop.Speed.X = above.AttrFloat("speedx", 0f);
			}
			if (child.HasAttr("speedy")) {
				backdrop.Speed.Y = child.AttrFloat("speedy", 0f);
			} else if (above != null && above.HasAttr("speedy")) {
				backdrop.Speed.Y = above.AttrFloat("speedy", 0f);
			}
			backdrop.Color = Color.White;
			if (child.HasAttr("color")) {
				backdrop.Color = Util.HexToColor(child.Attr("color", ""));
			} else if (above != null && above.HasAttr("color")) {
				backdrop.Color = Util.HexToColor(above.Attr("color", ""));
			}
			if (child.HasAttr("alpha")) {
				backdrop.Color = Color.FromArgb((int)(child.AttrFloat("alpha", 0f) * 255), backdrop.Color);
			} else if (above != null && above.HasAttr("alpha")) {
				backdrop.Color = Color.FromArgb((int)(above.AttrFloat("alpha", 0f) * 255), backdrop.Color);
			}
			if (child.HasAttr("flipx")) {
				backdrop.FlipX = child.AttrBool("flipx", false);
			} else if (above != null && above.HasAttr("flipx")) {
				backdrop.FlipX = above.AttrBool("flipx", false);
			}
			if (child.HasAttr("flipy")) {
				backdrop.FlipY = child.AttrBool("flipy", false);
			} else if (above != null && above.HasAttr("flipy")) {
				backdrop.FlipY = above.AttrBool("flipy", false);
			}
			if (child.HasAttr("loopx")) {
				backdrop.LoopX = child.AttrBool("loopx", false);
			} else if (above != null && above.HasAttr("loopx")) {
				backdrop.LoopX = above.AttrBool("loopx", false);
			}
			if (child.HasAttr("loopy")) {
				backdrop.LoopY = child.AttrBool("loopy", false);
			} else if (above != null && above.HasAttr("loopy")) {
				backdrop.LoopY = above.AttrBool("loopy", false);
			}
			string text2 = null;
			if (child.HasAttr("exclude")) {
				text2 = child.Attr("exclude", "");
			} else if (above != null && above.HasAttr("exclude")) {
				text2 = above.Attr("exclude", "");
			}
			if (text2 != null) {
				backdrop.ExcludeFrom = ParseLevelsList(text2);
			}
			string text3 = null;
			if (child.HasAttr("only")) {
				text3 = child.Attr("only", "");
			} else if (above != null && above.HasAttr("only")) {
				text3 = above.Attr("only", "");
			}
			if (text3 != null) {
				backdrop.OnlyIn = ParseLevelsList(text3);
			}
			string text4 = null;
			if (child.HasAttr("flag")) {
				text4 = child.Attr("flag", "");
			} else if (above != null && above.HasAttr("flag")) {
				text4 = above.Attr("flag", "");
			}
			if (text4 != null) {
				backdrop.OnlyIfFlag = text4;
			}
			string text5 = null;
			if (child.HasAttr("notflag")) {
				text5 = child.Attr("notflag", "");
			} else if (above != null && above.HasAttr("notflag")) {
				text5 = above.Attr("notflag", "");
			}
			if (text5 != null) {
				backdrop.OnlyIfNotFlag = text5;
			}
			string text6 = null;
			if (child.HasAttr("always")) {
				text6 = child.Attr("always", "");
			} else if (above != null && above.HasAttr("always")) {
				text6 = above.Attr("always", "");
			}
			if (text6 != null) {
				backdrop.AlsoIfFlag = text6;
			}
			bool? dreaming = null;
			if (child.HasAttr("dreaming")) {
				dreaming = new bool?(child.AttrBool("dreaming", false));
			} else if (above != null && above.HasAttr("dreaming")) {
				dreaming = new bool?(above.AttrBool("dreaming", false));
			}
			if (dreaming != null) {
				backdrop.Dreaming = dreaming;
			}
			if (child.HasAttr("instantIn")) {
				backdrop.InstantIn = child.AttrBool("instantIn", false);
			} else if (above != null && above.HasAttr("instantIn")) {
				backdrop.InstantIn = above.AttrBool("instantIn", false);
			}
			if (child.HasAttr("instantOut")) {
				backdrop.InstantOut = child.AttrBool("instantOut", false);
			} else if (above != null && above.HasAttr("instantOut")) {
				backdrop.InstantOut = above.AttrBool("instantOut", false);
			}
			string text7 = null;
			if (child.HasAttr("fadex")) {
				text7 = child.Attr("fadex", "");
			} else if (above != null && above.HasAttr("fadex")) {
				text7 = above.Attr("fadex", "");
			}
			if (text7 != null) {
				backdrop.FadeX = new Fader();
				string[] array = text7.Split(':');
				for (int i = 0; i < array.Length; i++) {
					string[] array2 = array[i].Split(',');
					if (array2.Length == 2) {
						string[] array3 = array2[0].Split('-');
						string[] array4 = array2[1].Split('-');
						float fadeFrom = float.Parse(array4[0], CultureInfo.InvariantCulture);
						float fadeTo = float.Parse(array4[1], CultureInfo.InvariantCulture);
						int num = 1;
						int num2 = 1;
						if (array3[0][0] == 'n') {
							num = -1;
							array3[0] = array3[0].Substring(1);
						}
						if (array3[1][0] == 'n') {
							num2 = -1;
							array3[1] = array3[1].Substring(1);
						}
						backdrop.FadeX.Add((float)(num * int.Parse(array3[0])), (float)(num2 * int.Parse(array3[1])), fadeFrom, fadeTo);
					}
				}
			}
			string text8 = null;
			if (child.HasAttr("fadey")) {
				text8 = child.Attr("fadey", "");
			} else if (above != null && above.HasAttr("fadey")) {
				text8 = above.Attr("fadey", "");
			}
			if (text8 != null) {
				backdrop.FadeY = new Fader();
				string[] array = text8.Split(':');
				for (int i = 0; i < array.Length; i++) {
					string[] array5 = array[i].Split(',');
					if (array5.Length == 2) {
						string[] array6 = array5[0].Split('-');
						string[] array7 = array5[1].Split('-');
						float fadeFrom2 = float.Parse(array7[0], CultureInfo.InvariantCulture);
						float fadeTo2 = float.Parse(array7[1], CultureInfo.InvariantCulture);
						int num3 = 1;
						int num4 = 1;
						if (array6[0][0] == 'n') {
							num3 = -1;
							array6[0] = array6[0].Substring(1);
						}
						if (array6[1][0] == 'n') {
							num4 = -1;
							array6[1] = array6[1].Substring(1);
						}
						backdrop.FadeY.Add((float)(num3 * int.Parse(array6[0])), (float)(num4 * int.Parse(array6[1])), fadeFrom2, fadeTo2);
					}
				}
			}
			return backdrop;
		}
		private static HashSet<string> ParseLevelsList(string list) {
			HashSet<string> hashSet = new HashSet<string>();
			string[] array = list.Split(',');
			int i = 0;
			while (i < array.Length) {
				string text = array[i];
				if (text.IndexOf('*') >= 0) {
					string pattern = Regex.Escape(text).Replace("\\*", ".*") + "$";

					//if (Regex.IsMatch(levelData.Name, pattern)) {
					//	hashSet.Add(levelData.Name);
					//}
				}
				i++;

				hashSet.Add(text);
			}
			return hashSet;
		}
	}

	public class Fader {
		private List<Fader.Segment> Segments = new List<Fader.Segment>();

		public Fader Add(float posFrom, float posTo, float fadeFrom, float fadeTo) {
			Segments.Add(new Fader.Segment {
				PositionFrom = posFrom,
				PositionTo = posTo,
				From = fadeFrom,
				To = fadeTo
			});
			return this;
		}
		public float Value(float position) {
			float num = 1f;
			foreach (Fader.Segment segment in Segments) {
				num *= Util.ClampedMap(position, segment.PositionFrom, segment.PositionTo, segment.From, segment.To);
			}
			return num;
		}

		private struct Segment {
			public float PositionFrom;
			public float PositionTo;
			public float From;
			public float To;
		}
	}
}