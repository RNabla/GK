using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Project3.Annotations;
using Project3.IMyColorSpaces;

namespace Project3.MyColorSpaces
{
    public class MyRgb : IRgb, INotifyPropertyChanged, IComparable<SolidColorBrush>
    {
        private byte _b;
        private byte _g;
        private byte _r;

        public MyRgb(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public double R
        {
            get => _r;
            set
            {
                if (MathExtender.CheckRange(0, 255, (int) value))
                {
                    if (Math.Abs(_r - value) >= 1){
                        _r = (byte) value;
                        OnPropertyChanged(nameof(R));
                    }
                }
            }
        }

        public double G
        {
            get => _g;
            set
            {
                if (MathExtender.CheckRange(0, 255, (int) value))
                {
                    if (Math.Abs(_g - value) >= 1)
                    {
                        _g = (byte)value;
                        OnPropertyChanged(nameof(G));
                    }
                }
            }
        }

        public double B
        {
            get => _b;
            set
            {
                if (MathExtender.CheckRange(0, 255, (int) value))
                {
                    if (Math.Abs(_b - value) >= 1)
                    {
                        _b = (byte)value;
                        OnPropertyChanged(nameof(B));
                    }
                }
            }
        }

        public ICmyk ToCmyk()
        {
            var c = (byte) (0xff - _r);
            var m = (byte) (0xff - _g);
            var y = (byte) (0xff - _b);
            var k = Math.Min(Math.Min(c, m), y);

            c -= k;
            m -= k;
            y -= k;
            return new MyCmyk(c, m, y, k);
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
            var dr = other.Color.R - _r;
            if (dr != 0)
                return dr;
            var dg = other.Color.G - _g;
            if (dg != 0)
                return dg;
            return other.Color.B - _b;

        }
    }
}