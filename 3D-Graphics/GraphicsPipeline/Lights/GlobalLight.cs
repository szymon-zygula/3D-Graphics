using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Graphics {
    public class GlobalLight : Light {
        private Vec3 _Direction;
        public Vec3 Direction {
            get {
                return _Direction;
            }
            set {
                _Direction = value.Normalize();
            }
        }

        public GlobalLight(Vec3 direction, Vec3 color) : base(color) {
            Direction = direction;
        }

        public override Vec3 GetDirectionTo(Vec3 point) {
            return Direction;
        }
    }
}
