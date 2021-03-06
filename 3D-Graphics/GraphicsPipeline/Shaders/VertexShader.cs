using System;
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

    public class TransformVertexShader : IVertexShader {
        Matrix<double> Transform;
        Matrix<double> InvTransTransform;

        public TransformVertexShader(Matrix<double> transform) {
            Transform = transform;
            InvTransTransform = transform.Inverse().Transpose();
        }

        public Triangle Shade(Triangle triangle) {
            Triangle transformedTriangle = new Triangle(triangle.FragmentShader);
            transformedTriangle.TextureCoords = triangle.TextureCoords;

            transformedTriangle.Vertices[0] = Transform * triangle.Vertices[0];
            transformedTriangle.Vertices[1] = Transform * triangle.Vertices[1];
            transformedTriangle.Vertices[2] = Transform * triangle.Vertices[2];

            transformedTriangle.Normals[0] = InvTransTransform * triangle.Normals[0];
            transformedTriangle.Normals[1] = InvTransTransform * triangle.Normals[1];
            transformedTriangle.Normals[2] = InvTransTransform * triangle.Normals[2];

            return transformedTriangle;
        }
    }

    public class ProjectionVertexShader : IVertexShader {
        Camera Camera;
        double Width;
        double Height;

        public ProjectionVertexShader(Camera camera, double width, double height) {
            Width = width;
            Height = height;
            Camera = camera;
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
            projectedTriangle.Normals[0] = PerspectiveNormal(triangle.Normals[0]);
            projectedTriangle.Normals[0][3] = 0;
            projectedTriangle.Normals[1] = PerspectiveNormal(triangle.Normals[1]);
            projectedTriangle.Normals[1][3] = 0;
            projectedTriangle.Normals[2] = PerspectiveNormal(triangle.Normals[2]);
            projectedTriangle.Normals[2][3] = 0;

            return projectedTriangle;
        }

        private Vector<double> PerspectiveVector(Vector<double> v) {
            Vector<double> Vc = Camera.ProjectionMatrix * Camera.ViewMatrix * v;
            return Vc / Vc[3];
        }
        private Vector<double> PerspectiveNormal(Vector<double> n) {
            Vector<double> Nc = Camera.InvTransProjectionMatrix * n;
            //Nc[2] = Nc[3];
            //Nc[3] = 0;
            return Nc;
        }

        private Vector<double> DisplayPerspective(Vector<double> v) {
            return CreateVector.DenseOfArray(new double[4] {
                Math.Round(Width * (1.0 + v[0]) / 2.0),
                Math.Round(Height * (1.0 - v[1]) / 2.0),
                v[2],
                1.0
            });
        }
    }
}
