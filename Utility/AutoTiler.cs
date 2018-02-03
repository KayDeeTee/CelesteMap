using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Xml;
using CelesteMap.Backdrops;
namespace CelesteMap.Utility {
	public class AutoTiler {
		public List<Rectangle> LevelBounds = new List<Rectangle>();
		private Dictionary<char, TerrainType> lookup = new Dictionary<char, TerrainType>();
		private byte[] adjacent = new byte[9];

		public AutoTiler(string xmlDescription) {
			XmlDocument document = new XmlDocument();
			document.LoadXml(xmlDescription);
			Dictionary<char, XmlElement> dictionary = new Dictionary<char, XmlElement>();
			foreach (object obj in document.GetElementsByTagName("Tileset")) {
				XmlElement xmlElement = (XmlElement)obj;
				char c = xmlElement.AttrChar("id");
				Bitmap tileImages = Gameplay.GetImage("tilesets/" + xmlElement.Attr("path"));
				TileGrid tileset = new TileGrid(8, 8, tileImages.Width / 8, tileImages.Height / 8);
				tileset.Load(tileImages);
				TerrainType terrainType = new TerrainType(c);
				ReadInto(terrainType, tileset, xmlElement);
				if (xmlElement.HasAttr("copy")) {
					char key = xmlElement.AttrChar("copy");
					if (!dictionary.ContainsKey(key)) {
						throw new Exception("Copied tilesets must be defined before the tilesets that copy them!");
					}
					ReadInto(terrainType, tileset, dictionary[key]);
				}
				if (xmlElement.HasAttr("ignores")) {
					foreach (string text in xmlElement.Attr("ignores").Split(',')) {
						if (text.Length > 0) {
							terrainType.Ignores.Add(text[0]);
						}
					}
				}
				dictionary.Add(c, xmlElement);
				lookup.Add(c, terrainType);
			}
		}
		public void SetLevelBounds(XmlNodeList levels) {
			LevelBounds.Clear();
			foreach (XmlNode level in levels) {
				int x = int.Parse(level.Attr("x")) / 8;
				int y = int.Parse(level.Attr("y")) / 8;
				int widthTiles = int.Parse(level.Attr("width")) / 8;
				int heightTiles = int.Parse(level.Attr("height")) / 8;

				LevelBounds.Add(new Rectangle(x, y, widthTiles, heightTiles));
			}
		}
		private void ReadInto(TerrainType data, TileGrid tileset, XmlElement xml) {
			foreach (object obj in xml) {
				if (!(obj is XmlComment)) {
					XmlElement xml2 = obj as XmlElement;
					string text = xml2.Attr("mask");
					Tiles tiles;
					if (text == "center") {
						tiles = data.Center;
					} else if (text == "padding") {
						tiles = data.Padded;
					} else {
						Masked masked = new Masked();
						tiles = masked.Tiles;
						int i = 0;
						int num = 0;
						while (i < text.Length) {
							if (text[i] == '0') {
								masked.Mask[num++] = 0;
							} else if (text[i] == '1') {
								masked.Mask[num++] = 1;
							} else if (text[i] == 'x' || text[i] == 'X') {
								masked.Mask[num++] = 2;
							}
							i++;
						}
						data.Masked.Add(masked);
					}
					string[] array = xml2.Attr("tiles").Split(';');
					for (int j = 0; j < array.Length; j++) {
						string[] array2 = array[j].Split(',');
						int x = int.Parse(array2[0]);
						int y = int.Parse(array2[1]);
						Bitmap item = tileset.Tiles[x, y];
						tiles.Textures.Add(item);
					}
					if (xml2.HasAttr("sprites")) {
						foreach (string item2 in xml2.Attr("sprites").Split(',')) {
							tiles.OverlapSprites.Add(item2);
						}
						tiles.HasOverlays = true;
					}
				}
			}
			data.Masked.Sort(delegate (Masked a, Masked b) {
				int num2 = 0;
				int num3 = 0;
				for (int k = 0; k < 9; k++) {
					if (a.Mask[k] == 2) {
						num2++;
					}
					if (b.Mask[k] == 2) {
						num3++;
					}
				}
				return num2 - num3;
			});
		}

