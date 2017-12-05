using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project3.IMyColorSpaces;

namespace Project3.MyColorSpaces
{
    public class MyXyz : IXyz
    {
        public static double Precision = 1e-4;
        public double X
        {
            get => _x;
            set
            {
                if (MathExtender.CheckRange(0, 255, value))
                {
                    if (Math.Abs(_x - value) >= Precision)
                        _x = value;
                }
            }
        }

        public double Y {
            get => _y;
            set
            {
                if (MathExtender.CheckRange(0, 255, value))
                {
                    if (Math.Abs(_y - value) >= Precision)
                        _y = value;
                }
            }
        }
        public double Z {
            get => _z;
            set
            {
                if (MathExtender.CheckRange(0, 255, value))
                {
                    if (Math.Abs(_z - value) >= Precision)
                        _z = value;
                }
            }
        }

        public IRgb ToRgb()
        {
            var r = (byte)(+0.418470000 * _x + -0.1586600 * _y + +0.082835 * _z);
            var g = (byte)(-0.091169000 * _x + +0.2524300 * _y + +0.015708 * _z);
            var b = (byte)(+0.000920900 * _x + -0.0025498 * _y + +0.178600 * _z);
            return new MyRgb(r, g, b);
        }

        private double _x;
        private double _y;
        private double _z;

        public MyXyz(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
        public bool Equals(IXyz other)
        {
            return Math.Abs(_x - other.X) < Precision && 
                Math.Abs(_y - other.Y) < Precision && 
                Math.Abs(_z - other.Z) < Precision;
        }
    }
}
