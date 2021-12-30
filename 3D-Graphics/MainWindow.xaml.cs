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
        Camera MainCamera;

        public MainWindow() {
            InitializeComponent();

            DrawingPlane = new Texture((int)ImageCanvas.Width, (int)ImageCanvas.Height);
            DrawingPlane.Clean(new Vec3(0.5, 0.25, 0.75));
            ImageCanvas.Source = DrawingPlane.CreateBitmapSource();

            MainScene = new Scene();
            MainScene.Entities = new Entity[1];
            MainScene.Entities[0] = Entity.CreateSphere(0.5, 50, 50, () => new GouraudFragmentShaderDecorator(FlatFragmentShader.RandomToned(new Vec3(0.5, 0.3, 0.7)), new Vec3(-1.0, 1.0, 0.0)));
            //MainScene.Entities[0] = new Entity("C:\\Users\\zbroj\\Desktop\\african_head.obj", "C:\\Users\\zbroj\\Desktop\\african_head_diffuse.png");
            //MainScene.Entities[1] = new Entity("C:\\Users\\zbroj\\Desktop\\african_head.obj", "C:\\Users\\zbroj\\Desktop\\african_head_diffuse.png");
            MainScene.Entities[0].Transform(MatrixUtils.TranslateMatrix(new Vec3(1.0, 0.0, 0.0)));

            MainCamera = new Camera() {
                Fov = 1.0,
                ClosePlane = 0.1,
                FarPlane = 0.9,
                ObservedPoint = new Vec3(0.0, 0.0, 0.0),
                Position = new Vec3(0.0, 0.0, -3.0),
                Up = new Vec3(0.0, 1.0, 0.0)
            };
            MainCamera.UpdateProjectionMatrix((double)DrawingPlane.Width / (double)DrawingPlane.Height);
            MainCamera.UpdateViewMatrix();

            a = 0.05;
            frames = 0;

            CompositionTarget.Rendering += RenderFrame;

            sw = new Stopwatch();
            sw.Start();
        }

        private void RenderFrame(object sender, EventArgs e) {
            a += 0.03;

            MainCamera.Position.X = Math.Sin(a) * -3.0;
            MainCamera.Position.Z = Math.Cos(a) * 3.0;
            MainCamera.UpdateViewMatrix();

            Matrix<double> rot = CreateMatrix.DenseOfArray(new double[4, 4] {
                { 1, 0, 0, 0 },
                { 0, Math.Cos(-0.28),  -Math.Sin(-0.28), 0},
                { 0, Math.Sin(-0.28),  Math.Cos(-0.28), 0},
                { 0, 0, 0, 1 }
            });

            //MainScene.Entities[1].Transform(rot);

            DrawingPlane.Clean(new Vec3(0.5, 0.75, 0.25));
            SceneDrawer.DrawOnto(MainScene, DrawingPlane, new ProjectionVertexShader(MainCamera, DrawingPlane.Width, DrawingPlane.Height));
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
