using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Project3.IMyColorSpaces;
using Project3.MyColorSpaces;

namespace Project3
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MyCmyk _myCmyk = new MyCmyk(0, 0, 0, 0);
        private readonly MyHsl _myHsl = new MyHsl(0, 0, 0);
        private readonly MyHsv _myHsv = new MyHsv(0, 0, 0);
        private readonly MyLab _myLab = new MyLab(0, 0, 0);
        private readonly MyRgb _myRgb = new MyRgb(-1, 0, 0);
        private readonly MyXyz _myXyz = new MyXyz(0, 0, 0);
        private readonly MyYCbCr _myYCbCr = new MyYCbCr(0, 0, 0);
        private readonly MyYuv _myYuv = new MyYuv(0, 0, 0);
        private bool _isUpdating;


        public MainWindow()
        {
            InitializeComponent();
            UpdateAllFromLab(_myLab);
            UpdateFillRectangle();
        }

        private void ColorChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            if (sender is Slider slider)
            {
                SelectInitialColorSpace(slider);
                UpdateFillRectangle();
            }
            _isUpdating = false;
        }

        private void UpdateVisibility()
        {
            var visibility = _myRgb.IsVisible ? Visibility.Hidden : Visibility.Visible;
            RgbVisible.Visibility = visibility;
            CmykVisible.Visibility = visibility;
            HsvVisible.Visibility = visibility;
            HslVisible.Visibility = visibility;
            YuvVisible.Visibility = visibility;
            YCbCrVisible.Visibility = visibility;
        }

        private void UpdateAllFromLab(ILab myLab, bool force = false)
        {
            UpdateLab(myLab);
            var xyz = myLab.ToXyz();
            var rgb = xyz.ToRgb();
            UpdateXyz(xyz);
            UpdateRgb(rgb);
            UpdateCmyk(rgb.ToCmyk());
            UpdateYuv(rgb.ToYuv());
            UpdateYCbCr(rgb.ToYCbCr());
            UpdateHsv(rgb.ToHsv());
            UpdateHsl(rgb.ToHsl());
        }

        private void UpdateRgb(IRgb myRgb)
        {
            myRgb.UpdateMyRgb(_myRgb);
            RgbSliderR.Value = myRgb.R;
            RgbSliderG.Value = myRgb.G;
            RgbSliderB.Value = myRgb.B;
            UpdateLabels("Rgb");
        }

        private void UpdateLab(ILab myLab)
        {
            _myLab.L = myLab.L;
            _myLab.A = myLab.A;
            _myLab.B = myLab.B;
            LabSliderL.Value = myLab.L;
            LabSliderA.Value = myLab.A;
            LabSliderB.Value = myLab.B;
            UpdateLabels("Lab");
        }

        private void UpdateCmyk(ICmyk myCmyk)
        {
            _myCmyk.C = myCmyk.C;
            _myCmyk.M = myCmyk.M;
            _myCmyk.Y = myCmyk.Y;
            _myCmyk.K = myCmyk.K;
            CmykSliderC.Value = myCmyk.C;
            CmykSliderM.Value = myCmyk.M;
            CmykSliderY.Value = myCmyk.Y;
            CmykSliderK.Value = myCmyk.K;
            UpdateLabels("Cmyk");
        }

        private void UpdateXyz(IXyz myXyz)
        {
            _myXyz.X = myXyz.X;
            _myXyz.Y = myXyz.Y;
            _myXyz.Z = myXyz.Z;
            XyzSliderX.Value = myXyz.X;
            XyzSliderY.Value = myXyz.Y;
            XyzSliderZ.Value = myXyz.Z;
            UpdateLabels("Xyz");
        }

        private void UpdateHsl(IHsl myHsl)
        {
            _myHsl.H = myHsl.H;
            _myHsl.S = myHsl.S;
            _myHsl.L = myHsl.L;
            HslSliderH.Value = myHsl.H;
            HslSliderS.Value = myHsl.S;
            HslSliderL.Value = myHsl.L;
            UpdateLabels("Hsl");
        }

        private void UpdateHsv(IHsv myHsv)
        {
            _myHsv.H = myHsv.H;
            _myHsv.S = myHsv.S;
            _myHsv.V = myHsv.V;
            HsvSliderH.Value = myHsv.H;
            HsvSliderS.Value = myHsv.S;
            HsvSliderV.Value = myHsv.V;
            UpdateLabels("Hsv");
        }

        private void UpdateYuv(IYuv myYuv)
        {
            _myYuv.Y = myYuv.Y;
            _myYuv.U = myYuv.U;
            _myYuv.V = myYuv.V;
            YuvSliderY.Value = myYuv.Y;
            YuvSliderU.Value = myYuv.U;
            YuvSliderV.Value = myYuv.V;
            UpdateLabels("Yuv");
        }

        private void UpdateYCbCr(IYCbCr myYCbCr)
        {
            _myYCbCr.Y = myYCbCr.Y;
            _myYCbCr.Cb = myYCbCr.Cb;
            _myYCbCr.Cr = myYCbCr.Cr;
            YCbCrSliderY.Value = myYCbCr.Y;
            YCbCrSliderCb.Value = myYCbCr.Cb;
            YCbCrSliderCr.Value = myYCbCr.Cr;
            UpdateLabels("YCbCr");
        }

        private void UpdateFillRectangle()
        {
            FillRectangle.Fill = new SolidColorBrush(Color.FromRgb(
                (byte) _myRgb.R,
                (byte) _myRgb.G,
                (byte) _myRgb.B
            ));
            UpdateVisibility();
        }

        private void SelectInitialColorSpace(Slider slider)
        {
            var sliderName = slider.Name;
            IMyColor initialColor = null;
            if (sliderName.StartsWith("Rgb"))
                initialColor = new MyRgb(
                    RgbSliderR.Value,
                    RgbSliderG.Value,
                    RgbSliderB.Value
                );
            else if (sliderName.StartsWith("Cmyk"))
                initialColor = new MyCmyk(
                    CmykSliderC.Value,
                    CmykSliderM.Value,
                    CmykSliderY.Value,
                    CmykSliderK.Value
                );
            else if (sliderName.StartsWith("Hsl"))
                initialColor = new MyHsl(
                    HslSliderH.Value,
                    HslSliderS.Value,
                    HslSliderL.Value
                );
            else if (sliderName.StartsWith("Hsv"))
                initialColor = new MyHsv(
                    HsvSliderH.Value,
                    HsvSliderS.Value,
                    HsvSliderV.Value
                );
            else if (sliderName.StartsWith("Yuv"))
                initialColor = new MyYuv(
                    YuvSliderY.Value,
                    YuvSliderU.Value,
                    YuvSliderV.Value
                );
            else if (sliderName.StartsWith("YCbCr"))
                initialColor = new MyYCbCr(
                    YCbCrSliderY.Value,
                    YCbCrSliderCb.Value,
                    YCbCrSliderCr.Value
                );
            else if (sliderName.StartsWith("Xyz"))
                initialColor = new MyXyz(
                    XyzSliderX.Value,
                    XyzSliderY.Value,
                    XyzSliderZ.Value
                );
            else if (sliderName.StartsWith("Lab"))
                initialColor = new MyLab(
                    LabSliderL.Value,
                    LabSliderA.Value,
                    LabSliderB.Value
                );

            if (initialColor == null) return;
            var lab = initialColor.ToLab();
            UpdateAllFromLab(lab);
        }

        private void TextBoxValueOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox textBox)
                try
                {
                    var name = textBox.Name;
                    switch (name)
                    {
                        case "RgbValueR":
                            RgbSliderR.Value = Convert.ToDouble(RgbValueR.Text);
                            break;
                        case "RgbValueG":
                            RgbSliderG.Value = Convert.ToDouble(RgbValueG.Text);
                            break;
                        case "RgbValueB":
                            RgbSliderB.Value = Convert.ToDouble(RgbValueB.Text);
                            break;
                        case "CmykValueC":
                            CmykSliderC.Value = Convert.ToDouble(CmykValueC.Text);
                            break;
                        case "CmykValueM":
                            CmykSliderM.Value = Convert.ToDouble(CmykValueM.Text);
                            break;
                        case "CmykValueY":
                            CmykSliderY.Value = Convert.ToDouble(CmykValueY.Text);
                            break;
                        case "CmykValueK":
                            CmykSliderK.Value = Convert.ToDouble(CmykValueK.Text);
                            break;
                        case "HslValueH":
                            HslSliderH.Value = Convert.ToDouble(HslValueH.Text);
                            break;
                        case "HslValueS":
                            HslSliderS.Value = Convert.ToDouble(HslValueS.Text);
                            break;
                        case "HslValueL":
                            HslSliderL.Value = Convert.ToDouble(HslValueL.Text);
                            break;
                        case "HsvValueH":
                            HsvSliderH.Value = Convert.ToDouble(HsvValueH.Text);
                            break;
                        case "HsvValueS":
                            HsvSliderS.Value = Convert.ToDouble(HsvValueS.Text);
                            break;
                        case "HsvValueV":
                            HsvSliderV.Value = Convert.ToDouble(HsvValueV.Text);
                            break;
                        case "YuvValueY":
                            YuvSliderY.Value = Convert.ToDouble(YuvValueY.Text);
                            break;
                        case "YuvValueU":
                            YuvSliderU.Value = Convert.ToDouble(YuvValueU.Text);
                            break;
                        case "YuvValueV":
                            YuvSliderV.Value = Convert.ToDouble(YuvValueV.Text);
                            break;
                        case "YCbCrValueY":
                            YCbCrSliderY.Value = Convert.ToDouble(YCbCrValueY.Text);
                            break;
                        case "YCbCrValueCb":
                            YCbCrSliderCb.Value = Convert.ToDouble(YCbCrValueCb.Text);
                            break;
                        case "YCbCrValueCr":
                            YCbCrSliderCr.Value = Convert.ToDouble(YCbCrValueCr.Text);
                            break;
                        case "XyzValueX":
                            XyzSliderX.Value = Convert.ToDouble(XyzValueX.Text);
                            break;
                        case "XyzValueY":
                            XyzSliderY.Value = Convert.ToDouble(XyzValueY.Text);
                            break;
                        case "XyzValueZ":
                            XyzSliderZ.Value = Convert.ToDouble(XyzValueZ.Text);
                            break;
                        case "LabValueL":
                            LabSliderL.Value = Convert.ToDouble(LabValueL.Text);
                            break;
                        case "LabValueA":
                            LabSliderA.Value = Convert.ToDouble(LabValueA.Text);
                            break;
                        case "LabValueB":
                            LabSliderB.Value = Convert.ToDouble(LabValueB.Text);
                            break;
                        default:
                            MessageBox.Show("Gdzie znalazłeś ten dodatkowy suwak?");
                            break;
                    }
                }
                catch
                {
                    while (true)
                    {
                        MessageBox.Show(
                            "To co wprowadziłeś, chyba nie było liczbą, co?\r\nW każdym razie ja tego nie zrozumiałem");
                        MessageBox.Show("Nie rób już tak...");
                        MessageBox.Show("Dobrze?");
                        if (MessageBox.Show("Pamiętasz pytanie?", "hehe xd", MessageBoxButton.YesNo) !=
                            MessageBoxResult.Yes) continue;
                        MessageBox.Show("Trzymam za słowo");
                        break;
                    }
                }
        }

        public void UpdateLabels(string name)
        {
            if (name == "Rgb")
            {
                RgbValueR.Text = $"{_myRgb.R:F0}";
                RgbValueG.Text = $"{_myRgb.G:F0}";
                RgbValueB.Text = $"{_myRgb.B:F0}";
            }
            else if (name == "Lab")
            {
                LabValueL.Text = $"{_myLab.L:F3}";
                LabValueA.Text = $"{_myLab.A:F3}";
                LabValueB.Text = $"{_myLab.B:F3}";
            }
            else if (name == "Cmyk")
            {
                CmykValueC.Text = $"{_myCmyk.C}";
                CmykValueM.Text = $"{_myCmyk.M}";
                CmykValueY.Text = $"{_myCmyk.Y}";
                CmykValueK.Text = $"{_myCmyk.K}";
            }
            else if (name == "Xyz")
            {
                XyzValueX.Text = $"{_myXyz.X:F3}";
                XyzValueY.Text = $"{_myXyz.Y:F3}";
                XyzValueZ.Text = $"{_myXyz.Z:F3}";
            }
            else if (name == "Hsv")
            {
                HsvValueH.Text = $"{_myHsv.H:F0}";
                HsvValueS.Text = $"{_myHsv.S:F0}";
                HsvValueV.Text = $"{_myHsv.V:F0}";
            }
            else if (name == "Hsl")
            {
                HslValueH.Text = $"{_myHsl.H:F0}";
                HslValueS.Text = $"{_myHsl.S:F0}";
                HslValueL.Text = $"{_myHsl.L:F0}";
            }
            else if (name == "YCbCr")
            {
                YCbCrValueY.Text = $" {_myYCbCr.Y:F3}";
                YCbCrValueCb.Text = $"{_myYCbCr.Cb:F3}";
                YCbCrValueCr.Text = $"{_myYCbCr.Cr:F3}";
            }
            else if (name == "Yuv")
            {
                YuvValueY.Text = $"{_myYuv.Y:F3}";
                YuvValueU.Text = $"{_myYuv.U:F3}";
                YuvValueV.Text = $"{_myYuv.V:F3}";
            }
        }
    }

    public static class MathExtender
    {
        public static bool CheckRange(int minInclusive, int maxInclusive, int value)
        {
            if (minInclusive > maxInclusive)
                throw new ArgumentException();
            return minInclusive <= value && value <= maxInclusive;
        }

        public static bool CheckRange(double minInclusive, double maxInclusive, double value)
        {
            if (minInclusive > maxInclusive)
                throw new ArgumentException();
            return minInclusive <= value && value <= maxInclusive;
        }
    }
}