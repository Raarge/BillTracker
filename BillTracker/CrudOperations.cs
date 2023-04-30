using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillTracker
{
    
    public class CrudOperations
    {
        private static string sqlConnectionString = @"Data Source = DESKTOP-NCVPBAF\SQLEXPRESS;initial catalog=BillTracker; Integrated Security=true;";
       
        public static int InsertPayee(Payee newPayee)
        {
            using (var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                var affectedRows = connection.Execute("Insert into PaymentAccounts (PayeeName, DateDue, AmountDue, URL) values (@PayeeName, @DateDue, @AmountDue, @URL)",
                    new {PayeeName = newPayee.PayeeName, DateDue = newPayee.DateDue, AmountDue = newPayee.Amountdue, URL = newPayee.URL});
                connection.Close();
                return affectedRows;
            }
        }

        public static int InsertPayment(Payment newPayment)
        {
            using (var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                var affectedRows = connection.Execute("Insert into Payments (PayeeName, AmountDue, AmountPaid, DateDue, ConfirmationNumber) values (@PayeeName, @AmountDue, @AmountPaid, @DateDue, @ConfirmationNumber)",
                    new { PayeeName = newPayment.PayeeName, AmountDue = newPayment.AmountDue, AmountPaid = newPayment.AmountPaid, DateDue = newPayment.DateDue, ConfirmationNumber = newPayment.ConfirmationNumber });
                connection.Close();
                return affectedRows;
            }
        }

        public static int InsertPayCycle(PayCycle newPayCycle)
        {
            using (var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                var affectedRows = connection.Execute("Insert into PayCycles (PayDate, EndDate) values (@PayDate, @EndDate)",
                    new { PayDate = newPayCycle.PayDate, EndDate = newPayCycle.EndDate });
                connection.Close();

                return affectedRows;
            }
        }

        public static bool SearchPayee(string payeeName)
        {
            var name = "";
            using(var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                var payee =  connection.Query<string>("SELECT PayeeName FROM PaymentAccounts WHERE PayeeName = @pn", new {pn = payeeName});
                connection.Close();
                foreach (var row in payee)
                {
                    name = row;
                }
            }
            
            if (name is null || name == "")
            {
                return false;
            }
            else
                return true;
        }

        public static List<Payee> GetPayees()
        {
            using(var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                var payees = connection.Query<Payee>("SELECT * from PaymentAccounts").ToList();
                connection.Close();

                return payees;
            }
        }

        public static string GetLastPayCycleInTable()
        { 
            List<PayCycle> payCyclesEntered = new List<PayCycle>();
            PayCycle lastPayCycle = new PayCycle();
            var lastCycle = string.Empty;
            
            using(var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                payCyclesEntered = connection.Query<PayCycle>("Select * from PayCycles ORDER BY PayDate DESC").ToList();
                connection.Close();
            }

            lastPayCycle = payCyclesEntered[0];

            lastCycle = $" Pay Day: {lastPayCycle.PayDate.ToShortDateString()}, End Cycle Day: {lastPayCycle.EndDate.ToShortDateString()}";

            return lastCycle;
        }

        public static List<PayCycle> GetAllPayCycles()
        {
            List<PayCycle> payCycles = new List<PayCycle>();

            using (var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                payCycles = (List<PayCycle>) connection.Query<PayCycle>("Select * from PayCycles");
                connection.Close();
            }

            return payCycles;
        }

        public static List<Payee> getAllBills()
        {
            List<Payee> bills = new List<Payee>();

            using ( var connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                bills = (List<Payee>)connection.Query<Payee>("Select * from PaymentAccounts order by PayeeName");
                connection.Close();
            }

            return bills;
        }
    }
}
