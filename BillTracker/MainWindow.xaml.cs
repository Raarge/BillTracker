using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            var payee = GetPayee();
            var validFlag = checkValidity(payee);
            var exists = CrudOperations.SearchPayee(payee.PayeeName);
            if (exists == true)
            {
                MessageBox.Show(payee.PayeeName + " already exists.  Use Update Tab Instead!");
                clearNewPayee();                
            }
            else if (validFlag == 1)
            {

            }
            else
            {
                var affectedrows = CrudOperations.InsertPayee(payee);
                if (affectedrows > 0)
                {
                    MessageBox.Show(payee.PayeeName + " added!");
                    clearNewPayee();
                }
                else
                {
                    MessageBox.Show(payee.PayeeName + " was not added!");
                }
            }
            
            
        }
        private string CurrentAmountStart()
        {
            if(tbCurrentAmount.Text == string.Empty)
                tbCurrentAmount.Text = "$0.00";
            
            return tbCurrentAmount.Text;

        }

        public Payee GetPayee()
        {
            Payee payee = new Payee();
            payee.PayeeName = tbPayeeName.Text;
            payee.DateDue = Convert.ToInt32(tbDateDue.Text);
            payee.Amountdue = Convert.ToDecimal(tbAmountDue.Text);
            payee.URL = tbURL.Text;

            return payee;
        }

        public void clearNewPayee()
        {
            tbPayeeName.Text = string.Empty;
            tbDateDue.Text = string.Empty;
            tbAmountDue.Text = string.Empty;
            tbURL.Text = string.Empty;
            tbPayeeName.Focus();
        }

        public int checkValidity(Payee payee)
        {
            int flag = 0;
            var first7 = payee.URL.Substring(0, 7);
            var first8 = payee.URL.Substring(0, 8);

            if (payee.PayeeName.Length > 50) 
            {
                MessageBox.Show("Payee Name is Greater than 50 characters.  This is not allowed.");
                flag = 1;
                tbPayeeName.Focus();
            }
            else if(payee.DateDue < 1 || payee.DateDue > 31)
            {
                MessageBox.Show("Date Due is not a valid date");
                flag = 1;
                tbDateDue.Focus();
            }
            else if(payee.Amountdue < 0.01m)
            {
                MessageBox.Show("Amount Due is not valid");
                flag = 1;
                tbAmountDue.Focus();
            }
            else if(first7 != "HTTP://" && first7 != "http://" && first8 != "HTTPS://" && first8 != "https://")
            {
                MessageBox.Show("Url is not valid");
                flag = 1;
                tbURL.Focus();
            }

            return flag;
        }

        
    }
}
