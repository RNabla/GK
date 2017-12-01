using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Project2
{
    public class Scene
    {
        private readonly Point3D[,] _posVectors;
        private readonly List<Line> _previewLines = new List<Line>();

        private readonly List<Point> _previewPoints = new List<Point>();
        private readonly Vector3D N = new Vector3D(0, 0, 1);

        private readonly Thread _thread;
        private Vector3D[,] _normalVectors;
        public Animator Animator;
        public Image Background;

        public WriteableBitmap bitmap = new WriteableBitmap(1920, 1080, 96, 96, PixelFormats.Bgr32, null);
        public WriteableBitmap blankBitmap = new WriteableBitmap(1920, 1080, 96, 96, PixelFormats.Bgr32, null);
        public Canvas Canvas;
        public Label fpsCounter;
        public uint[,] heightmap;
        private bool HeightMapUsed;
        public int maxVelocity = 100;
        public int minVelocity = 1;
        public MyLight MyLight = new MyLight();

        public byte[] OneLine = new byte[1920 * 4];
        private bool SolidColor = true;

        public uint[,] texture;
        public int velocityMax = 30;
        public int velocityMin = 10;

        public Scene(Canvas canvas, Image background)
        {
            Canvas = canvas;
            Animator = new Animator(Canvas, this);
            Background = background;
            Background.Source = bitmap;
            _thread = new Thread(PaintAll);
            _thread.Start();
            _posVectors = new Point3D[1920, 1080];
            for (var i = 0; i < 1920; i++)
            for (var j = 0; j < 1080; j++)
                _posVectors[i, j] = new Point3D(i, j, 0);
        }

        public ObservableCollection<MyPolygon> Polygons { get; set; }
            = new ObservableCollection<MyPolygon>();

        public WriteableBitmap HeightMap { get; set; }

        public WriteableBitmap BackgroundMap { get; set; }

        public Color LightColor { get; set; }

        public string RandomPolygonCount
        {
            get => Animator.RandomPolygonsCount;
            set => Animator.RandomPolygonsCount = value;
        }

        public string Fps
        {
            get => Animator.Fps;
            set => Animator.Fps = value;
        }

        public string VelocityMax
        {
            get => $"{velocityMax}";
            set
            {
                if (int.TryParse(value, out var vMax) && vMax >= velocityMin && vMax <= maxVelocity)
                    velocityMax = vMax;
            }
        }

        public string VelocityMin
        {
            get => $"{velocityMin}";
            set
            {
                if (int.TryParse(value, out var vMin) && vMin <= velocityMax && vMin >= minVelocity)
                    velocityMin = vMin;
            }
        }

        private uint PixelColor(int R, int G, int B)
        {
            return (uint) ((0 << 24) + (R << 16) + (G << 8) + B);
        }

        public void SetTexture(string fileName)
        {
            var bmp = new BitmapImage(new Uri(fileName));
            var tex = new byte[4 * bmp.PixelHeight * bmp.PixelWidth];
            bmp.CopyPixels(tex, 4 * bmp.PixelWidth, 0);
            texture = new uint[bmp.PixelHeight, bmp.PixelWidth];
            for (var i = 0; i < texture.GetLength(0); i++)
            for (var j = 0; j < texture.GetLength(1); j++)
                texture[i, j] = PixelColor(tex[4 * (i * texture.GetLength(1) + j) + 2],
                    tex[4 * (i * texture.GetLength(1) + j) + 1], tex[4 * (i * texture.GetLength(1) + j)]);
            SolidColor = false;
        }

        private uint ColorPixel(Color color)
        {
            return (uint) (color.R << (16 + color.G) << (8 + color.B));
        }

        public void SetTextureSolidColor(Color color)
        {
            texture = new uint[1, 1];
            texture[0, 0] = ColorPixel(color);
            SolidColor = true;
        }

        public void SetTextureSolidColor(byte r, byte g, byte b)
        {
            texture = new uint[1, 1];
            texture[0, 0] = PixelColor(r, g, b);
            SolidColor = true;
        }

        public void SetHeight(string fileName)
        {
            var bmp = new BitmapImage(new Uri(fileName));
            var tex = new byte[4 * bmp.PixelHeight * bmp.PixelWidth];
            bmp.CopyPixels(tex, 4 * bmp.PixelWidth, 0);
            heightmap = new uint[bmp.PixelHeight, bmp.PixelWidth];
            _normalVectors = new Vector3D[bmp.PixelHeight, bmp.PixelWidth];
            var T = new Vector3D(1, 0, 0);
            var B = new Vector3D(0, 1, 0);
            for (var i = 0; i < heightmap.GetLength(0); i++)
            for (var j = 0; j < heightmap.GetLength(1); j++)
            {
                heightmap[i, j] = PixelColor(tex[4 * (i * heightmap.GetLength(1) + j) + 2],
                    tex[4 * (i * heightmap.GetLength(1) + j) + 1], tex[4 * (i * heightmap.GetLength(1) + j)]);

                _normalVectors[i, j] = N + T * (GetHeightAt(i + 1, j) - GetTextureAt(i, j)) +
                                       B * (GetTextureAt(i, j + 1) - GetTextureAt(i, j));
            }
            HeightMapUsed = true;
        }

        public int GetTextureAt(int x, int y)
        {
            return (int) texture[y % texture.GetLength(0), x % texture.GetLength(1)];
        }

        public int GetHeightAt(int x, int y)
        {
            return (int) (heightmap[y % heightmap.GetLength(0), x % heightmap.GetLength(1)] & 0xff);
        }


        public void DisableHeight()
        {
            HeightMapUsed = false;
            _normalVectors = new Vector3D[1, 1];
            _normalVectors[0, 0] = new Vector3D(0, 0, 1);
            heightmap = new uint[1, 1];
            heightmap[0, 0] = 0;
        }

        public Vector3D GetNormalVectorAt(int x, int y)
        {
            return _normalVectors[y % _normalVectors.GetLength(0), x % _normalVectors.GetLength(1)];
        }


        public void FinishThreadJob()
        {
            _thread.Abort();
            _thread.Join();
        }

        public void PaintAll()
        {
            Thread.Sleep(1000);
            try
            {
                while (true)
                {
                    Animator.DoTick();
                    RedrawAll();
                    foreach (var randomPolygon in Animator.RandomPolygons)
                    foreach (var userPolygon in Polygons)
                    {
                        var x = SutherlandHodgman.GetIntersectedPolygon(userPolygon, randomPolygon);
                        if (x.Length <= 2) continue;
                        var clippedPolygon = new MyPolygon(x, Canvas, this, 0);
                        FillPolygon(clippedPolygon);
                    }
                    bitmap.Dispatcher.Invoke(() =>
                    {
                        bitmap.Lock();
                        bitmap.AddDirtyRect(new Int32Rect(0, 0, 1920, 1080));
                        bitmap.Unlock();
                    });
                    Thread.Sleep(1);
                }
            }
            catch (Exception exception)
            {
            }
        }

        public void RedrawAll()
        {
            bitmap.Dispatcher.Invoke(() =>
            {
                bitmap.Lock();
                for (var i = 0; i < 1080; i++)
                    bitmap.WritePixels(new Int32Rect(0, i, 1920, 1), OneLine, OneLine.Length, 0);
                bitmap.AddDirtyRect(new Int32Rect(0, 0, 1920, 1080));
                bitmap.Unlock();
            });
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
                    StrokeThickness = 1
                };
                Canvas.Children.Add(line);
                return line;
            });
            return line;
        }

        public void RemoveLineFromCanvas(Line line)
        {
            Canvas.Dispatcher.Invoke(() => Canvas.Children.Remove(line));
        }

        public void RemovePolygon(MyPolygon polygon)
        {
            Polygons.Remove(polygon);
        }

        public void TransformLines(IEnumerable<Line> lines, int dx)
        {
            foreach (var line in lines)
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

        public void AddPoint(Point point)
        {
            _previewPoints.Add(point);
            if (_previewPoints.Count >= 2)
            {
                var p1 = _previewPoints[_previewPoints.Count - 2];
                var p2 = _previewPoints.Last();
                var line = new Line
                {
                    X1 = p1.X,
                    X2 = p2.X,
                    Y1 = p1.Y,
                    Y2 = p2.Y,
                    Fill = Brushes.DarkRed,
                    Visibility = Visibility.Visible,
                    Stroke = Brushes.Blue,
                    StrokeThickness = 1
                };
                _previewLines.Add(line);
                Canvas.Children.Add(line);
            }
        }

        public void EndDrawingPolygon()
        {
            if (_previewPoints.Count <= 2)
                goto end;
            var myPolygon = new MyPolygon(_previewPoints, Canvas, this, new Random().Next(velocityMin, velocityMax));
            Polygons.Add(myPolygon);
            myPolygon.AddLinesToCanvas(Brushes.Green);
            end:
            _previewLines.ForEach(l => Canvas.Children.Remove(l));
            _previewLines.Clear();
            _previewPoints.Clear();
        }

        public Queue<Tuple<Point, Point>> CreateET(MyPolygon polygon)
        {
            var buckets = new List<Tuple<Point, Point>>[1080];
            var n = polygon.Points.Count;
            var edges = new List<Tuple<Point, Point>>(n);
            for (int i = n - 1, j = 0; j < n; i = j, j++)
            {
                var p1 = polygon.Points[i];
                var p2 = polygon.Points[j];
                edges.Add(new Tuple<Point, Point>(p1, p2));
            }
            foreach (var edge in edges)
            {
                var minY = (int) Math.Min(edge.Item1.Y, edge.Item2.Y);
                var tuple = edge;
                if (edge.Item1.Y > edge.Item2.Y)
                    tuple = new Tuple<Point, Point>(edge.Item2, edge.Item1);
                if (buckets[minY] == null)
                    buckets[minY] = new List<Tuple<Point, Point>>();
                buckets[minY].Add(tuple);
            }
            var et = new Queue<Tuple<Point, Point>>();
            for (var i = 0; i < buckets.Length; i++)
            {
                if (buckets[i] == null) continue;
                foreach (var tuple in buckets[i])
                    et.Enqueue(tuple);
            }
            return et;
        }

        private int PeekY(Queue<Tuple<Point, Point>> et)
        {
            var element = et.FirstOrDefault();
            if (element == null) return -1;
            return (int) element.Item1.Y;
        }


        public void DrawLineBetweenPairs(List<EdgeStruct> aet, int y)
        {
            var first = default(EdgeStruct);
            var flag = false;
            foreach (var element in aet)
            {
                flag ^= true;
                if (flag)
                {
                    first = element;
                    continue;
                }
                var begin = Math.Min((int) first.Xmin, 1920);
                var end = Math.Min((int) element.Xmin, 1920);
                if (begin >= 1920) continue;
                begin = Math.Max(0, begin);
                end = Math.Max(0, end);
                //var clr = new byte[(end - begin) * 3];
                //for (var i = 0; i < clr.Length; i += 3)
                //    clr[i] = 255;
                //bitmap.WritePixels(new Int32Rect(begin,y,end-begin,1),clr,clr.Length,0 );
                for (var i = begin; i < end; i++)
                    DrawPixel(i, y);
            }
        }

        public void DrawPixel(int x, int y)
        {
            if (x < 0 || x >= 1920 || y < 0 || y >= 1080)
                return;
            //var color = new byte[] {(byte) (bgrd[x, y] >>24), (byte)(bgrd[x, y] >> 16) , (byte)(bgrd[x, y] >> 8) , (byte)(bgrd[x, y] >> 0) };
            //bitmap.WritePixels(new Int32Rect(x,y,1,1), color,color.Length,0);

            //var lightVector = MyLight.LightPos - _posVectors[x, y];
            //var normalVector = GetNormalVectorAt(x, y);
            //var angle = Vector3D.AngleBetween(normalVector, lightVector);
            //var cos = Math.Cos(angle * 2 * Math.PI / 360);
            //if (double.IsNaN(cos))
            //    cos = 1.0;
            unsafe
            {
                // byte* p = (byte*)_bitmap.BackBuffer.ToPointer();


                // Get a pointer to the back buffer.
                var pBackBuffer = (int) bitmap.BackBuffer;

                // Find the address of the pixel to draw.
                pBackBuffer += y * bitmap.BackBufferStride;
                pBackBuffer += x * 4;

                // Assign the color data to the pixel.
                //*((int*) pBackBuffer) = PixelColorLight(cos, (uint) GetTextureAt(x, y));
                  *((int*) pBackBuffer) = GetTextureAt(x, y);
            }
            //var hMap = new byte[4];
            //HeightMap.CopyPixels(new Int32Rect(x, y, 1, 1), hMap, 4, 0);
            //bitmap.WritePixels(new Int32Rect(x,y,1,1), hMap,hMap.Length,0);
            //  bitmap.AddDirtyRect(new Int32Rect(x,y,1,1));
        }

        public int PixelColorLight(double cos, uint color)
        {
            var r = (color >> 16) & 0xff;
            var g = (color >> 08) & 0xff;
            var b = (color >> 00) & 0xff;
            var tR = Math.Max(Math.Min(r * cos * MyLight.R / 255, 255), 0);
            var tG = Math.Max(Math.Min(g * cos * MyLight.G / 255, 255), 0);
            var tB = Math.Max(Math.Min(b * cos * MyLight.B / 255, 255), 0);
            return (byte) tR << (16 + (byte) tG) << (8 + (byte) tB) << 0;
        }

        public void FillPolygon(MyPolygon polygon)
        {
            bitmap.Dispatcher.Invoke(() =>
            {
                bitmap.Lock();
                var et = CreateET(polygon);
                var aet = new List<EdgeStruct>();
                var yMin = polygon.MinY;
                while (aet.Count + et.Count > 0 && yMin <= polygon.MaxY)
                {
                    while (PeekY(et) == yMin - 1)
                    {
                        var tuple = et.Dequeue();
                        var m = (tuple.Item2.X - tuple.Item1.X) / (tuple.Item2.Y - tuple.Item1.Y);
                        var edgeStruct = new EdgeStruct(tuple, m);
                        aet.Add(edgeStruct);
                    }
                    // posortuj liste aet wg x
                    aet.Sort();
                    // wypelnij pixele pomiedzy parami 
                    DrawLineBetweenPairs(aet, yMin);
                    // usuń z AET te elementy, dla których y=ymax
                    aet.RemoveAll(edgeStruct => edgeStruct.Ymax == yMin);
                    // zwieksz y o 1
                    yMin++;
                    // dla kazdej w AET uaktualnij x
                    foreach (var element in aet)
                        element.AddM();
                }
                bitmap.Unlock();
            });
        }

        public class EdgeStruct : IComparer<EdgeStruct>, IComparable<EdgeStruct>
        {
            public double M; // dx/dy
            public double Xmin;
            public int Ymax;

            public EdgeStruct(Tuple<Point, Point> tuple, double m)
            {
                Ymax = (int) tuple.Item2.Y;
                Xmin = tuple.Item1.X;
                M = m;
            }

            public int CompareTo(EdgeStruct other)
            {
                var xCompare = Xmin.CompareTo(other.Xmin);
                if (xCompare == 0)
                    return other.Ymax.CompareTo(Ymax);
                return xCompare;
            }

            public int Compare(EdgeStruct x, EdgeStruct y)
            {
                return x.Xmin.CompareTo(y.Xmin);
            }

            public void AddM()
            {
                Xmin += M;
            }
        }
    }

    public static class LinkedListExtensions
    {
        // stolenborrowed from stack
        public static void RemoveAll<T>(this LinkedList<T> linkedList,
            Func<T, bool> predicate)
        {
            for (var node = linkedList.First; node != null;)
            {
                var next = node.Next;
                if (predicate(node.Value))
                    linkedList.Remove(node);
                node = next;
            }
        }
    }
}