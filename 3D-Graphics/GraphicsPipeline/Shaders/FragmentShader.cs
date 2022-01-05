using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _3D_Graphics {
    public interface IFragmentShader {
        Vec3 Shade(Triangle triangle, Vec3 bary, Triangle unshaded);
    }

    public class WrapperFragmentShader : IFragmentShader {
        public IFragmentShader InnerShader { get; set; }
        public WrapperFragmentShader(IFragmentShader innerShader) {
            InnerShader = innerShader;
        }

        public Vec3 Shade(Triangle triangle, Vec3 bary, Triangle unshaded) {
            return InnerShader.Shade(triangle, bary, unshaded);
        }
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

        public Vec3 Shade(Triangle _triangle, Vec3 _bary, Triangle _unshaded) {
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

        public Vec3 Shade(Triangle _triangle, Vec3 bary, Triangle _unshaded) {
            return Color1 * bary.X + Color2 * bary.Y + Color3 * bary.Z;
        }
    }

    public class TextureFragmentShader : IFragmentShader {
        Texture Texture;

        public TextureFragmentShader(Texture texture) {
            Texture = texture;
        }

        public Vec3 Shade(Triangle triangle, Vec3 bary, Triangle _unshaded) {
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
        public LightList Lights;

        public GouraudFragmentShaderDecorator(IFragmentShader innerShader, LightList lightList) {
            InnerShader = innerShader;
            Lights = lightList;
        }

        public Vec3 Shade(Triangle triangle, Vec3 bary, Triangle unshaded) {
            Vector<double> normal =
                unshaded.Normals[0] * bary.X +
                unshaded.Normals[1] * bary.Y +
                unshaded.Normals[2] * bary.Z;

            Vec3 point = new Vec3(
                unshaded.Vertices[0] * bary.X +
                unshaded.Vertices[1] * bary.Y +
                unshaded.Vertices[2] * bary.Z);

            Vec3 color = new Vec3(0.0, 0.0, 0.0);
            foreach(Light light in Lights.Lights) {
                double intensity = -normal * light.GetDirectionTo(point).ToHomogenousDirection() * 2.0;
                if (intensity <= 0) continue;
                color += light.Color * intensity;
            }
            return Vec3.CoefficientProduct(color, InnerShader.Shade(triangle, bary, unshaded));
        }
    }

    public class FlatLightFragmentShaderDecorator : IFragmentShader {
        IFragmentShader InnerShader;
        public LightList Lights;

        public FlatLightFragmentShaderDecorator(IFragmentShader innerShader, LightList lights) {
            InnerShader = innerShader;
            Lights = lights;
        }

        public Vec3 Shade(Triangle triangle, Vec3 bary, Triangle unshaded) {
            Vector<double> normal = (
                unshaded.Normals[0] +
                unshaded.Normals[1] +
                unshaded.Normals[2]).Normalize(2);

            Vec3 point = new Vec3(
                unshaded.Vertices[0] +
                unshaded.Vertices[1] +
                unshaded.Vertices[2]
            ) / 3.0;

            Vec3 color = new Vec3(0.0, 0.0, 0.0);
            foreach(Light light in Lights.Lights) {
                double intensity = -normal * light.GetDirectionTo(point).ToHomogenousDirection() * 2.0;
                if (intensity <= 0) continue;
                color += light.Color * intensity;
            }
            return Vec3.CoefficientProduct(color, InnerShader.Shade(triangle, bary, unshaded));
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

        public Vec3 Shade(Triangle triangle, Vec3 bary, Triangle _unshaded) {
            Vector<double> interpol =
                triangle.Vertices[0] * bary.X +
                triangle.Vertices[1] * bary.Y +
                triangle.Vertices[2] * bary.Z;

            if (interpol[2] >= OpaqueDistance) {
                return Color;
            }
            else if (interpol[2] <= ClearDistance) {
                return InnerShader.Shade(triangle, bary, _unshaded);
            }

            double fc = (OpaqueDistance - interpol[2]) / (OpaqueDistance - ClearDistance);

            return fc * InnerShader.Shade(triangle, bary, _unshaded) + (1.0 - fc) * Color;
        }
    }
}
