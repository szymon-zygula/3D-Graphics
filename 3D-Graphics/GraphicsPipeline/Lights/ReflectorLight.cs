using System;

namespace _3D_Graphics {
    public class ReflectorLight : Light {
        public Vec3 Position;
        public double ViewAngle;
        private Vec3 _Direction;
        public Vec3 Direction {
            get {
                return _Direction;
            }
            set {
                _Direction = value.Normalize();
            }
        }

        public ReflectorLight(Vec3 position, Vec3 direction, double viewAngle, Vec3 color) : base(color) {
            Position = position;
            Direction = direction;
            ViewAngle = viewAngle;
        }

        public override Vec3 GetDirectionTo(Vec3 point) {
            Vec3 toPoint = (point - Position).Normalize();

            if(Math.Acos(Vec3.DotProduct(toPoint, Direction)) <= 0.5 * ViewAngle) {
                return toPoint;
            }

            return new Vec3(0.0, 0.0, 0.0);
        }
    }
}
