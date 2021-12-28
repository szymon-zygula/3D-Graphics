using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Graphics {
    public interface IFragmentShader {
        Vec3 Shade(Vec2 p1, Vec2 p2, Vec2 p3);
    }

    public class SolidFragmentShader : IFragmentShader {
        readonly Vec3 Color;

        public SolidFragmentShader(Vec3 color) {
            Color = color;
        }

        public Vec3 Shade(Vec2 _p1, Vec2 _p2, Vec2 _p3) {
            return Color;
        }
    }
}
