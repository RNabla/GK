using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Project3.Annotations;
using Project3.IMyColorSpaces;

namespace Project3.MyColorSpaces
{
    public class MyCmyk : ICmyk , IMyColor
    {
        /*
         * Wartości od 0 do 255
         */
        private double _c;
        private double _k;
        private double _m;
        private double _y;

        

        public MyCmyk(double c, double m, double y, double k)
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
            var c = _c + _k;
            var m = _m + _k;
            var y = _y + _k;

            var r = (byte) (0xff - Math.Min(c, 0xff));
            var g = (byte) (0xff - Math.Min(m, 0xff));
            var b = (byte) (0xff - Math.Min(y, 0xff));
            return new MyRgb(r, g, b);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ILab ToLab()
        {
            return ToRgb().ToXyz().ToLab();
        }
    }
}