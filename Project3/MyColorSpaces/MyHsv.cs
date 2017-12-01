using System;
using static Project3.MathExtender;
namespace Project3.MyColorSpaces
{
    internal class MyHsv
    {
        public int H
        {
            get => _h;
            set
            {
                if (CheckRange(0, 359, value))
                    _h = value;
            }
        }

        public int S
        {
            get => _s;
            set
            {
                if (CheckRange(0, 100, value))
                    _s = value;
            }
        }
        public int V
        {
            get => _v;
            set
            {
                if (CheckRange(0, 100, value))
                    _v = value;
            }
        }

        private int _h;
        private int _s;
        private int _v;
        public MyHsv(int h, int s, int v)
        {
            H = h;
            S = s;
            V = v;
        }

        public void FromMyRgb(MyRgb myRgb)
        {
            var r = myRgb.R;
            var g = myRgb.G;
            var b = myRgb.B;

            var temp = Math.Min(Math.Min(r, g), b);
            V = (int)Math.Max(Math.Max(r, g), b);
            if (V == temp)
            {
                H = 0;
            }
            else
            {
                if (V == r)
                    H = (int)(0 + (g - b) * 60 / (V - temp));
                else if (V == g)
                    H = (int)(120 + (b - r) * 60 / (V - temp));
                else if (V == b)
                    H = (int)(240 + (r - g) * 60 / (V - temp));
            }
            if (H < 0)
                H += 360;

            if (V == 0)
                S = 0;
            else
                S = (int)((V - temp) * 100 / V);
            V = (100 * V) / 0xff;
        }

        public void ToMyRgb()
        {
            var part = H / 60.0;
            var range = (int) Math.Floor(part) % 6;
            var frac = part - Math.Floor(part);

            var v = V * 0xff;
            var p = v * (100 - S);
            var q = v * (100 - frac * S);
            var t = v * (100 - (1 - frac) * S);


        }
    }
}