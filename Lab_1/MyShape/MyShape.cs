using System.Windows;
using System.Windows.Media;

namespace Lab_1
{
    internal abstract class MyShape
    {
        protected Point Center;
        protected Color Color;
        protected int Thickness;
        public abstract void Draw(Sketch sketch);

        protected MyShape(Color color, int thickness)
        {
            Color = color;
            Thickness = thickness;
        }
    }

    internal class Vertex
    {
        public Point VertexPoint;

        public Vertex(Point point)
        {
            VertexPoint = point;
        }
    }

    internal class Edge
    {
        public Vertex Vertex1;
        public Vertex Vertex2;
        private Edge _previousEdge;
        private Edge _nextEdge;
        public Edge(Vertex previous, Vertex next)
        {
            Vertex1 = previous;
            Vertex2 = next;
        }
    }
}
