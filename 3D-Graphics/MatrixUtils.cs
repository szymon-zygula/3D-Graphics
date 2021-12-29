using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public static class MatrixUtils {
        public static Matrix<double> TranslateMatrix(Vec3 v) {
            return CreateMatrix.DenseOfArray(new double[4, 4] {
                { 1.0, 0.0, 0.0, v.X },
                { 0.0, 1.0, 0.0, v.Y },
                { 0.0, 0.0, 1.0, v.Z },
                { 0.0, 0.0, 0.0, 1.0 }
            });
        }
    }
}
