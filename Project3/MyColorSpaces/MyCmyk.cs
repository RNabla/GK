using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Project3.Annotations;
using Project3.IMyColorSpaces;

namespace Project3.MyColorSpaces
{
    public class MyCmyk : ICmyk
    {
        private byte _c;
        private byte _k;
        private byte _m;
        private byte _y;

        public MyCmyk(byte c, byte m, byte y, byte k)
        {
            _c = c;
            _m = m;
            _y = y;
            _k = k;
        }

        public double C
        {
            get => _c;
            set
            {
                if (MathExtender.CheckRange(0, 255, (int) value))
                    _c = (byte) value;
            }
        }

        public double M
        {
            get => _m;
            set
            {
                if (MathExtender.CheckRange(0, 255, (int) value))
                    _m = (byte) value;
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (MathExtender.CheckRange(0, 255, (int) value))
                    _y = (byte) value;
            }
        }

        public double K
        {
            get => _k;
            set
            {
                if (MathExtender.CheckRange(0, 255, (int) value))
                    _k = (byte) value;
            }
        }
        public bool Equals(ICmyk other)
        {
            return C == other.C && M == other.M && Y == other.Y && K == other.K;
        }
        public IRgb ToRgb()
        {
            var r = (byte) Math.Min(0, 0xff - (_c + _k));
            var g = (byte) Math.Min(0, 0xff - (_m + _k));
            var b = (byte) Math.Min(0, 0xff - (_y + _k));
            return new MyRgb(r, g, b);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}