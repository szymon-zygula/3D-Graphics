using System;
using System.Drawing;

namespace _3D_Graphics {

    // This class has to be used instead of System.Drawing.Bitmap,
    // because System.Drawing.Bitmap.GetPixel too slow to be used in real time rendering
    public class Texture {
        public Vec3[,] Pixels;
        public int Width { get { return Coefficients.GetLength(0); } }
        public int Height { get { return Coefficients.GetLength(1); } }
        public Texture(Bitmap bitmap) {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds(ref unit);

            Pixels = new Vec3[bounds.Width, bounds.Height];

            for(int x = 0; x < bounds.Width; ++x) {
                for(int y = 0; y < bounds.Height; ++y) {
                    int color = bitmap.GetPixel(x, y).ToArgb();
                    UInt32 convertedColor;
                    unsafe {
                        convertedColor = *(UInt32*)(void*)&color;
                    }

                    Pixels[x, y] = new Vec3(convertedColor);
                }
            }
        }
    }
}
