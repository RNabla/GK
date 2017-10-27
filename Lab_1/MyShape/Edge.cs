using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        public Restriction Restriction = Restriction.None;

        public double LengthRestrictionValue;


        public Line Line;
        public TextBlock TextBlock;
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
                TextBlock = new TextBlock
                {
                    Background =  Brushes.Transparent,
                    Foreground = Brushes.Red
                };
                canvas.Children.Add(TextBlock);
                Panel.SetZIndex(Line, 4);
                Panel.SetZIndex(TextBlock, 4);
                #region thicc

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
                Line.ContextMenu.Items.Add(thicknessMenu);
                #endregion

                #region restr

                var restrictionMenu = new MenuItem
                {
                    Header = "Ustaw ograniczenie",
                };
                var horizontalRestriction = new MenuItem
                {
                    Header = "Pozioma",
                    //IsChecked = Restriction == Restriction.Horizontal
                };
                var verticalRestriction = new MenuItem
                {
                    //IsChecked = Restriction == Restriction.Vertical,
                    Header = "Pionowa"
                };
                var noneRestriction = new MenuItem
                {
                    //IsChecked = Restriction == Restriction.None,
                    Header = "Brak"
                };

                var lengthRestriction = new MenuItem
                {
                    Header = "D³ugoœæ"
                };

                noneRestriction.Click += (sender, args) =>
                {
                    Restriction = Restriction.None;
                    LineChanged?.Invoke(this,null);
                };
                verticalRestriction.Click += (sender, args) =>
                {
                    if (false == SetOrientationRestriction(Restriction.Vertical))
                    {
                        MessageBox.Show("Nie uda³o siê ustawiæ ograniczenia");
                    }
                    LineChanged?.Invoke(this, null);
                };
                horizontalRestriction.Click += (sender, args) =>
                {
                    if (false == SetOrientationRestriction(Restriction.Horizontal))
                    {
                        MessageBox.Show("Nie uda³o siê ustawiæ ograniczenia");
                    }
                    LineChanged?.Invoke(this, null);
                };
                lengthRestriction.Click += (sender, args) =>
                {
                    if (false == SetLengthRestriction(200))
                    {
                        MessageBox.Show("Nie uda³o siê ustawiæ ograniczenia");
                    }
                    LineChanged?.Invoke(this, null);
                };

                restrictionMenu.Items.Add(noneRestriction);
                restrictionMenu.Items.Add(verticalRestriction);
                restrictionMenu.Items.Add(horizontalRestriction);
                restrictionMenu.Items.Add(lengthRestriction);
                Line.ContextMenu.Items.Add(restrictionMenu);

                #endregion
                
                // Line.ContextMenu.Items.Add(setLengthMenu);
                //Line.ContextMenu.Items.Add(item);


                Line.MouseDown += (sender, args) =>
                {
                    if (args.ClickCount > 1)
                    {
                        NewVertex?.Invoke(this, null);
                    }
                };
            }
            Line.X1 = Vertex1.VertexPoint.X;
            Line.Y1 = Vertex1.VertexPoint.Y;
            Line.X2 = Vertex2.VertexPoint.X;
            Line.Y2 = Vertex2.VertexPoint.Y;
            TextBlock.Text = Restriction != Restriction.None ? Restriction.ToString() : "";
            Canvas.SetLeft(TextBlock, (Line.X1 + Line.X2) / 2 + 10);
            Canvas.SetTop(TextBlock, (Line.Y1 + Line.Y2) / 2 + 10);
            return Line;
        }

        private bool SetOrientationRestriction(Restriction restriction)
        {
            if (restriction != Restriction.Vertical && restriction != Restriction.Horizontal)
                return false;

            var fullMask = Restriction.None | (Restriction & Restriction.Length) | restriction;

            if (PreviousEdge.Restriction.HasFlag(restriction) || NextEdge.Restriction.HasFlag(restriction))
                return false;

            if (restriction == Restriction.Vertical && Vertex1.VertexPoint.X == Vertex2.VertexPoint.X)
                goto end;
            
            if (restriction == Restriction.Horizontal && Vertex1.VertexPoint.Y == Vertex2.VertexPoint.Y)
                goto end;

            if (PreviousEdge.Restriction.HasFlag(Restriction.Length) && NextEdge.Restriction.HasFlag(Restriction.Length))
                return false;
            if (PreviousEdge.Restriction.HasFlag(Restriction.Length) || NextEdge.Restriction.HasFlag(Restriction.Length))
            {
                var lengthEdge = PreviousEdge.Restriction.HasFlag(Restriction.Length) ? PreviousEdge : NextEdge;
                var otherEdge = PreviousEdge == lengthEdge ? NextEdge : PreviousEdge;
                if (otherEdge.Restriction.HasFlag(Restriction.Horizontal) ||
                    otherEdge.Restriction.HasFlag(Restriction.Vertical))
                    return false;
            }

                

            var v1 = Vertex1;
            var v2 = Vertex2;

            if (PreviousEdge.Restriction == Restriction.Length || NextEdge.Restriction == Restriction.Length)
            {
                var e1 = PreviousEdge.Restriction.HasFlag(Restriction.Length) ? NextEdge : PreviousEdge;
                if (e1.Restriction.HasFlag(Restriction.Vertical | Restriction.Horizontal))
                    return false;
                if (e1 == NextEdge)
                {
                    v1 = Vertex2;
                    v2 = Vertex1;
                }

            }
            if (restriction == Restriction.Vertical)
                v1.VertexPoint.X = v2.VertexPoint.X;
            if (restriction == Restriction.Horizontal)
                v1.VertexPoint.Y = v2.VertexPoint.Y;
            end:
            Restriction = fullMask;
            //if (Restriction.HasFlag(Restriction.Length))

            return true;
     
        }


        private bool SetLengthRestriction(double length)
        {
            if (Math.Abs(Length - length) < 1.5)
                return true;

            if (PreviousEdge.Restriction.HasFlag(Restriction.Length) && NextEdge.Restriction.HasFlag(Restriction.Length))
                return false;
            if (PreviousEdge.Restriction.HasFlag(Restriction.Length) || NextEdge.Restriction.HasFlag(Restriction.Length))
            {
                var lengthEdge = PreviousEdge.Restriction.HasFlag(Restriction.Length) ? PreviousEdge : NextEdge;
                var otherEdge = PreviousEdge == lengthEdge ? NextEdge : PreviousEdge;
                if (otherEdge.Restriction.HasFlag(Restriction.Horizontal) ||
                    otherEdge.Restriction.HasFlag(Restriction.Vertical))
                    return false;
            }

            if (PreviousEdge.Restriction.HasFlag(Restriction.Vertical) &&
                NextEdge.Restriction.HasFlag(Restriction.Horizontal))
                return false;
            if (PreviousEdge.Restriction.HasFlag(Restriction.Horizontal) &&
                NextEdge.Restriction.HasFlag(Restriction.Vertical))
                return false;

            if (Restriction.HasFlag(Restriction.None))
            {
                var v = Vertex1.VertexPoint - Vertex2.VertexPoint;
                v.Normalize();
                Vertex2.VertexPoint = Vertex1.VertexPoint - v * length;
                Restriction |= Restriction.Length;
                LengthRestrictionValue = length;
                return true;
            }
            return false;
        }

        private bool IsAtleastOneRestrictionSetOnSides(Restriction restriction)
        {
            var next = NextEdge.Restriction & restriction;
            var prev = PreviousEdge.Restriction & restriction;

            return prev == restriction || next == restriction;
        }

        private double Length
        {
            get
            {
                var dx = Vertex1.VertexPoint.X - Vertex2.VertexPoint.X;
                var dy = Vertex1.VertexPoint.Y - Vertex2.VertexPoint.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
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
    [Flags]
    internal enum Restriction
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        Length = 4
    }
}