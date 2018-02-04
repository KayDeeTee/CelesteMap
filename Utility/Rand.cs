using System.Runtime.CompilerServices;
namespace CelesteMap.Utility {
	public class Rand {
		private const ulong lower_mask = 0x7FFFFFFF;
		private const ulong upper_mask = ~lower_mask;

		private ulong[] data = new ulong[312];
		private ulong index = 313;
		private long seed;

		public Rand() {
			Seed = 0x123456789abcedf0;
		}
		public Rand(long seed) {
			Seed = seed;
		}
		public long Seed {
			get { return seed; }
			set {
				seed = value;
				index = 312;
				data[0] = (ulong)seed;

				for (int i = 1; i < 312; ++i) {
					data[i] = (6364136223846793005 * (data[i - 1] ^ (data[i - 1] >> 62)) + (ulong)i);
				}
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Next() {
			if (index >= 312) {
				Twist();
			}

			ulong y = data[index];
			y = y ^ ((y >> 29) & 0x5555555555555555);
			y = y ^ ((y << 17) & 0x71D67FFFEDA60000);
			y = y ^ ((y << 37) & 0xFFF7EEE000000000);
			y = y ^ (y >> 43);

			++index;

			return (int)y & 0x7fffffff;
		}
		public int Next(int max) {
			return Next() % max;
		}
		private void Twist() {
			ulong x, xA;
			for (int i = 0; i < 156; ++i) {
				x = (data[i] & upper_mask) + (data[i + 1] & lower_mask);
				xA = x >> 1;

				if ((x & 1) != 0) {
					xA = xA ^ 0xB5026F5AA96619E9;
				}

				data[i] = data[i + 156] ^ xA;
			}

			for (int i = 156; i < 311; ++i) {
				x = (data[i] & upper_mask) + (data[i + 1] & lower_mask);
				xA = x >> 1;

				if ((x & 1) != 0) {
					xA = xA ^ 0xB5026F5AA96619E9;
				}

				data[i] = data[i - 156] ^ xA;
			}

			x = (data[311] & upper_mask) + (data[0] & lower_mask);
			xA = x >> 1;

			if ((x & 1) != 0) {
				xA = xA ^ 0xB5026F5AA96619E9;
			}

			data[311] = data[155] ^ xA;

			index = 0;
		}
	}
}