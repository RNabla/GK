using System;
using Project3.IMyColorSpaces;
namespace Project3.MyColorSpaces
{
    internal class MyHsv : IHsv
    {

        /*
         * H - <0,360>
         * S - <0,100>
         * L - <0,50>
         */
        private double _h;
        private double _s;
        private double _v;

        public MyHsv(double h, double s, double v)
        {
            _h = h;
            _s = s;
            _v = v;
        }

        public double H { get => _h; set => _h = value; }
        public double S { get => _s; set => _s = value; }
        public double V { get => _v; set => _v = value; }
        public IRgb ToRgb()
        {
            double r, g, b;

            var c = _v * _s;
            var x = c * (1 - Math.Abs((_h / 60.0) % 2.0 - 1));
            var m = _v - c;

            if (MathExtender.CheckRange(0, 60, x)) { r = c; g = x; b = 0; }
            else if (MathExtender.CheckRange(60, 120, x)) { r = x; g = c; b = 0; }
            else if (MathExtender.CheckRange(120, 180, x)) { r = 0; g = c; b = x; }
            else if (MathExtender.CheckRange(180, 240, x)) { r = 0; g = x; b = c; }
            else if (MathExtender.CheckRange(240, 300, x)) { r = x; g = 0; b = c; }
            else if (MathExtender.CheckRange(300, 360, x)) { r = c; g = 0; b = x; }
            else throw new NotSupportedException();

            return new MyRgb(
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255)
            );
        }
    }
}