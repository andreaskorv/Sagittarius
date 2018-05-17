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

namespace Sagittarius
{
    /// <summary>
    /// Логика взаимодействия для EnterCount.xaml
    /// </summary>
    public partial class EnterCount : Window
    {
        public int iCount;
        public bool bVisibility;
        public EnterCount()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int iForTryParse;
            if (int.TryParse(textBox.Text, out iForTryParse) && int.Parse(textBox.Text) < 10000)
            {
                iCount = int.Parse(textBox.Text);
                bVisibility = (bool)checkBox.IsChecked;
                DialogResult = true;
                Close();
            }
        }
    }
}