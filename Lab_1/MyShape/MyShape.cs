using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab_1
{
    internal abstract class MyShape
    {
        public Vertex Center;
        public Color Color;
        public abstract void Draw(Sketch sketch);

        protected MyShape(Color color)
        {
            Color = color;
        }

        public delegate void ShapeChangedHandler(object obj, RoutedEventArgs e);
        public abstract System.Collections.Generic.IEnumerable<Shape> GetUiElements();
        public abstract event ShapeChangedHandler ShapeChanged;
        public abstract event ShapeChangedHandler ShapeDeleted; /* występuje jeżeli usunięto figurę */
        public abstract event ShapeChangedHandler ComponentDeleted; /* występuje jeżeli usunięto wierzchołek*/
        public abstract event ShapeChangedHandler ComponentAdded; /* występuje jeżeli usunięto wierzchołek*/
        public abstract event ShapeChangedHandler ColorChanged;
        public abstract void ColorChangedEvent();
    }
}
