using System.Data;
using System.IO;
using System.Resources;
using System.Windows;
using Oracle.ManagedDataAccess.Client;

namespace AccountantApp
{
    public static class Requests
    {
        public static string ConnectionString { get { return "DATA SOURCE = //workpc:1521/bank_pdb; USER ID=bank_user; password=user"; } }
        private static OracleConnection? connection = null;
        private static string PassportId = "";
        private static string Password = "";
        private static string OrganizationId = "AB000000000003";
        private static Invoice? RootInvoice = null;

        public static void Connect()
        {
            connection = new OracleConnection(ConnectionString);
            connection.Open();
        }

        public static Worker? GetPersonalData(string passportId, string userType, string password)
        {
            try
            {
                Worker? worker = null;
                using (OracleCommand command = new OracleCommand($"select * from table(root_admin.get_personal_data('{passportId}', '{userType}', '{password}'))", connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MessageBox.Show(reader["SURNAME"].ToString() + ' ' + reader["GIVEN_NAME"].ToString() + ' ' + reader["PASSPORT_NUMBER"].ToString());
                            worker = new Worker(passportId, reader["SURNAME"].ToString(), reader["GIVEN_NAME"].ToString(), reader["PASSPORT_NUMBER"].ToString(), reader["TELEPHONE"].ToString(), reader["EMAIL"].ToString(), reader["ADDRESS"].ToString(), 0);
                        }
                    }
                }

                PassportId = passportId;
                Password = password;
                return worker;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        private static Dictionary<uint, Invoice>? GetInvoices()
        {
            try
            {
                Dictionary<uint, Invoice> invoices = new Dictionary<uint, Invoice>();

                using (OracleCommand command = new OracleCommand($"select * from table(root_admin.get_user_invoices('{PassportId}', 'accountant', '{Password}'))", connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint root = uint.Parse(reader["ROOT"].ToString());
                            if (invoices.ContainsKey(root) == true)
                                continue;

                            invoices.Add(root, new Invoice(uint.Parse(reader["ID"].ToString()), reader["CURRENCY"].ToString(), double.Parse(reader["VALUE"].ToString()), null, new List<Invoice>(), new List<Worker>()));
                        }
                    }
                }

                return invoices;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        public static Invoice? GetRootInvoice()
        {
            try
            {
                Dictionary<uint, Invoice>? invoices = GetInvoices();
                if (invoices == null)
                    return null;

                foreach (var invoice in invoices)
                {
                    if (invoices.ContainsKey(invoice.Value.Id) == true)
                    {
                        invoice.Value.Childrens.Add(invoices[invoice.Value.Id]);
                        invoices.Remove(invoice.Value.Id);
                    }
                }

                if (invoices.Count == 1)
                    MessageBox.Show("Success fiend childs invoices " + invoices.Count);
                return RootInvoice = invoices.First().Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        public static void InviteWorker(string passportId, string jobTitle, uint salary)
        {
            try
            {
                using (OracleCommand command = new OracleCommand("root_admin.register_worker", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_organization_id", OracleDbType.Char).Value = OrganizationId;
                    command.Parameters.Add("p_organization_invoice", OracleDbType.Int32).Value = RootInvoice.Id;
                    command.Parameters.Add("p_worker_id", OracleDbType.Char).Value = passportId;
                    command.Parameters.Add("p_job_title", OracleDbType.Varchar2).Value = jobTitle;
                    command.Parameters.Add("p_salary", OracleDbType.Int32).Value = salary;

                    command.ExecuteNonQuery();
                    MessageBox.Show("Invite");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool FireWorker(string invoiceId, string workerId)
        {
            return true;
        }

        public static Worker? GetAdmin(string invoiceId)
        {
            return null;
        }
    }
}