using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
namespace CelesteMap.Utility {
	public class Texture {
		public static Image Read(string filename) {
			byte[] data = new byte[524288];
			byte[] textureData = null;
			int width = 0;
			int height = 0;
			bool hasAlpha = false;
			using (FileStream fileStream = File.OpenRead(filename)) {
				fileStream.Read(data, 0, 524288);

				int offset = 0;
				width = BitConverter.ToInt32(data, offset);
				height = BitConverter.ToInt32(data, offset + 4);
				hasAlpha = data[offset + 8] == 1;
				offset += 9;

				int totalBytes = width * height * 4;
				textureData = new byte[totalBytes];

				int j = 0;
				while (j < totalBytes) {
					int repeat = (int)data[offset] * 4;

					if (hasAlpha) {
						byte alpha = data[offset + 1];
						if (alpha > 0) {
							textureData[j] = data[offset + 2];
							textureData[j + 1] = data[offset + 3];
							textureData[j + 2] = data[offset + 4];
							textureData[j + 3] = alpha;
							offset += 5;
						} else {
							textureData[j] = 0;
							textureData[j + 1] = 0;
							textureData[j + 2] = 0;
							textureData[j + 3] = 0;
							offset += 2;
						}
					} else {
						textureData[j] = data[offset + 1];
						textureData[j + 1] = data[offset + 2];
						textureData[j + 2] = data[offset + 3];
						textureData[j + 3] = byte.MaxValue;
						offset += 4;
					}

					if (repeat > 4) {
						int count = j + repeat;
						byte c1 = textureData[j++];
						byte c2 = textureData[j++];
						byte c3 = textureData[j++];
						byte c4 = textureData[j++];
						while (j < count) {
							textureData[j++] = c1;
							textureData[j++] = c2;
							textureData[j++] = c3;
							textureData[j++] = c4;
						}
					} else {
						j += 4;
					}

					if (offset > 524256) {
						int extra = 524288 - offset;
						for (int k = 0; k < extra; k++) {
							data[k] = data[offset + k];
						}
						fileStream.Read(data, extra, 524288 - extra);
						offset = 0;
					}
				}
			}

			GCHandle ptr = GCHandle.Alloc(textureData, GCHandleType.Pinned);
			Bitmap bmp = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, ptr.AddrOfPinnedObject());
			ptr.Free();
			return bmp;
		}
	}
}