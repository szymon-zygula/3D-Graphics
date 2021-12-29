﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public abstract class SceneDrawer {
        public static void DrawOnto(Scene scene, Texture texture, IVertexShader vertexShader) {
            for (int i = 0; i < scene.Entities.Length; ++i) {
                for (int j = 0; j < scene.Entities[i].Triangles.Length; ++j) {
                    FillTriangle(texture, vertexShader.Shade(scene.Entities[i].Triangles[j]));
                }
            }
        }

        private static void DrawScanline(Texture plane, int xmin, int xmax, int y, Triangle triangle) {
            if (y < 0 || y >= plane.Height) {
                return;
            }

            xmin = Math.Max(xmin, 0);
            xmax = Math.Min(xmax, plane.Width - 1);
            for (int x = xmin; x <= xmax; ++x) {
                plane.Pixels[x, y] = triangle.ShadeAt(Barycentric(triangle, x, y));
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

            if (Math.Abs(cross.Y) < 1) {
                return new Vec3(-1f, 1f, 1f);
            }

            return new Vec3(1f - (cross.X + cross.Y) / cross.Z, cross.Y / cross.Z, cross.X / cross.Z);
        }

        public static void FillTriangle(Texture plane, Triangle triangle) {
            if (triangle.Vertices[0][1] == triangle.Vertices[1][1] && triangle.Vertices[0][1] == triangle.Vertices[2][1]) {
                return;
            }

            Vector<double>[] vertices = triangle.VerticesSortedByY();

            int height = (int)Math.Round(vertices[2][1] - vertices[0][1]);
            int ymin = (int)Math.Round(vertices[0][1]);
            int ymax = ymin + height;

            double diff1 = Differential(vertices[0], vertices[2]);
            double diff2 = Differential(vertices[0], vertices[1]);
            double xmin = vertices[0][0];
            double xmax = vertices[0][0];

            for (int y = ymin; y < ymax; y++) { 
                if(y == Math.Round(vertices[1][1])) {
                    xmax = vertices[1][0];
                    diff2 = Differential(vertices[1], vertices[2]);
                }

                if (xmin > xmax) {
                    DrawScanline(plane, (int)Math.Round(xmax), (int)Math.Round(xmin), y, triangle);
                }
                else {
                    DrawScanline(plane, (int)Math.Round(xmin), (int)Math.Round(xmax), y, triangle);
                }

                xmin += diff1;
                xmax += diff2;
            } 
        }
    }
}
