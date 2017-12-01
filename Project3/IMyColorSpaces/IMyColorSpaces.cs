namespace Project3.IMyColorSpaces
{
    public interface ILab
    {
        double L { get; set; }
        double A { get; set; }
        double B { get; set; }
    }

    public interface IXyz
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }

    public interface IYuv
    {
        double Y { get; set; }
        double U { get; set; }
        double V { get; set; }
    }

    public interface IHsl
    {
        double H { get; set; }
        double S { get; set; }
        double L { get; set; }
    }

    public interface IHsv
    {
        double H { get; set; }
        double S { get; set; }
        double V { get; set; }
    }

    public interface IYCbCr
    {
        double Y { get; set; }
        double Cb { get; set; }
        double Cr { get; set; }
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
        bool Equals(IRgb other);

    }
}