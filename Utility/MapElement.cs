using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace CelesteMap.Utility {
	public class MapElement {
		public string Name;
		public Dictionary<string, object> Attributes = new Dictionary<string, object>();
		public List<MapElement> Children = new List<MapElement>();

		public bool HasAttr(string name) {
			return Attributes.ContainsKey(name);
		}
		public string Attr(string name, string defaultValue = "") {
			object obj;
			if (!Attributes.TryGetValue(name, out obj)) {
				obj = defaultValue;
			}
			return obj.ToString();
		}
		public bool AttrBool(string name, bool defaultValue = false) {
			object obj;
			if (!Attributes.TryGetValue(name, out obj)) {
				obj = defaultValue;
			}
			if (obj is bool) {
				return (bool)obj;
			}
			return bool.Parse(obj.ToString());
		}
		public float AttrFloat(string name, float defaultValue = 0f) {
			object obj;
			if (!Attributes.TryGetValue(name, out obj)) {
				obj = defaultValue;
			}
			if (obj is float) {
				return (float)obj;
			}
			return float.Parse(obj.ToString(), CultureInfo.InvariantCulture);
		}
		public int AttrInt(string name, int defaultValue = 0) {
			object obj;
			if (!Attributes.TryGetValue(name, out obj)) {
				obj = defaultValue;
			}
			if (obj is int) {
				return (int)obj;
			}
			return int.Parse(obj.ToString(), CultureInfo.InvariantCulture);
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<string, object> pair in Attributes) {
				sb.Append(pair.Key).Append(' ').Append(pair.Value).Append(' ');
			}
			return $"{Name} {sb.ToString()}";
		}
	}
}