		public TileGrid GenerateMap(VirtualMap<char> mapData, bool paddingIgnoreOutOfLevel) {
			Behaviour behaviour = new Behaviour {
				EdgesExtend = true,
				EdgesIgnoreOutOfLevel = false,
				PaddingIgnoreOutOfLevel = paddingIgnoreOutOfLevel
			};
			return Generate(mapData, 0, 0, mapData.Columns, mapData.Rows, 0, '0', behaviour);
		}

		public TileGrid GenerateBox(char id, int tilesX, int tilesY) {
			return Generate(null, 0, 0, tilesX, tilesY, 2, id, default(Behaviour));
		}

		public TileGrid GenerateOverlay(char id, int x, int y, int tilesX, int tilesY, VirtualMap<char> mapData) {
			Behaviour behaviour = new Behaviour {
				EdgesExtend = true,
				EdgesIgnoreOutOfLevel = true,
				PaddingIgnoreOutOfLevel = true
			};
			return Generate(mapData, x, y, tilesX, tilesY, 1, id, behaviour);
		}

		private TileGrid Generate(VirtualMap<char> mapData, int startX, int startY, int tilesX, int tilesY, int forceSolid, char forceID, Behaviour behaviour) {
			TileGrid tileGrid = new TileGrid(8, 8, tilesX, tilesY);
			Rectangle empty = Rectangle.Empty;
			if (forceSolid != 0) {
				empty = new Rectangle(startX, startY, tilesX, tilesY);
			}
			if (mapData != null) {
				for (int i = startX; i < startX + tilesX; i += 50) {
					for (int j = startY; j < startY + tilesY; j += 50) {
						if (!mapData.AnyInSegmentAtTile(i, j)) {
							j = j / 50 * 50;
						} else {
							int k = i;
							int num = Math.Min(i + 50, startX + tilesX);
							while (k < num) {
								int l = j;
								int num2 = Math.Min(j + 50, startY + tilesY);
								while (l < num2) {
									Tiles tiles = TileHandler(mapData, k, l, forceSolid, empty, forceID, behaviour);
									if (tiles != null) {
										tileGrid.Tiles[k - startX, l - startY] = Util.Random.Choose(tiles.Textures);
									}
									l++;
								}
								k++;
							}
						}
					}
				}
			} else {
				for (int m = startX; m < startX + tilesX; m++) {
					for (int n = startY; n < startY + tilesY; n++) {
						Tiles tiles2 = TileHandler(null, m, n, forceSolid, empty, forceID, behaviour);
						if (tiles2 != null) {
							tileGrid.Tiles[m - startX, n - startY] = Util.Random.Choose(tiles2.Textures);
						}
					}
				}
			}
			return tileGrid;
		}

