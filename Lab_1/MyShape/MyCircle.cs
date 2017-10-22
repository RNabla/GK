using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab_1
{
    internal class MyCircle : MyShape
    {
        public int Radius;
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
        private Canvas _canvas;
        public MyCircle(int x, int y, int radius, Canvas canvas, Color color) : base(color)
        {
            _canvas = canvas;
            Radius = radius;
            Center = new Vertex(new Point(x, y), canvas,this,true);
            Center.VertexChanged += (obj, args) =>
            {
                ShapeChanged?.Invoke(this,null);
            };
            Center.VertexDeleted += (o, args) =>
            {
                ShapeDeleted?.Invoke(this, null);
            };
        }

        public override string ToString()
        {
            return $"Circle: @{Center}";
        }
        public override void ColorChangedEvent()
        {
            ColorChanged?.Invoke(this, null);
        }
    }
}
