using MathNet.Numerics.LinearAlgebra;
using System;

namespace _3D_Graphics {
    public struct Triangle {
        public Vector<float>[] Vertices;
        public IFragmentShader FragmentShader;

        public Triangle(IFragmentShader fragmentShader) {
            Vertices = new Vector<float>[3];
            FragmentShader = fragmentShader;
        }

        public Vec3 ShadeAt(Vec3 bary) {
            return FragmentShader.Shade(this, bary);
        }

        public Vector<float>[] VerticesSortedByY() {
            Vector<float>[] vertices = new Vector<float>[3];
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
