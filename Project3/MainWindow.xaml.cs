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
        private readonly MyRgb _myRgb = new MyRgb(0, 0, 0);
        private readonly MyXyz _myXyz = new MyXyz(0, 0, 0);

        private bool _isUpdating;

        public MainWindow()
        {
            InitializeComponent();
            InitRgb();
            InitCmyk();
            InitXyz();
            UpdateFromRgb();
            UpdateFillRectangle();
        }

        private void InitRgb()
        {
            RGBRSlider.DataContext = _myRgb;
            RGBGSlider.DataContext = _myRgb;
            RGBBSlider.DataContext = _myRgb;
            RGBRValue.DataContext = _myRgb;
            RGBGValue.DataContext = _myRgb;
            RGBBValue.DataContext = _myRgb;
        }
        private void InitXyz()
        {
            XYZXSlider.DataContext = _myXyz;
            XYZYSlider.DataContext = _myXyz;
            XYZZSlider.DataContext = _myXyz;
            XYZXValue.DataContext = _myXyz;
            XYZYValue.DataContext = _myXyz;
            XYZZValue.DataContext = _myXyz;
        }

        private void InitCmyk()
        {
            CMYKCSlider.DataContext = _myCmyk;
            CMYKMSlider.DataContext = _myCmyk;
            CMYKYSlider.DataContext = _myCmyk;
            CMYKKSlider.DataContext = _myCmyk;
            CMYKCValue.DataContext = _myCmyk;
            CMYKMValue.DataContext = _myCmyk;
            CMYKYValue.DataContext = _myCmyk;
            CMYKKValue.DataContext = _myCmyk;
        }

        private void ColorChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            if (sender is Slider slider)
                SelectInitialColorSpace(slider);
            UpdateFillRectangle();
            _isUpdating = false;
        }

        private void UpdateFromRgb()
        {
            var rgb = new MyRgb(
                (byte) RGBRSlider.Value,
                (byte) RGBGSlider.Value,
                (byte) RGBBSlider.Value
            );
            UpdateRgb(rgb);
            var cmyk = _myRgb.ToCmyk();
            UpdateCmyk(cmyk);
            UpdateXyz(_myRgb.ToXyz());
        }

        private void UpdateFromCmyk()
        {
            var cmyk = new MyCmyk(
                (byte) CMYKCSlider.Value,
                (byte) CMYKMSlider.Value,
                (byte) CMYKYSlider.Value,
                (byte) CMYKKSlider.Value
            );
            UpdateCmyk(cmyk);
            var rgb = _myCmyk.ToRgb();
            UpdateRgb(rgb);
            UpdateXyz(rgb.ToXyz());
        }

        private void UpdateRgb(IRgb myRgb, bool force = false)
        {
            if (force || !myRgb.Equals(_myRgb))
            {
                _myRgb.R = myRgb.R;
                _myRgb.G = myRgb.G;
                _myRgb.B = myRgb.B;
                RGBRSlider.Value = myRgb.R;
                RGBGSlider.Value = myRgb.G;
                RGBBSlider.Value = myRgb.B;
                RGBRValue.Text = $"{myRgb.R}";
                RGBGValue.Text = $"{myRgb.G}";
                RGBBValue.Text = $"{myRgb.B}";
            }
        }

        private void UpdateCmyk(ICmyk myCmyk)
        {
            if (!myCmyk.Equals(_myCmyk))
            {
                _myCmyk.C = myCmyk.C;
                _myCmyk.M = myCmyk.M;
                _myCmyk.Y = myCmyk.Y;
                _myCmyk.K = myCmyk.K;
                CMYKCSlider.Value = myCmyk.C;
                CMYKMSlider.Value = myCmyk.M;
                CMYKYSlider.Value = myCmyk.Y;
                CMYKKSlider.Value = myCmyk.K;
                CMYKCValue.Text = $"{myCmyk.C}";
                CMYKMValue.Text = $"{myCmyk.M}";
                CMYKYValue.Text = $"{myCmyk.Y}";
                CMYKKValue.Text = $"{myCmyk.K}";
            }
        }

        private void UpdateXyz(IXyz myXyz)
        {
            if (!myXyz.Equals(_myXyz))
            {
                _myXyz.X = myXyz.X;
                _myXyz.Y = myXyz.Y;
                _myXyz.Z = myXyz.Z;
                XYZXSlider.Value = (int)myXyz.X;
                XYZYSlider.Value = (int)myXyz.Y;
                XYZZSlider.Value = (int)myXyz.Z;
                XYZXValue.Text = $"{myXyz.X:F1}";
                XYZYValue.Text = $"{myXyz.Y:F1}";
                XYZZValue.Text = $"{myXyz.Z:F1}";
            }
        }

        private void UpdateFillRectangle()
        {
            if (FillRectangle.Fill is SolidColorBrush scb && _myRgb.CompareTo(scb) == 0)
                return;
            FillRectangle.Fill = new SolidColorBrush(Color.FromRgb(
                (byte) _myRgb.R,
                (byte) _myRgb.G,
                (byte) _myRgb.B
            ));
        }

        private void SelectInitialColorSpace(Slider slider)
        {
            var sliderName = slider.Name;
            if (sliderName.StartsWith("RGB")) UpdateFromRgb();
            else if (sliderName.StartsWith("CMYK")) UpdateFromCmyk();
        }

        private void TextBoxValueOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                if (sender is TextBox textBox)
                {
                    var colorSpaceName = textBox.Name;
                    if (colorSpaceName.StartsWith("RGB"))
                    {
                        try
                        {
                            var rgb = new MyRgb(
                                Convert.ToByte(RGBRValue.Text),
                                Convert.ToByte(RGBGValue.Text),
                                Convert.ToByte(RGBBValue.Text)
                            );
                            UpdateRgb(rgb);
                        }
                        catch
                        {
                            UpdateRgb(_myRgb, true);
                        }
                    }
                    else if (colorSpaceName.StartsWith("CMYK"))
                    {
                        var cmyk = new MyCmyk(
                            Convert.ToByte(CMYKCValue.Text),
                            Convert.ToByte(CMYKMValue.Text),
                            Convert.ToByte(CMYKYValue.Text),
                            Convert.ToByte(CMYKKValue.Text)
                        );
                        UpdateCmyk(cmyk);
                    }
                }
            }
            catch
            {
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