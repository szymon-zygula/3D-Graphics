using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public abstract class SceneDrawer {
        public static void DrawOnto(Scene scene, Texture texture) {
            for (int i = 0; i < scene.Entities.Length; ++i) {
                for (int j = 0; j < scene.Entities[i].Triangles.Length; ++j) {
                    ScanLineFiller.FillTriangle(texture, scene.Entities[i].Triangles[j]);
                }
            }
        }
    }

    public static class ScanLineFiller {
        private static void DrawScanline(Texture plane, int xmin, int xmax, int y, Triangle triangle) {
            for (int x = xmin; x <= xmax; ++x) {
                plane.Pixels[x, y] = triangle.ShadeAt(new Vec3(1f, 1f, 1f));
            }
        }

        private static float Differential(Vector<float> v, Vector<float> u) {
            if (v[1] == u[1]) {
                return float.NaN;
            }

            return
               (u[0] - v[0]) /
               (u[1] - v[1]);
        }

        public static void FillTriangle(Texture plane, Triangle triangle) {
            if (triangle.Vertices[0][1] == triangle.Vertices[1][1] && triangle.Vertices[0][1] == triangle.Vertices[2][1]) {
                return;
            }

            triangle.SortVerticesByY();

            int height = (int)Math.Round(triangle.Vertices[2][1] - triangle.Vertices[0][1]);
            int ymin = (int)Math.Round(triangle.Vertices[0][1]);
            int ymax = ymin + height;

            float diff1 = Differential(triangle.Vertices[0], triangle.Vertices[2]);
            float diff2 = Differential(triangle.Vertices[0], triangle.Vertices[1]);
            float xmin = triangle.Vertices[0][0];
            float xmax = triangle.Vertices[0][0];

            if(triangle.Vertices[0][1] == triangle.Vertices[1][1]) {
                xmax = triangle.Vertices[1][0];
            }

            for (int y = ymin; y < ymax; y++) { 
                if(y == Math.Round(triangle.Vertices[1][1])) {
                    diff2 = Differential(triangle.Vertices[1], triangle.Vertices[2]);
                }

                if (xmin > xmax) {
                    DrawScanline(plane, (int)xmax, (int)xmin, y, triangle);
                }
                else {
                    DrawScanline(plane, (int)xmin, (int)xmax, y, triangle);
                }

                xmin += diff1;
                xmax += diff2;
            } 
        }
    }
}
