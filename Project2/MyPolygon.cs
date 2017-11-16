using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;

namespace Project2
{
    public class MyPolygon
    {
        public List<Point> Points;
        public List<Line> Lines;
        public Canvas Canvas;
        public Scene Scene;
        public int Speed;
        public int MaxX;
        public int MinX;
        public MyPolygon(IEnumerable<Point> points, Canvas canvas, Scene scene, Brush brush, int speed)
        {
            Canvas = canvas;
            Scene = scene;
            Speed = speed;
            Points = new List<Point>(points);
            MaxX = (int) Points.Max(point => point.X);
            MinX = (int) Points.Min(point => point.X);
            if (Points.Count <= 2)
                throw new NotSupportedException("Chyba cie pojebalo");
            Lines = new List<Line>();
            AddLinesToCanvas(brush);
        }

        public void Transform()
        {
            MaxX -= Speed;
            MinX -= Speed;
            Scene.TransformLines(Lines, Speed);
        }
        public void AddLinesToCanvas(Brush brush)
        {
            var n = Points.Count;
            for (int i = n - 1, j = 0; j < n; i = j, j++)
            {
                var p1 = Points[i];
                var p2 = Points[j];
                var line = Scene.AddLineToCanvas(p1,p2, brush);
                Lines.Add(line);
            }
        }
        public static MyPolygon GenerateRandomPolygon(int width, int height, Random rng, Canvas canvas, Scene scene, int minSpeed, int maxSpeed)
        {
            const int safeArea = 300;
            var r = rng.Next(100, 500);
            var x = rng.Next(safeArea, width - safeArea) + width;
            var y = rng.Next(safeArea, height - safeArea);
            var n = rng.Next(3, 9);

            var angles = new double[n];
            for (var i = 0; i < n; i++)
                angles[i] = rng.NextDouble();
            var sum = angles.Sum() /(2*Math.PI);
            for (var i = 0; i < n; i++)
                angles[i] /= sum;
            for (var i = 1; i < n; i++)
                angles[i] += angles[i - 1];
            var list = new List<Point>(n);
            for (var i = 0; i < n; i++)
            {
                var dx = r * Math.Sin(angles[i]);
                var dy = r * Math.Cos(angles[i]);
                list.Add(new Point(x + dx, y + dy));
            }
            var speed = rng.Next(minSpeed, maxSpeed+1);
            return new MyPolygon(list, canvas, scene, Brushes.Red, speed);
        }
    }
    
}
