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
    }
}
