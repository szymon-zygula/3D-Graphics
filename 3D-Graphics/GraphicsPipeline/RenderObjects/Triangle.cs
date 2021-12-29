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
