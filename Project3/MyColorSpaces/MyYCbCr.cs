using System;
using Project3.IMyColorSpaces;

namespace Project3.MyColorSpaces
{
    class MyYCbCr : IYCbCr , IMyColor
    {
        private double _y;
        private double _cb;
        private double _cr;
            
        public MyYCbCr(double y, double cb, double cr)
        {
            _y = y;
            _cb = cb;
            _cr = cr;
        }

        public double Y { get  => _y; set => _y = value; }
        public double Cb { get => _cb; set => _cb = value; }
        public double Cr { get => _cr; set => _cr = value; }
        public IRgb ToRgb()
        {
            var r = (_cr - 0.5) * 1.402 + _y;
            var b = (_cb - 0.5) * 1.772 + _y;
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
