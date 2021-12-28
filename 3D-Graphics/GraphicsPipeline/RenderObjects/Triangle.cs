using MathNet.Numerics.LinearAlgebra;

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

        public void SortVerticesByY() {
            if (Vertices[0][1] > Vertices[1][1]) {
                (Vertices[0], Vertices[1]) = (Vertices[1], Vertices[0]);
            }
            if (Vertices[0][1] > Vertices[2][1]) {
                (Vertices[0], Vertices[2]) = (Vertices[2], Vertices[0]);
            }
            if (Vertices[1][1] > Vertices[2][1]) {
                (Vertices[1], Vertices[2]) = (Vertices[2], Vertices[1]);
            }
        }
    }
}
