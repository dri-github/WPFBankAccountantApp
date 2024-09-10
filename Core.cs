using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

/*
 public class Invoice : DependencyObject
    {
        public string id { get; set; }
        public long value { get; set; }
        public string currency { get; set; }
        public List<Invoice> childrens { get; set; }
        public Worker? owner { get; set; }
        public List<Worker> workers { get; private set; }

        public static readonly DependencyProperty ValueProperty;
        public static readonly DependencyProperty OwnerProperty;
        public static readonly DependencyProperty ChildrensProperty;
        public static readonly DependencyProperty WorkersProperty;

        public Invoice() { }
        public Invoice(string id, long value, string currency, Invoice parent, Worker? user = null)
        {
            this.id = id;
            this.value = value;
            this.currency = currency;
            this.owner = user;
            childrens = new List<Invoice>();
            workers = new List<Worker>();
        }
        public Invoice(string id, long value, string currency, Worker? user, Invoice? parent = null)
        {
            this.id = id;
            this.value = value;
            this.currency = currency;
            this.owner = user;
            childrens = new List<Invoice>();
            workers = new List<Worker>();
        }

        public void RecruitWorker(Worker worker)
        {
            workers.Add(worker);
        }
    }
 */

/*
 public class Invoice : DependencyObject
    {
        public string id { get; set; }
        public long value { get; set; }
        public string currency { get; set; }

        public static readonly DependencyProperty OwnerProperty;
        public static readonly DependencyProperty ChildrensProperty;
        public static readonly DependencyProperty WorkersProperty;

        static Invoice()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
            //metadata.CoerceValueCallback = new CoerceValueCallback(CorrectValue);

            OwnerProperty     = DependencyProperty.Register("Owner", typeof(Worker), typeof(Invoice));
            ChildrensProperty = DependencyProperty.Register("Childrens", typeof(List<Invoice>), typeof(Invoice));
            WorkersProperty   = DependencyProperty.Register("Workers", typeof(List<Worker>), typeof(Invoice));
        }

        public Worker Owner
        {
            get { return (Worker)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }
        public ObservableCollection<Invoice> Childrens
        {
            get { return (ObservableCollection<Invoice>)GetValue(ChildrensProperty); }
            set { SetValue(OwnerProperty, value); }
        }
        public List<Worker> Workers
        {
            get { return (List<Worker>)GetValue(WorkersProperty); }
            set { SetValue(OwnerProperty, value); }
        }
    }

public class Worker
    {
        public string id { get; set; }
        public string surname { get; set; }
        public string givenName { get; set; }
        public string passportNumber { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string password { get; set; }
        public int salary { get; set; }

        private Worker() { }
        public Worker(string id, string surname, string givenName, string passportNumber, string telephone, string email, string address, string password)
        {
            this.id = id;
            this.surname = surname;
            this.givenName = givenName;
            this.passportNumber = passportNumber;
            this.telephone = telephone;
            this.email = email;
            this.address = address;
            this.password = password;
        }
    }
 */

namespace AccountantApp
{
    public class SystemCommands
    {
        public static RoutedUICommand Commit { get; set; }

        static SystemCommands()
        {
            Commit = new RoutedUICommand("Commit changes in session", "Commit", typeof(MainWindow));
        }
    }

    public class Worker : DependencyObject
    {
        public string Id { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }
        public string PassportNumber { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int Salary { get; set; }

        static Worker() { }
        public Worker(string id, string surname, string givenName, string passportNumber, string telephone, string email, string address, int salary)
        {
            Id = id;
            Surname = surname;
            GivenName = givenName;
            PassportNumber = passportNumber;
            Telephone = telephone;
            Email = email;
            Address = address;
            Salary = salary;
        }
    }

    public class Invoice : DependencyObject
    {
        public uint Id { get; set; }
        public string Currency { get; set; }
        public double Value { get; set; }

        public static readonly DependencyProperty OwnerProperty;
        public static readonly DependencyProperty ChildrensProperty;
        public static readonly DependencyProperty WorkersProperty;

        static Invoice()
        {
            OwnerProperty = DependencyProperty.Register("Owner", typeof(Worker), typeof(Invoice));
            ChildrensProperty = DependencyProperty.Register("Childrens", typeof(List<Invoice>), typeof(Invoice));
            WorkersProperty = DependencyProperty.Register("Workers", typeof(List<Worker>), typeof(Invoice));
        }
        public Invoice() { }
        public Invoice(uint id, string currency, double value, Worker owner, List<Invoice> childrens, List<Worker> workers)
        {
            Id = id;
            Currency = currency;
            Value = value;
            Owner = owner;
            Childrens = childrens;
            Workers = workers;
        }

        public Worker Owner
        {
            get { return (Worker)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }
        public List<Invoice> Childrens
        {
            get { return (List<Invoice>)GetValue(ChildrensProperty); }
            set { SetValue(ChildrensProperty, value); }
        }
        public List<Worker> Workers
        {
            get { return (List<Worker>)GetValue(WorkersProperty); }
            set { SetValue(WorkersProperty, value); }
        }
    }
}