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
    /// Interaction logic for RadiusPrompt.xaml
    /// </summary>
    public partial class RadiusPrompt : Window
    {
        private readonly MyCircle _myCircle;
        internal RadiusPrompt(MyCircle myCircle)
        {
            _myCircle= myCircle;
            InitializeComponent();
            RadiusInput.Text = $"{myCircle.Radius:F0}";
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = (sender as Button)?.Name == "OkButton";

            if (DialogResult.Value)
            {
                try
                {
                    var newRadius = Convert.ToDouble(RadiusInput.Text);
                    if (newRadius >= 1.0)
                        _myCircle.Radius = (int) newRadius;
                }
                catch
                {
                    
                }
            }
            Close();
        }

    }
}
