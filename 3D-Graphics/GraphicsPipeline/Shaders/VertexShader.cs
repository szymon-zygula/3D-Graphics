using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public interface IVertexShader {
        Triangle Shade(Triangle triangle);
    }

    public class IdentityVertexShader : IVertexShader {
        public Triangle Shade(Triangle v) {
            return v;
        }
    }

    public class ProjectionVertexShader : IVertexShader {
        double Fov;
        double FarCull;
        double CloseCull;
        double Width;
        double Height;
        Matrix<double> ProjectionMatrix;

        public ProjectionVertexShader(double fov, double farCull, double closeCull, double width, double height) {
            Fov = fov;
            FarCull = farCull;
            CloseCull = closeCull;
            Width = width;
            Height = height;

            ProjectionMatrix = CreateMatrix.DenseOfArray(new double[4, 4] {
                { 1.0 / ((double)Math.Tan(Fov / 2.0) * (Width / Height)), 0.0, 0.0, 0.0 },
                { 0.0, 1.0 / (double)Math.Tan(Fov / 2), 0.0, 0.0 },
                { 0.0, 0.0, (FarCull + CloseCull) / (FarCull - CloseCull), -2.0 * FarCull * CloseCull / (FarCull - CloseCull) },
                { 0.0, 0.0, 1.0, 0.0 }
            });
        }

        public Triangle Shade(Triangle triangle) {
            Triangle projectedTriangle = new Triangle(triangle.FragmentShader);

            projectedTriangle.Vertices[0] = PerspectiveVector(triangle.Vertices[0]);
            projectedTriangle.Vertices[1] = PerspectiveVector(triangle.Vertices[1]);
            projectedTriangle.Vertices[2] = PerspectiveVector(triangle.Vertices[2]);

            projectedTriangle.Vertices[0] = DisplayPerspective(projectedTriangle.Vertices[0]);
            projectedTriangle.Vertices[1] = DisplayPerspective(projectedTriangle.Vertices[1]);
            projectedTriangle.Vertices[2] = DisplayPerspective(projectedTriangle.Vertices[2]);

            return projectedTriangle;
        }

        private Vector<double> PerspectiveVector(Vector<double> v) {
            Vector<double> Vc = ProjectionMatrix * MatrixUtils.TranslateMatrix(new Vec3(0.0, 0.0, 2.0)) * v;
            return Vc / Vc[3];
        }

        private Vector<double> DisplayPerspective(Vector<double> v) {
            return CreateVector.DenseOfArray(new double[4] {
                Width * (1.0 + v[0]) / 2.0,
                Height * (1.0 - v[1]) / 2.0,
                v[2],
                1.0
            });
        }
    }
}
