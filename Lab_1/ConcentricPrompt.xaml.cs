using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ConcentricPrompt.xaml
    /// </summary>
    public partial class ConcentricPrompt : Window
    {
        public object Selected = null;
        internal ConcentricPrompt(IEnumerable<MyCircle> circles)
        {
            InitializeComponent();
            var oc = new ObservableCollection<MyCircle>(circles);
            comboBox.ItemsSource = oc;

            //var dict = new Dictionary<string,MyCircle>();
            //foreach (var circle in circles)
            //    dict.Add(circle.ToString(),circle);
            //comboBox.DisplayMemberPath = "Value";
            //comboBox.Value
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = (sender as Button)?.Name == "Ok";
            Selected = comboBox.SelectedItem;
        }
    }
}
