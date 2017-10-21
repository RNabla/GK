using System.Windows;
using System.Windows.Media;

namespace Lab_1
{
    internal class MyCircle : MyShape
    {
        public int Radius;
        public override void Draw(Sketch sketch)
        {
            var x = (int) Center.X;
            var y = (int)Center.Y;
            sketch.DrawCircle(x, y, Radius, Color, Thickness);
        }

        public MyCircle(int x, int y, int radius, Color color, int thickness = 1) : base(color,thickness)
        {
            Center = new Point(x,y);
            Radius = radius;
        }
    }
}
