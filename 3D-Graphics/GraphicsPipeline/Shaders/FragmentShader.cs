using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Graphics {
    public interface IFragmentShader {
        Vec3 Shade(Triangle triangle);
    }

    public class SolidFragmentShader : IFragmentShader {
        readonly Vec3 Color;

        public SolidFragmentShader(Vec3 color) {
            Color = color;
        }

        public Vec3 Shade(Triangle triangle) {
            return Color;
        }
    }
}
