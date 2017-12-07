using System;
using Project3.IMyColorSpaces;

namespace Project3.MyColorSpaces
{
    internal class MyYuv : IYuv, IMyColor
    {
        private double _u;
        private double _v;
        private double _y;

        public MyYuv(double y, double u, double v)
        {
            _y = y;
            _u = u;
            _v = v;
        }

        private const double Precision = 1e-3;

        public double Y
        {
            get => _y;
            set
            {
                if (MathExtender.CheckRange(0, 1.0, value))
                    if (Math.Abs(_y - value) > Precision)
                        _y = value;
            }
        }

        public double U
        {
            get => _u;
            set
            {
                if (MathExtender.CheckRange(-0.289, 0.436, value))
                    if (Math.Abs(_u - value) > Precision)
                        _u = value;
            }
        }

        public double V
        {
            get => _v;
            set
            {
                if (MathExtender.CheckRange(-0.615, 0.615, value))
                    if (Math.Abs(_v - value) > Precision)
                        _v = value;
            }
        }

        public IRgb ToRgb()
        {
            var r = _y + _v / 0.877;
            var b = _y + _u / 0.492;
            var g = (_y - 0.299 * r - 0.114 * b) / 0.587;

            return new MyRgb(
                Math.Round(r * 255.0),
                Math.Round(g * 255.0),
                Math.Round(b * 255.0)
            );
        }
        public ILab ToLab()
        {
            return ToRgb().ToXyz().ToLab();
        }
    }
}