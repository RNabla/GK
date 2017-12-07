using System;
using Project3.IMyColorSpaces;

namespace Project3.MyColorSpaces
{
    internal class MyHsv : IHsv, IMyColor
    {
        /*
         * H - <0,360>
         * S - <0,100>
         * L - <0,100>
         */
        public MyHsv(double h, double s, double v)
        {
            H = h;
            S = s;
            V = v;
        }

        public double H { get; set; }

        public double S { get; set; }

        public double V { get; set; }

        public IRgb ToRgb()
        {
            double r, g, b;
            var h = H;
            var s = S / 100.0;
            var v = V / 100.0;

            var c = v * s;
            var x = c * (1 - Math.Abs(h / 60.0 % 2.0 - 1));
            var m = v - c;

                 if (MathExtender.CheckRange(  0,  60, h)) { r = c; g = x; b = 0; }
            else if (MathExtender.CheckRange( 60, 120, h)) { r = x; g = c; b = 0; }
            else if (MathExtender.CheckRange(120, 180, h)) { r = 0; g = c; b = x; }
            else if (MathExtender.CheckRange(180, 240, h)) { r = 0; g = x; b = c; }
            else if (MathExtender.CheckRange(240, 300, h)) { r = x; g = 0; b = c; }
            else if (MathExtender.CheckRange(300, 360, h)) { r = c; g = 0; b = x; }
            else throw new NotSupportedException();

            return new MyRgb(
                (r + m) * 255,
                (g + m) * 255,
                (b + m) * 255
            );
        }

        public ILab ToLab()
        {
            return ToRgb().ToXyz().ToLab();
        }
    }
}