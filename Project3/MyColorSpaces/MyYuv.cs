using System.Windows.Media.Media3D;

namespace Project3.MyColorSpaces
{
    internal class MyYuv
    {
        // const stollen borrowed from https://en.wikipedia.org/wiki/YUV
        private const double UMax = 0.436;

        private const double VMax = 0.615;

        private static readonly Matrix3D RgbToYuvTransformationMatrix;
        private static readonly Matrix3D YuvToRgbTransformationMatrix;
        public double U;
        public double V;
        public double Y;

        static MyYuv()
        {
            RgbToYuvTransformationMatrix = new Matrix3D(
                0.299, 0.587, 0.114, 0,
                -0.14713, -0.28886, 0.436, 0,
                0.615, -0.51499, -0.10001, 0,
                0, 0, 0, 1
            );
            YuvToRgbTransformationMatrix = new Matrix3D(
                1, 0, 1.13983, 0,
                1, -0.39465, -0.58060, 0,
                1, 2.03211, 0, 0,
                0, 0, 0, 1
            );
        }

        public MyYuv(MyRgb myRgb)
        {
            var r = new Vector3D(128, 0, 0);
            var output = Vector3D.Multiply(r, RgbToYuvTransformationMatrix);
        }

        public void From(MyRgb myRgb)
        {
            var r = myRgb.R / 256.0;
            var g = myRgb.G / 256.0;
            var b = myRgb.B / 256.0;
            Y = 0.299 * r +
                0.587 * g +
                0.114 + b;
            U = 0.492 * (b - Y);
            V = 0.877 * (r - Y);
        }
    }
}