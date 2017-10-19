using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab_1
{
    internal class Sketch
    {
        private const int Width = 1280;
        private const int Height = 720;
        private const int DpiX = 96;
        private const int DpiY = 96;
        private readonly WriteableBitmap _bitmap;
        private readonly Canvas _canvas;
        private bool _click;
        private Point _start;
        public Drawing Drawing { get; set; }
        private readonly List<Point> _points = new List<Point>();

        //public Sketch(Canvas canvasContext)
        //{
        //    _canvas = canvasContext;
        //    _bitmap = new WriteableBitmap(Width, Height, DpiX, DpiY, PixelFormats.Rgb24, null);
        //    _canvas.Background = new ImageBrush(_bitmap);
        //}

        public Sketch(Canvas canvasContext, Image imageContext)
        {
            _canvas = canvasContext;
            _bitmap = new WriteableBitmap(Width, Height, DpiX, DpiY, PixelFormats.Rgb24, null);
            //_canvas.Background = new ImageBrush(_bitmap);
            imageContext.Source = _bitmap;
            _canvas.MouseDown += ClickHandler;
            Drawing = Drawing.None;
        }

        public void RepaintAll()
        {
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            _bitmap.Lock();
            if (y2 < x1 + y1 - x2) // punkty leżące poniżej prostej y = -x + B + A, gdzie A = x1, B = y1
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

        public void DrawCircle(int x, int y, int radius, Color color, int thickness = 1)
        {
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
        }

        public void DrawSymmetricalAround(int x, int y, int dx, int dy, Color color)
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

        public void DrawPixel(int x, int y, Color color)
        {
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
                    // new Circle (default radius)
                    Drawing = Drawing.None;
                    break;
                case Drawing.Polygon:
                    if (_points.Count > 1)
                    {
                        var p1 = _points[_points.Count - 2];
                        var p2 = _points[_points.Count - 1];
                        DrawLine((int)p1.X,(int)p1.Y,(int)p2.X,(int)p2.Y,Color.FromRgb(255,255,255));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void KeyHitHandler(object sender, KeyEventArgs e)
        {
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
            if (Drawing.Polygon == Drawing)
            {
                if (_points.Count > 2)
                {
                    var p1 = _points[_points.Count - 1];
                    var p2 = _points[0];
                    DrawLine((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, Color.FromRgb(255, 255, 255));
                }
                _points.Clear();
            }
        }

        public void CancelDrawing()
        {
            _points.Clear();
        }
    }

    internal enum Drawing
    {
        None,
        Circle,
        Polygon
    }
}