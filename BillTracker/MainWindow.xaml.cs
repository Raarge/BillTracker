using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        public List<string> payeeNames
        {
            get; set;
        }

        public MainWindow()
        {
            InitializeComponent();
            CurrentAmountStart();
            cbPayeeDropdown.SetBinding(
                ItemsControl.ItemsSourceProperty,
                new Binding { Source = payeeNamesList() });
            

            
        }

        private void btnStartingAmount_OnClick(object sender, RoutedEventArgs e)
        {
            var amount = Convert.ToDecimal( tbStartingAmount.Text.Trim());
            tbCurrentAmount.Text = string.Empty;
            tbCurrentAmount.Text = string.Format("{0:C}", amount);
        }

        private void btnSubmitPayment_OnClick(object sender, RoutedEventArgs e)
        {
            var payment = GetPayment();

            var flag = checkPaymentValidity();

            if (flag == 0)
            {
                var returnCode = CrudOperations.InsertPayment(payment);
                if(returnCode > 0) 
                {
                    MessageBox.Show($"{payment.PayeeName} was paid");
                    clearPayment();
                    // look for a way to reset the pulldown cb
                }
                
                
            }
            else
            {
                MessageBox.Show("Too Bad Not Valid");
            }
            


            
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
                    cbPayeeDropdown.SetBinding(
                     ItemsControl.ItemsSourceProperty,
                     new Binding { Source = payeeNamesList() });
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

        public Payment GetPayment()
        {
            Payment payment = new Payment();

            payment.PayeeName = Convert.ToString( cbPayeeDropdown.SelectedValue) ;
            payment.AmountDue = Convert.ToDecimal(tbAmount2Pay.Text.Trim());
            payment.AmountPaid = Convert.ToDecimal(tbAmountPaid.Text.Trim());
            payment.DateDue = Convert.ToDateTime($"{tbDueDate.Text.Trim()}");
            payment.ConfirmationNumber = tbConfirmationNum.Text.Trim();

            return payment;

        }

        public void clearPayment()
        {
            tbAmountPaid.Text = string.Empty;
            tbConfirmationNum.Text = string.Empty;
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

        public int checkPaymentValidity()
        {
            int flag = 0;

            if(tbAmountPaid.Text == "" || Convert.ToDecimal(tbAmountPaid.Text) <= 0)
            {
                flag = 1;
                MessageBox.Show("Amount paid must be greater than 0");
                tbAmountPaid.Focus();
            }
            else if(tbConfirmationNum.Text is null || tbConfirmationNum.Text.Length == 0)
            {
                flag = 1;
                MessageBox.Show("You must have a confirmation number");
                tbConfirmationNum.Focus();
            }

            return flag;
        }

        public static List<string> payeeNamesList()
        {
            var payees = CrudOperations.GetPayees();
            List<string> payeeNames = new List<string>();

            foreach (var payee in payees)
            {
                payeeNames.Add(payee.PayeeName);
            }

            return payeeNames;
        }

        public static Payee payeeFullList(string payeeName)
        {
            List<Payee> fullList = CrudOperations.GetPayees();

            for(int i = 0; i < fullList.Count; i++)
            {
                if (payeeName == fullList[i].PayeeName)
                    return fullList[i];
              
                    ;
            }
            return fullList[0];
        }

        private void cbPayeeDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var payeeSentFromPulldown = Convert.ToString( e.AddedItems[0]);
            
            tbAmount2Pay.Text = string.Empty;
            var selectedPayee = payeeFullList(payeeSentFromPulldown);
            tbAmount2Pay.Text = Convert.ToString(selectedPayee.Amountdue);
            if (selectedPayee.DateDue < DateTime.Now.Day)
                tbDueDate.Text = $"{DateTime.Now.AddMonths(1).Month}/{selectedPayee.DateDue}/{DateTime.Now.Year} ";
            else
                tbDueDate.Text = $"{DateTime.Now.Month}/{selectedPayee.DateDue}/{DateTime.Now.Year} ";
            tbURLpay.Text = selectedPayee.URL;
            
        }
    }
}

