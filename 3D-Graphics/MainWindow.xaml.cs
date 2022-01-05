using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace _3D_Graphics {
    public partial class MainWindow : Window {
        Stopwatch FPSStopWatch;

        Texture DrawingPlane;
        Scene MainScene;

        Camera CurrentCamera;
        Camera StaticCamera;
        Camera StaticFollowerCamera;
        Camera DynamicFollowerCamera;

        LightList Lights;
        Vec3 CleanColor;

        private void InitDrawingSpace() {
            CleanColor = new Vec3(0.0, 0.0, 1.0);
            DrawingPlane = new Texture((int)ImageCanvas.Width, (int)ImageCanvas.Height);
            DrawingPlane.Clean(CleanColor);
            ImageCanvas.Source = DrawingPlane.CreateBitmapSource();

            CompositionTarget.Rendering += RenderFrame;

            FPSStopWatch = new Stopwatch();
            FPSStopWatch.Start();
        }

        private void InitCameras() {
            StaticCamera = new Camera() {
                Fov = 1.0,
                ClosePlane = 0.5,
                FarPlane = 0.95,
                ObservedPoint = new Vec3(0.0, 0.0, 0.0),
                Position = new Vec3(-3.0, 1.0, -4.0),
                Up = new Vec3(0.0, 1.0, 0.0)
            };
            StaticCamera.UpdateProjectionMatrix((double)DrawingPlane.Width / (double)DrawingPlane.Height);
            StaticCamera.UpdateViewMatrix();

            StaticFollowerCamera = new Camera() {
                Fov = 1.0,
                ClosePlane = 0.5,
                FarPlane = 0.95,
                ObservedPoint = new Vec3(0.0, 0.0, 0.0),
                Position = new Vec3(-3.0, 1.0, -1.0),
                Up = new Vec3(0.0, 1.0, 0.0)
            };
            StaticFollowerCamera.UpdateProjectionMatrix((double)DrawingPlane.Width / (double)DrawingPlane.Height);
            StaticFollowerCamera.UpdateViewMatrix();

            DynamicFollowerCamera = new Camera() {
                Fov = 1.0,
                ClosePlane = 0.5,
                FarPlane = 0.95,
                ObservedPoint = new Vec3(0.0, 0.0, 0.0),
                Position = new Vec3(-3.0, 1.0, -1.0),
                Up = new Vec3(0.0, 1.0, 0.0)
            };
            DynamicFollowerCamera.UpdateProjectionMatrix((double)DrawingPlane.Width / (double)DrawingPlane.Height);
            DynamicFollowerCamera.UpdateViewMatrix();

            CurrentCamera = StaticCamera;
        }

        private void InitLights() {
            Lights = new LightList();
            Lights.Lights.Add(new GlobalLight(new Vec3(0.0, 0.0, 1.0), new Vec3(1.0, 1.0, 1.0)));
            Lights.Lights.Add(new ReflectorLight(new Vec3(0.0, 0.0, -2.0), new Vec3(1.0, 1.0, 1.0), Math.PI / 10.0, new Vec3(0.0, 2.0, 2.0)));
        }

        private void InitScene() {
            MainScene = new Scene();
            MainScene.Entities = new Entity[2];
            IFragmentShader ballShader = new PhongFragmentShaderDecorator(new FlatFragmentShader(new Vec3(0.5, 0.3, 0.7)), Lights, new Vec3(0.1, 0.1, 0.1), 3.6, 0.45, 75.0, CurrentCamera);
            MainScene.Entities[0] = Entity.CreateSphere(
                0.5, 50, 50,
                () => ballShader
            );
            TextureFragmentShader textureShader = new TextureFragmentShader(new Texture(new System.Drawing.Bitmap( ".\\Resources\\african_head_diffuse.png")));
            IFragmentShader headShader = new PhongFragmentShaderDecorator(textureShader, Lights, new Vec3(0.1, 0.1, 0.1), 3.4, 0.6, 30.0, CurrentCamera);
            MainScene.Entities[1] = new Entity(
                ".\\Resources\\african_head.obj",
                new FogFragmentShaderDecorator(headShader, CleanColor, 2.9, 2.7)
            );
            MainScene.Entities[0].Transform(MatrixUtils.TranslateMatrix(new Vec3(1.5, 0.0, 0.0)));
        }

        public MainWindow() {
            InitializeComponent();

            InitDrawingSpace();
            InitCameras();
            InitLights();
            InitScene();
        }

        private void UpdateFPSTitle() {
            FPSStopWatch.Stop();
            Title = $"FPS = {1.0 / FPSStopWatch.ElapsedMilliseconds * 1000}";
            FPSStopWatch.Restart();
        }

        private void RenderFrame(object sender, EventArgs e) {
            DrawingPlane.Clean(CleanColor);
            SceneDrawer.DrawOnto(MainScene, DrawingPlane, new ProjectionVertexShader(CurrentCamera, DrawingPlane.Width, DrawingPlane.Height), CurrentCamera.ClosePlane);
            ImageCanvas.Source = DrawingPlane.CreateBitmapSource();

            UpdateFPSTitle();
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
