using System.Collections.Generic;
using System.Linq;
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
        private readonly Sketch _sketch;
        public int Radius;

        public MyCircle(int x, int y, int radius, Sketch sketch, Canvas canvas, Color color) : base(color)
        {

            Name = $"Circle {_id++}";
            _sketch = sketch;
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

        public IEnumerable<MyCircle> GetOtherCircles()
        {
            return _sketch.GetConcentricOptions(this);
        }
        public override void ColorChangedEvent()
        {
            ColorChanged?.Invoke(this, null);
        }
        private readonly List<MyCircle> _concentricCircles = new List<MyCircle>();

        public void SetNewPositionForConcentric(Point point)
        {
            foreach (var circle in _concentricCircles)
                circle.Center.VertexPoint = point;
        }

        public static void BoundConcentric(MyCircle a, MyCircle b, Point center)
        {
            var aList = a._concentricCircles.ToArray();
            var bList = b._concentricCircles.ToArray();
            var list = aList.Concat(bList).Concat(new[] {a, b}).Distinct().ToArray();
            foreach (var circle in list)
            {
                circle._concentricCircles.Clear();
                circle._concentricCircles.AddRange(list.Where(c => c != circle));
                circle.Center.VertexPoint = center;
            }
        }

        public static void DisboundConcentric(MyCircle a)
        {
            // a jest odbindowany od pozostalych figur
            var circles = a._concentricCircles;
            foreach (var circle in circles)
                circle._concentricCircles.RemoveAll(c => c == a);
            circles.Clear();

        }
    }
}