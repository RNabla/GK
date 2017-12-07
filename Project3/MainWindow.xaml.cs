using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly MyLab _myLab = new MyLab(0, 0, 0);
        private readonly MyRgb _myRgb = new MyRgb(-1, 0, 0);
        private readonly MyXyz _myXyz = new MyXyz(0, 0, 0);
        private readonly MyHsl _myHsl = new MyHsl(0,0,0);
        private readonly MyHsv _myHsv = new MyHsv(0,0,0);
        private readonly MyYuv _myYuv = new MyYuv(0, 0, 0);
        private readonly MyYCbCr _myYCbCr = new MyYCbCr(0, 0, 0);
        private bool _isUpdating = false;

        public MainWindow()
        {
            InitializeComponent();
            UpdateFromRgb();
            _myRgb.R = -1;
            RgbVisible.DataContext = _myRgb;
            //var rgb = new MyRgb(255, 0, 0);
            //xyz = rgb.ToXyz();
            //var backrgb = xyz.ToRgb();
            //rgb = new MyRgb(255, 255,255);
            //xyz = rgb.ToXyz();
            //rgb = new MyRgb(192, 192,192);
            //rgb.ToXyz();
            //rgb = new MyRgb(0, 0,255);
            //rgb.ToXyz();
            //rgb = new MyRgb(255, 0,255);
            //rgb.ToXyz();

            //rgb = new MyRgb(187,231,54);
            //xyz = rgb.ToXyz();
            //var lab = xyz.ToLab();
            //var newxyz = lab.ToXyz();
            //var newrgb = newxyz.ToRgb();

            UpdateFillRectangle();
         //   var l = new MyXyz(0.49 * 255, 0.177 * 255, 0).ToLab();


            //for (var r = 0; r < 255; r++)
            //{
            //    for (var g = 0; g < 255; g++)
            //    {
            //        for (var b = 0; b < 255; b++)
            //        {
            //            var initialRgb = new MyRgb(r,g,b);
            //            var initialXyz = initialRgb.ToXyz();
            //            var lab = initialXyz.ToLab();
            //            var endXyz = lab.ToXyz();
            //            var endRgb = initialXyz.ToRgb();
            //            if (initialRgb.R != endRgb.R || initialRgb.G != endRgb.G || initialRgb.B != initialRgb.B)
            //                throw null;
            //        }
            //    }
            //}

            //var dx = 0.01;
            //for (var x = 0.0; x < 1; x += dx)
            //{
            //    for (var y = 0.0; y < 1; y += dx)
            //    {
            //        for (var z = 0.0; z < 1; z += dx)
            //        {
            //            var refXyz = new MyXyz(x, y, z);

            //        }
            //    }
            //}
        }

        #region starybinding

        

        //private void InitLab()
        //{
        //    LabSliderL.DataContext = _myLab;
        //    LabSliderA.DataContext = _myLab;
        //    LabSliderB.DataContext = _myLab;
        //    LabValueL.DataContext = _myLab;
        //    LabValueA.DataContext = _myLab;
        //    LabValueB.DataContext = _myLab;
        //}

        //private void InitRgb()
        //{
        //    RgbSliderR.DataContext = _myRgb;
        //    RgbSliderG.DataContext = _myRgb;
        //    RgbSliderB.DataContext = _myRgb;
        //    RgbValueR.DataContext = _myRgb;
        //    RgbValueG.DataContext = _myRgb;
        //    RgbValueB.DataContext = _myRgb;
        //}

        //private void InitXyz()
        //{
        //    XyzSliderX.DataContext = _myXyz;
        //    XyzSliderY.DataContext = _myXyz;
        //    XyzSliderZ.DataContext = _myXyz;
        //    XyzValueX.DataContext = _myXyz;
        //    XyzValueY.DataContext = _myXyz;
        //    XyzValueZ.DataContext = _myXyz;
        //}

        //private void InitCmyk()
        //{
        //    CmykSliderC.DataContext = _myCmyk;
        //    CmykSliderM.DataContext = _myCmyk;
        //    CmykSliderY.DataContext = _myCmyk;
        //    CmykSliderK.DataContext = _myCmyk;
        //    CmykValueC.DataContext = _myCmyk;
        //    CmykValueM.DataContext = _myCmyk;
        //    CmykValueY.DataContext = _myCmyk;
        //    CmykValueK.DataContext = _myCmyk;
        //}
        #endregion

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
                (byte) RgbSliderR.Value,
                (byte) RgbSliderG.Value,
                (byte) RgbSliderB.Value
            );
            UpdateRgb(rgb);
            var second = rgb.ToXyz().ToLab().ToXyz().ToRgb();
            var cmyk = _myRgb.ToCmyk();
            UpdateCmyk(cmyk);
            UpdateHsl(_myRgb.ToHsl());
            UpdateHsv(_myRgb.ToHsv());
            UpdateXyz(_myRgb.ToXyz());
            UpdateYuv(_myRgb.ToYuv());
            UpdateYCbCr(_myRgb.ToYCbCr());
            UpdateLab(_myRgb.ToXyz().ToLab());
        }

        private void UpdateAllFromLab(ILab myLab, bool force = false)
        {
            UpdateLab(myLab, force);
            var rgb = myLab.ToXyz().ToRgb();
            UpdateRgb(rgb);
            UpdateCmyk(rgb.ToCmyk());
            UpdateXyz(myLab.ToXyz());
        }

        private void UpdateFromCmyk()
        {
            var cmyk = new MyCmyk(
                (byte) CmykSliderC.Value,
                (byte) CmykSliderM.Value,
                (byte) CmykSliderY.Value,
                (byte) CmykSliderK.Value
            );
            UpdateCmyk(cmyk);
            var rgb = _myCmyk.ToRgb();
            UpdateRgb(rgb);
            UpdateXyz(rgb.ToXyz());
        }

        private void UpdateFromLab(ILab lab)
        {
            UpdateLab(lab);
            var xyz = lab.ToXyz();
            UpdateXyz(xyz);
            var rgb = xyz.ToRgb();
            UpdateRgb(rgb);
            UpdateCmyk(rgb.ToCmyk());
            UpdateHsv(rgb.ToHsv());
            UpdateHsl(rgb.ToHsl());
            UpdateYuv(rgb.ToYuv());
            UpdateYCbCr(rgb.ToYCbCr());
        }

        private void UpdateRgb(IRgb myRgb, bool force = false)
        {
            if (force || !myRgb.Equals(_myRgb))
            {
                _myRgb.R = myRgb.R;
                _myRgb.G = myRgb.G;
                _myRgb.B = myRgb.B;
                RgbSliderR.Value = myRgb.R;
                RgbSliderG.Value = myRgb.G;
                RgbSliderB.Value = myRgb.B;
                RgbValueR.Text = $"{myRgb.R}";
                RgbValueG.Text = $"{myRgb.G}";
                RgbValueB.Text = $"{myRgb.B}";
            }
        }

        private void UpdateLab(ILab myLab, bool force = false)
        {
            if (force || !myLab.Equals(_myLab))
            {
                _myLab.L = myLab.L;
                _myLab.A = myLab.A;
                _myLab.B = myLab.B;

                LabSliderL.Value = myLab.L;
                LabSliderA.Value = myLab.A;
                LabSliderB.Value = myLab.B;

                LabValueL.Text = $"{myLab.L:F3}";
                LabValueA.Text = $"{myLab.A:F3}";
                LabValueB.Text = $"{myLab.B:F3}";
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
                CmykSliderC.Value = myCmyk.C;
                CmykSliderM.Value = myCmyk.M;
                CmykSliderY.Value = myCmyk.Y;
                CmykSliderK.Value = myCmyk.K;
                CmykValueC.Text = $"{myCmyk.C}";
                CmykValueM.Text = $"{myCmyk.M}";
                CmykValueY.Text = $"{myCmyk.Y}";
                CmykValueK.Text = $"{myCmyk.K}";
            }
        }

        private void UpdateXyz(IXyz myXyz)
        {
            if (!myXyz.Equals(_myXyz))
            {
                _myXyz.X = myXyz.X;
                _myXyz.Y = myXyz.Y;
                _myXyz.Z = myXyz.Z;
                XyzSliderX.Value = myXyz.X;
                XyzSliderY.Value = myXyz.Y;
                XyzSliderZ.Value = myXyz.Z;
                XyzValueX.Text = $"{myXyz.X:F3}";
                XyzValueY.Text = $"{myXyz.Y:F3}";
                XyzValueZ.Text = $"{myXyz.Z:F3}";
            }
        }

        private void UpdateHsl(IHsl myHsl)
        {
            _myHsl.H = myHsl.H;
            _myHsl.S = myHsl.S;
            _myHsl.L = myHsl.L;

            HslSliderH.Value = (int) myHsl.H;
            HslSliderS.Value = (int) myHsl.S;
            HslSliderL.Value = (int) myHsl.L;

            HslValueH.Text = $"{myHsl.H:F0}";
            HslValueS.Text = $"{myHsl.S:F0}";
            HslValueL.Text = $"{myHsl.L:F0}";
        }

        private void UpdateHsv(IHsv myHsv)
        {
            _myHsv.H = myHsv.H;
            _myHsv.S = myHsv.S;
            _myHsv.V = myHsv.V;

            HsvSliderH.Value = (int)myHsv.H;
            HsvSliderS.Value = (int)myHsv.S;
            HsvSliderV.Value = (int)myHsv.V;
              
            HsvValueH.Text = $"{myHsv.H:F0}";
            HsvValueS.Text = $"{myHsv.S:F0}";
            HsvValueV.Text = $"{myHsv.V:F0}";

        }

        private void UpdateYuv(IYuv myYuv)
        {
            _myYuv.Y = myYuv.Y;
            _myYuv.U = myYuv.U;
            _myYuv.V = myYuv.V;

            YuvSliderY.Value = myYuv.Y;
            YuvSliderU.Value = myYuv.U;
            YuvSliderV.Value = myYuv.V;

            YuvValueY.Text = $"{myYuv.Y:F3}";
            YuvValueU.Text = $"{myYuv.U:F3}";
            YuvValueV.Text = $"{myYuv.V:F3}";
        }

        private void UpdateYCbCr(IYCbCr myYCbCr)
        {
            _myYCbCr.Y = myYCbCr.Y;
            _myYCbCr.Cb = myYCbCr.Cb;
            _myYCbCr.Cr = myYCbCr.Cr;

            YCbCrSliderY.Value = myYCbCr.Y;
            YCbCrSliderCb.Value = myYCbCr.Cb;
            YCbCrSliderCr.Value = myYCbCr.Cr;

            YCbCrValueY.Text = $"{myYCbCr.Y:F3}";
            YCbCrValueCb.Text = $"{myYCbCr.Cb:F3}";
            YCbCrValueCr.Text = $"{myYCbCr.Cr:F3}";
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
            if (sliderName.StartsWith("Rgb"))
            {
                var rgb = new MyRgb(
                    (byte)RgbSliderR.Value,
                    (byte)RgbSliderG.Value,
                    (byte)RgbSliderB.Value
                );
                var lab = rgb.ToXyz().ToLab();
                var refRgb = lab.ToXyz().ToRgb();
                //UpdateFromLab(rgb.ToXyz().ToLab());
                UpdateFromRgb();
            };
            //else if (sliderName.StartsWith("Cmyk")) UpdateFromCmyk();
            //if (sliderName.StartsWith("Lab"))
            //{
            //    var lab = new MyLab(
            //         LabSliderL.Value,    
            //         LabSliderA.Value,    
            //         LabSliderB.Value
            //    );
            //    UpdateFromLab(lab);
            //}
        }

        private void TextBoxValueOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                if (sender is TextBox textBox)
                {
                    var colorSpaceName = textBox.Name;
                    if (colorSpaceName.StartsWith("Rgb"))
                    {
                        try
                        {
                            var rgb = new MyRgb(
                                Convert.ToByte(RgbValueR.Text),
                                Convert.ToByte(RgbValueG.Text),
                                Convert.ToByte(RgbValueB.Text)
                            );
                            UpdateRgb(rgb);
                        }
                        catch
                        {
                            UpdateRgb(_myRgb, true);
                        }
                    }
                    else if (colorSpaceName.StartsWith("Cmyk"))
                    {
                        var cmyk = new MyCmyk(
                            Convert.ToByte(CmykValueC.Text),
                            Convert.ToByte(CmykValueM.Text),
                            Convert.ToByte(CmykValueY.Text),
                            Convert.ToByte(CmykValueK.Text)
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