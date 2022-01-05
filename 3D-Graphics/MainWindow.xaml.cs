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
        Vec3 Light;
        GouraudFragmentShaderDecorator HeadLight;
        FlatLightFragmentShaderDecorator BallLight;
        Vec3 CleanColor;

        public MainWindow() {
            InitializeComponent();

            DrawingPlane = new Texture((int)ImageCanvas.Width, (int)ImageCanvas.Height);
            CleanColor = new Vec3(0.5, 0.75, 0.25);
            DrawingPlane.Clean(CleanColor);
            ImageCanvas.Source = DrawingPlane.CreateBitmapSource();

            Light = new Vec3(0.0, 0.0, 1.0);


            MainScene = new Scene();
            MainScene.Entities = new Entity[2];
            BallLight = new FlatLightFragmentShaderDecorator(new FlatFragmentShader(new Vec3(0.5, 0.3, 0.7)), Light);
            MainScene.Entities[0] = Entity.CreateSphere(
                0.5, 50, 50,
                () => BallLight
            );
            //MainScene.Entities[0] = new Entity("C:\\Users\\zbroj\\Desktop\\african_head.obj", "C:\\Users\\zbroj\\Desktop\\african_head_diffuse.png");
            TextureFragmentShader textureShader = new TextureFragmentShader(new Texture(new System.Drawing.Bitmap( "C:\\Users\\zbroj\\Desktop\\african_head_diffuse.png")));
            HeadLight = new GouraudFragmentShaderDecorator(textureShader, Light);
            MainScene.Entities[1] = new Entity(
                "C:\\Users\\zbroj\\Desktop\\african_head.obj",
                "C:\\Users\\zbroj\\Desktop\\african_head_diffuse.png",
                new FogFragmentShaderDecorator(HeadLight, CleanColor, 2.5, 2.0)
            );
            MainScene.Entities[0].Transform(MatrixUtils.TranslateMatrix(new Vec3(1.5, 0.0, 0.0)));

            MainCamera = new Camera() {
                Fov = 1.0,
                ClosePlane = 0.5,
                FarPlane = 0.95,
                ObservedPoint = new Vec3(0.0, 0.0, 0.0),
                Position = new Vec3(0.0, 0.0, -4.0),
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

            //MainCamera.Position.X = Math.Sin(a) * -3.0;
            MainCamera.Position.Z = Math.Cos(5*a) - 3.0;
            MainCamera.UpdateViewMatrix();

            Light.X = Math.Cos(10 * a + 0.3);
            Light.Z = Math.Sin(10 * a + 0.3);
            //HeadLight.Direction = Light;
            BallLight.Direction = Light;

            Matrix<double> rot = CreateMatrix.DenseOfArray(new double[4, 4] {
                { 1, 0, 0, 0 },
                { 0, Math.Cos(-0.28),  -Math.Sin(-0.28), 0},
                { 0, Math.Sin(-0.28),  Math.Cos(-0.28), 0},
                { 0, 0, 0, 1 }
            });

            //MainScene.Entities[1].Transform(rot);

            DrawingPlane.Clean(CleanColor);
            SceneDrawer.DrawOnto(MainScene, DrawingPlane, new ProjectionVertexShader(MainCamera, DrawingPlane.Width, DrawingPlane.Height), MainCamera.ClosePlane);
            ImageCanvas.Source = DrawingPlane.CreateBitmapSource();

            frames += 1;
            if(frames % 60 == 0) {
                sw.Stop();
                //MessageBox.Show($"Average FPS: {(double)frames / (double)sw.ElapsedMilliseconds * 1000}");
                sw.Start();
            }
        }

        private void FlatShadeRadioButton_Checked(object sender, RoutedEventArgs e) {

        }

        private void GouraudShadeRadioButton_Checked(object sender, RoutedEventArgs e) {

        }

        private void PhongShadeRadioButton_Checked(object sender, RoutedEventArgs e) {

        }

        private void StaticCameraRadioButton_Checked(object sender, RoutedEventArgs e) {

        }

        private void StaticFollowingCameraRadioButton_Checked(object sender, RoutedEventArgs e) {

        }

        private void MovingFollowingCameraRadioButton_Checked(object sender, RoutedEventArgs e) {

        }
    }
}
