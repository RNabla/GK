using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab_1
{
    internal class Vertex
    {
        public Point VertexPoint;
        public Edge Edge1;
        public Edge Edge2;

        






        public Ellipse Ellipse = null;
        private bool _isHeld = false;
        private Canvas _canvas;
        private bool isDragging;
        private Point clickPosition;
        private bool _isCenter;
        private MyShape _myShape;
        public Vertex(Point point, Canvas canvas, MyShape myShape, bool isCenter = false)
        {
            _myShape = myShape;
            _canvas = canvas;
            VertexPoint = point;
            _isCenter = isCenter;
        }
        public delegate void VertexChangedHandler(object obj, RoutedEventArgs e);
        public event VertexChangedHandler VertexChanged;
        public event VertexChangedHandler VertexDeleted;
        public override string ToString()
        {
            return $"({(int)VertexPoint.X},{(int)VertexPoint.Y})";
        }

       
        public Ellipse GetUiElement(Canvas canvas)
        {
            if (Ellipse == null)
            {
                Ellipse = new Ellipse
                {
                    Height = 12,
                    Width = 12,
                    Stroke = _isCenter ? Brushes.Red : Brushes.White ,
                    Fill = _isCenter ? Brushes.Red : Brushes.White,
                    Visibility = Visibility.Visible
                };
                Panel.SetZIndex(Ellipse, 5);
                Ellipse.MouseMove += (sender, args) =>
                {
                    if (args.LeftButton == MouseButtonState.Pressed)
                    {
                        var point = args.GetPosition(canvas);
                        var x = point.X;
                        var y = point.Y;

                        if (false == _isCenter)
                        {
                            if (Edge1.Restriction.HasFlag(Restriction.Length) && Edge2.Restriction.HasFlag(Restriction.Length))
                                return;
                            if (Edge1.Restriction.HasFlag(Restriction.Horizontal) &&  
                                Edge2.Restriction.HasFlag(Restriction.Vertical))
                                return;
                            if (Edge2.Restriction.HasFlag(Restriction.Horizontal) &&
                                Edge1.Restriction.HasFlag(Restriction.Vertical))
                                return;
                            if (Edge1.Restriction.HasFlag(Restriction.Horizontal | Restriction.Vertical) &&
                                Edge2.Restriction.HasFlag(Restriction.Length))
                                return;
                            if (Edge2.Restriction.HasFlag(Restriction.Horizontal | Restriction.Vertical) &&
                                Edge1.Restriction.HasFlag(Restriction.Length))
                                return;


                            if (Edge1.Restriction.HasFlag(Restriction.Horizontal) || Edge2.Restriction.HasFlag(Restriction.Horizontal))
                                y = VertexPoint.Y;
                            if (Edge1.Restriction.HasFlag(Restriction.Vertical) || Edge2.Restriction.HasFlag(Restriction.Vertical))
                                x = VertexPoint.X;
                            if (Edge1.Restriction.HasFlag(Restriction.Length) || Edge2.Restriction.HasFlag(Restriction.Length))
                            {
                                if (Edge1.Restriction.HasFlag(Restriction.Horizontal) || Edge2.Restriction.HasFlag(Restriction.Horizontal))
                                    return;
                                if (Edge1.Restriction.HasFlag(Restriction.Vertical) || Edge2.Restriction.HasFlag(Restriction.Vertical))
                                    return;

                                var start = Edge1.Restriction.HasFlag(Restriction.Length) ? Edge1.Vertex1 : Edge2.Vertex2;
                                var length = Edge1.Restriction.HasFlag(Restriction.Length) ? Edge1.LengthRestrictionValue : Edge2.LengthRestrictionValue;
                                var vector = point - start.VertexPoint;
                                vector.Normalize();
                                vector *= length;
                                var end = start.VertexPoint + vector;
                                x = end.X;
                                y = end.Y;
                            }
                        }
                        

                        var dx = x - VertexPoint.X;
                        var dy = y - VertexPoint.Y;
                        //if (Math.Abs(dx * dx + dy * dy) >= 2)
                        //{
                            
                            VertexPoint.X = x;
                            VertexPoint.Y = y;
                            if (_myShape is MyCircle circle)
                            {
                                circle.SetNewPositionForConcentric(new Point(x, y));
                            }
                            if (_isCenter)
                            {
                                if (_myShape is MyPolygon myPolygon)
                                {
                                    foreach (var vertice in myPolygon._vertices)
                                    {
                                        vertice.VertexPoint.X += dx;
                                        vertice.VertexPoint.Y += dy;
                                    }
                                }
                            }
                            VertexChanged?.Invoke(this, null);
                        //}
                    }
                };

                Ellipse.MouseDown += (sender, args) =>
                {
                    if (args.ClickCount > 1)
                    {
                        VertexDeleted?.Invoke(this, null);
                    }
                };
                //var myCircle = _myShape as MyCircle;
                if (_myShape is MyCircle myCircle)
                {
                    Ellipse.ContextMenu = new ContextMenu();
                    var item1 = new MenuItem
                    {
                        Header = "Ustaw promień"
                    };
                    item1.Click += (obj, args) =>
                    {
                        var result = new RadiusPrompt(myCircle).ShowDialog();
                        if (result.HasValue && result.Value)
                        {
                            VertexChanged?.Invoke(this, null);
                        }

                    };
                    var item2 = new MenuItem
                    {
                        Header = "Ustaw koncentryczność"
                    };
                    item2.Click += (sender, args) =>
                    {
                        var circles = myCircle.GetOtherCircles();
                        var window = new ConcentricPrompt(circles);
                        var result = window.ShowDialog();
                        if (result.HasValue && result.Value)
                        {
                            if (window.Selected is MyCircle selectedCircle)
                            {
                                myCircle.Center.VertexPoint = selectedCircle.Center.VertexPoint;
                                MyCircle.BoundConcentric(myCircle, selectedCircle, selectedCircle.Center.VertexPoint);
                                myCircle.ColorChangedEvent();
                            }
                            else
                            {
                                MyCircle.DisboundConcentric(myCircle);
                            }
                        }

                    };

                    Ellipse.ContextMenu.Items.Add(item1);
                    Ellipse.ContextMenu.Items.Add(item2);
                }

            }
            Canvas.SetLeft(Ellipse, VertexPoint.X - 6);
            Canvas.SetTop(Ellipse, VertexPoint.Y - 6);
            return Ellipse;
        }
    }
}