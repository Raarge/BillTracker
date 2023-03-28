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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BillTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CurrentAmountStart();

            
        }

        private void btnStartingAmount_OnClick(object sender, RoutedEventArgs e)
        {
            var amount = Convert.ToDecimal( tbStartingAmount.Text.Trim());
            tbCurrentAmount.Text = string.Empty;
            tbCurrentAmount.Text = string.Format("{0:C}", amount);
        }

        private void btnSubmitPayment_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewAccount_OnClick(object sender, RoutedEventArgs e)
        {

        }
        private string CurrentAmountStart()
        {
            if(tbCurrentAmount.Text == string.Empty)
                tbCurrentAmount.Text = "$0.00";
            
            return tbCurrentAmount.Text;

        }
    }
}
