using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public abstract class SceneDrawer {
        public static void DrawOnto(Scene scene, Texture texture, IVertexShader vertexShader, double closePlane) {
            double[,] zBuffer = new double[texture.Width, texture.Height];

            for(int x = 0; x < texture.Width; ++x) {
                for(int y = 0; y < texture.Height; ++y) {
                    zBuffer[x, y] = double.PositiveInfinity;
                }
            }

            for (int i = 0; i < scene.Entities.Length; ++i) {
                for (int j = 0; j < scene.Entities[i].Triangles.Length; ++j) {
                    FillTriangle(texture, vertexShader.Shade(scene.Entities[i].Triangles[j]), zBuffer, closePlane);
                }
            }
        }

        private static void DrawScanline(Texture plane, int xmin, int xmax, int y, Triangle triangle, double[,] zBuffer, double closePlane) {
            if (y < 0 || y >= plane.Height) {
                return;
            }

            xmin = Math.Max(xmin, 0);
            xmax = Math.Min(xmax, plane.Width - 1);
            for (int x = xmin; x < xmax; ++x) {
                Vec3 bary = Barycentric(triangle, x, y);
                double depth = bary.X * triangle.Vertices[0][2] + bary.Y * triangle.Vertices[1][2] + bary.Z * triangle.Vertices[2][2];
                if(depth < zBuffer[x, y] && depth > closePlane) {
                    plane.Pixels[x, y] = triangle.ShadeAt(bary);
                    zBuffer[x, y] = depth;
                }
            }
        }

        private static double Differential(Vector<double> v, Vector<double> u) {
            if (v[1] == u[1]) {
                return double.NaN;
            }

            return
               (u[0] - v[0]) /
               (u[1] - v[1]);
        }

        private static Vec3 Barycentric(Triangle triangle, int x, int y) {
            Vec3 cross = Vec3.CrossProduct(
                new Vec3(triangle.Vertices[2][0] - triangle.Vertices[0][0], triangle.Vertices[1][0] - triangle.Vertices[0][0], triangle.Vertices[0][0] - x),
                new Vec3(triangle.Vertices[2][1] - triangle.Vertices[0][1], triangle.Vertices[1][1] - triangle.Vertices[0][1], triangle.Vertices[0][1] - y)
            );

            return new Vec3(1f - (cross.X + cross.Y) / cross.Z, cross.Y / cross.Z, cross.X / cross.Z);
        }

        public static void FillTriangle(Texture plane, Triangle triangle, double[,] zBuffer, double closePlane) {
            if(!triangle.CorrectWinding() || !triangle.WithinVisibleWindow(plane.Width, plane.Height, closePlane)) {
                return;
            }

            Vector<double>[] vertices = triangle.VerticesSortedByY();

            int ymin = (int)vertices[0][1];
            int ymax = (int)vertices[2][1];

            double diff1 = Differential(vertices[0], vertices[2]);
            double diff2 = Differential(vertices[0], vertices[1]);
            double xmin = vertices[0][0];
            double xmax = vertices[0][0];

            for (int y = ymin; y <= ymax; y++) { 
                if(y == vertices[1][1]) {
                    xmax = vertices[1][0];
                    diff2 = Differential(vertices[1], vertices[2]);
                }

                if (xmin > xmax) {
                    DrawScanline(plane, (int)Math.Round(xmax), (int)Math.Round(xmin), y, triangle, zBuffer, closePlane);
                }
                else {
                    DrawScanline(plane, (int)Math.Round(xmin), (int)Math.Round(xmax), y, triangle, zBuffer, closePlane);
                }

                xmin += diff1;
                xmax += diff2;
            } 
        }
    }
}
