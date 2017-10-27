using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab_1
{
    internal class MyPolygon : MyShape
    {
        private readonly Canvas _canvas;
        public readonly List<Edge> _edges = new List<Edge>();
        public readonly List<Vertex> _vertices = new List<Vertex>();
        private static int _id = 1;
        public MyPolygon(List<Point> points, Canvas canvas, Color color) : base(color)
        {
            Name = $"Polygon {_id++}";
            _canvas = canvas;
            var n = points.Count;
            for (var i = 0; i < n; i++)
            {
                var vertex = new Vertex(points[i], canvas, this);
                vertex.VertexChanged += VertexChangedHandler;
                vertex.VertexDeleted += VertexDeletedHandler;
                _vertices.Add(vertex);
            }

            for (int i = n - 1, j = 0;
                j < points.Count;
                i = j, j++)
            {
                var edge = new Edge(_vertices[i], _vertices[j], this);
                edge.NewVertex += NewVertexHandler;
                edge.LineChanged += DefaultShapeChangedHandler;
                _edges.Add(edge);
            }
            for (var i = 0; i < n; i++)
            {
                _edges[i].NextEdge = _edges[(i + 1) % n];
                _edges[i].PreviousEdge = _edges[(i - 1 + n) % n];
                _vertices[i].Edge1 = _edges[i];
                _vertices[i].Edge2 = _edges[(i + 1 + n) % n];
            }
            var dx = points.Sum(point => point.X);
            var dy = points.Sum(point => point.Y);
            Center = new Vertex(new Point(dx / n, dy / n), canvas, this, true);
            Center.VertexChanged += VertexChangedHandler;
            Center.VertexDeleted += VertexDeletedHandler;
        }

        private void DefaultShapeChangedHandler(object sender, RoutedEventArgs args)
        {
            ShapeChanged?.Invoke(this, null);
        }
        public void NewVertexHandler(object sender, RoutedEventArgs args)
        {
            var e = sender as Edge;
            e.Restriction = Restriction.None;

            var dx = (e.Vertex1.VertexPoint.X + e.Vertex2.VertexPoint.X) / 2;
            var dy = (e.Vertex1.VertexPoint.Y + e.Vertex2.VertexPoint.Y) / 2;
            var newVertex = new Vertex(new Point(dx, dy), _canvas, this);

            newVertex.VertexChanged += VertexChangedHandler;
            newVertex.VertexDeleted += VertexDeletedHandler;

            var edge1 = new Edge(e.Vertex1, newVertex, this);
            var edge2 = new Edge(newVertex, e.Vertex2, this);
            edge1.PreviousEdge = e.PreviousEdge;
            
            edge1.NextEdge = edge2;
            edge2.PreviousEdge = edge1;
            edge2.NextEdge = e.NextEdge;

            //e.PreviousEdge.Vertex2 = newVertex;
            //e.NextEdge.Vertex1 = newVertex;

            edge1.NewVertex += NewVertexHandler;
            edge2.NewVertex += NewVertexHandler;
            edge1.LineChanged += DefaultShapeChangedHandler;
            edge2.LineChanged += DefaultShapeChangedHandler;

            newVertex.Edge1 = edge1;
            newVertex.Edge2 = edge2;


            e.Vertex1.Edge2 = edge1;
            e.Vertex2.Edge1 = edge2;

            _vertices.Add(newVertex);
            _edges.Add(edge1);
            _edges.Add(edge2);
            _edges.Remove(e);
            ComponentAdded?.Invoke(newVertex.GetUiElement(_canvas), null);
            ComponentAdded?.Invoke(edge1.GetUiElement(_canvas), null);
            ComponentAdded?.Invoke(edge2.GetUiElement(_canvas), null);
            ComponentDeleted?.Invoke(e.GetUiElement(_canvas), null);
        }

        public void VertexChangedHandler(object sender, RoutedEventArgs args)
        {
            ShapeChanged?.Invoke(this, null);
        }

        public void VertexDeletedHandler(object sender, RoutedEventArgs args)
        {
            if (sender is Vertex v)
            {
                if (_vertices.Count == 3 || v.Equals(Center))
                {
                    ShapeDeleted?.Invoke(this, null);
                    foreach (var edge in _edges)
                        edge.TextBlock.Text = "";
                    return;
                }
                var newEdge = new Edge(v.Edge1.Vertex1, v.Edge2.Vertex2, this)
                {
                    PreviousEdge = v.Edge1.PreviousEdge,
                    NextEdge = v.Edge2.NextEdge
                };
                newEdge.NewVertex += NewVertexHandler;
                newEdge.LineChanged += DefaultShapeChangedHandler;
                v.Edge1.Vertex1.Edge2 = newEdge;
                v.Edge2.Vertex2.Edge1 = newEdge;
                _vertices.Remove(v);
                var sumX = _vertices.Sum(point => point.VertexPoint.X);
                var sumY = _vertices.Sum(point => point.VertexPoint.Y);
                Center.VertexPoint.X = sumX / _vertices.Count;
                Center.VertexPoint.Y = sumY / _vertices.Count;


                v.Edge1.Restriction = v.Edge2.Restriction = Restriction.None;
                _edges.Remove(v.Edge1);
                _edges.Remove(v.Edge2);
                _edges.Add(newEdge);
                ComponentAdded?.Invoke(newEdge.GetUiElement(_canvas), null);
                ComponentDeleted?.Invoke(v.Edge1.GetUiElement(_canvas), null);
                ComponentDeleted?.Invoke(v.Edge2.GetUiElement(_canvas), null);
                ComponentDeleted?.Invoke(v.GetUiElement(_canvas), null);
            }
        }

        public override void Draw(Sketch sketch)
        {
            foreach (var edge in _edges)
            {
                sketch.DrawLine(edge.Vertex1.VertexPoint, edge.Vertex2.VertexPoint, Color,edge.Thickness);
                var shapeUi = edge.GetUiElement(_canvas);
                sketch.AddGuiToCanvas(shapeUi);
            }
            foreach (var vertice in _vertices)
            {
                var shapeUi = vertice.GetUiElement(_canvas);
                sketch.AddGuiToCanvas(shapeUi);
            }
            RecalculateCenter();
            var centerUi = Center.GetUiElement(_canvas);
            sketch.AddGuiToCanvas(centerUi);
        }

        private void RecalculateCenter()
        {
            var x = _vertices.Average(vertex => vertex.VertexPoint.X);
            var y = _vertices.Average(vertex => vertex.VertexPoint.Y);
            Center.VertexPoint.X = x;
            Center.VertexPoint.Y = y;
        }

        public override IEnumerable<Shape> GetUiElements()
        {
            foreach (var vertex in _vertices)
                yield return vertex.GetUiElement(_canvas);
            foreach (var edge in _edges)
                yield return edge.GetUiElement(_canvas);
            yield return Center.GetUiElement(_canvas);
        }

        public override string ToString()
        {
            return Name;
        }

        public override event ShapeChangedHandler ShapeChanged;
        public override event ShapeChangedHandler ShapeDeleted;
        public override event ShapeChangedHandler ComponentDeleted;
        public override event ShapeChangedHandler ComponentAdded;
        public override event ShapeChangedHandler ColorChanged;

        public override void ColorChangedEvent()
        {
            ColorChanged?.Invoke(this,null);
        }
    }
}