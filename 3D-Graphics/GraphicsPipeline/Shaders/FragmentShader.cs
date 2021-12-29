using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            return Color1 * bary.X + Color2 * bary.Y + Color3 * bary.Z;
        }
    }

    public class TextureFragmentShader : IFragmentShader {
        Texture Texture;

        public TextureFragmentShader(Texture texture) {
            Texture = texture;
        }

        public Vec3 Shade(Triangle triangle, Vec3 bary) {
            Vec2 uv =
                triangle.TextureCoords[0] * bary.X +
                triangle.TextureCoords[1] * bary.Y +
                triangle.TextureCoords[2] * bary.Z;

            int u = (int)Math.Max(0.0, Math.Min(Math.Round(uv.X * (Texture.Width - 1)), Texture.Width - 1));
            int v = (int)Math.Max(0.0, Math.Min(Math.Round((1.0 - uv.Y) * (Texture.Height - 1)), Texture.Height - 1));
            return Texture.Pixels[u, v];
        }
    }

    public class GouraudFragmentShaderDecorator : IFragmentShader {
        IFragmentShader InnerShader;
        Vec3 Direction;

        public GouraudFragmentShaderDecorator(IFragmentShader innerShader, Vec3 direction) {
            InnerShader = innerShader;
            Direction = direction;
        }

        public Vec3 Shade(Triangle triangle, Vec3 bary) {
            Vector<double> normal =
                triangle.Normals[0] * bary.X +
                triangle.Normals[1] * bary.Y +
                triangle.Normals[2] * bary.Z;
            Vec3 normalVec3 = new Vec3(normal);
            double intensity = Vec3.DotProduct(normalVec3, Direction) * 2.0;

            return intensity * InnerShader.Shade(triangle, bary);
        }
    }
}
