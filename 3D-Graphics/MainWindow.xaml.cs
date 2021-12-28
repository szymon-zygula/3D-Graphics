using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathNet.Numerics.LinearAlgebra;

namespace _3D_Graphics {
    public partial class MainWindow : Window {
        Texture DrawingPlane;
        double a;
        int frames;
        Stopwatch sw;
        Scene MainScene;

        public MainWindow() {
            InitializeComponent();

            DrawingPlane = new Texture((int)ImageCanvas.Width, (int)ImageCanvas.Height);
            DrawingPlane.Clean(new Vec3(0.5f, 0.25f, 0.75f));
            ImageCanvas.Source = DrawingPlane.CreateBitmapSource();

            MainScene = new Scene();
            MainScene.Entities = new Entity[1];
            MainScene.Entities[0] = new Entity(1);
            MainScene.Entities[0].Triangles[0] = new Triangle(
                new InterpolationFragmentShader(
                    new Vec3(0.0f, 0.0f, 1.0f),
                    new Vec3(1.0f, 0.0f, 0.0f),
                    new Vec3(0.0f, 1.0f, 0.0f)
                )
            );
            MainScene.Entities[0].Triangles[0].Vertices[0] = CreateVector.DenseOfArray(new double[] { -0.25f, 0.0f, 0.5f, 1.0f });
            MainScene.Entities[0].Triangles[0].Vertices[1] = CreateVector.DenseOfArray(new double[] { 0.25f, 0.0f, 0.5f, 1.0f });
            MainScene.Entities[0].Triangles[0].Vertices[2] = CreateVector.DenseOfArray(new double[] { 0.0f, 0.5f, 0.5f, 1.0f });

            a = 0.0;
            frames = 0;

            CompositionTarget.Rendering += RenderFrame;

            sw = new Stopwatch();
            sw.Start();
        }

        private void RenderFrame(object sender, EventArgs e) {
            a += 0.04;
            MainScene.Entities[0].Triangles[0].Vertices[2][0] = (double)Math.Cos(a);
            MainScene.Entities[0].Triangles[0].Vertices[2][1] = (double)Math.Sin(a);
            MainScene.Entities[0].Triangles[0].Vertices[2][2] = ((double)Math.Sin(2 * a) + 1.5f) / 2.0f;

            DrawingPlane.Clean(new Vec3(0.5f, 0.75f, 0.25f));
            SceneDrawer.DrawOnto(MainScene, DrawingPlane, new ProjectionVertexShader(1.5f, 0.1f, 0.90f, DrawingPlane.Width, DrawingPlane.Height));
            ImageCanvas.Source = DrawingPlane.CreateBitmapSource();

            frames += 1;
            if(frames % 100 == 0) {
                sw.Stop();
                MessageBox.Show($"Average FPS: {(double)frames / (double)sw.ElapsedMilliseconds * 1000}");
                sw.Start();
            }
        }
    }
}
