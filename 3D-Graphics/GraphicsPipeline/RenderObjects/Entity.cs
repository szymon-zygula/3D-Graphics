using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using System.Windows;

namespace _3D_Graphics {
    public class Entity {
        public Triangle[] Triangles;
        public Texture Texture;

        public Entity(int triangleCount) {
            Triangles = new Triangle[triangleCount];
        }

        public Entity(string modelPath, string texturePath) {
            List<string[]> lines = new List<string[]>();
            List<int[]> faces = new List<int[]>();
            List<double[]> vertices = new List<double[]>();
            List<double[]> normals = new List<double[]>();
            List<Vec2> textureCoords = new List<Vec2>();

            foreach(string line in File.ReadLines(modelPath)) {
                string[] parsed = line.Split(' ', '/');
                if(parsed.Length == 0 || parsed[0].Length == 0 || parsed[0][0] == '#') {
                    continue;
                }

                lines.Add(parsed);

                switch(parsed[0]) {
                    case "f":
                        faces.Add(new int[] {
                            int.Parse(parsed[1]), int.Parse(parsed[2]), int.Parse(parsed[3]),
                            int.Parse(parsed[4]), int.Parse(parsed[5]), int.Parse(parsed[6]),
                            int.Parse(parsed[7]), int.Parse(parsed[8]), int.Parse(parsed[9]),
                        });
                        break;
                    case "v":
                        vertices.Add(new double[] {
                            double.Parse(parsed[1]), double.Parse(parsed[2]), double.Parse(parsed[3])
                        });
                        break;
                    case "vt":
                        textureCoords.Add(new Vec2 (
                            double.Parse(parsed[2]),
                            double.Parse(parsed[3])
                        ));
                        break;
                    case "vn":
                        normals.Add(new double[] {
                            double.Parse(parsed[2]), double.Parse(parsed[3]), double.Parse(parsed[4])
                        });
                        break;
                    case "g":
                        break;
                    case "s":
                        break;
                    default:
                        //throw new InvalidDataException("Model data is in invalid format");
                        break;
                }
            }
            TextureFragmentShader textureShader = new TextureFragmentShader(new Texture(new System.Drawing.Bitmap(texturePath)));
            Triangles = new Triangle[faces.Count];

            // Z negowany by przejsc do ukladu prawoskretnego
            for (int i = 0; i < faces.Count; ++i) {
                Triangles[i] = new Triangle(new GouraudFragmentShaderDecorator(textureShader, new Vec3(-1, 1.5, 0)));

                Triangles[i].Vertices[0] = CreateVector.DenseOfArray(new double[] {
                    vertices[faces[i][0] - 1][0],
                    vertices[faces[i][0] - 1][1],
                    -vertices[faces[i][0] - 1][2],
                    1.0
                });
                Triangles[i].Normals[0] = CreateVector.DenseOfArray(new double[] {
                    vertices[faces[i][2] - 1][0],
                    vertices[faces[i][2] - 1][1],
                    -vertices[faces[i][2] - 1][2],
                    0.0
                });
                Triangles[i].TextureCoords[0] = textureCoords[faces[i][1] - 1];

                Triangles[i].Vertices[1] = CreateVector.DenseOfArray(new double[]{
                    vertices[faces[i][3] - 1][0],
                    vertices[faces[i][3] - 1][1],
                    -vertices[faces[i][3] - 1][2],
                    1.0
                });
                Triangles[i].Normals[1] = CreateVector.DenseOfArray(new double[] {
                    vertices[faces[i][5] - 1][0],
                    vertices[faces[i][5] - 1][1],
                    -vertices[faces[i][5] - 1][2],
                    0.0
                });
                Triangles[i].TextureCoords[1] = textureCoords[faces[i][4] - 1];

                Triangles[i].Vertices[2] = CreateVector.DenseOfArray(new double[]{
                    vertices[faces[i][6] - 1][0],
                    vertices[faces[i][6] - 1][1],
                    -vertices[faces[i][6] - 1][2],
                    1.0
                });
                Triangles[i].Normals[2] = CreateVector.DenseOfArray(new double[] {
                    vertices[faces[i][8] - 1][0],
                    vertices[faces[i][8] - 1][1],
                    -vertices[faces[i][8] - 1][2],
                    0.0
                });
                Triangles[i].TextureCoords[2] = textureCoords[faces[i][7] - 1];
            }
        }

        public void Transform(Matrix<double> transform) {
            Matrix<double> invTrans = transform.Inverse().Transpose();
            foreach(Triangle triangle in Triangles) {
                triangle.Vertices[0] = transform * triangle.Vertices[0];
                triangle.Vertices[1] = transform * triangle.Vertices[1];
                triangle.Vertices[2] = transform * triangle.Vertices[2];

                triangle.Normals[0] = invTrans * triangle.Normals[0];
                triangle.Normals[1] = invTrans * triangle.Normals[1];
                triangle.Normals[2] = invTrans * triangle.Normals[2];
            }
        }
    }
}
