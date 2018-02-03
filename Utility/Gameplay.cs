using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;
using CelesteMap.Backdrops;
using CelesteMap.Entities;
namespace CelesteMap.Utility {
	public class Gameplay {
		private static Bitmap TileSet;
		private static Dictionary<string, Sprite> Images = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
		private static AutoTiler foreground, background;
		private static char DefaultTile = '1';

		static Gameplay() {
			using (Stream stream = Util.ReadResourceStream("Resources.Gameplay0.png")) {
				TileSet = new Bitmap(stream);
			}
			ParseItems();
			foreground = new AutoTiler(Util.ReadResource("Resources.ForegroundTiles.xml"));
			background = new AutoTiler(Util.ReadResource("Resources.BackgroundTiles.xml"));
		}

		public static AutoTiler GetTiler(bool bg) {
			return bg ? background : foreground;
		}
		public Bitmap GenerateMap(XmlDocument xmlMap) {
			TileGrid.DefaultBackground = Util.HexToColor("000000");//Util.HexToColor("48106e");

			XmlNodeList levels = xmlMap.SelectNodes(".//levels/level");
			Rectangle chapterBounds = GetChapterBounds(levels);

			Rectangle viewport = new Rectangle(0, 0, chapterBounds.Width, chapterBounds.Height);
			//Rectangle viewport = new Rectangle(250, 3000, 300, 200);
			//Rectangle viewport = GetLevelBounds(levels, chapterBounds, "lvl_2");
			Bitmap chapter = new Bitmap(viewport.Width, viewport.Height, PixelFormat.Format32bppArgb);
			XmlNode bgs = xmlMap.SelectSingleNode(".//Style/Backgrounds");
			XmlNode fgs = xmlMap.SelectSingleNode(".//Style/Foregrounds");

			background.SetLevelBounds(levels);
			foreground.SetLevelBounds(levels);
			string sceneryTileset = "scenery";
			TileGrid scenery = GetTileset(sceneryTileset);

			foreach (XmlNode level in levels) {
				string name = level.Attr("name");
				int x = level.AttrInt("x", 0);
				int y = level.AttrInt("y", 0);
				int width = level.AttrInt("width", 0);
				int height = level.AttrInt("height", 0);
				int widthTiles = width / 8;
				int heightTiles = height / 8;
				Point pos = new Point(x - chapterBounds.X, y - chapterBounds.Y);
				Point offset = new Point(pos.X - viewport.X, pos.Y - viewport.Y);
				Rectangle levelBounds = new Rectangle(pos.X, pos.Y, width, height);
				if (!levelBounds.IntersectsWith(viewport)) { continue; }

				TileGrid tiles = GenerateLevelTiles(level, "bg", widthTiles, heightTiles, background, out VirtualMap<char> solids);
				string tileset = level.SelectSingleNode("bgtiles").Attr("tileset", "Scenery");
				if (tileset.Equals("terrain", StringComparison.OrdinalIgnoreCase)) {
					tileset = "scenery";
				}
				if (tileset != sceneryTileset) {
					scenery = GetTileset(tileset);
					sceneryTileset = tileset;
				}
				tiles.Overlay(level, "bgtiles", widthTiles, heightTiles, scenery);

				using (Bitmap map = tiles.DisplayMap(Backdrop.CreateBackdrops(bgs), new Rectangle(pos, chapterBounds.Size), true)) {
					OverlayDecals(level.SelectNodes(".//bgdecals/decal"), map);
					tiles = GenerateLevelTiles(level, "solids", widthTiles, heightTiles, foreground, out solids);
					OverlayEntities(level.SelectSingleNode(".//entities"), map, solids);
					Util.CopyTo(chapter, map, offset);
				}

				tileset = level.SelectSingleNode("fgtiles").Attr("tileset", "Scenery");
				if (tileset.Equals("terrain", StringComparison.OrdinalIgnoreCase)) {
					tileset = "scenery";
				}
				if (tileset != sceneryTileset) {
					scenery = GetTileset(tileset);
					sceneryTileset = tileset;
				}

				tiles.Overlay(level, "fgtiles", widthTiles, heightTiles, scenery);
				using (Bitmap map = tiles.DisplayMap(Backdrop.CreateBackdrops(fgs), new Rectangle(pos, chapterBounds.Size), false)) {
					OverlayDecals(level.SelectNodes(".//fgdecals/decal"), map);
					Util.CopyTo(chapter, map, offset);
				}

				//XmlNode objtiles = level.SelectSingleNode("objtiles");
			}

			levels = xmlMap.SelectNodes(".//Filler/rect");
			foreach (XmlNode level in levels) {
				int x = level.AttrInt("x", 0);
				int y = level.AttrInt("y", 0);
				int width = level.AttrInt("w", 0);
				int height = level.AttrInt("h", 0);
				Point pos = new Point(x * 8 - chapterBounds.X, y * 8 - chapterBounds.Y);
				Point offset = new Point(pos.X - viewport.X, pos.Y - viewport.Y);
				Rectangle levelBounds = new Rectangle(pos.X, pos.Y, width, height);
				if (!levelBounds.IntersectsWith(viewport)) { continue; }

				TileGrid tiles = foreground.GenerateOverlay(DefaultTile, 0, 0, width, height, null);
				using (Bitmap map = tiles.DisplayMap(null, chapterBounds, false)) {
					Util.CopyTo(chapter, map, pos);
				}
			}
			return chapter;
		}
		private void OverlayEntities(XmlNode entities, Bitmap map, VirtualMap<char> solids) {
			using (Graphics g = Graphics.FromImage(map)) {
				List<Entity> ents = new List<Entity>();
				foreach (XmlNode child in entities) {
					Entity entity = null;
					if (child.Name.IndexOf("spikes", StringComparison.OrdinalIgnoreCase) == 0) {
						entity = Spikes.FromElement(child);
					} else if (child.Name.IndexOf("strawberry", StringComparison.OrdinalIgnoreCase) == 0) {
						entity = Strawberry.FromElement(child);
					} else if (child.Name.Equals("goldenBerry", StringComparison.OrdinalIgnoreCase)) {
						entity = Strawberry.FromElement(child);
					} else if (child.Name.Equals("bonfire", StringComparison.OrdinalIgnoreCase)) {
						entity = Bonfire.FromElement(child);
					} else if (child.Name.Equals("jumpThru", StringComparison.OrdinalIgnoreCase)) {
						entity = JumpThru.FromElement(child);
					} else if (child.Name.Equals("memorial", StringComparison.OrdinalIgnoreCase)) {
						entity = Memorial.FromElement(child);
					} else if (child.Name.Equals("player", StringComparison.OrdinalIgnoreCase)) {
						entity = PlayerSpawn.FromElement(child);
					} else if (child.Name.Equals("zipMover", StringComparison.OrdinalIgnoreCase)) {
						entity = ZipMover.FromElement(child);
					} else if (child.Name.Equals("wire", StringComparison.OrdinalIgnoreCase)) {
						entity = Wire.FromElement(child);
					} else if (child.Name.Equals("crumbleBlock", StringComparison.OrdinalIgnoreCase)) {
						entity = CrumbleBlock.FromElement(child);
					} else if (child.Name.Equals("refill", StringComparison.OrdinalIgnoreCase)) {
						entity = Refill.FromElement(child);
					} else if (child.Name.Equals("spring", StringComparison.OrdinalIgnoreCase)) {
						entity = Spring.FromElement(child);
					} else if (child.Name.Equals("fakeWall", StringComparison.OrdinalIgnoreCase)) {
						entity = FakeWall.FromElement(child);
					} else if (child.Name.Equals("lightBeam", StringComparison.OrdinalIgnoreCase)) {
						entity = LightBeam.FromElement(child);
					} else if (child.Name.Equals("cassette", StringComparison.OrdinalIgnoreCase)) {
						entity = Cassette.FromElement(child);
					} else if (child.Name.Equals("flutterBird", StringComparison.OrdinalIgnoreCase)) {
						entity = FlutterBird.FromElement(child);
					} else if (child.Name.Equals("checkpoint", StringComparison.OrdinalIgnoreCase)) {
						entity = Checkpoint.FromElement(child);
					} else if (child.Name.Equals("fallingBlock", StringComparison.OrdinalIgnoreCase)) {
						entity = FallingBlock.FromElement(child);
					} else if (child.Name.Equals("cassetteBlock", StringComparison.OrdinalIgnoreCase)) {
						//entity = CassetteBlock.FromElement(child);
					} else if (child.Name.Equals("dashBlock", StringComparison.OrdinalIgnoreCase)) {
						entity = DashBlock.FromElement(child);
					} else if (child.Name.Equals("coverupWall", StringComparison.OrdinalIgnoreCase)) {
						entity = CoverupWall.FromElement(child);
					} else if (child.Name.Equals("npc", StringComparison.OrdinalIgnoreCase)) {
						entity = NPC.FromElement(child);
					} else if (child.Name.Equals("birdForsakenCityGem", StringComparison.OrdinalIgnoreCase)) {
						entity = ForsakenCityGem.FromElement(child);
					} else {
						Console.WriteLine(child.Name);
					}
					if (entity != null) {
						ents.Add(entity);
					}
				}

				ents.Sort(delegate (Entity one, Entity two) {
					int comp = two.Depth.CompareTo(one.Depth);
					return comp == 0 ? one.ID.CompareTo(two.ID) : comp;
				});

				for (int i = 0; i < ents.Count; i++) {
					Entity entity = ents[i];
					entity.Render(g, solids);
				}
			}
		}
		private void OverlayDecals(XmlNodeList decals, Bitmap map) {
			if (decals.Count == 0) { return; }
			using (Graphics g = Graphics.FromImage(map)) {
				foreach (XmlNode decal in decals) {
					string path = "decals/" + decal.Attr("texture").Replace('\\', '/');
					path = path.Substring(0, path.Length - 4);

					float x = decal.AttrFloat("x", 0);
					float y = decal.AttrFloat("y", 0);
					float scaleX = decal.AttrFloat("scaleX", 1);
					float scaleY = decal.AttrFloat("scaleY", 1);

					Bitmap img = GetImage(path);
					if (img != null) {
						if (scaleX < 0 && scaleY < 0) {
							img.RotateFlip(RotateFlipType.RotateNoneFlipXY);
							scaleX = -scaleX;
							scaleY = -scaleY;
						} else if (scaleX < 0) {
							img.RotateFlip(RotateFlipType.RotateNoneFlipX);
							scaleX = -scaleX;
						} else if (scaleY < 0) {
							img.RotateFlip(RotateFlipType.RotateNoneFlipY);
							scaleY = -scaleY;
						}

						x -= img.Width / 2;
						y -= img.Height / 2;

						g.DrawImage(img, (int)x, (int)y, img.Width * scaleX, img.Height * scaleY);
						img.Dispose();
					}
				}
			}
		}
		private TileGrid GetTileset(string tilesetName) {
			TileGrid tileset = null;
			using (Bitmap sceneryTileset = GetImage("tilesets/" + tilesetName)) {//16 padding
				tileset = new TileGrid(8, 8, sceneryTileset.Width / 8, sceneryTileset.Height / 8);
				tileset.Load(sceneryTileset);
			}
			return tileset;
		}
		private TileGrid GenerateLevelTiles(XmlNode level, string objects, int width, int height, AutoTiler tiler, out VirtualMap<char> map) {
			XmlNode tileData = level.SelectSingleNode(objects);
			map = ReadMapChar(tileData == null ? string.Empty : tileData.InnerText, width, height);
			return tiler.GenerateMap(map, true);
		}
		private VirtualMap<char> ReadMapChar(string tiles, int width, int height) {
			VirtualMap<char> mapData = new VirtualMap<char>(width, height, '0');
			int length = tiles.Length;
			int i = 0;
			int col = 0, row = 0;

			while (i < length) {
				char val = tiles[i++];
				if (val == '\r' || val == '\n') {
					if (val == '\n') {
						row++;
						col = 0;
					}
					continue;
				}
				mapData[col++, row] = val;
			}
			return mapData;
		}
		private Rectangle GetChapterBounds(XmlNodeList levels) {
			int minX = int.MaxValue, maxXW = int.MinValue, minY = int.MaxValue, maxYH = int.MinValue;

			//int levelCount = 0;
			foreach (XmlNode level in levels) {
				string name = level.Attr("name");
				try {
					int x = int.Parse(level.Attr("x"));
					int y = int.Parse(level.Attr("y"));
					int width = int.Parse(level.Attr("width"));
					int height = int.Parse(level.Attr("height"));

					if (x < minX) {
						minX = x;
					}
					if (x + width > maxXW) {
						maxXW = x + width;
					}
					if (y < minY) {
						minY = y;
					}
					if (y + height > maxYH) {
						maxYH = y + height;
					}
					//levelCount++;
				} catch {
					throw new Exception("Failed to read level properties for level: " + name);
				}
			}
			//Console.WriteLine(levelCount);

			return new Rectangle(minX, minY, maxXW - minX, maxYH - minY);
		}
		private Rectangle GetLevelBounds(XmlNodeList levels, Rectangle chapterBounds, string levelName) {
			foreach (XmlNode level in levels) {
				string name = level.Attr("name");
				if (!name.Equals(levelName, StringComparison.OrdinalIgnoreCase)) { continue; }

				int x = level.AttrInt("x", 0);
				int y = level.AttrInt("y", 0);
				int width = level.AttrInt("width", 0);
				int height = level.AttrInt("height", 0);
				int widthTiles = width / 8;
				int heightTiles = height / 8;
				return new Rectangle(x - chapterBounds.X, y - chapterBounds.Y, width, height);
			}

			return chapterBounds;
		}
		private static void ParseItems() {
			using (Stream stream = Util.ReadResourceStream("Resources.Gameplay.meta")) {
				using (BinaryReader reader = new BinaryReader(stream)) {
					reader.ReadInt32();
					reader.ReadString();
					reader.ReadInt32();

					int count = reader.ReadInt16();
					for (int i = 0; i < count; i++) {
						string dataFile = reader.ReadString();
						int spriteCount = reader.ReadInt16();
						for (int j = 0; j < spriteCount; j++) {
							string path = reader.ReadString().Replace('\\', '/');
							int x = reader.ReadInt16();
							int y = reader.ReadInt16();
							int width = reader.ReadInt16();
							int height = reader.ReadInt16();

							int xOffset = reader.ReadInt16();
							int yOffset = reader.ReadInt16();
							int realWidth = reader.ReadInt16();
							int realHeight = reader.ReadInt16();

							Images.Add(path, new Sprite(x, y, width, height, xOffset, yOffset, realWidth, realHeight));
						}
					}
				}
			}
		}
		public static Bitmap GetImage(string path) {
			if (!Images.TryGetValue(path, out Sprite bounds)) { return null; }

			return Util.GetSubImage(TileSet, bounds);
		}
		public static List<Bitmap> GetAllImages(string path) {
			List<Bitmap> images = new List<Bitmap>();
			foreach (string key in Images.Keys) {
				if (key.IndexOf(path, StringComparison.OrdinalIgnoreCase) == 0) {
					images.Add(GetImage(key));
				}
			}

			return images;
		}
		public static Bitmap GetRandomImage(string path) {
			List<string> keysFound = new List<string>();
			foreach (string key in Images.Keys) {
				if (key.IndexOf(path, StringComparison.OrdinalIgnoreCase) == 0) {
					keysFound.Add(key);
				}
			}

			if (keysFound.Count == 0) { return null; }
			int rnd = Util.Random.Next() % keysFound.Count;
			return GetImage(keysFound[rnd]);
		}
	}
}