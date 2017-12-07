using System;
using Project3.IMyColorSpaces;

namespace Project3.MyColorSpaces
{
    public class MyXyz : IXyz, IMyColor
    {
        public const double Xr = 94.81;
        public const double Yr = 100.0;
        public const double Zr = 107.3;
        public const double Delta = 6.0 / 29.0;
        public const double DeltaSquare = Delta*Delta;
        public const double DeltaCube = DeltaSquare*Delta;
       
        public static double Precision = 1e-3;
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

        public double Y
        {
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
            //var r = (byte)(+0.418470000 * _x + -0.1586600 * _y + +0.082835 * _z);
            //var g = (byte)(-0.091169000 * _x + +0.2524300 * _y + +0.015708 * _z);
            //var b = (byte)(+0.000920900 * _x + -0.0025498 * _y + +0.178600 * _z);
            //var r = +2.36461 * _x + -0.896541 * _y + -0.468073 * _z;
            //var g = -0.515166 * _x + +1.42641 * _y + +0.0887581 * _z;
            //var b = +0.0052037 * _x + -0.0144082 * _y + +1.00092 * _z;

            //var r = _x * 3.1338561 + _y * -1.6168667 + _z * -0.4906146;
            //var g = _x * -0.9787684 + _y * 1.9161415 + _z * 0.0334540;
            //var b = _x * 0.0719453 + _y * -0.22289914 + _z * 1.4052427;

            var r = _x *  3.2404542 + _y * -1.5371385 + _z * -0.4985314;
            var g = _x * -0.9692660 + _y *  1.8760108 + _z *  0.0415560;
            var b = _x *  0.0556434 + _y * -0.2040259 + _z *  1.0572252;
            return new MyRgb(
                Math.Round(2.55 * r),
                Math.Round(2.55 * g),
                Math.Round(2.55 * b)
            );
        }

        public ILab ToLab()
        {
            var xRatio = _x / Xr;
            var yRatio = _y / Yr;
            var zRatio = _z / Zr;

            var l = (yRatio > 0.008856) ? (116 * Cubic(yRatio) - 16) : (903.3 * yRatio);
            var a = 500 * (Cubic(xRatio) - Cubic(yRatio));
            var b = 200 * (Cubic(yRatio) - Cubic(zRatio));
            return new MyLab(l, a, b);
        }

        private static double Cubic(double n)
        {
            return Math.Pow(n, 1.0 / 3.0);
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

        public enum MyLuminant
        {
            D65,
            D50
        }

        public MyXyz(double sR, double sG, double sB, MyLuminant luminant = MyLuminant.D65)
        {
            Illuminate(luminant, sR, sG, sB);
        } 

        public void Illuminate(MyLuminant luminant, double r, double g, double b)
        {
            switch (luminant)
            {
                case MyLuminant.D65:
                    _x = 0.4124564 * r + 0.3575761 * g + 0.1804375 *b;
                    _y = 0.2126729 * r + 0.7151522 * g + 0.0721750 *b;
                    _z = 0.0193339 * r + 0.1191920 * g + 0.9503041 *b;
                    break;
                case MyLuminant.D50:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(luminant), luminant, null);
            }
        }
        public bool Equals(IXyz other)
        {
            return Math.Abs(_x - other.X) < Precision && 
                Math.Abs(_y - other.Y) < Precision && 
                Math.Abs(_z - other.Z) < Precision;
        }
    }
}
