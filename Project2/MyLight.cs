using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Numerics;
using System.Windows.Media.Media3D;

namespace Project2
{
    public class MyLight
    {
        public MyLight()
        {
            X = 960;
            Y = 540;
            Z = 500;
            Color = Colors.White;
            lightVector = new Vector3(X,Y,Z);
        }

        public Point3D LightPos = new Point3D(960,540,500);
        private Vector3 lightVector;
        public int X
        {
            get => (int)lightVector.X;
            set
            {
                if (value <= 0)
                    return;
                lightVector.X = value;
                
            }
            
        }
        public int Y
        {
            get => (int)lightVector.Y;
            set
            {
                if (value <= 0)
                    return;
                lightVector.Y = value;
                LightPos = new Point3D(X, Y, Z);
            }
        }

        public int Z
        {
            get => (int)lightVector.Z;
            set
            {
                if (value <= 0)
                    return;
                lightVector.Z = value;
                LightPos = new Point3D(X, Y, Z);
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
