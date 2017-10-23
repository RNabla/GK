using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab_1
{
    internal class Edge
    {
        public Vertex Vertex1;
        public Vertex Vertex2;
        public Edge PreviousEdge;
        public Edge NextEdge;





        public Line Line;
        public int Thickness;
        private MyShape _myShape;
        public delegate void LineChangedHandler(object obj, RoutedEventArgs e);
        public event LineChangedHandler LineChanged;
        public event LineChangedHandler NewVertex;
        public Edge(Vertex previous, Vertex next, MyShape myShape)
        {
            _myShape = myShape;
            Vertex1 = previous;
            Vertex2 = next;
        }
        public override string ToString()
        {
            return $"[{Vertex1} -> {Vertex2}]";
        }

        public Line GetUiElement(Canvas canvas)
        {
            if (Line == null)
            {
                
                Line = new Line
                {
                    Stroke = Brushes.Transparent,
                    Fill = Brushes.Transparent,
                    Visibility = Visibility.Visible,
                    X1 = Vertex1.VertexPoint.X,
                    Y1 = Vertex1.VertexPoint.Y,
                    X2 = Vertex2.VertexPoint.X,
                    Y2 = Vertex2.VertexPoint.Y,
                    StrokeThickness = 4,
                    ContextMenu = new ContextMenu()
                };
                Panel.SetZIndex(Line, 4);
                var thicknessMenu = new MenuItem
                {
                    Header = "Thickness"
                };
                var thickness1 = new MenuItem
                {
                    Header = "1 px"
                };
                var thickness3 = new MenuItem
                {
                    Header = "3 px"
                };
                var thickness5 = new MenuItem
                {
                    Header = "5 px"
                };
                var thickness7 = new MenuItem
                {
                    Header = "7 px"
                };
                thickness1.Click += ThicknessMenuItemHandler;
                thickness3.Click += ThicknessMenuItemHandler;
                thickness5.Click += ThicknessMenuItemHandler;
                thickness7.Click += ThicknessMenuItemHandler;


                thicknessMenu.Items.Add(thickness1);
                thicknessMenu.Items.Add(thickness3);
                thicknessMenu.Items.Add(thickness5);
                thicknessMenu.Items.Add(thickness7);

                //var setLengthMenu = new MenuItem()
                //{
                //    Header = "Ustaw d³ugoœæ"
                //};
                //setLengthMenu.Click += (obj,args) =>
                //{
                //    var x1 = Vertex1.VertexPoint.X;
                //    var x2 = Vertex2.VertexPoint.X;
                //    var y1 = Vertex1.VertexPoint.Y;
                //    var y2 = Vertex2.VertexPoint.Y;


                //    var midX = (x1+x2) / 2;
                //    var midY = (y1+y2) / 2;

                //    var length = 300; // prompted
                //    var midPoint = new Point(midX,midY);

                //    var rightVect = new Vector(midX-x1,midY-y1);
                //    rightVect.Normalize();
                //    rightVect *= length/2;

                //    var leftVect = new Vector(midX-x2,midY-y2);
                //    leftVect.Normalize();
                //    leftVect *= length/2;

                //    Vertex1.VertexPoint = Point.Add(midPoint, rightVect);
                //    Vertex2.VertexPoint = Point.Add(midPoint, leftVect);
                //    LineChanged?.Invoke(this, null);

                //};
                Line.ContextMenu.Items.Add(thicknessMenu);
               // Line.ContextMenu.Items.Add(setLengthMenu);
                //Line.ContextMenu.Items.Add(item);

               
                Line.MouseDown += (sender, args) =>
                {
                    if (args.ClickCount > 1)
                    {
                        NewVertex?.Invoke(this,null);
                    }
                };
                //Line.MouseUp += (sender, args) =>
                //{
                //    //_isHeld = false;
                //    args.GetPosition(Line);
                //    MessageBox.Show($"drag o {args.GetPosition(Line)}");
                //};
                //Line.MouseMove += (sender, args) =>
                //{
                //    //var left = Canvas.GetLeft(Line);
                //    //var top = Canvas.GetTop(Line);

                //    //Canvas.SetRight(Line, args.GetPosition(canvas).X - 3);
                //    //Canvas.SetRight(Line, args.GetPosition(canvas).Y - 3);
                //};
            }
            Line.X1 = Vertex1.VertexPoint.X;
            Line.Y1 = Vertex1.VertexPoint.Y;
            Line.X2 = Vertex2.VertexPoint.X;
            Line.Y2 = Vertex2.VertexPoint.Y;
            
            return Line;
        }

        private void ThicknessMenuItemHandler(object sender, RoutedEventArgs args)
        {
            if (sender is MenuItem mi)
            {
                var thickness = Convert.ToInt32(mi.Header.ToString()[0]-'0');
                Thickness = thickness;
                LineChanged?.Invoke(this,null);
            }
        }
    }
}