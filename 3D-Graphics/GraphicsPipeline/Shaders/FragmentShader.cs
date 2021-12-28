using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Graphics {
    public interface IFragmentShader {
        Vec3 Shade(Triangle triangle, Vec3 bary);
    }

    public class FlatFragmentShader : IFragmentShader {
        readonly Vec3 Color;

        public FlatFragmentShader(Vec3 color) {
            Color = color;
        }

        public Vec3 Shade(Triangle triangle, Vec3 _bary) {
            return Color;
        }
    }
}
