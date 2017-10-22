using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab_1
{
    /// <summary>
    /// Interaction logic for ColorPrompt.xaml
    /// </summary>
    public partial class ColorPrompt : Window
    {
        private MyShape _myShape;
        internal ColorPrompt(MyShape myShape)
        {
            InitializeComponent();
            _myShape = myShape;
            ColorInput.Text = $"{_myShape.Color}";
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = (sender as Button)?.Name == "OkButton";

            if (DialogResult.Value)
            {
                try
                {
                    var convertFromString = ColorConverter.ConvertFromString(ColorInput.Text);
                    if (convertFromString != null)
                    {
                        var color = (Color) convertFromString;
                        _myShape.Color = color;
                    }
                }
                catch
                {

                }
            }
            Close();
        }
    }
}
