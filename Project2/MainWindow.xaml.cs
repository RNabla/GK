using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Project2
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public Scene MyScene;
        public WriteableBitmap DummyBitmap = new WriteableBitmap(1, 1, 96, 96, PixelFormats.Rgb24, null);
        public readonly Int32Rect OnePixel = new Int32Rect(0,0,1,1);
        //private WriteableBitmap _hBitmap; // heightmap
        private WriteableBitmap nBitmap; // normalmap
        private WriteableBitmap bBitmap; // backgrmap
        private WriteableBitmap constNBitmap;
        private WriteableBitmap constHBitmap;
        public MainWindow()
        {
            InitializeComponent();
            DummyBitmap.WritePixels(OnePixel, new byte[]{0,0,0}, 3, 0);
            bBitmap = DummyBitmap;
            nBitmap = DummyBitmap;
           //s _hBitmap = DummyBitmap;
            constNBitmap = new WriteableBitmap(1, 1, 96, 96, PixelFormats.Rgb24, null);
            constHBitmap = new WriteableBitmap(1, 1, 96, 96, PixelFormats.Rgb24, null);
            constNBitmap.WritePixels(OnePixel, new byte[] {0x00, 0x00, 0xff}, 3, 0);
            constHBitmap.WritePixels(OnePixel, new byte[] {0x00, 0x00, 0x00}, 3, 0);
            BackgroundImagePreview.Source = DummyBitmap;
            HeightMapImagePreview.Source = DummyBitmap;
            ImageScene.Source = new WriteableBitmap(1920, 1080, 96, 96, PixelFormats.Rgb24, null);
            var rGradient = new LinearGradientBrush(Colors.Black, Colors.Red, 0);
            var gGradient = new LinearGradientBrush(Colors.Black, Colors.Green, 0);
            var bGradient = new LinearGradientBrush(Colors.Black, Colors.Blue, 0);
            RLightSlider.Background = rGradient;
            GLightSlider.Background = gGradient;
            BLightSlider.Background = bGradient;
            RBackgroundSlider.Background = rGradient;
            GBackgroundSlider.Background = gGradient;
            BBackgroundSlider.Background = bGradient;
            MyScene = new Scene(CanvasScene, ImageScene);
            MyyPolygons.ItemsSource = MyScene.Polygons;
            MyyPolygons.ContextMenu = new ContextMenu();
            var item = new MenuItem
            {
                Header = "Delete"
            };
            item.Click += (obj, args) =>
            {
                var si = MyyPolygons.SelectedItem;
                (si as MyPolygon).RemoveAllLines();
            };
            MyyPolygons.ContextMenu.Items.Add(item);
            LightParametrs.DataContext = MyScene.MyLight;
            VMaxTextBox.DataContext = MyScene;
            VMinTextBox.DataContext = MyScene;
            CountRPolygon.DataContext = MyScene;
            FpsCounter.DataContext = MyScene;
            MyScene.fpsCounter = FpsCounter;
            Closing += (sender, args) =>
            {
                MyScene.FinishThreadJob();
            };
            BackgroundColorCheckbox.IsChecked = true;
            ConstNoneHeightMap.IsChecked = true;
        }

        private void LoadBitmap(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            var name = (sender as Button).Name;
            var result = ofd.ShowDialog();
            if (result.HasValue && result == true)
            {
                var fileName = ofd.FileName;
                var bitmap = new BitmapImage(new Uri(fileName));
                var map = new WriteableBitmap(bitmap);
                switch (name)
                {
                    case "NormalMapButton":
                        //MyScene.NormalMap = map;
                        //nBitmap = map;
                        //NormalMapImagePreview.Source = bitmap;
                        break;
                    case "HeightMapButton":
                        MyScene.SetHeight(fileName);
                       // MyScene.HeightMap = map;
                        //_hBitmap = map;
                        HeightMapImagePreview.Source = bitmap;
                        break;
                    case "TextureButton":
                        MyScene.SetTexture(fileName);
                       // MyScene.SetBackground(bitmap);
                        BackgroundImagePreview.Source = bitmap;
                     //   bBitmap = map;
                        MyScene.BackgroundMap = map;
                        break;
                    default:
                        return;
                }
            }
        }

        private void ColorChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider) sender;
            if (slider == null) return;
            var senderName = slider.Name;
            var value = (byte) e.NewValue;
            switch (senderName)
            {
                case "RLightSlider":
                    MyScene.MyLight.R = value;
                    break;
                case "GLightSlider":
                    MyScene.MyLight.G = value;
                    break;
                case "BLightSlider":
                    MyScene.MyLight.B = value;
                    break;
                case "RBackgroundSlider":
                case "GBackgroundSlider":
                case "BBackgroundSlider":
                    //var wbmp = new WriteableBitmap(1, 1, 96, 96, PixelFormats.Bgr32, null);
                    var r = (byte)RBackgroundSlider.Value;
                    var g = (byte)GBackgroundSlider.Value;
                    var b = (byte)BBackgroundSlider.Value;
                    //wbmp.WritePixels(OnePixel, new[] {0xff,r, g, b}, 4, 0);
                    //MyScene.BackgroundMap = wbmp;
                    MyScene.SetTextureSolidColor(r, g, b);
                    break;
            }
        }

        private void RadioButtonChanged(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (radioButton == null) return;
            var senderName = radioButton.Name;
            switch (senderName)
            {
                case "BackgroundColorCheckbox":
                    var wbmp = new WriteableBitmap(1,1,96,86,PixelFormats.Bgr32,null);
                    var r = (byte)RBackgroundSlider.Value;
                    var g = (byte)GBackgroundSlider.Value;
                    var b = (byte)BBackgroundSlider.Value;
                    //wbmp.WritePixels(OnePixel, new[] {0xff,r, g, b }, 4, 0);
                    MyScene.SetTextureSolidColor(r,g,b);
                    break;
                case "TextureBackgroundCheckbox":
                    MyScene.BackgroundMap = bBitmap;
                    break;
                case "ConstNoneHeightMap":
                    MyScene.DisableHeight();
                    break;
                case "HeightMapCheckbox":
                    //MyScene.HeightMap = _hBitmap;
                    break;
            }

        }

        private void Scene_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var pos = e.GetPosition(ImageScene);
                MyScene.AddPoint(pos);
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                MyScene.EndDrawingPolygon();
            }
          
        }
    }
}