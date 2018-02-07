using CelesteMap.Backdrops;
using CelesteMap.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace CelesteMap.Utility {
	public class Gameplay {
		private static Bitmap TileSet;
		private static Dictionary<string, Sprite> Images = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
		private static AutoTiler foreground, background;
		private static char DefaultTile = 'a';

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
		public Bitmap GenerateMap(MapElement element) {
			string package = element.Attr("_package", "map");
			TileGrid.DefaultBackground = Util.HexToColor("48106e");//Util.HexToColor("000000");

			List<MapElement> levels = element.Select("levels", "level");
			Rectangle chapterBounds = GetChapterBounds(levels);

			Rectangle viewport = new Rectangle(0, 0, chapterBounds.Width, chapterBounds.Height);
			//Rectangle viewport = new Rectangle(250, 3000, 300, 200);
			//Rectangle viewport = GetLevelBounds(levels, chapterBounds, "lvl_d3");
			Bitmap chapter = new Bitmap(viewport.Width, viewport.Height, PixelFormat.Format32bppArgb);
			MapElement bgs = element.SelectFirst("Style", "Backgrounds");
			MapElement fgs = element.SelectFirst("Style", "Foregrounds");

			background.SetLevelBounds(levels);
			foreground.SetLevelBounds(levels);
			string sceneryTileset = "scenery";
			TileGrid scenery = GetTileset(sceneryTileset);

			for (int i = 0; i < levels.Count; i++) {
				MapElement level = levels[i];

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
				string tileset = level.SelectFirst("bgtiles").Attr("tileset", "Scenery");
				if (tileset.Equals("terrain", StringComparison.OrdinalIgnoreCase)) {
					tileset = "scenery";
				}
				if (tileset != sceneryTileset) {
					scenery = GetTileset(tileset);
					sceneryTileset = tileset;
				}
				tiles.Overlay(level, "bgtiles", widthTiles, heightTiles, scenery);

				using (Bitmap map = tiles.DisplayMap(Backdrop.CreateBackdrops(bgs), new Rectangle(pos, chapterBounds.Size), true)) {
					OverlayDecals(level.Select("bgdecals", "decal"), map);
					tiles = GenerateLevelTiles(level, "solids", widthTiles, heightTiles, foreground, out solids);
					OverlayEntities(level.SelectFirst("entities"), map, solids, true);
					Util.CopyTo(chapter, map, offset);
				}

				tileset = level.SelectFirst("fgtiles").Attr("tileset", "Scenery");
				if (tileset.Equals("terrain", StringComparison.OrdinalIgnoreCase)) {
					tileset = "scenery";
				}
				if (tileset != sceneryTileset) {
					scenery = GetTileset(tileset);
					sceneryTileset = tileset;
				}

				tiles.Overlay(level, "fgtiles", widthTiles, heightTiles, scenery);
				using (Bitmap map = tiles.DisplayMap(Backdrop.CreateBackdrops(fgs), new Rectangle(pos, chapterBounds.Size), false)) {
					OverlayDecals(level.Select("fgdecals", "decal"), map);
					OverlayEntities(level.SelectFirst("entities"), map, solids, false);
					Util.CopyTo(chapter, map, offset);
				}

				//XmlNode objtiles = level.SelectSingleNode("objtiles");
			}

			levels = element.Select("Filler", "rect");
			for (int i = 0; i < levels.Count; i++) {
				MapElement level = levels[i];

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
		private void OverlayEntities(MapElement entities, Bitmap map, VirtualMap<char> solids, bool background) {
			using (Graphics g = Graphics.FromImage(map)) {
				List<Entity> ents = new List<Entity>();
				for (int i = entities.Children.Count - 1; i >= 0; i--) {
					MapElement child = entities.Children[i];

					Entity entity = null;
					if (child.Name.IndexOf("spikes", StringComparison.OrdinalIgnoreCase) == 0) {
						entity = background ? Spikes.FromElement(child) : null;
					} else if (child.Name.IndexOf("triggerSpikes", StringComparison.OrdinalIgnoreCase) == 0) {
						entity = background ? TriggerSpikes.FromElement(child) : null;
					} else if (child.Name.IndexOf("strawberry", StringComparison.OrdinalIgnoreCase) == 0) {
						entity = background ? Strawberry.FromElement(child) : null;
					} else if (child.Name.Equals("goldenBerry", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? Strawberry.FromElement(child) : null;
					} else if (child.Name.Equals("redBlocks", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? ClutterBlock.FromElement(child) : null;
					} else if (child.Name.Equals("greenBlocks", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? ClutterBlock.FromElement(child) : null;
					} else if (child.Name.Equals("yellowBlocks", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? ClutterBlock.FromElement(child) : null;
					} else if (child.Name.Equals("memorialTextController", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? Strawberry.FromElement(child) : null;
					} else if (child.Name.Equals("bonfire", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? Bonfire.FromElement(child) : null;
					} else if (child.Name.Equals("jumpThru", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? JumpThru.FromElement(child) : null;
					} else if (child.Name.Equals("memorial", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? Memorial.FromElement(child) : null;
					} else if (child.Name.Equals("player", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? PlayerSpawn.FromElement(child) : null;
					} else if (child.Name.Equals("zipMover", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? ZipMover.FromElement(child) : null;
					} else if (child.Name.Equals("wire", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? Wire.FromElement(child) : null;
					} else if (child.Name.Equals("crumbleBlock", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? CrumbleBlock.FromElement(child) : null;
					} else if (child.Name.Equals("refill", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? Refill.FromElement(child) : null;
					} else if (child.Name.Equals("spring", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? Spring.FromElement(child) : null;
					} else if (child.Name.Equals("fakeWall", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? FakeWall.FromElement(child) : null;
					} else if (child.Name.Equals("exitBlock", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? FakeWall.FromElement(child) : null;
					} else if (child.Name.Equals("lightBeam", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? LightBeam.FromElement(child) : null;
					} else if (child.Name.Equals("cassette", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? Cassette.FromElement(child) : null;
					} else if (child.Name.Equals("flutterBird", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? FlutterBird.FromElement(child) : null;
					} else if (child.Name.Equals("checkpoint", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? Checkpoint.FromElement(child) : null;
					} else if (child.Name.Equals("fallingBlock", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? FallingBlock.FromElement(child) : null;
					} else if (child.Name.Equals("cassetteBlock", StringComparison.OrdinalIgnoreCase)) {
						//entity = background ? CassetteBlock.FromElement(child) : null;
					} else if (child.Name.Equals("dashBlock", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? DashBlock.FromElement(child) : null;
					} else if (child.Name.Equals("coverupWall", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? CoverupWall.FromElement(child) : null;
					} else if (child.Name.Equals("npc", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? NPC.FromElement(child) : null;
					} else if (child.Name.Equals("birdForsakenCityGem", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? ForsakenCityGem.FromElement(child) : null;
					} else if (child.Name.Equals("floatingDebris", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? FloatingDebris.FromElement(child) : null;
					} else if (child.Name.Equals("hangingLamp", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? HangingLamp.FromElement(child) : null;
					} else if (child.Name.Equals("heartGem", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? HeartGem.FromElement(child) : null;
					} else if (child.Name.Equals("blackGem", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? HeartGem.FromElement(child) : null;
					} else if (child.Name.Equals("dreamMirror", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? DreamMirror.FromElement(child) : null;
					} else if (child.Name.Equals("darkChaser", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? DarkChaser.FromElement(child) : null;
					} else if (child.Name.Equals("dreamBlock", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? DreamBlock.FromElement(child) : null;
					} else if (child.Name.Equals("touchSwitch", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? TouchSwitch.FromElement(child) : null;
					} else if (child.Name.Equals("switchGate", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? SwitchGate.FromElement(child) : null;
					} else if (child.Name.Equals("invisibleBarrier", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? InvisibleBarrier.FromElement(child) : null;
					} else if (child.Name.Equals("payphone", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? Payphone.FromElement(child) : null;
					} else if (child.Name.Equals("spinner", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? Spinner.FromElement(child) : null;
					} else if (child.Name.Equals("towerViewer", StringComparison.OrdinalIgnoreCase)) {
						entity = background ? TowerViewer.FromElement(child) : null;
					} else if (child.Name.Equals("foregroundDebris", StringComparison.OrdinalIgnoreCase)) {
						entity = !background ? ForegroundDebris.FromElement(child) : null;
					} else if (background) {
						//Console.WriteLine(child.Name);
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
		private void OverlayDecals(List<MapElement> decals, Bitmap map) {
			if (decals.Count == 0) { return; }
			using (Graphics g = Graphics.FromImage(map)) {
				for (int i = 0; i < decals.Count; i++) {
					MapElement decal = decals[i];
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
			using (Bitmap sceneryTileset = GetImage("tilesets/" + tilesetName)) {
				tileset = new TileGrid(8, 8, sceneryTileset.Width / 8, sceneryTileset.Height / 8);
				tileset.Load(sceneryTileset);
			}
			return tileset;
		}
		private TileGrid GenerateLevelTiles(MapElement level, string objects, int width, int height, AutoTiler tiler, out VirtualMap<char> map) {
			MapElement tileData = level.SelectFirst(objects);
			map = ReadMapChar(tileData == null ? string.Empty : tileData.Attr("InnerText"), width, height);
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
		private Rectangle GetChapterBounds(List<MapElement> levels) {
			int minX = int.MaxValue, maxXW = int.MinValue, minY = int.MaxValue, maxYH = int.MinValue;

			//int levelCount = 0;
			for (int i = 0; i < levels.Count; i++) {
				MapElement level = levels[i];
				string name = level.Attr("name");
				try {
					int x = level.AttrInt("x", 0);
					int y = level.AttrInt("y", 0);
					int width = level.AttrInt("width", 0);
					int height = level.AttrInt("height", 0);

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
		private Rectangle GetLevelBounds(List<MapElement> levels, Rectangle chapterBounds, string levelName) {
			for (int i = 0; i < levels.Count; i++) {
				MapElement level = levels[i];
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