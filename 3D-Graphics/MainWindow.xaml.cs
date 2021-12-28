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

namespace _3D_Graphics {
    public partial class MainWindow : Window {
        Texture DrawingPlane;
        double a;
        int frames;
        Stopwatch sw;

        public MainWindow() {
            InitializeComponent();

            CompositionTarget.Rendering += RenderFrame;

            DrawingPlane = new Texture((int)ImageCanvas.Width, (int)ImageCanvas.Height);
            DrawingPlane.Clean(new Vec3(0.5, 0.25, 0.75));
            ImageCanvas.Source = DrawingPlane.CreateBitmapSource();
            a = 0.0;
            frames = 0;

            sw = new Stopwatch();

            sw.Start();
        }

        private void RenderFrame(object sender, EventArgs e) {
            a += 0.01;
            DrawingPlane.Clean(new Vec3(0.5, 0.75, 0.25));
            DrawingPlane.Pixels[(int)((DrawingPlane.Width - 20) * (Math.Cos(a) / 2 + 0.5)), (int)((DrawingPlane.Height - 20) * (Math.Sin(a) / 2 + 0.5))] = new Vec3(0.0, 0.0, 0.0);
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
