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
            ScanLineFiller.FillTriangle(texture, points, fragmentShader);
        }
    }

    public static class ScanLineFiller {
        private class ActiveEdge {
            public int From;
            public int To;
            public float X;
            public float Diff;
        }

        private static float Differential(Vec2[] points, int i, int j) {
            if (points[i].Y == points[j].Y) {
                return float.NaN;
            }

            return
                (float)(points[j].X - points[i].X) /
                (float)(points[j].Y - points[i].Y);
        }

        private static void AddToAET(List<ActiveEdge> aet, ActiveEdge ae) {
            if (!float.IsNaN(ae.Diff)) {
                aet.Add(ae);
            }
        }

        private static void AddTopPointToAET(Vec2[] points, int point, List<ActiveEdge> aet) {
            float x = points[point].X;
            int nextPoint = (point + 1) % points.Length;
            int prevPoint = point == 0 ? points.Length - 1 : point - 1;

            AddToAET(aet, new ActiveEdge() {
                X = x,
                Diff = Differential(points, point, nextPoint),
                From = point,
                To = nextPoint,
            });

            AddToAET(aet, new ActiveEdge() {
                X = x,
                Diff = Differential(points, point, prevPoint),
                From = point,
                To = prevPoint,
            });
        }

        private static (List<ActiveEdge>, int) InitAET(Vec2[] points, int[] perm) {
            List<ActiveEdge> aet = new List<ActiveEdge>();

            for (int i = 0; i < points.Length; ++i) {
                if (points[perm[i]].Y != points[perm[0]].Y) {
                    return (aet, i);
                }

                AddTopPointToAET(points, perm[i], aet);
            }

            return (aet, points.Length);
        }

        private static void HandleNewEdge(Vec2[] points, List<ActiveEdge> aet, int i, int j) {
            if (points[j].Y >= points[i].Y) {
                AddToAET(aet, new ActiveEdge() {
                    X = points[i].X,
                    Diff = Differential(points, i, j),
                    From = i,
                    To = j,
                });
            }
            else {
                aet.RemoveAll((ActiveEdge ae) => ae.To == i && ae.From == j);
            }
        }

        private static void DrawScanline(Texture plane, List<ActiveEdge> aet, int y, IFragmentShader fragmentShader) {
            for (int i = 0; i < aet.Count - 1; i += 2) {
                ActiveEdge ae1 = aet[i];
                ActiveEdge ae2 = aet[i + 1];

                for (int x = (int)Math.Round(ae1.X); x <= (int)Math.Round(ae2.X); ++x) {
                    plane.Pixels[x, y] = fragmentShader.Shade(new Vec2(0.0f, 0.0f), new Vec2(0.0f, 0.0f), new Vec2(0.0f, 0.0f));
                }

                ae1.X += ae1.Diff;
                ae2.X += ae2.Diff;
            }
        }

        private static void HandleNewPoints(Vec2[] points, int[] perm, ref int nextToProcess, List<ActiveEdge> aet, int y) {
            while ((int)Math.Round(points[perm[nextToProcess]].Y) == y - 1) {
                int nextPoint = (perm[nextToProcess] + 1) % points.Length;
                int prevPoint = perm[nextToProcess] == 0 ? points.Length - 1 : perm[nextToProcess] - 1;

                HandleNewEdge(points, aet, perm[nextToProcess], prevPoint);
                HandleNewEdge(points, aet, perm[nextToProcess], nextPoint);

                nextToProcess += 1;
            }
        }

        public static void FillTriangle(Texture plane, Vec2[] points, IFragmentShader fragmentShader) {
            int[] perm = Enumerable.Range(0, points.Length).ToArray();
            Array.Sort(perm, (int a, int b) => points[a].Y.CompareTo(points[b].Y));
            (List<ActiveEdge> aet, int nextToProcess) = InitAET(points, perm);

            int ymin = (int)Math.Round(points[perm[0]].Y);
            int ymax = (int)Math.Round(points[perm[points.Length - 1]].Y);

            for (int y = ymin + 1; y <= ymax; ++y) {
                HandleNewPoints(points, perm, ref nextToProcess, aet, y);

                aet.Sort((ActiveEdge a, ActiveEdge b) => a.X.CompareTo(b.X));
                DrawScanline(plane, aet, y, fragmentShader);
            }
        }
    }
}
