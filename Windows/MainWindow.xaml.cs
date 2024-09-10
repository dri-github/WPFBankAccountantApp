using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Resources;

namespace AccountantApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Invoice? root = null;
        private ObservableCollection<TreeViewItemNode> treeViewItemNodes;

        public MainWindow(Invoice invoice)
        {
            root = invoice;

            InitializeComponent();
            
            CommandBinding commandBinding = new CommandBinding();
            commandBinding.Command = ApplicationCommands.Help;
            commandBinding.Executed += CommandHelp_Executed;
            aboutMenuItem.CommandBindings.Add(commandBinding);

            currentNameBox.Text = root.Owner.Surname;
            currentIDBox.Text = root.Owner.Id;

            invoicesTree.Items.Clear();
            treeViewItemNodes = new ObservableCollection<TreeViewItemNode> { InvoiceToTree(root) };
            invoicesTree.ItemsSource = treeViewItemNodes;

            ChangesManager.bind(changesGrid);
        }

        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Database saved");
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CommandHelp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("I create the program");
        }

        private TreeViewItemNode InvoiceToTree(Invoice invoice)
        {
            TreeViewItemNode node = new TreeViewItemNode { Title = invoice.Id.ToString(), Account = invoice, ImageSource = "D:/Progects/C#/AccountantApp/AccountantApp/Resources/72.ico" };
            foreach (Invoice child in invoice.Childrens)
            {
                node.Childrens.Add(InvoiceToTree(child));
            }
            foreach (Worker worker in invoice.Workers)
            {
                node.Childrens.Add(new TreeViewItemNode { Title = worker.Surname + " " + worker.GivenName, Account = worker, ImageSource = "D:/Progects/C#/AccountantApp/AccountantApp/Resources/user.png" });
            }

            return node;
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.WindowState = WindowState;
            loginWindow.Show();
            Close();
        }

        private double Search(string text, string query, out int position)
        {
            double result = 0;
            int _position = -1;

            for (int i = 0; i < text.Length; i++)
            {
                int j = 0;
                if (text[i] == query[j])
                {
                    for (j++; j < Math.Min(query.Length, text.Length - i); j++)
                    {
                        if (query[j] != text[i + j])
                            break;
                    }

                    double value = (double)j / query.Length;
                    if (value > result)
                    {
                        _position = i;
                        result = value;
                    }
                    if (result == 1)
                    {
                        position = _position;
                        return result;
                    }
                }
            }

            position = _position;
            return result;
        }

        private SortedDictionary<double, List<TreeViewItemNode>> Fiend(TreeViewItemNode node, string query)
        {
            SortedDictionary<double, List<TreeViewItemNode>> items = new SortedDictionary<double, List<TreeViewItemNode>>();

            while (node.Childrens.Count > 0)
            {
                TreeViewItemNode child = node.Childrens[0];
                node.Childrens.Remove(child);
                foreach (var it in Fiend(child, query))
                {
                    if (!items.ContainsKey(it.Key))
                        items.Add(it.Key, new List<TreeViewItemNode>());
                    items[it.Key].AddRange(it.Value);
                }

                int position;
                double value = Search(child.Title, query, out position);
                if (position != -1)
                {
                    if (!items.ContainsKey(value))
                        items.Add(value, new List<TreeViewItemNode>());
                    items[value].Add(child);
                }
            }

            return items;
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            // ~!
            typeSortedBox_SelectionChanged(null, null);
            sortedByBox_SelectionChanged(null, null);

            var sortedList = Fiend(treeViewItemNodes[0], searchTextBox.Text).ToList();
            sortedList.Reverse();
            foreach (var it in sortedList)
            {
                foreach (var item in it.Value)
                {
                    treeViewItemNodes[0].Childrens.Add(item);
                }
            }
        }

        private ObservableCollection<TreeViewItemNode> GetTreeItems<T>(TreeViewItemNode node)
        {
            ObservableCollection<TreeViewItemNode> items = new ObservableCollection<TreeViewItemNode>();

            for (int i = 0; i < node.Childrens.Count; i++)
            {
                TreeViewItemNode child = node.Childrens[i];
                foreach (var it in GetTreeItems<T>(child))
                    node.Childrens.Add(it);
                if (child.Account is T)
                {
                    items.Add(child);
                } else
                {
                    node.Childrens.Remove(child);
                    i--;
                }
            }

            return items;
        }

        private void typeSortedBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (root != null && invoicesTree != null)
            {
                var items = sortedByBox.Items;
                items.Clear();
                items.Add("None");
                sortedByBox.SelectedIndex = 0;
                treeViewItemNodes[0] = InvoiceToTree(root);
                switch (((ComboBoxItem)typeSortedBox.SelectedItem).Content.ToString())
                {
                    //case "All":
                    //    items.Add("Personal");
                    //    break;
                    case "Invoices":
                        items.Add("Ascending");
                        items.Add("Descending");

                        GetTreeItems<Invoice>(treeViewItemNodes[0]);
                        break;
                    case "Workers":
                        items.Add("Name");
                        items.Add("Asc salary");
                        items.Add("Dec salary");

                        treeViewItemNodes[0].Childrens.Concat(GetTreeItems<Worker>(treeViewItemNodes[0]));
                        break;
                }
            }
        }

        private void sortingBy<T>(ObservableCollection<TreeViewItemNode> items, Func<T, T, bool> func)
        {
            int i = 0;
            while (true)
            {
                i++;
                if (i >= items.Count)
                    break;

                T first = (T)items[i - 1].Account;
                T second = (T)items[i].Account;

                if (func(first, second))
                {
                    (items[i - 1], items[i]) = (items[i], items[i - 1]);
                    i = 0;
                }
            }
        }

        private void cascadeSorting<T>(TreeViewItemNode item, Func<T, T, bool> func)
        {
            sortingBy(item.Childrens, func);
            foreach (var child in item.Childrens)
            {
                cascadeSorting(child, func);
            }
        }

        private void sortedByBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sortedByBox == null)
                return;

            sortedByBox.Visibility = Visibility.Visible;
            var selectedItem = sortedByBox.SelectedItem;
            if (selectedItem == null)
                return;

            switch (selectedItem.ToString())
            {
                case "Ascending":
                    cascadeSorting(treeViewItemNodes[0], (Invoice first, Invoice second) => { return first.Value > second.Value; });
                    break;
                case "Descending":
                    cascadeSorting(treeViewItemNodes[0], (Invoice first, Invoice second) => { return first.Value < second.Value; });
                    break;
                case "Name":
                    sortingBy(treeViewItemNodes[0].Childrens, (Worker first, Worker second) => { return first.Surname[0] > second.Surname[0]; }); //!
                    break;
                case "Asc salary":
                    sortingBy(treeViewItemNodes[0].Childrens, (Worker first, Worker second) => { return first.Salary > second.Salary; });
                    break;
                case "Dec salary":
                    sortingBy(treeViewItemNodes[0].Childrens, (Worker first, Worker second) => { return first.Salary < second.Salary; });
                    break;
            }
        }

        private bool removeAccount(object account, ObservableCollection<TreeViewItemNode> items)
        {
            foreach (var item in items)
            {
                if (item.Account == account)
                {
                    items.Remove(item);
                    return true;
                }

                if (removeAccount(account, item.Childrens))
                    return true;
            }

            return false;
        }

        private void invoicesTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItemNode selectedNode = (TreeViewItemNode)invoicesTree.SelectedItem;
            if (selectedNode == null)
                return;

            info.Children.Clear();
            if (selectedNode.Account is Invoice)
            {
                info.Children.Add(new InvoicePage((Invoice)selectedNode.Account));
            } else if (selectedNode.Account is Worker)
            {
                Worker worker = (Worker)selectedNode.Account;
                WorkerPage workerPage = new WorkerPage(worker);
                workerPage.fireButton.Click += (object sender, RoutedEventArgs e) => 
                {
                    bool isContaint = false;
                    foreach (var item in treeViewItemNodes[0].Childrens)
                    {
                        if (item.Account == worker)
                        {
                            ChangesManager.Change change = new ChangesManager.Change();
                            change.status = "Success";
                            change.name = "Увольнение";
                            change.description = "Уволен " + worker.Surname + " " + worker.GivenName + " по причине " + "none";
                            change.undoRedoAction = new ChangesManager.UndoRedoAction<object, object>((o, p) => { var t = ((ObservableCollection<TreeViewItemNode>)o); if (!t.Contains((TreeViewItemNode)p)) t.Add((TreeViewItemNode)p); else t.Remove((TreeViewItemNode)p); }, treeViewItemNodes[0].Childrens, item, item);
                            ChangesManager.addChange(change);

                            treeViewItemNodes[0].Childrens.Remove(item);
                            Requests.FireWorker(root.Id.ToString(), ((Worker)selectedNode.Account).Id);
                            info.Children.Clear();
                            isContaint = true;
                            break;
                        }
                    }

                    if (isContaint == false)
                        MessageBox.Show("Отказано в доступе", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                };
                info.Children.Add(workerPage);
            } else
            {
                MessageBox.Show("Internal error. Unreal type TreeView item", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InviteWorkerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            InviteWindow inviteWindow = new InviteWindow(ref root);
            inviteWindow.inviteWorker += (Worker? worker) =>
            {
                if (worker != null)
                {
                    var item = new TreeViewItemNode { Title = worker.Surname + " " + worker.GivenName, Account = worker, ImageSource = "D:/Progects/C#/AccountantApp/AccountantApp/user.png" };
                    treeViewItemNodes[0].Childrens.Add(item);
                    
                    ChangesManager.Change change = new ChangesManager.Change();
                    change.status = "Wait";
                    change.name = "Приглашение";
                    change.description = "Отправлено приглашение " + worker.Surname + " " + worker.GivenName;
                    change.undoRedoAction = new ChangesManager.UndoRedoAction<object, object>((o, p) => { var t = ((ObservableCollection<TreeViewItemNode>)o); if (!t.Contains((TreeViewItemNode)p)) t.Add((TreeViewItemNode)p); else t.Remove((TreeViewItemNode)p); }, treeViewItemNodes[0].Childrens, item, item);
                    ChangesManager.addChange(change);
                }
            };
            inviteWindow.ShowDialog();
        }

        private void ruLanguageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (((MenuItem)sender).Header)
            {
                case "eng":
                    dict.Source = new Uri("eng_dictionary.xaml", UriKind.Relative);
                    break;
                case "ru":
                    dict.Source = new Uri("ru_dictionary.xaml", UriKind.Relative);
                    break;
                default:
                    //dict.Source = new Uri("..\\Resources\\StringResources.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void CommandUndo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangesManager.Undo();
        }

        private void CommandRedo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangesManager.Redo();
        }

        private void CommandCommit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Commit command execute");
        }
    }

    public class TreeViewItemNode
    {
        public string Title { get; set; }
        public string ImageSource { get; set; }
        public object Account { get; set; }
        public ObservableCollection<TreeViewItemNode> Childrens { get; set; }

        public TreeViewItemNode()
        {
            Childrens = new ObservableCollection<TreeViewItemNode>();
        }
    }
}