		private Tiles TileHandler(VirtualMap<char> mapData, int x, int y, int forceSolid, Rectangle forceFill, char forceID, Behaviour behaviour) {
			char tile = GetTile(mapData, x, y, forceFill, forceID, behaviour);
			if (IsEmpty(tile)) {
				return null;
			}
			TerrainType terrainType = lookup[tile];
			if (forceSolid == 1 && mapData == null) {
				return terrainType.Center;
			}
			bool flag = true;
			int num = 0;
			for (int i = -1; i < 2; i++) {
				for (int j = -1; j < 2; j++) {
					bool flag2 = CheckTile(terrainType, mapData, x + j, y + i, forceFill, behaviour);
					if (!flag2 && behaviour.EdgesIgnoreOutOfLevel && !CheckForSameLevel(x, y, x + j, y + i)) {
						flag2 = true;
					}
					adjacent[num++] = (flag2 ? (byte)1 : (byte)0);
					if (!flag2) {
						flag = false;
					}
				}
			}
			if (!flag) {
				foreach (Masked masked in terrainType.Masked) {
					bool flag3 = true;
					int num2 = 0;
					while (num2 < 9 && flag3) {
						if (masked.Mask[num2] != 2 && masked.Mask[num2] != adjacent[num2]) {
							flag3 = false;
						}
						num2++;
					}
					if (flag3) {
						return masked.Tiles;
					}
				}
				return null;
			}
			bool flag4;
			if (!behaviour.PaddingIgnoreOutOfLevel) {
				flag4 = (!CheckTile(terrainType, mapData, x - 2, y, forceFill, behaviour) || !CheckTile(terrainType, mapData, x + 2, y, forceFill, behaviour) || !CheckTile(terrainType, mapData, x, y - 2, forceFill, behaviour) || !CheckTile(terrainType, mapData, x, y + 2, forceFill, behaviour));
			} else {
				flag4 = ((!CheckTile(terrainType, mapData, x - 2, y, forceFill, behaviour) && CheckForSameLevel(x, y, x - 2, y)) || (!CheckTile(terrainType, mapData, x + 2, y, forceFill, behaviour) && CheckForSameLevel(x, y, x + 2, y)) || (!CheckTile(terrainType, mapData, x, y - 2, forceFill, behaviour) && CheckForSameLevel(x, y, x, y - 2)) || (!CheckTile(terrainType, mapData, x, y + 2, forceFill, behaviour) && CheckForSameLevel(x, y, x, y + 2)));
			}
			if (flag4) {
				return terrainType.Padded;
			}
			return terrainType.Center;
		}
		private bool CheckForSameLevel(int x1, int y1, int x2, int y2) {
			return true;
			//foreach (Rectangle rectangle in LevelBounds) {
			//	if (rectangle.Contains(x1, y1) && rectangle.Contains(x2, y2)) {
			//		return true;
			//	}
			//}
			//return false;
		}
		private bool CheckTile(TerrainType set, VirtualMap<char> mapData, int x, int y, Rectangle forceFill, Behaviour behaviour) {
			if (forceFill.Contains(x, y)) {
				return true;
			}
			if (mapData == null) {
				return behaviour.EdgesExtend;
			}
			if (x >= 0 && y >= 0 && x < mapData.Columns && y < mapData.Rows) {
				char c = mapData[x, y];
				return !IsEmpty(c) && !set.Ignore(c);
			}
			if (!behaviour.EdgesExtend) {
				return false;
			}
			char c2 = mapData[Util.Clamp(x, 0, mapData.Columns - 1), Util.Clamp(y, 0, mapData.Rows - 1)];
			return !IsEmpty(c2) && !set.Ignore(c2);
		}
		private char GetTile(VirtualMap<char> mapData, int x, int y, Rectangle forceFill, char forceID, Behaviour behaviour) {
			if (forceFill.Contains(x, y)) {
				return forceID;
			}
			if (mapData == null) {
				if (!behaviour.EdgesExtend) {
					return '0';
				}
				return forceID;
			} else {
				if (x >= 0 && y >= 0 && x < mapData.Columns && y < mapData.Rows) {
					return mapData[x, y];
				}
				if (!behaviour.EdgesExtend) {
					return '0';
				}
				int x2 = Util.Clamp(x, 0, mapData.Columns - 1);
				int y2 = Util.Clamp(y, 0, mapData.Rows - 1);
				return mapData[x2, y2];
			}
		}
		private bool IsEmpty(char id) {
			return id == '0' || id == '\0';
		}

		private class TerrainType {
			public char ID;
			public HashSet<char> Ignores = new HashSet<char>();
			public List<Masked> Masked = new List<Masked>();
			public Tiles Center = new Tiles();
			public Tiles Padded = new Tiles();

