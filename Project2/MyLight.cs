using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Numerics;
namespace Project2
{
    public class MyLight
    {
        public MyLight()
        {
            X = 960;
            Y = 540;
            Z = 500;
            Color = Colors.DarkRed;
            _ligthVector = new Vector3(X,Y,Z);
        }

        private Vector3 _ligthVector;
        public int X
        {
            get => (int)_ligthVector.X;
            set
            {
                if (value <= 0)
                    return;
                _ligthVector.X = value;
            }
            
        }
        public int Y
        {
            get => (int)_ligthVector.Y;
            set
            {
                if (value <= 0)
                    return;
                _ligthVector.Y = value;
            }
        }

        public int Z
        {
            get => (int)_ligthVector.Z;
            set
            {
                if (value <= 0)
                    return;
                _ligthVector.Z = value;
            }
        }

        public Color Color
        {
            get => Color.FromRgb(R, G, B); 
            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
        }

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}
