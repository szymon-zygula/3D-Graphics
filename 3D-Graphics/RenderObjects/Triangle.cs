using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public class Triangle {
        public Vector<float>[] Vertices;

        public Triangle() {
            Vertices = new Vector<float>[3];
        }
    }
}
