using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Graphics {
    public class Entity {
        public Triangle[] Triangles;

        public Entity(int triangleCount) {
            Triangles = new Triangle[triangleCount];
        }
    }
}
