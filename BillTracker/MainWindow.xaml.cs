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
            populateLastCycle();
            getCurrentPayCycle();
            printAllBillsDue();
            cbPayeeDropdown.SetBinding(
                ItemsControl.ItemsSourceProperty,
                new Binding { Source = payeeNamesList() });
            

            
        }

        private void btnStartingAmount_OnClick(object sender, RoutedEventArgs e)
        {
            var amount = Convert.ToDecimal( tbStartingAmount.Text.Trim());
            tbCurrentAmount.Text = string.Empty;
            tbCurrentAmount.Text = string.Format("{0:C}", amount);

            BillTabs.SelectedIndex = 0;
            cbPayeeDropdown.Focus();
        }

        private void btnSubmitPayment_OnClick(object sender, RoutedEventArgs e)
        {
            var payment = GetPayment();
            var flag = checkPaymentValidity();
            var currentAmount = Convert.ToDecimal(tbCurrentAmount.Text.Trim().Substring(1));

            if (flag == 0)
            {
                var returnCode = CrudOperations.InsertPayment(payment);
                if (returnCode > 0)
                {
                    MessageBox.Show($"{payment.PayeeName} was paid");
                    tbCurrentAmount.Text = string.Format("{0:C}", currentAmount - payment.AmountPaid);
                    txblkCurrentPaid.Text = $"{txblkCurrentPaid.Text}  {DateTime.Now.ToShortDateString()} - Payee: {payment.PayeeName}, Amount Paid: ${Convert.ToString(payment.AmountPaid)} \n";
                    clearPayment();

                    cbPayeeDropdown.SelectedIndex = 0; // look for a way to reset the pulldown cb
                }


            }
            else
            {
                MessageBox.Show("Too Bad Not Valid");
            }            
        }

        private void btnInsertPayCycle_OnClick(object sender, RoutedEventArgs e)
        {
            var payCycle = GetNewPayCycle();
            var validFlag = checkPayCycleValidity(payCycle);

            if (validFlag == 0)
            {
                var returnCode = CrudOperations.InsertPayCycle(payCycle);
                if(returnCode > 0)
                {
                    MessageBox.Show("Pay cycle was Added.");
                    clearPayCycle();
                    populateLastCycle();
                }
            }
            else
            {
                
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

                    txblkBillsDue.Text = string.Empty;
                    printAllBillsDue();
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

        public PayCycle GetNewPayCycle()
        {
            PayCycle payCycle = new PayCycle();
            payCycle.PayDate = Convert.ToDateTime(dptbPayDate.Text);
            payCycle.EndDate = Convert.ToDateTime(tbEndDate.Text);

            return payCycle;
        }

        public Payee GetPayee()
        {
            Payee payee = new Payee();
            var dateDue = Convert.ToInt32(tbDateDue.Text);

            payee.PayeeName = tbPayeeName.Text;
            if(DateTime.Now.Month == 2 && dateDue > 28)
            {
                payee.DateDue = 28;
            }
            else if(dateDue > 30)
            {
                payee.DateDue = 30;
            }
            else
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
            payment.AmountPaid = Convert.ToDecimal(string.Format("{0:0.00}", tbAmountPaid.Text.Trim()));
            payment.DateDue = Convert.ToDateTime($"{tbDueDate.Text.Trim()}");
            payment.ConfirmationNumber = tbConfirmationNum.Text.Trim();

            return payment;

        }

        public void clearPayment()
        {
            tbAmountPaid.Text = string.Empty;
            tbConfirmationNum.Text = string.Empty;
        }

        public void clearPayCycle()
        {
            dptbPayDate.Text = string.Empty;
            tbEndDate.Text = string.Empty;
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

        public int checkPayCycleValidity(PayCycle sentPayCycle)
        {
            int flag = 0;

            if(Convert.ToString(sentPayCycle.PayDate) is null)
            {
                flag = 1;
                MessageBox.Show("You must choose a date");
                dptbPayDate.Focus();
            }
            else if (tbEndDate.Text is null)
            {
                flag = 1;
                MessageBox.Show("Error in Calcultion!");
                tbEndDate.Focus();
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

            payeeNames.Sort();

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
            var startAmount = tbCurrentAmount.Text.Substring(1);
            if (Convert.ToDecimal(startAmount) == 0.00M)
            {
                MessageBox.Show($"The starting amount is still set to $0.00.  Correct and Try Again.");

                BillTabs.SelectedIndex = 1;
                tbStartingAmount.Focus();
            }
            else
            {
                var payeeSentFromPulldown = Convert.ToString(e.AddedItems[0]);

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

        private void dptbPayDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dptbPayDate.Text.Length > 0)
            {
                tbEndDate.Text = Convert.ToString(Convert.ToDateTime(dptbPayDate.Text).AddDays(13).Date.ToShortDateString());
            }
            
        }

        public void populateLastCycle()
        {
            tblkLastEntered.Text = CrudOperations.GetLastPayCycleInTable();
;       }

        public void getCurrentPayCycle()
        {
            var allCycles = CrudOperations.GetAllPayCycles();
            var payCycle = new PayCycle();

            foreach(var cycle in allCycles)
            {
                if (Convert.ToDateTime(cycle.PayDate) >= DateTime.Now.AddDays(-7) && Convert.ToDateTime(cycle.PayDate) <= DateTime.Now.AddDays(13))
                {
                    payCycle = cycle;
                }
               
            }

            tbPayPeriod.Text = $"{payCycle.PayDate.ToShortDateString()} - {payCycle.EndDate.ToShortDateString()}";
        }

        public List<Payee> getCurrentBillsToPay()
        {
            var allCycles = CrudOperations.GetAllPayCycles();
            var payCycle = new PayCycle();
            var allBills = CrudOperations.getAllBills();
            var bills = new List<Payee>();
            var payDate = 0;
            var endDate = 0;

            foreach (var cycle in allCycles)
            {
                if (Convert.ToDateTime(cycle.PayDate) >= DateTime.Now.AddDays(-7) && Convert.ToDateTime(cycle.PayDate) <= DateTime.Now.AddDays(13))
                {
                    payCycle = cycle;
                }
            }

            payDate = Convert.ToInt32(payCycle.PayDate.Day);
            endDate = Convert.ToInt32(payCycle.EndDate.Day);

            
            foreach(var bill  in allBills)
            {
                if(bill.DateDue >= payDate && bill.DateDue <= endDate)
                {
                    bills.Add(bill);
                }
            }
            
            
            return bills;
        }       

        public void printAllBillsDue()
        {
            var billsDue = getCurrentBillsToPay();
            var billString = "";

            foreach (var bill in billsDue)
            {
                if(bill.PayeeName != "--Please Select--")
                    billString = $"{billString}  Payee Name: {bill.PayeeName}  Due Date: {bill.DateDue} Amount Due: ${bill.Amountdue} \n";
            }

            txblkBillsDue.Text = billString;
        }
    }
}

