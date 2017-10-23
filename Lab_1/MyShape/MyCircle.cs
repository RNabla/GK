using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab_1
{
    internal class MyCircle : MyShape
    {
        private static int _id = 1;
        private readonly Canvas _canvas;
        public int Radius;

        public MyCircle(int x, int y, int radius, Canvas canvas, Color color) : base(color)
        {
            Name = $"Circle {_id++}";
            _canvas = canvas;
            Radius = radius;
            Center = new Vertex(new Point(x, y), canvas, this, true);
            Center.VertexChanged += (obj, args) => { ShapeChanged?.Invoke(this, null); };
            Center.VertexDeleted += (o, args) => { ShapeDeleted?.Invoke(this, null); };
        }

        public override void Draw(Sketch sketch)
        {
            var x = (int) Center.VertexPoint.X;
            var y = (int) Center.VertexPoint.Y;
            sketch.DrawCircle(x, y, Radius, Color);
            var centerUi = Center.GetUiElement(_canvas);
            sketch.AddGuiToCanvas(centerUi);
        }

        public override IEnumerable<Shape> GetUiElements()
        {
            yield return Center.GetUiElement(_canvas);
        }

        public override event ShapeChangedHandler ShapeChanged;
        public override event ShapeChangedHandler ShapeDeleted;
        public override event ShapeChangedHandler ComponentDeleted;
        public override event ShapeChangedHandler ComponentAdded;
        public override event ShapeChangedHandler ColorChanged;

        public override string ToString()
        {
            return Name;
        }

        public override void ColorChangedEvent()
        {
            ColorChanged?.Invoke(this, null);
        }
    }
}