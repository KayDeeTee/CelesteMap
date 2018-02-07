using CelesteMap.Utility;
using System;
using System.Drawing;
namespace CelesteMap.Entities {
	public class Bonfire : Entity {
		public Fire Type;
		public enum Fire {
			Unlit,
			Lit,
			Dream,
			Smoking
		}
		public Bonfire(Fire type) {
			Depth = -5;
			Type = type;
		}
		public static Bonfire FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);
			string mode = node.Attr("mode", "unlit");
			if (!Enum.TryParse<Fire>(mode, true, out Fire fireType)) { return null; }

			Bonfire entity = new Bonfire(fireType);
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			string path = "objects/campfire/fire00";
			if (Type == Fire.Dream) {
				path = "objects/campfire/dream00";
			} else if (Type == Fire.Lit) {
				path = "objects/campfire/fire10";
			} else if (Type == Fire.Smoking) {
				path = "objects/campfire/smoking00";
			}

			Bitmap img = Gameplay.GetImage(path);
			if (img == null) { return; }

			map.DrawImage(img, Position.X - img.Width / 2, Position.Y - img.Height);
			img.Dispose();
		}
	}
}