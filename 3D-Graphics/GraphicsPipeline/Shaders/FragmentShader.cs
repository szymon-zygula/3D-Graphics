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

        public static FlatFragmentShader Random() {
            return new FlatFragmentShader(Vec3.Random());
        }

        public static FlatFragmentShader RandomToned(Vec3 color) {
            return new FlatFragmentShader(color * MathUtils.Random(0.8, 1.0));
        }

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

            int u = (int)MathUtils.Clamp(Math.Round(uv.X * (Texture.Width - 1)), 0.0, Texture.Width - 1);
            int v = (int)MathUtils.Clamp(Math.Round((1.0 - uv.Y) * (Texture.Height - 1)), 0.0, Texture.Height - 1);
            return Texture.Pixels[u, v];
        }
    }

    public class GouraudFragmentShaderDecorator : IFragmentShader {
        IFragmentShader InnerShader;
        public Vec3 Direction;

        public GouraudFragmentShaderDecorator(IFragmentShader innerShader, Vec3 direction) {
            InnerShader = innerShader;
            Direction = direction;
        }

        public Vec3 Shade(Triangle triangle, Vec3 bary) {
            Vector<double> normal =
                triangle.Normals[0] * bary.X +
                triangle.Normals[1] * bary.Y +
                triangle.Normals[2] * bary.Z;

            double intensity = -normal * Direction.ToHomogenousDirection() * 2.0;
            return intensity * InnerShader.Shade(triangle, bary);
        }
    }

    public class FlatLightFragmentShaderDecorator : IFragmentShader {
        IFragmentShader InnerShader;
        public Vec3 Direction;

        public FlatLightFragmentShaderDecorator(IFragmentShader innerShader, Vec3 direction) {
            InnerShader = innerShader;
            Direction = direction;
        }

        public Vec3 Shade(Triangle triangle, Vec3 bary) {
            Vector<double> normal = (
                triangle.Normals[0] +
                triangle.Normals[1] +
                triangle.Normals[2]).Normalize(2);

            double intensity = -normal * Direction.ToHomogenous() * 2.0;
            return intensity * InnerShader.Shade(triangle, bary);
        }
    }

    public class FogFragmentShaderDecorator : IFragmentShader {
        IFragmentShader InnerShader;
        Vec3 Color;
        double OpaqueDistance;
        double ClearDistance;

        public FogFragmentShaderDecorator(IFragmentShader innerShader, Vec3 color, double opaqueDistance, double clearDistance) {
            InnerShader = innerShader;
            Color = color;
            OpaqueDistance = opaqueDistance;
            ClearDistance = clearDistance;
        }

        public Vec3 Shade(Triangle triangle, Vec3 bary) {
            Vector<double> interpol =
                triangle.Vertices[0] * bary.X +
                triangle.Vertices[1] * bary.Y +
                triangle.Vertices[2] * bary.Z;

            if (interpol[2] >= OpaqueDistance) {
                return Color;
            }
            else if (interpol[2] <= ClearDistance) {
                return InnerShader.Shade(triangle, bary);
            }

            double fc = (OpaqueDistance - interpol[2]) / (OpaqueDistance - ClearDistance);

            return fc * InnerShader.Shade(triangle, bary) + (1.0 - fc) * Color;
        }
    }
}
