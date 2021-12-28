using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public struct Triangle {
        public Vector<float>[] Vertices;
        public IFragmentShader FragmentShader;

        public Triangle(IFragmentShader fragmentShader) {
            Vertices = new Vector<float>[3];
            FragmentShader = fragmentShader;
        }
    }
}
