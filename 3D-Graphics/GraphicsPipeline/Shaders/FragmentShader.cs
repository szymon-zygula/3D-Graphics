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

    public class InterpolationFragmentShader : IFragmentShader {
        Vec3 Color1;
        Vec3 Color2;
        Vec3 Color3;

        public InterpolationFragmentShader(Vec3 color1, Vec3 color2, Vec3 color3) {
            Color1 = color1;
            Color2 = color2;
            Color3 = color3;
        }

        public Vec3 Shade(Triangle triangle, Vec3 bary) {
            return (Color1 * bary.X + Color2 * bary.Y + Color3 * bary.Z);
        }
    }
}
