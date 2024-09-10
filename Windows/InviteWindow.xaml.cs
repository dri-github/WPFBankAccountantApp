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

namespace AccountantApp
{
    /// <summary>
    /// Логика взаимодействия для InviteWindow.xaml
    /// </summary>
    public partial class InviteWindow : Window
    {
        private Invoice invoice;
        public Action<Worker?> inviteWorker;

        public InviteWindow(ref Invoice invoice)
        {
            InitializeComponent();

            this.invoice = invoice;
        }

        private void surnameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableInvoiceButton();
        }

        private void givenNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableInvoiceButton();
        }

        private void workerIdBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableInvoiceButton();
        }

        private void EnableInvoiceButton()
        {
            inviteButton.IsEnabled = (PassportId.Text.Length > 0 && JobTitle.Text.Length > 0 && Salary.Text.Length > 0);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void inviteButton_Click(object sender, RoutedEventArgs e)
        {
            Requests.InviteWorker(PassportId.Text, JobTitle.Text, uint.Parse(Salary.Text));
            Close();
        }
    }
}

/*
 private Worker worker { get; set; }

        public WorkerPage(Worker worker)
        {
            InitializeComponent();

            this.worker = worker;
            surname.Text = worker.surname;
            givenName.Text = worker.givenName;
            additionalName.Text = "XXX";
            numberPassport.Text = worker.passportNumber;
            idPassport.Text = worker.passportNumber;

            mail.Text = worker.email;
            telephone.Text = worker.telephone;
            address.Text = worker.address;

            salaryBox.Text = worker.salary.ToString();
        }

        private void salaryBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            worker.salary = Convert.ToInt32(salaryBox.Text);
        }

        private void fireButton_Click(object sender, RoutedEventArgs e)
        {

        }
 */