using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project3.IMyColorSpaces;

namespace Project3.MyColorSpaces
{
    class MyHsl : IHsl
    {
        /*
         * H - <0,360>
         * S - <0,100>
         * L - <0,100>
         */
        private double _h;
        private double _s;
        private double _l;

        public MyHsl(double h, double s, double l)
        {
            _h = h;
            _s = s;
            _l = l;
        }

        public double H { get => _h; set => _h = value; }
        public double S { get => _s; set => _s = value; }
        public double L { get => _l; set => _l = value; }
        public IRgb ToRgb()
        {
            double r, g, b;

            var c = (1 - Math.Abs(2 * _l - 1)) * _s;
            var x = c * (1 - Math.Abs((_h / 60.0) % 2.0 - 1));
            var m = _l - c / 2.0;
                                                                       
            if (MathExtender.CheckRange(0, 60, x))         { r = c; g = x; b = 0; }
            else if (MathExtender.CheckRange(60, 120, x))  { r = x; g = c; b = 0; }
            else if (MathExtender.CheckRange(120, 180, x)) { r = 0; g = c; b = x; }
            else if (MathExtender.CheckRange(180, 240, x)) { r = 0; g = x; b = c; }
            else if (MathExtender.CheckRange(240, 300, x)) { r = x; g = 0; b = c; }
            else if (MathExtender.CheckRange(300, 360, x)) { r = c; g = 0; b = x; }
            else throw new NotSupportedException();

            return new MyRgb(
                (byte) ((r + m) * 255),
                (byte) ((g + m) * 255),
                (byte) ((b + m) * 255)
            );
        }
    }
}
