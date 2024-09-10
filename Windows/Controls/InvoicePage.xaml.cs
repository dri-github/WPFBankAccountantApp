using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AccountantApp
{
    /// <summary>
    /// Логика взаимодействия для InvoicePage.xaml
    /// </summary>
    public partial class InvoicePage : UserControl
    {
        public Invoice? Invoice { get; set; }

        public InvoicePage()
        {
            InitializeComponent();
        }

        public InvoicePage(Invoice invoice)
        {
            InitializeComponent();

            invoiceNumberBox.Text = invoice.Id.ToString();
            typeBox.Text = "clone";
            valueBox.Text = invoice.Value.ToString();

            Worker? admin = invoice.Owner;
            if (admin == null)
            {
                MessageBox.Show("Отсутствует администратор");
                return;
            }

            adminIdBox.Text = admin.Id.ToString();
            adminSurnameBox.Text = admin.Surname.ToString();
            adminGivenNameBox.Text = admin.GivenName.ToString();

            //InvoicePage.ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InvoicePage));
        }

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        // обертка над событием
        public event RoutedEventHandler Click
        {
            add
            {
                // добавление обработчика
                base.AddHandler(ButtonBase.ClickEvent, value);
            }
            remove
            {
                // удаление обработчика
                base.RemoveHandler(ButtonBase.ClickEvent, value);
            }
        }
    }

    public class Inv : DependencyObject
    {
        public static readonly DependencyProperty TitleProperty;

        static Inv()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
            metadata.CoerceValueCallback = new CoerceValueCallback(CorrectValue);

            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Inv), metadata, new ValidateValueCallback(ValidateValue));
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private static object CorrectValue(DependencyObject d, object baseValue)
        {
            string currentValue = (string)baseValue;
            if (currentValue != null && currentValue.Length > 3)
                return "Not";
            return currentValue;
        }

        private static bool ValidateValue(object value)
        {
            string currentValue = (string)value;
            if (currentValue == null || currentValue.Length > 0)
                return true;
            return false;
        }
    }
}
