using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Windows;

namespace _3D_Graphics {
    // This class has to be used instead of System.Drawing.Bitmap,
    // because System.Drawing.Bitmap.GetPixel too slow to be used in real time rendering
    public class Texture {
        public Vec3[,] Pixels;
        private UInt32[] RawPixels;

        public int Width { get { return Pixels.GetLength(0); } }
        public int Height { get { return Pixels.GetLength(1); } }

        public Texture(int width, int height) {
            Pixels = new Vec3[width, height];
            RawPixels = new UInt32[Width * Height];
        }

        public Texture(Bitmap bitmap) {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds(ref unit);

            Pixels = new Vec3[(int)bounds.Width, (int)bounds.Height];
            RawPixels = new UInt32[Width * Height];

            for(int x = 0; x < Width; ++x) {
                for(int y = 0; y < Height; ++y) {
                    int color = bitmap.GetPixel(x, y).ToArgb();
                    UInt32 convertedColor;
                    unsafe {
                        convertedColor = *(UInt32*)(void*)&color;
                    }

                    Pixels[x, y] = new Vec3(convertedColor);
                }
            }
        }

        private Texture(Texture texture) {
            Pixels = new Vec3[texture.Width, texture.Height];
            RawPixels = new UInt32[Width * Height];
            for (int x = 0; x < Width; ++x) {
                for (int y = 0; y < Height; ++y) {
                    Pixels[x, y] = texture.Pixels[x, y];
                }
            }
        }

        public Texture Clone() {
            Texture clone = new Texture(this);
            return clone;
        }
        
        public void Clean(Vec3 color) {
            Parallel.For(0, Width, (x) => {
                for (int y = 0; y < Height; ++y) {
                    Pixels[x, y] = color;
                }
            });
        }

        public BitmapSource CreateBitmapSource() {
            Parallel.For(0, Width, (x) => {
                for(int y = 0; y < Height; ++y) {
                    RawPixels[x + y * Width] = Pixels[x, y].ToColor();
                }
            });

            return BitmapSource.Create(
                Width, Height,
                96, 96,
                PixelFormats.Bgra32,
                null,
                RawPixels,
                Width * 4
            );
        }
    }
}
