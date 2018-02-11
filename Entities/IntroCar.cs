using CelesteMap.Utility;
using System.Drawing;
namespace CelesteMap.Entities {
	public class IntroCar : Entity {
		public IntroCar() {
			Depth = 3;
		}
		public static IntroCar FromElement(MapElement node) {
			int x = node.AttrInt("x", 0);
			int y = node.AttrInt("y", 0);

			IntroCar entity = new IntroCar();
			entity.Position = new Vector2(x, y);
			entity.ID = node.AttrInt("id", 0);
			return entity;
		}
		public override void Render(Graphics map, VirtualMap<char> solids) {
			Bitmap body = Gameplay.GetImage("scenery/car/body");
			Bitmap wheel = Gameplay.GetImage("scenery/car/wheels");
			Bitmap barrier = Gameplay.GetImage("scenery/car/barrier");
			if (body == null) { return; }

			map.DrawImage(wheel, Position.X - wheel.Width / 2, Position.Y - wheel.Height);
			map.DrawImage(body, Position.X - body.Width / 2, Position.Y - body.Height);

			map.DrawImage(barrier, Position.X - barrier.Width / 2 + 32, Position.Y - barrier.Height);
			map.DrawImage(barrier, Position.X - barrier.Width / 2 + 41, Position.Y - barrier.Height);

			barrier.Dispose();
			wheel.Dispose();
			body.Dispose();
		}
	}
}