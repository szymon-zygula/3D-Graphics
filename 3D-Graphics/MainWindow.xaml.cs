using MathNet.Numerics.LinearAlgebra;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace _3D_Graphics {
    public partial class MainWindow : Window {
        double TimeMeasure = 0.0;
        const double TIME_DELTA = 0.001;

        const string FLOOR_MODEL = "Resources\\floor.obj";
        const string FLOOR_TEXTURE = "Resources\\floor_diffuse.png";
        const string BODY_MODEL = "Resources\\body.obj";
        const string BODY_TEXTURE = "Resources\\body_diffuse.png";
        const string HEAD_MODEL = "Resources\\african_head.obj";
        const string HEAD_TEXTURE = "Resources\\african_head_diffuse.png";
        const string DIABLO_MODEL = "Resources\\diablo.obj";
        const string DIABLO_TEXTURE = "Resources\\diablo_diffuse.png";

        Stopwatch FPSStopWatch;

        Texture DrawingPlane;
        Scene MainScene;

        Camera CurrentCamera;
        Camera StaticCamera;
        Camera StaticFollowerCamera;
        Camera DynamicFollowerCamera;
        Vec3 StaticCameraPosition = new Vec3(3.0, 3.0, -3.0);

        LightList Lights;
        Vec3 CleanColor = new Vec3(0.0, 0.0, 0.0);

        Entity Body;
        IFragmentShader BodyShader;
        Entity Ball;
        IFragmentShader BallShader;
        Entity Diablo;
        IFragmentShader DiabloShader;

        Vec3 AmbientLight = new Vec3(0.5, 0.5, 0.5);
        ReflectorLight StaticReflector;
        ReflectorLight DynamicReflector;

        double FogFar = 5.0;
        double FogClose = 1.0;

        private void InitDrawingSpace() {
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
                Position = StaticCameraPosition,
                Up = new Vec3(0.0, 1.0, 0.0)
            };
            StaticCamera.UpdateProjectionMatrix((double)DrawingPlane.Width / (double)DrawingPlane.Height);
            StaticCamera.UpdateViewMatrix();

            StaticFollowerCamera = new Camera() {
                Fov = 1.0,
                ClosePlane = 0.5,
                FarPlane = 0.95,
                ObservedPoint = new Vec3(0.0, 0.0, 0.0),
                Position = StaticCameraPosition,
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
            DynamicReflector = new ReflectorLight(new Vec3(0.0, 0.0, -2.0), new Vec3(0.0, 0.0, 1.0), Math.PI / 30.0, new Vec3(10.0, 10.0, 2.0));

            Lights = new LightList();
            Lights.Lights.Add(DynamicReflector);
        }

        private void InitBody() {

        }

        private IFragmentShader ApplyFog(IFragmentShader shader) {
            return new FogFragmentShaderDecorator(shader, CleanColor, FogFar, FogClose);
        }

        private void InitDiablo() {
            DiabloShader = new TextureFragmentShader(new Texture(new System.Drawing.Bitmap(DIABLO_TEXTURE)));

            IFragmentShader currentDiabloShader = new PhongFragmentShaderDecorator(DiabloShader, Lights, AmbientLight, 3.4, 0.6, 30.0, CurrentCamera);

            Diablo = new Entity(
                DIABLO_MODEL,
                ApplyFog(currentDiabloShader)
            );
        }

        private void InitBall() {
            BallShader = new FlatFragmentShader(new Vec3(0.5, 0.3, 0.7));

            IFragmentShader currentBallShader = new PhongFragmentShaderDecorator(BallShader, Lights, AmbientLight, 3.6, 0.45, 75.0, CurrentCamera);

            Ball = Entity.CreateSphere(
                0.5, 50, 50,
                () => ApplyFog(currentBallShader)
            );
            Ball.Transform(MatrixUtils.TranslateMatrix(new Vec3(1.5, 0.0, 0.0)));
        }

        private void InitScene() {
            InitBody();
            InitDiablo();
            InitBall();

            MainScene = new Scene();
            MainScene.Entities = new Entity[] {
                Ball, Diablo
            };
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

        private void MoveDiablo() {
            double t = TimeMeasure * 200;
            Matrix<double> rotation = CreateMatrix.DenseOfArray(new double[4, 4] {
                { Math.Cos(t), 0, -Math.Sin(t), 0 },
                { 0, 1, 0, 0 },
                { Math.Sin(t), 0, Math.Cos(t), 0 },
                { 0, 0, 0, 1 }
            });

            Matrix<double> translation = MatrixUtils.TranslateMatrix(2.0 * new Vec3(Math.Cos(t / 4.0), 0.0, 2.0 * Math.Sin(t / 4.0)));

            Diablo.LocalTransform = new TransformVertexShader(translation * rotation);
        }

        private void ApplyTransforms() {
            MoveDiablo();
        }

        private void RenderFrame(object sender, EventArgs e) {
            ApplyTransforms();

            DrawingPlane.Clean(CleanColor);
            SceneDrawer.DrawOnto(MainScene, DrawingPlane, new ProjectionVertexShader(CurrentCamera, DrawingPlane.Width, DrawingPlane.Height), CurrentCamera.ClosePlane);
            ImageCanvas.Source = DrawingPlane.CreateBitmapSource();

            TimeMeasure += TIME_DELTA;
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
