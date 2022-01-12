using System;

namespace _3D_Graphics {
    public class ReflectorLight : Light {
        public Vec3 Position;
        public double MaxViewAngle;
        private Vec3 _Direction;
        private double Multiplier;

        public Vec3 Direction {
            get {
                return _Direction;
            }
            set {
                _Direction = value.Normalize();
            }
        }

        public ReflectorLight(Vec3 position, Vec3 direction, double maxViewAngle, Vec3 color, double multiplier = 1.0) : base(color) {
            Position = position;
            Direction = direction;
            MaxViewAngle = maxViewAngle;
            Multiplier = multiplier;
        }

        public override Vec3 GetDirectionTo(Vec3 point) {
            Vec3 toPoint = (point - Position).Normalize();

            if(Vec3.DotProduct(toPoint, Direction) >= Math.Cos(0.5 * MaxViewAngle)) {
                return toPoint * Math.Pow(Vec3.DotProduct(toPoint, Direction), 30.0 / MaxViewAngle * Math.PI * Multiplier) * 1.0;
            }

            return new Vec3(0.0, 0.0, 0.0);
        }
    }
}
