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
            Shapes.ItemsSource = _sketch._myShapes;
            Shapes.ContextMenu = new ContextMenu();
            var item = new MenuItem
            {
                Header = "Set Color"
            };
            item.Click += (obj, args) =>
            {
                var si = Shapes.SelectedItem;
                if (si is MyShape myShape)
                {
                    new ColorPrompt(myShape).ShowDialog();
                    myShape.ColorChangedEvent();
                }
            };
            
            Shapes.ContextMenu.Items.Add(item);
            
            _sketch.OnDrawingFinished += (sender, args) =>
            {
                ToggleButtons(true);
            };
            _sketch.OnFinishAvailable += (sender, args) =>
            {
                ToggleButtons(false);
                ToggleFinishButton(true);
            };
            _sketch.OnDrawingCanceled += (sender, args) =>
            {
                ToggleButtons(true);
            };
            
            var rand = new Random(0x00C0FFEE);
            ToggleButtons(true);
        }

        private void ToggleDrawingPolygon(object sender, RoutedEventArgs e)
        {
            _sketch.Drawing = Drawing.Polygon;
            ToggleButtons(false);
            ToggleFinishButton(false);

        }

        private void ToggleDrawingCircle(object sender, RoutedEventArgs e)
        {
            _sketch.Drawing = Drawing.Circle;
            ToggleButtons(false);
            ToggleFinishButton(false);
        }

        private void FinishDrawing(object sender, RoutedEventArgs e)
        {
            _sketch.FinishDrawing();
        }

        private void CancelDrawing(object sender, RoutedEventArgs e)
        {
            _sketch.CancelDrawing();
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
        public void ToggleFinishButton(bool isEnabled)
        {
            FinishDrawingButton.IsEnabled = isEnabled;
        }
        public void ToggleButtons(bool drawingEnabled)
        {
            ToggleDrawingButtons(drawingEnabled);
            ToggleEndButtons(!drawingEnabled);
        }
    }
}
