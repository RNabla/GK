using Project3.MyColorSpaces;

namespace Project3.IMyColorSpaces
{
    public interface IMyColor
    {
        ILab ToLab();
    }
    public interface ILab
    {
        double L { get; set; }
        double A { get; set; }
        double B { get; set; }
        IXyz ToXyz();
    }

    public interface IXyz
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
        IRgb ToRgb();
        ILab ToLab();
        bool Equals(IXyz other);
    }

    public interface IYuv
    {
        double Y { get; set; }
        double U { get; set; }
        double V { get; set; }
        IRgb ToRgb();
    }

    public interface IHsl
    {
        double H { get; set; }
        double S { get; set; }
        double L { get; set; }
        IRgb ToRgb();
    }

    public interface IHsv
    {
        double H { get; set; }
        double S { get; set; }
        double V { get; set; }
        IRgb ToRgb();
    }

    public interface IYCbCr
    {
        double Y { get; set; }
        double Cb { get; set; }
        double Cr { get; set; }
        IRgb ToRgb();
    }

    public interface ICmyk
    {
        double C { get; set; }
        double M { get; set; }
        double Y { get; set; }
        double K { get; set; }
        IRgb ToRgb();
    }

    public interface IRgb
    {
        double R { get; set; }
        double G { get; set; }
        double B { get; set; }
        ICmyk ToCmyk();
        IXyz ToXyz();
        IHsl ToHsl();
        IHsv ToHsv();
        IYuv ToYuv();
        IYCbCr ToYCbCr();
        bool Equals(IRgb other);
        void UpdateMyRgb(MyRgb myRgb);
        bool IsVisible { get; }
    }
}