			public TerrainType(char id) {
				ID = id;
			}
			public bool Ignore(char c) {
				return ID != c && (Ignores.Contains(c) || Ignores.Contains('*'));
			}
		}
		private class Masked {
			public byte[] Mask = new byte[9];
			public Tiles Tiles = new Tiles();
		}
		private class Tiles {
			public List<Bitmap> Textures = new List<Bitmap>();
			public List<string> OverlapSprites = new List<string>();
			public bool HasOverlays;
		}
		public struct Behaviour {
			public bool PaddingIgnoreOutOfLevel;
			public bool EdgesIgnoreOutOfLevel;
			public bool EdgesExtend;
		}
	}
	public class TileGrid {
		public static Color DefaultBackground = Color.Black;
		public int Width, Height;
		public VirtualMap<Bitmap> Tiles;
		public TileGrid(int w, int h, int tilesX, int tilesY) {
			Width = w;
			Height = h;
			Tiles = new VirtualMap<Bitmap>(tilesX, tilesY, null);
		}
		public void Load(Bitmap images) {
			for (int i = 0; i < Tiles.Columns; i++) {
				for (int j = 0; j < Tiles.Rows; j++) {
					Tiles[i, j] = Util.GetSubImage(images, new Sprite(i * Width, j * Height, Width, Height, 0, 0, Width, Height));
				}
			}
		}
		public Bitmap DisplayMap(List<Backdrop> backdrops, Rectangle bounds, bool before) {
			if (Tiles.Columns == 0 || Tiles.Rows == 0) { return null; }

			Bitmap img = new Bitmap(Tiles.Columns * Width, Tiles.Rows * Height, PixelFormat.Format32bppArgb);
			using (Graphics g = Graphics.FromImage(img)) {
				if (before) {
					using (SolidBrush brush = new SolidBrush(DefaultBackground)) {
						g.FillRectangle(brush, 0, 0, img.Width, img.Height);
					}

					if (backdrops != null) {
						for (int i = 0; i < backdrops.Count; i++) {
							Backdrop bd = backdrops[i];
							bd.Render(bounds, g);
						}
					}
				}

				for (int i = 0; i < Tiles.Columns; i++) {
					for (int j = 0; j < Tiles.Rows; j++) {
						Bitmap tile = Tiles[i, j];
						if (tile != null) {
							g.DrawImage(tile, (float)i * Width, (float)j * Height);
						}
					}
				}

				if (!before && backdrops != null) {
					for (int i = 0; i < backdrops.Count; i++) {
						Backdrop bd = backdrops[i];
						bd.Render(bounds, g);
					}
				}
			}
			return img;
		}
		public Bitmap GenerateMap(VirtualMap<int> map) {
			if (map.Columns == 0 || map.Rows == 0) { return null; }

			Bitmap img = new Bitmap(map.Columns * Width, map.Rows * Height, PixelFormat.Format32bppArgb);
			using (Graphics g = Graphics.FromImage(img)) {
				for (int i = 0; i < map.Columns; i++) {
					for (int j = 0; j < map.Rows; j++) {
						Bitmap tile = this[map[i, j]];
						if (tile != null) {
							g.DrawImage(tile, (float)i * Width, (float)j * Height);
						}
					}
				}
			}
			return img;
		}
		public void Overlay(XmlNode level, string objects, int width, int height, TileGrid tileset) {
			XmlNode tileData = level.SelectSingleNode(objects);
			VirtualMap<int> map = ReadMapInt(tileData == null ? string.Empty : tileData.InnerText, width, height);

			if (map.Columns == 0 || map.Rows == 0) { return; }

			for (int i = 0; i < map.Columns; i++) {
				for (int j = 0; j < map.Rows; j++) {
					Bitmap tile = tileset[map[i, j]];
					if (tile != null) {
						this[i, j] = tile;
					}
				}
			}
		}
		private VirtualMap<int> ReadMapInt(string tiles, int width, int height) {
			VirtualMap<int> mapData = new VirtualMap<int>(width, height, -1);
			int length = tiles.Length;
			int i = 0;
			int col = 0, row = 0;
			StringBuilder sb = new StringBuilder();
			while (i < length) {
				char val = tiles[i++];
				if (char.IsNumber(val) || val == '-') {
					sb.Append(val);
				} else if (val == ',') {
					mapData[col++, row] = int.Parse(sb.ToString());
					sb.Length = 0;
				} else if (val == '\r' || val == '\n') {
					if (val == '\n') {
						if (sb.Length > 0) {
							mapData[col, row] = int.Parse(sb.ToString());
							sb.Length = 0;
						}
						row++;
						col = 0;
					}
					continue;
				}
			}
			return mapData;
		}
		public Bitmap this[int x, int y] {
			get {
				return Tiles[x, y];
			}
			set {
				Tiles[x, y] = value;
			}
		}
		public Bitmap this[int index] {
			get {
				if (index < 0) {
					return null;
				}
				return Tiles[index % Tiles.Columns, index / Tiles.Columns];
			}
			set {
				if (index < 0) {
					return;
				}
				Tiles[index % Tiles.Columns, index / Tiles.Columns] = value;
			}
		}
	}
}