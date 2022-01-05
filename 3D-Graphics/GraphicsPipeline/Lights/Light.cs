namespace _3D_Graphics {
    public abstract class Light {
        public Vec3 Color;

        protected Light(Vec3 color) {
            Color = color;
        }

        public abstract Vec3 GetDirectionTo(Vec3 point);

        public Vec3 GetDirectionFrom(Vec3 point) {
            return -GetDirectionTo(point);
        }

        public Vec3 GetReflectionUnitVector(Vec3 point, Vec3 normal) {
            Vec3 lightDirection = GetDirectionFrom(point);
            return 2.0 * Vec3.DotProduct(lightDirection, normal) * normal - lightDirection;
        }
    }
}
