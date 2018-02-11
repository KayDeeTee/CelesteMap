using CelesteMap.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace CelesteMap.Entities {
	public class BridgeFixed : Entity {
		public int Width;
		public BridgeFixed(int width) {
			Width = width;
			Depth = 0;
		}
		public static BridgeFixed FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			int width = node.AttrInt("width", 0);

			BridgeFixed entity = new BridgeFixed(width);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap img = Gameplay.GetImage("scenery/bridge_fixed");
			if (img == null) { return; }

			int x = (int)Position.X;
			int y = (int)Position.Y - 8;
			int pos = 0;
			while (pos < Width) {
				map.DrawImage(img, x, y);
				x += img.Width;
				pos += img.Width;
			}

			img.Dispose();
		}
	}
}