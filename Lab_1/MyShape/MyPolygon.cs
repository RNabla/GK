using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Lab_1
{
    class MyPolygon : MyShape
    {
        private readonly List<Vertex> _vertices = new List<Vertex>();
        private readonly List<Edge> _edges = new List<Edge>();
        public MyPolygon(List<Point> points, Color color, int thickness) : base(color, thickness)
        {
            _vertices.AddRange(points.Select(point => new Vertex(point)));   
            

            for (int i = points.Count - 1, j = 0;
                j < points.Count;
                i = j, j++)
            {
                _edges.Add(new Edge(_vertices[i], _vertices[j]));
            }
        }

        public override void Draw(Sketch sketch)
        {
            foreach (var edge in _edges)
            {
                sketch.DrawLine(edge.Vertex1.VertexPoint, edge.Vertex2.VertexPoint, Color);
            }
        }
    }
}
