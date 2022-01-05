using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Graphics {
    public class ReflectorLight : Light {
        public Vec3 Position;
        public Vec3 Direction;
        public double ViewAngle;

        public ReflectorLight(Vec3 position, Vec3 direction, double viewAngle, Vec3 color) : base(color) {
            Position = position;
            Direction = direction;
            ViewAngle = viewAngle;
        }

        public override Vec3 GetDirectionTo(Vec3 point) {
            return (point - Direction).Normalize();
        }
    }
}
