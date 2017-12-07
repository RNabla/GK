using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Project3.Annotations;
using Project3.IMyColorSpaces;

namespace Project3.MyColorSpaces
{
    public class MyRgb : IRgb, INotifyPropertyChanged, IComparable<SolidColorBrush>, IMyColor
    {
        /*
         * Wartości od 0 do 255
         */
        private double _r;
        private double _g;
        private double _b;


        public MyRgb(double r, double g, double b)
        {
            _r = r;
            _g = g;
            _b = b;
        }

        public double R
        {
            get => Math.Min(Math.Max(Math.Round(_r), 0), 0xff);
            set => _r = value;
        }

        public double G
        {
            get => Math.Min(Math.Max(Math.Round(_g), 0), 0xff);
            set => _g = value;
        }

        public double B
        {
            get => Math.Min(Math.Max(Math.Round(_b), 0), 0xff);
            set => _b = value;
        }

        public ICmyk ToCmyk()
        {
            var c = (byte) (0xff - R);
            var m = (byte) (0xff - G);
            var y = (byte) (0xff - B);
            var k = Math.Min(Math.Min(c, m), y);

            c -= k;
            m -= k;
            y -= k;

            return new MyCmyk(c, m, y, k);
        }

        public IXyz ToXyz()
        {
            var r = R / 2.55;
            var g = G / 2.55;
            var b = B / 2.55;
            return new MyXyz(r, g, b, MyXyz.MyLuminant.D65);
        }

        public IHsl ToHsl()
        {
            var r = R / 255.0;
            var g = G / 255.0;
            var b = B / 255.0;

            var cMax = Math.Max(Math.Max(r, g), b);
            var cMin = Math.Min(Math.Min(r, g), b);
            var delta = cMax - cMin;

            double h, s, l;

            if (delta == 0)
            {
                h = 0;
            }
            else if (cMax == r)
            {
                h = 60 * (g - b) / delta;
            }
            else if (cMax == g)
            {
                h = 120 + 60 * (b - r) / delta;
            }
            else
            {
                h = 240 + 60 * (r - g) / delta;
            }
            if (h <= 0)
                h += 360;
            if (h >= 360)
                h -= 360;


            l = (cMax + cMin) / 2;
            s = (delta == 0) ? (0) : (delta / (1 - Math.Abs(2 * l - 1)));

            s *= 100;
            l *= 100;
            return new MyHsl(h, s, l);
        }

        public IHsv ToHsv()
        {
            var r = R / 255.0;
            var g = G / 255.0;
            var b = B / 255.0;

            var cMin = Math.Min(Math.Min(r, g), b);
            var cMax = Math.Max(Math.Max(r, g), b);
            var delta = cMax - cMin;

            double h, s, v;
            if (delta == 0)
            {
                h = 0;
            }
            else if (cMax == r)
            {
                h = 60 * (g - b) / delta;
            }
            else if (cMax == g)
            {
                h = 120 + 60 * (b - r) / delta;
            }
            else
            {
                h = 240 + 60 * (r - g) / delta;
            }
            if (h <= 0)
                h += 360;
            if (h >= 360)
                h -= 360;

            s = (cMax == 0) ? (0) : delta / cMax;

            v = cMax;

            s *= 100;
            v *= 100;
            return new MyHsv(h, s, v);
        }

        public IYuv ToYuv()
        {
            var r = R / 255.0;
            var g = G / 255.0;
            var b = B / 255.0;

            //var y = 0.000 + 0.299 * r + 0.587 * g + 0.114 * b;
            //var u = 0.000 - 0.147 * r - 0.289 * g + 0.437 * b;
            //var v = 0.000 + 0.615 * r - 0.515 * g - 0.100 * b;

            var y = 0.299 * r + 0.587 * g + 0.114 * b;
            var u = 0.492 * (b - y);
            var v = 0.877 * (r - y);
            return new MyYuv(y, u, v);
        }

        public IYCbCr ToYCbCr()
        {
            var r = R / 255.0;
            var g = G / 255.0;
            var b = B / 255.0;

            var y = 0.299 * r + 0.587 * g + 0.114 * b;
            var cb = (b - y) / 1.772 + 0.5;
            var cr = (r - y) / 1.402 + 0.5;

            return new MyYCbCr(y, cb, cr);

        }

        public bool Equals(IRgb other)
        {
            return R == other.R && G == other.G && B == other.B;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(SolidColorBrush other)
        {
            var dr = ((double) other.Color.R).CompareTo(_r);
            if (dr != 0)
                return dr;
            var dg = ((double) other.Color.G).CompareTo(_g);
            if (dg != 0)
                return dg;
            return ((double) other.Color.B).CompareTo(_b);

        }

        public ILab ToLab()
        {
            return ToXyz().ToLab();
        }

        public Visibility IsRepresentable => IsVisible? Visibility.Hidden : Visibility.Visible;

        public void UpdateMyRgb(MyRgb myRgb)
        {
            myRgb.R = _r;
            myRgb.G = _g;
            myRgb.B = _b;
        }

        public bool IsVisible 
        {
            get
            {
                var r = MathExtender.CheckRange(0, 255, (int) _r);
                var g = MathExtender.CheckRange(0, 255, (int) _g);
                var b = MathExtender.CheckRange(0, 255, (int) _b);
                return r && g && b;
            }
        }
    }
}