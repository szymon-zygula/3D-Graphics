using MathNet.Numerics.LinearAlgebra;
using System;

namespace _3D_Graphics {
    public class Triangle {
        public Vector<double>[] Vertices;
        public Vector<double>[] Normals;
        public Vec2[] TextureCoords;
        public IFragmentShader FragmentShader;

        public Triangle(IFragmentShader fragmentShader) {
            Vertices = new Vector<double>[3];
            Normals = new Vector<double>[3];
            TextureCoords = new Vec2[3];
            FragmentShader = fragmentShader;
        }

        public Vec3 ShadeAt(Vec3 bary) {
            return FragmentShader.Shade(this, bary);
        }

        public bool CorrectWinding() {
            Vec3 a = new Vec3(Vertices[1] - Vertices[0]);
            Vec3 b = new Vec3(Vertices[2] - Vertices[0]);
            Vec3 cross = Vec3.CrossProduct(a, b);
            return cross.Z < 0;
        }

        private bool VertexWithinVisibleWindow(double width, double height, int v, double closePlane) {
            return
                Vertices[v][0] >= 0 && Vertices[v][1] >= 0 &&
                Vertices[v][0] < width && Vertices[v][1] < height &&
                Vertices[v][2] >= closePlane;
        }

        public bool WithinVisibleWindow(double width, double height, double closePlane) {
            return
                VertexWithinVisibleWindow(width, height, 0, closePlane) ||
                VertexWithinVisibleWindow(width, height, 1, closePlane) ||
                VertexWithinVisibleWindow(width, height, 2, closePlane);
        }

        public Vector<double>[] VerticesSortedByY() {
            Vector<double>[] vertices = new Vector<double>[3];
            Array.Copy(Vertices, vertices, 3);

            if (vertices[0][1] > vertices[1][1]) {
                (vertices[0], vertices[1]) = (vertices[1], vertices[0]);
            }

            if (vertices[0][1] > vertices[2][1]) {
                (vertices[0], vertices[2]) = (vertices[2], vertices[0]);
            }

            if (vertices[1][1] > vertices[2][1]) {
                (vertices[1], vertices[2]) = (vertices[2], vertices[1]);
            }

            return vertices;
        }
    }
}
