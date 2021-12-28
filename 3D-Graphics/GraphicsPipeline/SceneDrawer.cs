using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Graphics {
    public abstract class SceneDrawer {
        public static void DrawOnto(Scene scene, Texture texture) {
            for (int i = 0; i < scene.Entities.Length; ++i) {
                for (int j = 0; j < scene.Entities[i].Triangles.Length; ++j) {
                    DrawTriangle(scene.Entities[i].Triangles[j], scene.Entities[i].FragmentShader, texture);
                }
            }
        }

        private static void DrawTriangle(Triangle triangle, IFragmentShader fragmentShader, Texture texture) {
            Vec2[] points = new Vec2[3];
            points[0] = new Vec2(triangle.Vertices[0]);
            points[1] = new Vec2(triangle.Vertices[1]);
            points[2] = new Vec2(triangle.Vertices[2]);
            ScanLineFiller.FillSolidColor(texture, points, 0xFFFF0000);
        }
    }

    public static class ScanLineFiller {
        private class ActiveEdge {
            public int From;
            public int To;
            public float X;
            public float Diff;
            public Vec3 Color;
            public Vec3 ColorDiff;
        }

        private static float Differential(Vec2[] points, int i, int j) {
            if (points[i].Y == points[j].Y) {
                return float.NaN;
            }

            return
                (float)(points[j].X - points[i].X) /
                (float)(points[j].Y - points[i].Y);
        }

        private static Vec3 ColorDifferential(Vec2[] points, UInt32[] colors, int i, int j) {
            return
                (new Vec3(colors[j]) - new Vec3(colors[i])) /
                (float)(points[j].Y - points[i].Y);
        }

        private static void AddToAET(List<ActiveEdge> aet, ActiveEdge ae) {
            if (!float.IsNaN(ae.Diff)) {
                aet.Add(ae);
            }
        }

        private static void AddTopPointToAET(Vec2[] points, UInt32[] colors, int point, List<ActiveEdge> aet) {
            float x = points[point].X;
            int nextPoint = (point + 1) % points.Length;
            int prevPoint = point == 0 ? points.Length - 1 : point - 1;

            AddToAET(aet, new ActiveEdge() {
                X = x,
                Diff = Differential(points, point, nextPoint),
                From = point,
                To = nextPoint,
                Color = new Vec3(colors[point]),
                ColorDiff = ColorDifferential(points, colors, point, nextPoint)
            });

            AddToAET(aet, new ActiveEdge() {
                X = x,
                Diff = Differential(points, point, prevPoint),
                From = point,
                To = prevPoint,
                Color = new Vec3(colors[point]),
                ColorDiff = ColorDifferential(points, colors, point, prevPoint)
            });
        }

        private static (List<ActiveEdge>, int) InitAET(Vec2[] points, UInt32[] colors, int[] perm) {
            List<ActiveEdge> aet = new List<ActiveEdge>();

            for (int i = 0; i < points.Length; ++i) {
                if (points[perm[i]].Y != points[perm[0]].Y) {
                    return (aet, i);
                }

                AddTopPointToAET(points, colors, perm[i], aet);
            }

            return (aet, points.Length);
        }

        private static void HandleNewEdge(Vec2[] points, UInt32[] colors, List<ActiveEdge> aet, int i, int j) {
            if (points[j].Y >= points[i].Y) {
                AddToAET(aet, new ActiveEdge() {
                    X = points[i].X,
                    Diff = Differential(points, i, j),
                    From = i,
                    To = j,
                    Color = new Vec3(colors[i]),
                    ColorDiff = ColorDifferential(points, colors, i, j)
                });
            }
            else {
                aet.RemoveAll((ActiveEdge ae) => ae.To == i && ae.From == j);
            }
        }

        private static void DrawScanline(Texture plane, List<ActiveEdge> aet, int y) {
            for (int i = 0; i < aet.Count - 1; i += 2) {
                ActiveEdge ae1 = aet[i];
                ActiveEdge ae2 = aet[i + 1];
                Vec3 colorDiff = (ae2.Color - ae1.Color) / (ae2.X - ae1.X);
                Vec3 color = ae1.Color;

                for (int x = (int)Math.Round(ae1.X); x <= (int)Math.Round(ae2.X); ++x) {
                    plane.Pixels[x, y] = color;
                    color += colorDiff;
                }

                ae1.X += ae1.Diff;
                ae1.Color += ae1.ColorDiff;
                ae2.X += ae2.Diff;
                ae2.Color += ae2.ColorDiff;
            }
        }

        private static void HandleNewPoints(Vec2[] points, UInt32[] colors, int[] perm, ref int nextToProcess, List<ActiveEdge> aet, int y) {
            while ((int)Math.Round(points[perm[nextToProcess]].Y) == y - 1) {
                int nextPoint = (perm[nextToProcess] + 1) % points.Length;
                int prevPoint = perm[nextToProcess] == 0 ? points.Length - 1 : perm[nextToProcess] - 1;

                HandleNewEdge(points, colors, aet, perm[nextToProcess], prevPoint);
                HandleNewEdge(points, colors, aet, perm[nextToProcess], nextPoint);

                nextToProcess += 1;
            }
        }

        public static void FillSolidColor(Texture plane, Vec2[] points, UInt32 color) {
            UInt32[] colors = Enumerable.Repeat<UInt32>(color, points.Length).ToArray();
            FillVertexInterpolation(plane, points, colors);
        }

        public static void FillVertexInterpolation(Texture plane, Vec2[] points, UInt32[] colors) {
            int[] perm = Enumerable.Range(0, points.Length).ToArray();
            Array.Sort(perm, (int a, int b) => points[a].Y.CompareTo(points[b].Y));
            (List<ActiveEdge> aet, int nextToProcess) = InitAET(points, colors, perm);

            int ymin = (int)Math.Round(points[perm[0]].Y);
            int ymax = (int)Math.Round(points[perm[points.Length - 1]].Y);

            for (int y = ymin + 1; y <= ymax; ++y) {
                HandleNewPoints(points, colors, perm, ref nextToProcess, aet, y);

                aet.Sort((ActiveEdge a, ActiveEdge b) => a.X.CompareTo(b.X));
                DrawScanline(plane, aet, y);
            }
        }
    }
}
