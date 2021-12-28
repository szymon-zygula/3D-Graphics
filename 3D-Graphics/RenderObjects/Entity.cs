using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Graphics {
    public class Entity {
        public IFragmentShader FragmentShader;
        public Triangle[] Triangles;

        public Entity(IFragmentShader fragmentShader) {
            Triangles = new Triangle[0];
            FragmentShader = fragmentShader;
        }
    }
}
