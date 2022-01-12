using MathNet.Numerics.LinearAlgebra;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace _3D_Graphics {
    public partial class MainWindow : Window {
        double TimeMeasure = 0.0;
        const double TIME_DELTA = 0.001;

        const string BODY_MODEL = "Resources\\body.obj";
        const string BODY_TEXTURE = "Resources\\body_diffuse.png";
        const string BODY_NORMALS = "Resources\\body_normals.png";
        const string HEAD_MODEL = "Resources\\african_head.obj";
        const string HEAD_TEXTURE = "Resources\\african_head_diffuse.png";
        const string HEAD_NORMALS = "Resources\\african_head_normals.png";
        const string DIABLO_MODEL = "Resources\\diablo.obj";
        const string DIABLO_TEXTURE = "Resources\\diablo_diffuse.png";
        const string DIABLO_NORMALS = "Resources\\diablo_normals.png";

        Stopwatch FPSStopWatch;

        Texture DrawingPlane;
        Scene MainScene;

        Camera CurrentCamera;
        Camera StaticCamera;
        Camera StaticFollowerCamera;
        Camera DynamicFollowerCamera;
        Vec3 StaticCameraPosition = new Vec3(5.0, 5.0, -5.0);

        LightList Lights;
        Vec3 CleanColor = new Vec3(0.0, 0.0, 0.0);

        Entity Body;
        IFragmentShader ConstBodyShader;
        IFragmentShader PhongBodyShader;
        IFragmentShader GouraudBodyShader;
        IFragmentShader FlatBodyShader;
        WrapperFragmentShader BodyShader;
        Texture BodyNormals;
        Entity Ball;
        IFragmentShader ConstBallShader;
        IFragmentShader PhongBallShader;
        IFragmentShader GouraudBallShader;
        IFragmentShader FlatBallShader;
        WrapperFragmentShader BallShader;
        Entity Diablo;
        IFragmentShader ConstDiabloShader;
        IFragmentShader PhongDiabloShader;
        IFragmentShader GouraudDiabloShader;
        IFragmentShader FlatDiabloShader;
        WrapperFragmentShader DiabloShader;
        Texture DiabloNormals;

        Vec3 AmbientLight = new Vec3(0.5, 0.5, 0.5);
        ReflectorLight StaticReflector;
        ReflectorLight DynamicReflector;

        double FogFar = 3.0;
        double FogClose = 2.7;

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
            DynamicReflector = new ReflectorLight(new Vec3(0.0, 0.0, -2.0), new Vec3(0.0, 0.0, 1.0), Math.PI * 0.15, new Vec3(10.0, 10.0, 2.0));
            StaticReflector = new ReflectorLight(new Vec3(4.0, 0.0, 0.0), new Vec3(-1.0, 0.0, 0.0), Math.PI * 0.5, new Vec3(3.0, 3.0, 3.0));

            Lights = new LightList();
            Lights.Lights.Add(DynamicReflector);
            Lights.Lights.Add(StaticReflector);
        }

        private void InitBody() {
            BodyNormals = new Texture(new System.Drawing.Bitmap(BODY_NORMALS));

            ConstBodyShader = new TextureFragmentShader(new Texture(new System.Drawing.Bitmap(BODY_TEXTURE)));
            PhongBodyShader = ApplyFog(new PhongFragmentShaderDecorator(ConstBodyShader, Lights, AmbientLight, 0.2, 0.8, 1.0, CurrentCamera, BodyNormals));
            GouraudBodyShader = ApplyFog(new GouraudFragmentShaderDecorator(ConstBodyShader, Lights, AmbientLight, 0.2, 0.8, 1.0, CurrentCamera));
            FlatBodyShader = ApplyFog(new FlatLightFragmentShaderDecorator(ConstBodyShader, Lights, AmbientLight, 0.2, 0.8, 1.0, CurrentCamera));
            BodyShader = new WrapperFragmentShader(PhongBodyShader);

            Body = new Entity(
                BODY_MODEL,
                BodyShader
            );
            Body.Transform(MatrixUtils.TranslateMatrix(new Vec3(0.0, 0.0, -2.0)));
        }

        private IFragmentShader ApplyFog(IFragmentShader shader) {
            return new FogFragmentShaderDecorator(shader, CleanColor, FogFar, FogClose);
        }

        private void InitDiablo() {
            DiabloNormals = new Texture(new System.Drawing.Bitmap(DIABLO_NORMALS));

            ConstDiabloShader = new TextureFragmentShader(new Texture(new System.Drawing.Bitmap(DIABLO_TEXTURE)));
            PhongDiabloShader = ApplyFog(new PhongFragmentShaderDecorator(ConstDiabloShader, Lights, AmbientLight, 0.4, 0.6, 1.0, CurrentCamera, DiabloNormals));
            GouraudDiabloShader = ApplyFog(new GouraudFragmentShaderDecorator(ConstDiabloShader, Lights, AmbientLight, 0.4, 0.6, 1.0, CurrentCamera));
            FlatDiabloShader = ApplyFog(new FlatLightFragmentShaderDecorator(ConstDiabloShader, Lights, AmbientLight, 0.4, 0.6, 1.0, CurrentCamera));
            DiabloShader = new WrapperFragmentShader(PhongDiabloShader);

            Diablo = new Entity(
                DIABLO_MODEL,
                DiabloShader
            );
        }

        private void InitBall() {
            ConstBallShader = new FlatFragmentShader(new Vec3(0.5, 0.3, 0.7));
            PhongBallShader = ApplyFog(new PhongFragmentShaderDecorator(ConstBallShader, Lights, AmbientLight, 3.6, 0.45, 75.0, CurrentCamera));
            GouraudBallShader = ApplyFog(new GouraudFragmentShaderDecorator(ConstBallShader, Lights, AmbientLight, 3.6, 0.45, 75.0, CurrentCamera));
            FlatBallShader = ApplyFog(new FlatLightFragmentShaderDecorator(ConstBallShader, Lights, AmbientLight, 3.6, 0.45, 75.0, CurrentCamera));
            BallShader = new WrapperFragmentShader(PhongBallShader);

            Ball = Entity.CreateSphere(
                1.2, 50, 50,
                () => BallShader
            );
        }

        private void InitScene() {
            InitBody();
            InitDiablo();
            InitBall();

            MainScene = new Scene();
            MainScene.Entities = new Entity[] {
                Ball, Diablo, Body
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
            double t = TimeMeasure * 400;
            Matrix<double> rotation = CreateMatrix.DenseOfArray(new double[4, 4] {
                { Math.Cos(t), 0, -Math.Sin(t), 0 },
                { 0, 1, 0, 0 },
                { Math.Sin(t), 0, Math.Cos(t), 0 },
                { 0, 0, 0, 1 }
            });

            Vec3 position = 2.0 * new Vec3(Math.Cos(t / 8.0), 0.0, 2.0 * Math.Sin(t / 8.0));
            Matrix<double> translation = MatrixUtils.TranslateMatrix(position);
            Diablo.LocalTransform = new TransformVertexShader(translation * rotation);

            DynamicReflector.Position = position;
            DynamicReflector.Direction = new Vec3(Math.Sin(t), 0.0, -Math.Cos(t));

            StaticFollowerCamera.ObservedPoint = position;
            StaticFollowerCamera.UpdateViewMatrix();

            DynamicFollowerCamera.Position = 2.5 * position + new Vec3(0.0, 0.0, 0.0);
            DynamicFollowerCamera.ObservedPoint = position;
            DynamicFollowerCamera.UpdateViewMatrix();
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

        private void ResetCamera() {
            if (CurrentCamera == null) return;

            if(StaticCameraRadioButton.IsChecked.Value) {
                CurrentCamera = StaticCamera;
            }
            else if(StaticFollowingCameraRadioButton.IsChecked.Value) {
                CurrentCamera = StaticFollowerCamera;
            }
            else if(MovingFollowingCameraRadioButton.IsChecked.Value) {
                CurrentCamera = DynamicFollowerCamera;
            }

            ResetShaderCameras();
        }

        private void ResetShaderCameras() {
            PhongBodyShader = ApplyFog(new PhongFragmentShaderDecorator(ConstBodyShader, Lights, AmbientLight, 0.2, 0.8, 1.0, CurrentCamera, BodyNormals));
            PhongBallShader = ApplyFog(new PhongFragmentShaderDecorator(ConstBallShader, Lights, AmbientLight, 3.6, 0.45, 75.0, CurrentCamera));
            PhongDiabloShader = ApplyFog(new PhongFragmentShaderDecorator(ConstDiabloShader, Lights, AmbientLight, 0.4, 0.6, 1.0, CurrentCamera, DiabloNormals));
            GouraudBodyShader = ApplyFog(new GouraudFragmentShaderDecorator(ConstBodyShader, Lights, AmbientLight, 0.2, 0.8, 1.0, CurrentCamera));
            GouraudBallShader = ApplyFog(new GouraudFragmentShaderDecorator(ConstBallShader, Lights, AmbientLight, 3.6, 0.45, 75.0, CurrentCamera));
            GouraudDiabloShader = ApplyFog(new GouraudFragmentShaderDecorator(ConstDiabloShader, Lights, AmbientLight, 0.4, 0.6, 1.0, CurrentCamera));
            FlatBodyShader = ApplyFog(new FlatLightFragmentShaderDecorator(ConstBodyShader, Lights, AmbientLight, 0.2, 0.8, 1.0, CurrentCamera));
            FlatBallShader = ApplyFog(new FlatLightFragmentShaderDecorator(ConstBallShader, Lights, AmbientLight, 3.6, 0.45, 75.0, CurrentCamera));
            FlatDiabloShader = ApplyFog(new FlatLightFragmentShaderDecorator(ConstDiabloShader, Lights, AmbientLight, 0.4, 0.6, 1.0, CurrentCamera));
            ResetShaders();
        }

        private void ResetShaders() {
            if (BallShader == null) return;
            if(FlatShadeRadioButton.IsChecked.Value) {
                BallShader.InnerShader = FlatBallShader;
                DiabloShader.InnerShader = FlatDiabloShader;
                BodyShader.InnerShader = FlatBodyShader;
            }
            else if(GouraudShadeRadioButton.IsChecked.Value) {
                BallShader.InnerShader = GouraudBallShader;
                DiabloShader.InnerShader = GouraudDiabloShader;
                BodyShader.InnerShader = GouraudBodyShader;
            }
            else if(PhongShadeRadioButton.IsChecked.Value) {
                BallShader.InnerShader = PhongBallShader;
                DiabloShader.InnerShader = PhongDiabloShader;
                BodyShader.InnerShader = PhongBodyShader;
            }
        }

        private void FlatShadeRadioButton_Checked(object sender, RoutedEventArgs e) {
            ResetShaders();
        }

        private void GouraudShadeRadioButton_Checked(object sender, RoutedEventArgs e) {
            ResetShaders();
        }

        private void PhongShadeRadioButton_Checked(object sender, RoutedEventArgs e) {
            ResetShaders();
        }

        private void StaticCameraRadioButton_Checked(object sender, RoutedEventArgs e) {
            ResetCamera();
        }

        private void StaticFollowingCameraRadioButton_Checked(object sender, RoutedEventArgs e) {
            ResetCamera();
        }

        private void MovingFollowingCameraRadioButton_Checked(object sender, RoutedEventArgs e) {
            ResetCamera();
        }
    }
}
