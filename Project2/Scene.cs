using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace Project2
{
    public class Scene
    {
        public Animator Animator;
        public BlockingCollection<MyPolygon> bc = new BlockingCollection<MyPolygon>(10);
        public Canvas Canvas;
        public Image Background;
        public WriteableBitmap HeightMap { get; set; }
        public WriteableBitmap NormalMap { get; set; }
        public WriteableBitmap BackgroundMap { get; set; }
        public Color LightColor { get; set; }
        public MyLight MyLight = new MyLight();
        public int minVelocity = 1;
        public int maxVelocity = 100;

        public string RandomPolygonCount
        {
            get => Animator.RandomPolygonsCount;
            set => Animator.RandomPolygonsCount = value;
        }
        public string VelocityMax
        {
            get => $"{velocityMax}";
            set
            {
                if (int.TryParse(value, out int vMax) && vMax >= velocityMin && vMax <= maxVelocity)
                {
                    velocityMax = vMax;
                }
            }
        }
        public string VelocityMin
        {
            get => $"{velocityMin}";
            set
            {
                if (int.TryParse(value, out int vMin) && vMin <= velocityMax && vMin >= minVelocity)
                {
                    velocityMin = vMin;
                }
            }
        }
        public int velocityMin = 10;
        public int velocityMax = 30;

        private List<Point> PreviewPoints = new List<Point>();
        private List<Line> PreviewLines = new List<Line>();

        public Scene(Canvas canvas, Image background)
        {
            Canvas = canvas;
            Animator = new Animator(Canvas, this);
            Background = background;
        }

        public Line AddLineToCanvas(Point p1, Point p2, Brush brush)
        {
            Line line = null;
            Canvas.Dispatcher.Invoke(() =>
            {
                line = new Line
                {
                    X1 = p1.X,
                    X2 = p2.X,
                    Y1 = p1.Y,
                    Y2 = p2.Y,
                    Visibility = Visibility.Visible,
                    Stroke = brush,
                    StrokeThickness = 1,
                };
                Canvas.Children.Add(line);
                return line;
            });
            return line;
        }
        public void RemoveLineFromCanvas(Line line)
        {
            Canvas.Dispatcher.Invoke(() =>
                Canvas.Children.Remove(line));
        }
        public void AddingRandomPolygon()
        {
            var rng = new Random();
            while (true)
                bc.Add(MyPolygon.GenerateRandomPolygon(1920, 1080, rng, Canvas, this, velocityMin, velocityMax));
        }

        public void TransformLines(IEnumerable<Line> lines, int dx)
        {
            foreach (var line in lines)
            {
                line.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        line.X1 -= dx;
                        line.X2 -= dx;
                    }
                    catch
                    {

                    }
                });
            }
        }
        public void AddPoint(Point point)
        {
            PreviewPoints.Add(point);
            if (PreviewPoints.Count >= 2)
            {
                var p1 = PreviewPoints[PreviewPoints.Count - 2];
                var p2 = PreviewPoints.Last();
                var line = new Line
                {
                    X1 = p1.X,
                    X2 = p2.X,
                    Y1 = p1.Y,
                    Y2 = p2.Y,
                    Fill = Brushes.DarkRed,
                    Visibility = Visibility.Visible,
                    Stroke = Brushes.Blue,
                    StrokeThickness =  1,
                };
                PreviewLines.Add(line);
                Canvas.Children.Add(line);
;            }
        }

        public void EndDrawingPolygon()
        {
            if (PreviewPoints.Count <= 2)
            {
                goto end;
            }
            var myPolygon = new MyPolygon(PreviewPoints, Canvas, this, Brushes.Green, new Random().Next(velocityMin, velocityMax));
            end:
            PreviewLines.ForEach(l => Canvas.Children.Remove(l));
            PreviewLines.Clear();
            PreviewPoints.Clear();
        }
    }
}