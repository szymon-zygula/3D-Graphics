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
        Matrix<double> ProjectionMatrix;
        Camera Camera;
        double Width;
        double Height;

        public ProjectionVertexShader(Camera camera, double width, double height) {
            Width = width;
            Height = height;
            Camera = camera;

            ProjectionMatrix = camera.ProjectionMatrix(width / height);
        }

        public Triangle Shade(Triangle triangle) {
            Triangle projectedTriangle = new Triangle(triangle.FragmentShader);

            projectedTriangle.Vertices[0] = PerspectiveVector(triangle.Vertices[0]);
            projectedTriangle.Vertices[1] = PerspectiveVector(triangle.Vertices[1]);
            projectedTriangle.Vertices[2] = PerspectiveVector(triangle.Vertices[2]);

            projectedTriangle.Vertices[0] = DisplayPerspective(projectedTriangle.Vertices[0]);
            projectedTriangle.Vertices[1] = DisplayPerspective(projectedTriangle.Vertices[1]);
            projectedTriangle.Vertices[2] = DisplayPerspective(projectedTriangle.Vertices[2]);

            projectedTriangle.TextureCoords = triangle.TextureCoords;

            return projectedTriangle;
        }

        private Vector<double> PerspectiveVector(Vector<double> v) {
            Vector<double> Vc = ProjectionMatrix * Camera.ViewMatrix() * v;
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
