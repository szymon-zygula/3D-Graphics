using MathNet.Numerics.LinearAlgebra;
using System;
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

    public class PhongFragmentShaderDecorator : IFragmentShader {
        IFragmentShader InnerShader;
        public LightList Lights;
        public Vec3 AmbientLight;
        public double SpecularComponent;
        public double DiffuseComponent;
        public double ShininessExponent;
        public Camera Camera;
        public Texture NormalMap;
        Matrix<double> A;
        Vector<double> V;

        public PhongFragmentShaderDecorator(
            IFragmentShader innerShader,
            LightList lightList,
            Vec3 ambientLight,
            double specularComponent,
            double diffuseComponent,
            double shininessExponent,
            Camera camera
        ) {
            InnerShader = innerShader;
            Lights = lightList;
            AmbientLight = ambientLight;
            SpecularComponent = specularComponent;
            DiffuseComponent = diffuseComponent;
            ShininessExponent = shininessExponent;
            Camera = camera;
        }

        public PhongFragmentShaderDecorator(
            IFragmentShader innerShader,
            LightList lightList,
            Vec3 ambientLight,
            double specularComponent,
            double diffuseComponent,
            double shininessExponent,
            Camera camera,
            Texture normalMap
        ) : this(innerShader, lightList, ambientLight, specularComponent, diffuseComponent, shininessExponent, camera) {
            NormalMap = normalMap;
            A = CreateMatrix.Dense<double>(3, 3);
            V = CreateVector.Dense<double>(3);
            V[2] = 0.0;
        }

        private Vec3 Normal(Triangle triangle, Vec3 bary, Triangle unshaded) {
            Vector<double> normal =
                    (unshaded.Normals[0] * bary.X +
                    unshaded.Normals[1] * bary.Y +
                    unshaded.Normals[2] * bary.Z).SubVector(0, 3);

            if(NormalMap == null) {
                return new Vec3(normal);
            }

            Vec2 uv =
                triangle.TextureCoords[0] * bary.X +
                triangle.TextureCoords[1] * bary.Y +
                triangle.TextureCoords[2] * bary.Z;

            int u = (int)MathUtils.Clamp(Math.Round(uv.X * (NormalMap.Width - 1)), 0.0, NormalMap.Width - 1);
            int v = (int)MathUtils.Clamp(Math.Round((1.0 - uv.Y) * (NormalMap.Height - 1)), 0.0, NormalMap.Height - 1);
            Vec3 nfm = 2.0 * (NormalMap.Pixels[u, v] - new Vec3(0.5, 0.5, 0.5));

            Vector<double> p0p1 = (unshaded.Vertices[1] - unshaded.Vertices[0]).SubVector(0, 3);
            Vector<double> p0p2 = (unshaded.Vertices[2] - unshaded.Vertices[0]).SubVector(0, 3);

            A.SetRow(0, p0p1);
            A.SetRow(1, p0p2);
            A.SetRow(2, normal);
            Matrix<double> invA = A.Inverse();

            V[0] = triangle.TextureCoords[1].X - triangle.TextureCoords[0].X;
            V[1] = triangle.TextureCoords[2].X - triangle.TextureCoords[0].X;
            Vector<double> i = (invA * V).Normalize(2);
            V[0] = triangle.TextureCoords[1].Y - triangle.TextureCoords[0].Y;
            V[1] = triangle.TextureCoords[2].Y - triangle.TextureCoords[0].Y;
            Vector<double> j = (invA * V).Normalize(2);

            return new Vec3(i * nfm.X + j * nfm.Y + normal * nfm.Z).Normalize();
        }

        public Vec3 Shade(Triangle triangle, Vec3 bary, Triangle unshaded) {
            Vec3 normal = Normal(triangle, bary, unshaded);

            Vec3 point = new Vec3(
                unshaded.Vertices[0] * bary.X +
                unshaded.Vertices[1] * bary.Y +
                unshaded.Vertices[2] * bary.Z
            );

            Vec3 color = new Vec3(0.0, 0.0, 0.0);
            foreach(Light light in Lights.Lights) {
                double diffuse = DiffuseComponent * Vec3.DotProduct(light.GetDirectionFrom(point), normal);

                double specDot = Vec3.DotProduct(light.GetReflectionUnitVector(point, normal), Vec3.UnitDirection(point, Camera.Position));
                double specular;
                if (specDot < 0 || double.IsNaN(specDot)) {
                    specular = 0;
                }
                else {
                    specular = SpecularComponent * Math.Pow(specDot, ShininessExponent + 0.0001);
                }

                diffuse = Math.Max(0.0, diffuse);
                specular = Math.Max(0.0, specular);
                color += light.Color * (diffuse + specular);
            }

            color += AmbientLight;
            return Vec3.CoefficientProduct(color, InnerShader.Shade(triangle, bary, unshaded));
        }
    }
}
