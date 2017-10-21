using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab_1
{
    internal class Sketch
    {
        public delegate void GuiUpdate(object sender, EventArgs e);

        private const int Width = 1280;
        private const int Height = 720;
        private const int DpiX = 96;
        private const int DpiY = 96;
        private readonly WriteableBitmap _bitmap;
        private readonly Canvas _canvas;
        private readonly Int32Rect _fullSize = new Int32Rect(0, 0, Width, Height);
        private readonly List<MyShape> _myShapes = new List<MyShape>();
        private readonly List<Point> _points = new List<Point>();
        private readonly Random _rand = new Random();
        private readonly byte[] _clearBuffer = new byte[Height * Width * 3];
        private bool _click;
        private readonly byte[] _currentContent = new byte[Height * Width * 3];
        private bool _isUpdated;
        private Point _start;


        public Sketch(Canvas canvasContext, Image imageContext)
        {
            _canvas = canvasContext;
            _bitmap = new WriteableBitmap(Width, Height, DpiX, DpiY, PixelFormats.Rgb24, null);
            imageContext.Source = _bitmap;
            _canvas.MouseDown += ClickHandler;
            _canvas.MouseDown += TestHandler;
            _canvas.MouseMove += DrawPreview;
            imageContext.MouseDown += TestHandler;
            Drawing = Drawing.None;
        }

        public Drawing Drawing { get; set; }

        public event GuiUpdate OnDrawingFinished;
        public event GuiUpdate OnDrawingCanceled;
        public event GuiUpdate OnFinishAvailable;

        private void DrawPreview(object sender, MouseEventArgs mouseEventArgs)
        {
            switch (Drawing)
            {
                case Drawing.Circle:
                    if (_points.Count == 1)
                    {
                        RepaintAll();
                        var p1 = _points[0];
                        var dist = Distance(p1, mouseEventArgs.GetPosition(_canvas));
                        DrawCircle((int) p1.X, (int) p1.Y, dist, Color.FromRgb(0xd3, 0xd3, 0xd3));
                    }
                    break;
                case Drawing.Polygon:
                    if (_points.Count > 0)
                    {
                        var color = Color.FromRgb(0xd3, 0xd3, 0xd3);
                        RepaintAll();
                        for (var i = 0; i < _points.Count - 1; i++)
                            DrawLine(_points[i], _points[i + 1], color);

                        var p1 = _points.Last();
                        var p2 = mouseEventArgs.GetPosition(_canvas);
                        DrawLine((int) p1.X, (int) p1.Y, (int) p2.X, (int) p2.Y, color);
                    }
                    break;
            }
        }

        private static int Distance(Point p1, Point p2)
        {
            var dx = p1.X - p2.X;
            var dy = p1.Y - p2.Y;
            return (int) Math.Sqrt(dx * dx + dy * dy);
        }

        public void TestHandler(object obj, MouseEventArgs e)
        {
            //  MessageBox.Show($"<CLICK> on {obj}");
        }

        private void ClearBitmap()
        {
            _bitmap.WritePixels(
                _fullSize,
                _isUpdated == false ? _clearBuffer : _currentContent,
                3 * Width,
                0);
        }

        private void UpdateCurrentContent()
        {
            if (_isUpdated)
                return;
            _bitmap.CopyPixels(_fullSize, _currentContent, 3 * Width, 0);
            _isUpdated = true;
        }

        private void RepaintAll()
        {
            _bitmap.Lock();
            ClearBitmap();
            // TODO: draw all figures
            foreach (var myShape in _myShapes)
                myShape.Draw(this);
            _bitmap.Unlock();
        }

        public void DrawLine(Point p1, Point p2, Color color)
        {
            DrawLine((int) p1.X, (int) p1.Y,
                (int) p2.X, (int) p2.Y, color);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            _bitmap.Lock();
            if (x2 + y2 < x1 + y1) // punkty leżące poniżej prostej y = -x + B + A, gdzie A = x1, B = y1
            {
                Swap(ref x1, ref x2);
                Swap(ref y1, ref y2);
            }
            if (x2 - x1 >= y2 - y1)
                DrawLineFromMinus45To45(x1, y1, x2, y2, color);
            else
                DrawLineFrom45To135(x1, y1, x2, y2, color);
            _bitmap.Unlock();
        }

        private static void Swap(ref int a, ref int b)
        {
            var aux = a;
            a = b;
            b = aux;
        }

        private void DrawLineFrom45To135(int x1, int y1, int x2, int y2, Color color)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;

            var reverse = x1 > x2;
            if (reverse)
                dx *= -1;
            var d = 2 * dx - dy;
            var incrN = 2 * dx;
            var incrNE = 2 * (dx - dy);

            var xf = x1;
            var yf = y1;
            var xb = x2;
            var yb = y2;
            DrawPixel(xf, yf, color);
            DrawPixel(xb, yb, color);

            while (yf < yb)
            {
                yf++;
                yb--;
                if (d < 0) //Choose E and W
                {
                    d += incrN;
                }
                else //Choose NE and SW
                {
                    d += incrNE;
                    if (!reverse)
                    {
                        xf++;
                        xb--;
                    }
                    else
                    {
                        xf--;
                        xb++;
                    }
                }
                DrawPixel(xf, yf, color);
                if (yf != yb)
                    DrawPixel(xb, yb, color);
            }
        }

        private void DrawLineFromMinus45To45(int x1, int y1, int x2, int y2, Color color)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;

            var reverse = y1 > y2;
            if (reverse)
                dy *= -1;
            var d = 2 * dy - dx;
            var incrE = 2 * dy;
            var incrNE = 2 * (dy - dx);

            var xf = x1;
            var yf = y1;
            var xb = x2;
            var yb = y2;
            DrawPixel(xf, yf, color);
            DrawPixel(xb, yb, color);

            while (xf < xb)
            {
                xf++;
                xb--;
                if (d < 0) //Choose E and W
                {
                    d += incrE;
                }
                else //Choose NE and SW
                {
                    d += incrNE;
                    if (!reverse)
                    {
                        yf++;
                        yb--;
                    }
                    else
                    {
                        yf--;
                        yb++;
                    }
                }
                DrawPixel(xf, yf, color);
                if (xf != xb)
                    DrawPixel(xb, yb, color);
            }
        }

        public void DrawCircle(int x, int y, int radius, Color color, int thickness)
        {
            var middle = thickness / 2;
            var bot = Math.Max(0, radius - middle);
            var top = radius + middle;
            for (var i = bot; i <= top; i++)
                DrawCircle(x, y, i, color);
        }

        private void DrawCircle(int x, int y, int radius, Color color)
        {
            _bitmap.Lock();
            var deltaE = 3;
            var deltaSE = 5 - 2 * radius;
            var d = 1 - radius;
            var xr = 0;
            var yd = radius;
            DrawSymmetricalAround(x, y, xr, yd, color);
            while (yd > xr)
            {
                if (d < 0)
                {
                    d += deltaE;
                    deltaSE += 2;
                }
                else
                {
                    d += deltaSE;
                    deltaSE += 4;
                    yd--;
                }
                deltaE += 2;
                xr++;
                DrawSymmetricalAround(x, y, xr, yd, color);
            }
            _bitmap.Unlock();
        }

        private void DrawSymmetricalAround(int x, int y, int dx, int dy, Color color)
        {
            DrawPixel(x + dx, y + dy, color);
            DrawPixel(x - dx, y + dy, color);
            DrawPixel(x + dx, y - dy, color);
            DrawPixel(x - dx, y - dy, color);

            DrawPixel(x + dy, y + dx, color);
            DrawPixel(x - dy, y + dx, color);
            DrawPixel(x + dy, y - dx, color);
            DrawPixel(x - dy, y - dx, color);
        }

        private void DrawPixel(int x, int y, Color color)
        {
            if (!(0 <= x && x < Width && 0 <= y && y < Height))
                return;
            var rect = new Int32Rect(x, y, 1, 1);

            var pixels = new[]
            {
                color.R, color.G, color.B
            };
            _bitmap.WritePixels(rect, pixels, pixels.Length, 0);
        }

        public void DrawPolygonClick(object sender, MouseButtonEventArgs e)
        {
            _click ^= true;
            if (_click)
            {
                _start = e.GetPosition(_canvas);
            }
            else
            {
                var pos = e.GetPosition(_canvas);
                DrawLine((int) _start.X, (int) _start.Y, (int) pos.X, (int) pos.Y, Color.FromRgb(255, 0, 0));
            }
        }

        public void ClickHandler(object sender, MouseEventArgs e)
        {
            if (Drawing == Drawing.None)
                return;
            _points.Add(e.GetPosition(_canvas));
            switch (Drawing)
            {
                case Drawing.None:
                    break;
                case Drawing.Circle:
                    if (_points.Count == 2)
                        FinishDrawing();
                    break;
                case Drawing.Polygon:
                    if (_points.Count > 1)
                    {
                        var p1 = _points[_points.Count - 2];
                        var p2 = _points[_points.Count - 1];
                        DrawLine((int) p1.X, (int) p1.Y, (int) p2.X, (int) p2.Y, Color.FromRgb(255, 255, 255));
                    }
                    if (_points.Count == 3)
                    {
                        OnFinishAvailable?.Invoke(null, EventArgs.Empty);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void KeyHitHandler(object sender, KeyEventArgs e)
        {
            return;
            if (e.Key == Key.Z && Drawing == Drawing.Polygon)
            {
                if (_points.Count > 1)
                    _points.RemoveAt(_points.Count - 1);
                return;
            }
            if (e.Key == Key.Enter)
            {
            }
        }

        public void FinishDrawing()
        {
            var rgb = new byte[3];
            _rand.NextBytes(rgb);
            var color= Color.FromRgb(rgb[0], rgb[1], rgb[2]);
            if (Drawing.Polygon == Drawing)
            {
                if (_points.Count > 2)
                {
                    var p1 = _points[_points.Count - 1];
                    var p2 = _points[0];
                    DrawLine((int) p1.X, (int) p1.Y, (int) p2.X, (int) p2.Y, color);
                    var poly = new MyPolygon(_points,color,1);
                    _myShapes.Add(poly);
                    _isUpdated = false;
                }
            }
            else if (Drawing.Circle == Drawing)
            {
                if (_points.Count == 2)
                {
                    var p1 = _points[0];
                    var p2 = _points[1];
                    var circle = new MyCircle((int) p1.X, (int) p1.Y, Distance(p1, p2),
                        color);
                    _myShapes.Add(circle);
                    _isUpdated = false;
                }
            }
            CancelDrawing();
            OnDrawingFinished?.Invoke(null, null);
        }

        public void CancelDrawing()
        {
            _points.Clear();
            RepaintAll();
            UpdateCurrentContent();
            OnDrawingCanceled?.Invoke(null, EventArgs.Empty);
            Drawing = Drawing.None;
        }
    }

    internal enum Drawing
    {
        None,
        Circle,
        Polygon
    }
}