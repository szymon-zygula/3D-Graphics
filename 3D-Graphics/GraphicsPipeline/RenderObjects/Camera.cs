using System;
using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public class Camera {
        public double Fov;
        public double ClosePlane;
        public double FarPlane;
        public Vec3 ObservedPoint;
        public Vec3 Position;
        public Vec3 Up;

        public Matrix<double> ViewMatrix { get; private set; }
        public Matrix<double> ProjectionMatrix { get; private set; }
        public Matrix<double> InvTransProjectionMatrix { get; private set; }

        public void UpdateViewMatrix() {
            Vec3 outOfCameraDir = (ObservedPoint - Position).Normalize();
            Vec3 rightDir = Vec3.CrossProduct(Up, outOfCameraDir).Normalize();
            Vec3 upCamera = Vec3.CrossProduct(outOfCameraDir, rightDir);

            ViewMatrix = CreateMatrix.DenseOfArray(new double[4, 4] {
                { rightDir.X, rightDir.Y, rightDir.Z, 0.0 },
                { upCamera.X, upCamera.Y, upCamera.Z, 0.0 },
                { outOfCameraDir.X, outOfCameraDir.Y, outOfCameraDir.Z, 0.0 },
                { 0.0, 0.0, 0.0, 1.0 }
            }) * CreateMatrix.DenseOfArray(new double[4, 4] {
                { 1.0, 0.0, 0.0, -Position.X },
                { 0.0, 1.0, 0.0, -Position.Y },
                { 0.0, 0.0, 1.0, -Position.Z },
                { 0.0, 0.0, 0.0, 1.0 }
            });
        }

        public void UpdateProjectionMatrix(double aspect) {
            ProjectionMatrix = CreateMatrix.DenseOfArray(new double[4, 4] {
                { 1.0 / ((double)Math.Tan(Fov / 2.0) * aspect), 0.0, 0.0, 0.0 },
                { 0.0, 1.0 / (double)Math.Tan(Fov / 2), 0.0, 0.0 },
                { 0.0, 0.0, (FarPlane + ClosePlane) / (FarPlane - ClosePlane), -2.0 * FarPlane * ClosePlane / (FarPlane - ClosePlane) },
                { 0.0, 0.0, 1.0, 0.0 }
            });

            InvTransProjectionMatrix = ProjectionMatrix.Inverse().Transpose();
        }
    }
}
