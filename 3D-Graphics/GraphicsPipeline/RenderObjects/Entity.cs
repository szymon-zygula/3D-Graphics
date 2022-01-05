using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public class Entity {
        public Triangle[] Triangles;
        public Texture Texture;
        public IVertexShader LocalTransform;

        public Entity(int triangleCount) {
            Triangles = new Triangle[triangleCount];
        }

        public Entity(string modelPath, IFragmentShader fragmentShader) {
            List<string[]> lines = new List<string[]>();
            List<int[]> faces = new List<int[]>();
            List<double[]> vertices = new List<double[]>();
            List<double[]> normals = new List<double[]>();
            List<Vec2> textureCoords = new List<Vec2>();

            foreach(string line in File.ReadLines(modelPath)) {
                string[] parsed = line.Split(' ', '/');
                if(parsed.Length == 0 || parsed.Length == 1 || parsed[0].Length == 0 || parsed[0][0] == '#') {
                    continue;
                }

                lines.Add(parsed);

                int offset = parsed[1].Length == 0 ? 1 : 0;
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
                            double.Parse(parsed[1 + offset]),
                            double.Parse(parsed[2 + offset])
                        ));
                        break;
                    case "vn":
                        normals.Add(new double[] {
                            double.Parse(parsed[1 + offset]),
                            double.Parse(parsed[2 + offset]),
                            double.Parse(parsed[3 + offset])
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
            Triangles = new Triangle[faces.Count];

            for (int i = 0; i < faces.Count; ++i) {
                Triangles[i] = new Triangle(fragmentShader);

                Triangles[i].Vertices[0] = CreateVector.DenseOfArray(new double[] {
                    vertices[faces[i][0] - 1][0],
                    vertices[faces[i][0] - 1][1],
                    -vertices[faces[i][0] - 1][2],
                    1.0
                });
                Triangles[i].Normals[0] = CreateVector.DenseOfArray(new double[] {
                    normals[faces[i][2] - 1][0],
                    normals[faces[i][2] - 1][1],
                    -normals[faces[i][2] - 1][2],
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
                    normals[faces[i][5] - 1][0],
                    normals[faces[i][5] - 1][1],
                    -normals[faces[i][5] - 1][2],
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
                    normals[faces[i][8] - 1][0],
                    normals[faces[i][8] - 1][1],
                    -normals[faces[i][8] - 1][2],
                    0.0
                });
                Triangles[i].TextureCoords[2] = textureCoords[faces[i][7] - 1];
            }
        }

        public delegate IFragmentShader FragmentShaderGenerator();

        // latitudeSamples nie powinno uwzgledniac biegunow
        public static Entity CreateSphere(double radius, int latitudeSamples, int longitudeSamples, FragmentShaderGenerator fragmentShaderGenerator) {
            // Pasy trojkatow rownolegle do rownika, na biegunach czapki
            Entity sphere = new Entity(longitudeSamples * 2 * latitudeSamples);
            for(int i = 0; i < sphere.Triangles.Length; ++i) {
                sphere.Triangles[i] = new Triangle(fragmentShaderGenerator());
            }

            double latitudeStep = Math.PI / (latitudeSamples + 1);
            double longitudeStep = 2.0 * Math.PI / longitudeSamples;
            double latitude = latitudeStep; // bieguny jako przypadki szczegolne
            double longitude = 0.0;

            // Polaczenie z biegunami
            for(int i = 0; i < longitudeSamples; ++i) {
                sphere.Triangles[i].Vertices[0] = CreateVector.DenseOfArray(new double[4] {
                    0.0, radius, 0.0, 1.0
                });
                sphere.Triangles[i].Normals[0] = CreateVector.DenseOfArray(new double[4] {
                    0.0, 1.0, 0.0, 0.0
                });

                sphere.Triangles[i + (latitudeSamples - 1) * longitudeSamples * 2 + longitudeSamples].Vertices[1] = CreateVector.DenseOfArray(new double[4] {
                    0.0, -radius, 0.0, 1.0
                });
                sphere.Triangles[i + (latitudeSamples - 1) * longitudeSamples * 2 + longitudeSamples].Normals[1] = CreateVector.DenseOfArray(new double[4] {
                    0.0, -1.0, 0.0, 0.0
                });
            }

            for(int i = 0; i < latitudeSamples; ++i) {
                for(int j = 0; j < longitudeSamples; ++j) {
                    Vector<double> pn = CreateVector.DenseOfArray(new double[4] {
                        Math.Sin(latitude) * Math.Cos(longitude),
                        Math.Cos(latitude),
                        Math.Sin(latitude) * Math.Sin(longitude),
                        0.0
                    });

                    Vector<double> p = pn * radius;
                    p[3] = 1.0;

                    int jm1mod = MathUtils.Mod(j - 1, longitudeSamples);

                    if(i == 0) {
                        sphere.Triangles[jm1mod].Vertices[2] = p.Clone();
                        sphere.Triangles[j].Vertices[1] = p.Clone();
                        sphere.Triangles[longitudeSamples + jm1mod].Vertices[2] = p.Clone();
                        sphere.Triangles[longitudeSamples + j].Vertices[0] = p.Clone();
                        sphere.Triangles[2 * longitudeSamples + j].Vertices[0] = p.Clone();

                        sphere.Triangles[jm1mod].Normals[2] = pn.Clone();
                        sphere.Triangles[j].Normals[1] = pn.Clone();
                        sphere.Triangles[longitudeSamples + jm1mod].Normals[2] = pn.Clone();
                        sphere.Triangles[longitudeSamples + j].Normals[0] = pn.Clone();
                        sphere.Triangles[2 * longitudeSamples + j].Normals[0] = pn.Clone();
                    }
                    else if(i == latitudeSamples - 1) {
                        sphere.Triangles[(latitudeSamples - 1) * longitudeSamples * 2 - longitudeSamples + jm1mod].Vertices[1] = p.Clone();
                        sphere.Triangles[(latitudeSamples - 1) * longitudeSamples * 2  + jm1mod].Vertices[2] = p.Clone();
                        sphere.Triangles[(latitudeSamples - 1) * longitudeSamples * 2  + j].Vertices[1] = p.Clone();
                        sphere.Triangles[(latitudeSamples - 1) * longitudeSamples * 2 + longitudeSamples + jm1mod].Vertices[2] = p.Clone();
                        sphere.Triangles[(latitudeSamples - 1) * longitudeSamples * 2 + longitudeSamples + j].Vertices[0] = p.Clone();

                        sphere.Triangles[(latitudeSamples - 1) * longitudeSamples * 2 - longitudeSamples + jm1mod].Normals[1] = pn.Clone();
                        sphere.Triangles[(latitudeSamples - 1) * longitudeSamples * 2 + jm1mod].Normals[2] = pn.Clone();
                        sphere.Triangles[(latitudeSamples - 1) * longitudeSamples * 2 + j].Normals[1] = pn.Clone();
                        sphere.Triangles[(latitudeSamples - 1) * longitudeSamples * 2 + longitudeSamples + jm1mod].Normals[2] = pn.Clone();
                        sphere.Triangles[(latitudeSamples - 1) * longitudeSamples * 2 + longitudeSamples + j].Normals[0] = pn.Clone();
                    }
                    else {
                        sphere.Triangles[longitudeSamples * (1 + 2 * (i - 1)) + jm1mod].Vertices[1] = p.Clone();
                        sphere.Triangles[longitudeSamples * (2 + 2 * (i - 1)) + jm1mod].Vertices[2] = p.Clone();
                        sphere.Triangles[longitudeSamples * (2 + 2 * (i - 1)) + j].Vertices[1] = p.Clone();
                        sphere.Triangles[longitudeSamples * (3 + 2 * (i - 1)) + jm1mod].Vertices[2] = p.Clone();
                        sphere.Triangles[longitudeSamples * (3 + 2 * (i - 1)) + j].Vertices[0] = p.Clone();
                        sphere.Triangles[longitudeSamples * (4 + 2 * (i - 1)) + j].Vertices[0] = p.Clone();

                        sphere.Triangles[longitudeSamples * (1 + 2 * (i - 1)) + jm1mod].Normals[1] = pn.Clone();
                        sphere.Triangles[longitudeSamples * (2 + 2 * (i - 1)) + jm1mod].Normals[2] = pn.Clone();
                        sphere.Triangles[longitudeSamples * (2 + 2 * (i - 1)) + j].Normals[1] = pn.Clone();
                        sphere.Triangles[longitudeSamples * (3 + 2 * (i - 1)) + jm1mod].Normals[2] = pn.Clone();
                        sphere.Triangles[longitudeSamples * (3 + 2 * (i - 1)) + j].Normals[0] = pn.Clone();
                        sphere.Triangles[longitudeSamples * (4 + 2 * (i - 1)) + j].Normals[0] = pn.Clone();
                    }

                    longitude += longitudeStep;
                }

                longitude = 0.0;
                latitude += latitudeStep;
            }

            return sphere;
        }

        public void Transform(Matrix<double> transform) {
            Matrix<double> invTrans = transform.Inverse().Transpose();
            foreach(Triangle triangle in Triangles) {
                triangle.Vertices[0] = transform * triangle.Vertices[0];
                triangle.Vertices[1] = transform * triangle.Vertices[1];
                triangle.Vertices[2] = transform * triangle.Vertices[2];

                triangle.Normals[0] = invTrans * triangle.Normals[0];
                triangle.Normals[0][3] = 0;
                triangle.Normals[1] = invTrans * triangle.Normals[1];
                triangle.Normals[1][3] = 0;
                triangle.Normals[2] = invTrans * triangle.Normals[2];
                triangle.Normals[2][3] = 0;
            }
        }
    }
}
