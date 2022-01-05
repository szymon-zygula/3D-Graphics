using System;

namespace _3D_Graphics {
    public static class MathUtils {
        private static Random Rnd = new Random();

        public static double Random(double min = 0.0, double max = 1.0) {
            return Rnd.NextDouble() * (max - min) + min;
        }

        public static double Clamp(double value, double min, double max) {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static int Mod(int num, int basis) {
            int r = num % basis;
            return r < 0 ? r + basis : r;
        }
    }
}
