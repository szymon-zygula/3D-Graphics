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
            MainScene.Entities[0] = new Entity("C:\\Users\\zbroj\\Desktop\\african_head.obj", "C:\\Users\\zbroj\\Desktop\\african_head_diffuse.png");

            a = 0.03;
            frames = 0;

            CompositionTarget.Rendering += RenderFrame;

            sw = new Stopwatch();
            sw.Start();
        }

        private void RenderFrame(object sender, EventArgs e) {
            //MainScene.Entities[0].Triangles[0].Vertices[2][0] = (double)Math.Cos(a);
            //MainScene.Entities[0].Triangles[0].Vertices[2][1] = (double)Math.Sin(a);
            //MainScene.Entities[0].Triangles[0].Vertices[2][2] = ((double)Math.Sin(2 * a) + 1.5f) / 2.0;

            Matrix<double> rot = CreateMatrix.DenseOfArray(new double[4, 4] {
                { Math.Cos(a), 0, -Math.Sin(a), 0},
                { 0, 1, 0, 0 },
                { Math.Sin(a), 0, Math.Cos(a), 0},
                { 0, 0, 0, 1}
            });

            MainScene.Entities[0].Transform(rot);

            DrawingPlane.Clean(new Vec3(0.5f, 0.75f, 0.25f));
            SceneDrawer.DrawOnto(MainScene, DrawingPlane, new ProjectionVertexShader(1.5f, 0.1f, 0.90f, DrawingPlane.Width, DrawingPlane.Height));
            ImageCanvas.Source = DrawingPlane.CreateBitmapSource();

            frames += 1;
            if(frames % 100 == 0) {
                sw.Stop();
                //MessageBox.Show($"Average FPS: {(double)frames / (double)sw.ElapsedMilliseconds * 1000}");
                sw.Start();
            }
        }
    }
}
