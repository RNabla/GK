using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab_1
{
    internal class Sketch
    {
        public delegate void GuiUpdate(object sender, EventArgs e);

        private const int Width = 1920;
        private const int Height = 1080;
        private const int DpiX = 96;
        private const int DpiY = 96;
        private readonly WriteableBitmap _bitmap;
        private readonly Canvas _canvas;
        private readonly byte[] _clearBuffer = new byte[Height * Width * 3];
        private readonly byte[] _currentContent = new byte[Height * Width * 3];
        private readonly Int32Rect _fullSize = new Int32Rect(0, 0, Width, Height);
        public readonly ObservableCollection<MyShape> _myShapes = new ObservableCollection<MyShape>();
        private readonly List<Point> _points = new List<Point>();
        private readonly Random _rand = new Random();
        private bool _click;
        private bool _isUpdated;
        private Point _start;
        private HashSet<Shape> _guiElements = new HashSet<Shape>();


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
            var rgb = new byte[3];
            _rand.NextBytes(rgb);
            var color = Color.FromRgb(rgb[0], rgb[1], rgb[2]);
            switch (Drawing)
            {   
                case Drawing.Circle:
                    if (_points.Count == 1)
                    {
                        RepaintAll();
                        var p1 = _points[0];
                        var dist = Distance(p1, mouseEventArgs.GetPosition(_canvas));
                        DrawCircle((int) p1.X, (int) p1.Y, dist, color);
                    }
                    break;
                case Drawing.Polygon:
                    if (_points.Count > 0)
                    {
                        RepaintAll();
                        for (var i = 0; i < _points.Count - 1; i++)
                            DrawLine(_points[i], _points[i + 1], color,1);
                        var p1 = _points.Last();
                        var p2 = mouseEventArgs.GetPosition(_canvas);  
                        DrawLine((int) p1.X, (int) p1.Y, (int) p2.X, (int) p2.Y, color,1);
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

        public void DrawLine(Point p1, Point p2, Color color, int thickness)
        {
            DrawLine((int) p1.X, (int) p1.Y,
                (int) p2.X, (int) p2.Y, color, thickness);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color, int thickness)
        {
            _bitmap.Lock();
            if (x2 + y2 < x1 + y1)
            {
                Swap(ref x1, ref x2);
                Swap(ref y1, ref y2);
            }
            if (x2 - x1 >= y2 - y1)
                DrawLineFromMinus45To45(x1, y1, x2, y2, color, thickness);
            else
                DrawLineFrom45To135(x1, y1, x2, y2, color, thickness);
            _bitmap.Unlock();
        }

        private static void Swap(ref int a, ref int b)
        {
            var aux = a;
            a = b;
            b = aux;
        }

        private void DrawLineFrom45To135(int x1, int y1, int x2, int y2, Color color, int thickness)
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
            DrawPixel(xf, yf, color, thickness);
            DrawPixel(xb, yb, color, thickness);

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
                DrawPixel(xf, yf, color, thickness);
                if (yf != yb)
                    DrawPixel(xb, yb, color, thickness);
            }
        }

        private void DrawLineFromMinus45To45(int x1, int y1, int x2, int y2, Color color, int thickness)
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
            DrawPixel(xf, yf, color,thickness);
            DrawPixel(xb, yb, color, thickness);

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
                DrawPixel(xf, yf, color, thickness);
                if (xf != xb)
                    DrawPixel(xb, yb, color, thickness);
            }
        }

        public void DrawCircle(int x, int y, int radius, Color color)
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
            DrawPixel(x + dx, y + dy, color,1);
            DrawPixel(x - dx, y + dy, color,1);
            DrawPixel(x + dx, y - dy, color,1);
            DrawPixel(x - dx, y - dy, color,1);

            DrawPixel(x + dy, y + dx, color,1);
            DrawPixel(x - dy, y + dx, color,1);
            DrawPixel(x + dy, y - dx, color,1);
            DrawPixel(x - dy, y - dx, color,1);
        }

        private void DrawPixel(int x, int y, Color color, int thickness)
        {
            if (thickness > 1)
            {
                DrawPixel2(x,y,color,thickness);
                return;
            }
            if (!(0 <= x && x < Width && 0 <= y && y < Height))
                return;
            var rect = new Int32Rect(x, y, 1, 1);

            var pixels = new[]
            {
                color.R, color.G, color.B
            };
            _bitmap.WritePixels(rect, pixels, pixels.Length, 0);
        }

        private void DrawPixel2(int xCenter, int yCenter, Color color, int thickness)
        {
            //int half = thickness / 2;
            //for (var x = xCenter - half; x <= xCenter + half; x++)
            //{
            //    for (var y = yCenter - half; y <= yCenter + half; y++)
            //    {
            //        if (!(0 <= x && x < Width && 0 <= y && y < Height))
            //            continue;
            //        var rect = new Int32Rect(x, y, 1, 1);
            //        var pixels = new[]
            //        {
            //            color.R, color.G, color.B
            //        };
            //        _bitmap.WritePixels(rect, pixels, pixels.Length, 0);
            //    }
            //}
            int half = thickness / 2;
            for (var x = xCenter - half; x <= xCenter + half; x++)
            {
                if (!(0 <= x && x < Width && 0 <= yCenter && yCenter < Height))
                    continue;
                var rect = new Int32Rect(x, yCenter, 1, 1);
                var pixels = new[]
                {
                    color.R, color.G, color.B
                };
                _bitmap.WritePixels(rect, pixels, pixels.Length, 0);
            }
            for (var y = yCenter - half; y <= yCenter + half; y++)
            {
                if (!(0 <= xCenter && xCenter < Width && 0 <= yCenter && yCenter < Height))
                    continue;
                var rect = new Int32Rect(xCenter, y, 1, 1);
                var pixels = new[]
                {
                    color.R, color.G, color.B
                };
                _bitmap.WritePixels(rect, pixels, pixels.Length, 0);
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
                        DrawLine((int) p1.X, (int) p1.Y, (int) p2.X, (int) p2.Y, Color.FromRgb(255, 255, 255),1);
                    }
                    if (_points.Count == 3)
                        OnFinishAvailable?.Invoke(null, EventArgs.Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

   
        public void AddGuiToCanvas(Shape shape)
        {
            if (_guiElements.Contains(shape))
                return;
            _guiElements.Add(shape);
            _canvas.Children.Add(shape);
        }

        public void DeleteShapeFromCanvas(MyShape myShape)
        {
            if (myShape == null)
                return;
            foreach (var uiElement in myShape.GetUiElements())
                DeleteUiElementFromCanvas(uiElement);
            _myShapes.Remove(myShape);
        }

        public void DeleteUiElementFromCanvas(Shape uiElement)
        {
            _guiElements.Remove(uiElement);
            _canvas.Children.Remove(uiElement);
        }
      
        public void FinishDrawing()
        {
            var rgb = new byte[3];
            _rand.NextBytes(rgb);
            var color = Color.FromRgb(rgb[0], rgb[1], rgb[2]);
            if (Drawing.Polygon == Drawing)
            {
                if (_points.Count > 2)
                {
                    var p1 = _points[_points.Count - 1];
                    var p2 = _points[0];
                    DrawLine((int) p1.X, (int) p1.Y, (int) p2.X, (int) p2.Y, color,1);
                    var poly = new MyPolygon(_points,_canvas, color);
                    poly.ShapeChanged += (o, args) =>
                    {
                        _isUpdated = false;
                        RepaintAll();
                    };
                    poly.ShapeDeleted += (o, args) =>
                    {
                        _isUpdated = false;
                        DeleteShapeFromCanvas(o as MyShape);
                        RepaintAll();
                    };
                    poly.ComponentDeleted += (o, args) =>
                    {
                        _isUpdated = false;
                        DeleteUiElementFromCanvas(o as Shape);
                        RepaintAll();
                    };
                    poly.ComponentAdded += (o, args) =>
                    {
                        _isUpdated = false;
                        AddGuiToCanvas(o as Shape);
                        RepaintAll();

                    };
                    poly.ColorChanged += (o, args) =>
                    {
                        _isUpdated = false;
                        RepaintAll();
                    };
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
                    var circle = new MyCircle((int) p1.X, (int) p1.Y, Distance(p1, p2),this, _canvas, color);
                    circle.ShapeChanged += (o, args) =>
                    {
                        _isUpdated = false;
                        RepaintAll();
                    };
                    circle.ShapeDeleted += (o, args) =>
                    {
                        _isUpdated = false;
                        DeleteShapeFromCanvas(o as MyShape);
                        RepaintAll();
                    };
                    circle.ColorChanged += (o, args) =>
                    {
                        _isUpdated = false;
                        RepaintAll();
                    };
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

        public IEnumerable<MyCircle> GetConcentricOptions(MyCircle myCircle)
        {
            return _myShapes.OfType<MyCircle>().Where(circle => circle != myCircle);
        }
    }

    internal enum Drawing
    {
        None,
        Circle,
        Polygon
    }
}