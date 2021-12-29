using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Graphics {
    public static class MathUtils {
        public static double Clamp(double value, double min, double max) {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
