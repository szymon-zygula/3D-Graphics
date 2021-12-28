using System;
using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public struct Vec2 {
        static Random Rnd = new Random();
        public float X, Y;

        public Vec2(Vector<float> mathV) {
            X = mathV[0];
            Y = mathV[1];
        }

        public Vec2(float x, float y) {
            X = x;
            Y = y;
        }

        public float Length() {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public Vec2 Normalize() {
            return this / Length();
        }

        public static Vec2 RandomNormal() {
            Vec2 u = new Vec2((float)Rnd.NextDouble() * 2.0f - 1.0f, (float)Rnd.NextDouble() * 2.0f - 1.0f);
            return u.Normalize();
        }

        public static Vec2 operator +(Vec2 u) => u;
        public static Vec2 operator -(Vec2 u) => new Vec2(-u.X, -u.Y);
        public static Vec2 operator +(Vec2 u, Vec2 v) => new Vec2(u.X + v.X, u.Y + v.Y);
        public static Vec2 operator -(Vec2 u, Vec2 v) => new Vec2(u.X - v.X, u.Y - v.Y);
        public static Vec2 operator *(Vec2 u, float a) => new Vec2(a * u.X, a * u.Y);
        public static Vec2 operator *(float a, Vec2 u) => u * a;
        public static Vec2 operator /(Vec2 u, float a) => new Vec2(u.X / a, u.Y / a);
    }

    public struct Vec3 {
        static Random Rnd = new Random();
        public float X, Y, Z;

        public Vec3(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3(Vector<float> mathV) {
            X = mathV[0];
            Y = mathV[1];
            Z = mathV[2];
        }

        public Vec3(UInt32 color) {
            X = (float)((color & 0x00FF0000) >> 16) / 255.0f;
            Y = (float)((color & 0x0000FF00) >> 8) / 255.0f;
            Z = (float)(color & 0x000000FF) / 255.0f;
        }

        public UInt32 ToColor() {
            UInt32 a = 0xFF000000;
            UInt32 r = (UInt32)(Math.Min(Math.Round(X * 255.0), 255.0)) << 16;
            UInt32 g = (UInt32)(Math.Min(Math.Round(Y * 255.0), 255.0)) << 8;
            UInt32 b = (UInt32)(Math.Min(Math.Round(Z * 255.0), 255.0));

            return a | r | g | b;
        }

        public float Length() {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vec3 Normalize() {
            return this / Length();
        }

        public static Vec3 RandomNormal() {
            Vec3 u = new Vec3(
                (float)Rnd.NextDouble() * 2.0f - 1.0f,
                (float)Rnd.NextDouble() * 2.0f - 1.0f,
                (float)Rnd.NextDouble() * 2.0f - 1.0f
            );
            return u.Normalize();
        }

        public static float AngleCosine(Vec3 u, Vec3 v) {
            return DotProduct(u, v) / (u.Length() * v.Length());
        }

        public static float DotProduct(Vec3 u, Vec3 v) {
            return u.X * v.X + u.Y * v.Y + u.Z * v.Z;
        }

        public static Vec3 UnitDirection(Vec3 from, Vec3 to) {
            return (to - from).Normalize();
        }

        public static Vec3 CrossProduct(Vec3 u, Vec3 v) {
            return new Vec3(
                u.Y * v.Z - u.Z * v.Y,
                u.Z * v.X - u.X * v.Z,
                u.X * v.Y - u.Y * v.X
            );
        }

        public static Vec3 operator +(Vec3 u) => u;
        public static Vec3 operator -(Vec3 u) => new Vec3(-u.X, -u.Y, -u.Z);
        public static Vec3 operator +(Vec3 u, Vec3 v) => new Vec3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);
        public static Vec3 operator -(Vec3 u, Vec3 v) => new Vec3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);
        public static Vec3 operator *(Vec3 u, float a) => new Vec3(a * u.X, a * u.Y, a * u.Z);
        public static Vec3 operator *(float a, Vec3 u) => u * a;
        public static Vec3 operator /(Vec3 u, float a) => new Vec3(u.X / a, u.Y / a, u.Z / a);
    }
}
