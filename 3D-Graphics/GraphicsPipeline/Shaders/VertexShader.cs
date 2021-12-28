using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public interface IVertexShader {
        Vector<float> Shade(Vector<float> v);
    }

    public class IdentityVertexShader : IVertexShader {
        public Vector<float> Shade(Vector<float> v) {
            return v;
        }
    }
}
