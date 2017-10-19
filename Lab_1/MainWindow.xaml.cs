using System;
using System.Collections.Generic;
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

namespace Lab_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Sketch _sketch;

        public MainWindow()
        {
            InitializeComponent();
            _sketch = new Sketch(SketchCanvas, SketchBitmap);
            _sketch.DrawCircle(500, 500, 20, Color.FromRgb(255, 0, 0));
            _sketch.DrawLine(300, 300, 400, 350, Color.FromRgb(0, 255, 255));
            _sketch.DrawLine(300, 300, 400, 250, Color.FromRgb(0, 000, 255));
            var rand = new Random(0x00C0FFEE);
            KeyDown += _sketch.KeyHitHandler;
            ToggleButtons(true);
        }

        private void ToggleDrawingPolygon(object sender, RoutedEventArgs e)
        {
            _sketch.Drawing = Drawing.Polygon;
            ToggleButtons(false);

        }

        private void ToggleDrawingCircle(object sender, RoutedEventArgs e)
        {
            _sketch.Drawing = Drawing.Circle;
            ToggleButtons(false);
        }

        private void FinishDrawing(object sender, RoutedEventArgs e)
        {
            _sketch.FinishDrawing();
            ToggleButtons(true);
        }

        private void CancelDrawing(object sender, RoutedEventArgs e)
        {
            _sketch.FinishDrawing();
            ToggleButtons(true);
        }

        public void ToggleDrawingButtons(bool isEnabled)
        {
            DrawCircleButton.IsEnabled = isEnabled;
            DrawPolygonButton.IsEnabled = isEnabled;
        }

        public void ToggleEndButtons(bool isEnabled)
        {
            FinishDrawingButton.IsEnabled = isEnabled;
            CancelDrawingButton.IsEnabled = isEnabled;
        }
        public void ToggleButtons(bool drawingEnabled)
        {
            ToggleDrawingButtons(drawingEnabled);
            ToggleEndButtons(!drawingEnabled);
        }
    }
}
