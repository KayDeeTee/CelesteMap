using System.Drawing;
using CelesteMap.Utility;
namespace CelesteMap.Entities {
	public abstract class Entity {
		public Vector2 Position;
		public int Depth;
		public int ID;
		public virtual void Render(Graphics map, VirtualMap<char> solids) {
		}
	}
}