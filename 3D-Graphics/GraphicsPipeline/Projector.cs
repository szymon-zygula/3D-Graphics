using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Graphics {
    public static class Projector {
        public static Scene Project(Scene scene, IVertexShader vShader) {
            Scene projectedScene = new Scene();
            projectedScene.Entities = new Entity[scene.Entities.Length];
            for (int i = 0; i < scene.Entities.Length; ++i) {
                projectedScene.Entities[i] = new Entity(scene.Entities[i].FragmentShader);
                projectedScene.Entities[i].Triangles = new Triangle[scene.Entities[i].Triangles.Length];
                for(int j = 0; j < scene.Entities[i].Triangles.Length; ++j) {
                    projectedScene.Entities[i].Triangles[j].Vertices[0] = vShader.Shade(scene.Entities[i].Triangles[j].Vertices[0]);
                    projectedScene.Entities[i].Triangles[j].Vertices[1] = vShader.Shade(scene.Entities[i].Triangles[j].Vertices[1]);
                    projectedScene.Entities[i].Triangles[j].Vertices[2] = vShader.Shade(scene.Entities[i].Triangles[j].Vertices[2]);
                }
            }

            return projectedScene;
        }
    }
}
