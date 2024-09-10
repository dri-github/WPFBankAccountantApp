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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            Requests.Connect();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            Worker? worker = Requests.GetPersonalData(loginBox.Text, "accountant", passwordBox.Password);
            if (worker != null)
            {
                MessageBox.Show("Worker success");
                Invoice? invoice = Requests.GetRootInvoice();
                if (invoice != null)
                {
                    invoice.Owner = worker;

                    MainWindow mainWindow = new MainWindow(invoice);
                    mainWindow.LayoutTransform = LayoutTransform.Clone();
                    mainWindow.WindowState = WindowState;
                    mainWindow.Show();
                    Close();
                }
            }
            passwordBox.Clear();
        }
    }
}
