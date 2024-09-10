using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Channels;
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

namespace AccountantApp
{
    /// <summary>
    /// Логика взаимодействия для WorkerPage.xaml
    /// </summary>
    public partial class WorkerPage : UserControl
    {
        public Action<Worker> fireWorker { get; set; }
        private Worker worker { get; set; }

        private int afterSalary;

        public WorkerPage(Worker worker)
        {
            InitializeComponent();

            this.worker = worker;
            surname.Text = worker.Surname;
            givenName.Text = worker.GivenName;
            additionalName.Text = "XXX";
            numberPassport.Text = worker.PassportNumber;
            idPassport.Text = worker.PassportNumber;

            mail.Text = worker.Email;
            telephone.Text = worker.Telephone;
            address.Text = worker.Address;

            salaryBox.Text = worker.Salary.ToString();
        }

        private void salaryBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (salaryBox.Text != "")
                worker.Salary = Convert.ToInt32(salaryBox.Text);
            else
                worker.Salary = 0;
        }

        private void salaryBox_Focused(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (salaryBox.IsFocused)
            {
                int currentSalary = Convert.ToInt32(salaryBox.Text);
                if (currentSalary != afterSalary)
                {
                    ChangesManager.Change change = new ChangesManager.Change();
                    change.status = "Success";
                    change.name = "Изменение заработной платы";
                    change.undoRedoAction = new ChangesManager.UndoRedoAction<object, object>((o, p) => { ((Worker)o).Salary = (int)p; }, worker, afterSalary, currentSalary);

                    if (currentSalary > afterSalary)
                        change.description = "Повышение с " + afterSalary.ToString() + " до " + salaryBox.Text + " (" + worker.Surname + " " + worker.GivenName + ")";
                    else
                        change.description = "Снижение с " + afterSalary.ToString() + " до " + salaryBox.Text + " (" + worker.Surname + " " + worker.GivenName + ")";

                    ChangesManager.addChange(change);
                }
            } else
            {
                afterSalary = Convert.ToInt32(salaryBox.Text);
            }
        }

        private void fireButton_Click(object sender, RoutedEventArgs e)
        {
            //fireWorker.Invoke(worker);
        }
    }
}
