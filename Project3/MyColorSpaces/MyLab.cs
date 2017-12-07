using System;
using Project3.IMyColorSpaces;

namespace Project3.MyColorSpaces
{
    class MyLab : ILab, IMyColor
    {
        private const double Precision = 1e-4;
        public double L
        {
            get => _l;
            set
            {
                if (MathExtender.CheckRange(-127.0, 128.0, value))
                {
                    if (Math.Abs(_l - value) > Precision)
                    {
                        _l = value;
                    }
                }
            } 
        }
        public double A
        {
            get => _a;
            set
            {
                if (MathExtender.CheckRange(-127.0, 128.0, value))
                {
                    if (Math.Abs(_a - value) > Precision)
                    {
                        _a = value;
                    }
                }
            }
        }
        public double B
        {
            get => _b;
            set
            {
                if (MathExtender.CheckRange(-127.0, 128.0, value))
                {
                    if (Math.Abs(_b - value) > Precision)
                    {
                        _b = value;
                    }
                }
            }
        }

        public IXyz ToXyz()
        {
            var yr = (_l < 7.9996248) ? (_l / 903.3) : (Math.Pow((_l + 16.0) / 116.0, 3.0));
            var xr = Math.Pow(Math.Pow(yr, 1.0 / 3.0) + _a / 500.0, 3.0);
            var zr = Math.Pow(Math.Pow(yr, 1.0 / 3.0) - _b / 200.0, 3.0);

            xr *= MyXyz.Xr;
            yr *= MyXyz.Yr;
            zr *= MyXyz.Zr;
            return new MyXyz(xr, yr, zr);
        }

        private double _l;
        private double _a;
        private double _b;
        public MyLab(double l, double a, double b)
        {
            _l = l;
            _a = a;
            _b = b;
        }

        public ILab ToLab()
        {
            return this;
        }

    }
}
