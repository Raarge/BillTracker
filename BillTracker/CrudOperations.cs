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
    }
}
