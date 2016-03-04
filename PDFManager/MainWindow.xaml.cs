using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.ComponentModel;
using System.IO;

namespace PDFManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
//     class OutPutArg
//     {
//         public string Filename;
//         public string BookDir;
//         public List<Book> Books;
//     }

    public partial class MainWindow : Window
    {
        string _bookDir = "..\\";
        string _dataFileName="myBooks.xml";
        string _labelFileName = "labels.xml";
        string _historyFileName = "HistoryBooks.xml";
        string _currentDir = System.Windows.Forms.Application.StartupPath;

        bool _panelDisabled = true;
        string _unclassified = "未分类";
        string _selectAll = "全部";
        XmlDocument _document;
        BackgroundWorker _bw;

        List<Book> _books;
        List<string> _labels;
        List<Book> _currentSelectBooks;

        AddLabel _addLabel;

        public MainWindow()
        {
            InitializeComponent();

            Initialize();
        }

        public bool AddLabels()
        {
            string newLabel = _addLabel.Label;
            if (_labels.Contains(newLabel))
            {
                return false;
            }

            _labels.Add(newLabel);
            System.Windows.Controls.CheckBox labelCheck = GenerateLabelCheckbox(newLabel);
            wrapPanel_label.Children.Add(labelCheck);
            comboBox_category.Items.Add(newLabel);
            FundamentalFunctions.OutputLabels(_labelFileName,_labels);
            
            return true;

        }

        private void Initialize()
        {
            this.Title = "PDF管理器";
            comboBox_category.Items.Add(_unclassified);
            comboBox_category.Items.Add(_selectAll);
            wrapPanel_label.IsEnabled = false;
            _addLabel = new AddLabel(this);
            _books = new List<Book>();
            _labels = new List<string>();
            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            SetListboxMenu();

            if (File.Exists(_dataFileName))
            {
                FundamentalFunctions.ReadBookInfo(_dataFileName,out _bookDir,out _books);
                listBox_book.ItemsSource = _books;

                _document = new XmlDocument();
                _document.Load(_dataFileName);
            }

            if (File.Exists(_labelFileName))
            {
                FundamentalFunctions.ReadLabels(_labelFileName,out _labels);

                foreach (string label in _labels)
                {
                    System.Windows.Controls.CheckBox labelCheck = GenerateLabelCheckbox(label);
                    wrapPanel_label.Children.Add(labelCheck);

                    comboBox_category.Items.Add(label);
                }
            }

            if (!File.Exists(_historyFileName))
            {
                XmlDocument document = new XmlDocument();
                XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "utf-8", "yes");
                document.AppendChild(declaration);
                XmlElement root = document.CreateElement("Books");
                document.AppendChild(root);

                XmlElement directory_elem = document.CreateElement("Directory");
                directory_elem.SetAttribute("Value", _bookDir);
                root.AppendChild(directory_elem);

                FileStream stream = new FileStream(_historyFileName, FileMode.Create);
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                document.WriteContentTo(writer);
                writer.Flush();
                writer.Close();
                stream.Close();
            }

            _books.Sort();
        }

        private void SetListboxMenu()
        {
             ContextMenu menu = new ContextMenu();
//             MenuItem menuItem1 = new MenuItem();
//             menuItem1.Header = "添加文件";
//             menuItem.Click += new RoutedEventHandler(menuItem_Click);
//             menu.Items.Add(menuItem1);

            MenuItem menuItem2 = new MenuItem();
            menuItem2.Header = "删除文件";
            menuItem2.Click += new RoutedEventHandler(listBox_book_RemoveBook);
            menu.Items.Add(menuItem2);
            listBox_book.ContextMenu = menu;

            MenuItem menuItem3 = new MenuItem();
            menuItem3.Header = "查看历史标签";
            menuItem3.Click += new RoutedEventHandler(listBox_book_ViewHistoryLabel);
            menu.Items.Add(menuItem3);
            listBox_book.ContextMenu = menu;
        }


        private void listBox_book_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBox_book.SelectedItems != null && listBox_book.SelectedItems.Count > 0)
            {
                Book url = listBox_book.SelectedItems[0] as Book;
                string path = url.Directory;
                if (File.Exists(path))
                    System.Diagnostics.Process.Start(path);
            }
        }

        private void button_initialize_Click(object sender, RoutedEventArgs e)
        {
            //Initialize();

            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "选择文件夹";
            fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
            fbd.SelectedPath = "D:\\ ";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _bookDir = fbd.SelectedPath;

                List<string> bookpath = new List<string>();
                var tmpBookNames = Directory.EnumerateFiles(_bookDir, "*.pdf");
                foreach (string currentFile in tmpBookNames)
                {
                    bookpath.Add(currentFile);
                }

                _books = new List<Book>();
                foreach (string path in bookpath)
                {
//                     string bookname = path.TrimStart(_bookDir.ToCharArray());
//                     bookname = bookname.TrimEnd(".pdf".ToCharArray());

                    string bookname = FundamentalFunctions.TrimBookName(path, _bookDir, ".pdf");
                    Book mybook = new Book();
                    mybook.Name = bookname;
                    mybook.Directory = path;
                    _books.Add(mybook);
                }

                _books.Sort();
                listBox_book.ItemsSource = _books;

                BackGourndOutputFile();
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            FundamentalFunctions.OutputBookInfo(_currentDir + "\\" + _dataFileName, _bookDir, _books);
        }

        private void BackGourndOutputFile()
        {
            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.RunWorkerAsync();

        }

        private void button_setLabel_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSelectBooks == null)
            {
                MessageBox.Show("请先选中一本书");
                return;
            }

            if (_panelDisabled)
            {
                listBox_book.IsEnabled = false;
                wrapPanel_label.IsEnabled = true;
                button_setLabel.Content = "确定";
                listBox_book.Focus();
                _panelDisabled = false;
                
            }
            else
            {
                listBox_book.IsEnabled = true;
                wrapPanel_label.IsEnabled = false;
                button_setLabel.Content = "设定标签";
                _panelDisabled = true;

                //get selected labels
                UIElementCollection labelItems = wrapPanel_label.Children;
                List<string> selectedLabels = new List<string>();
                foreach (UIElement ui in labelItems)
                {
                    CheckBox labelCheck = ui as CheckBox;
                    if (labelCheck == null) continue;

                    if (labelCheck.IsChecked == true)
                    {
                        selectedLabels.Add(labelCheck.Content as string);
                    }
                }

                if (_currentSelectBooks.Count == 1)
                {
                    Book book = FundamentalFunctions.GetBook(_books, _currentSelectBooks[0].Name);

                    if (book != null)
                    {
                        book.Labels = selectedLabels;
                    }
                }
                else
                {
                    foreach (Book selectedBook in _currentSelectBooks)
                    {
                        List<string> itsLabels = selectedBook.Labels;
                        foreach (string label in selectedLabels)
                        {
                            if (!itsLabels.Contains(label))
                            {
                                Book book = FundamentalFunctions.GetBook(_books, selectedBook.Name);
                                if (book != null)
                                {
                                    book.Labels.Add(label);
                                }
                            }
                        }
                    }

                    
                }
                
                BackGourndOutputFile();
            }
        }

        private void button_addLabel_Click(object sender, RoutedEventArgs e)
        {
            _addLabel.Visibility = Visibility.Visible;
            _addLabel.Top=this.Top+this.Height/3;
            _addLabel.Left = this.Left + this.Width / 3;
            _addLabel.Focus();
            _addLabel.textBox_label.ForceCursor = true;
//             if (_currentSelectBook == null)
//             {
//                 System.Windows.MessageBox.Show("请先选中一本书");
//             }
            this.IsEnabled = false;

        }

        private void listBox_book_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_book.SelectedItems.Count == 0) return;

            _currentSelectBooks = new List<Book>();
            for (int i = 0; i < listBox_book.SelectedItems.Count;++i )
            {
                Book book = listBox_book.SelectedItems[i] as Book;
                if(book==null)  continue;
                _currentSelectBooks.Add(book);
            }


            if (_currentSelectBooks == null) return;

            if (_currentSelectBooks.Count == 1)
            {
                List<string> labels = _currentSelectBooks[0].Labels;

                UIElementCollection labelItems = wrapPanel_label.Children;
                foreach (UIElement ui in labelItems)
                {
                    CheckBox labelCheck = ui as CheckBox;
                    if (labelCheck == null) continue;

                    string content = labelCheck.Content as string;
                    if (labels.Contains(content))
                    {
                        labelCheck.IsChecked = true;
                    }
                    else
                    {
                        labelCheck.IsChecked = false;
                    }
                }

                MenuItem newItem = new MenuItem();
                newItem.Header = "查看历史标签";
                newItem.Click += new RoutedEventHandler(listBox_book_ViewHistoryLabel);
                FundamentalFunctions.AddMenuItem(listBox_book.ContextMenu, newItem);
            }
            else
            {
                UIElementCollection labelItems = wrapPanel_label.Children;
                foreach (UIElement ui in labelItems)
                {
                    CheckBox labelCheck = ui as CheckBox;
                    if (labelCheck == null) continue;

                    labelCheck.IsChecked = false;
                }

                FundamentalFunctions.RmoveMenuItem(listBox_book.ContextMenu, "查看历史标签");
            }
            
            
        }

        private void button_refreshDir_Click(object sender, RoutedEventArgs e)
        {
            var tmpBookNames = Directory.EnumerateFiles(_bookDir, "*.pdf");
            List<bool> existed = new List<bool>();
            for (int i = 0; i < _books.Count; ++i)
            {
                existed.Add(false);
            }

            

            foreach (string path in tmpBookNames)
            {
                string bookname = FundamentalFunctions.TrimBookName(path, _bookDir, ".pdf");

                if (!FundamentalFunctions.IsBooknameExist(_books,bookname))
                {
                    
                    Book mybook = new Book();
                    mybook.Name = bookname;
                    mybook.Directory = path;
                    _books.Add(mybook);
                }
                else
                {
                    int index = FundamentalFunctions.GetIndexOfBook(_books, bookname);
                    existed[index] = true;
                }
            }

            for (int i = 0; i < existed.Count;++i )
            {
                if (existed[i] == false)
                {
                    Book book = _books[i];
                    _books.Remove(book);
                }
            }

            _books.Sort();
            listBox_book.Items.Refresh();
            BackGourndOutputFile();
        }

        private void comboBox_category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string label = comboBox_category.SelectedItem as string;

            if (label == null) return;

            if (label == _unclassified)
            {
                listBox_book.ItemsSource = FundamentalFunctions.GetUnlabeledBooks(_books);
                listBox_book.Items.Refresh();
            }
            else if (label == _selectAll)
            {
                listBox_book.ItemsSource = _books;
                listBox_book.Items.Refresh();
            }
            else
            {
                listBox_book.ItemsSource = FundamentalFunctions.GetLabeledBooks(label,_books);
                listBox_book.Items.Refresh();                
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FundamentalFunctions.OutputBookInfo(_dataFileName, _bookDir, _books);
            FundamentalFunctions.OutputLabels(_labelFileName, _labels);
            FundamentalFunctions.UpdateHistoryBookInfo(_historyFileName, _bookDir, _books);

        }

       

        private void listBox_book_AddNewBook(object sender, RoutedEventArgs e)
        {
//             System.Windows.Forms.OpenFileDialog fbd = new System.Windows.Forms.OpenFileDialog();
//             fbd.Title = "选择文件";
//             fbd.InitialDirectory = _bookDir;
//             fbd.DefaultExt = "*pdf|*pdf| ";
//             if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
//             {
// 
//             }
        }


        private void listBox_book_RemoveBook(object sender, RoutedEventArgs e)
        {
            foreach (Book toRemove in _currentSelectBooks)
            {
                if(_books.Contains(toRemove))
                    _books.Remove(toRemove);
            }

            listBox_book.Items.Refresh();

            BackGourndOutputFile();
        }

        private void listBox_book_ViewHistoryLabel(object sender, RoutedEventArgs e)
        {
            if (_currentSelectBooks.Count != 1) return;

            List<string> historyLabels = FundamentalFunctions.GetLabelsInXML(_historyFileName, _currentSelectBooks[0]);
            if (historyLabels.Count == 0)
            {
                MessageBox.Show("没有历史标签");
            }
            else
            {
                string message = "";
                foreach (string label in historyLabels)
                {
                    message += label + "\n";
                }
                MessageBox.Show(message);
            }
        }

        private CheckBox GenerateLabelCheckbox(string checkContent)
        {
            System.Windows.Controls.CheckBox labelCheck = new System.Windows.Controls.CheckBox();
            labelCheck.Content = checkContent;
            labelCheck.SetValue(WrapPanel.MarginProperty, new Thickness(10, 10, 0, 0));

            ContextMenu menu = new ContextMenu();
            MenuItem item = new MenuItem();
            item.Header = "删除";
            item.Click += new RoutedEventHandler(LabelCheck_RemoveLabel);
            item.Tag = labelCheck;
            menu.Items.Add(item);
            labelCheck.ContextMenu = menu;

            return labelCheck;
        }

        private void LabelCheck_RemoveLabel(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            CheckBox checkbox_toremove = menuItem.Tag as CheckBox;
            if (checkbox_toremove == null) return;

            string label_toremove = checkbox_toremove.Content as string;
            wrapPanel_label.Children.Remove(checkbox_toremove);
            _labels.Remove(label_toremove);
            foreach (Book book in _books)
            {
                if (book.Labels.Contains(label_toremove))
                {
                    book.Labels.Remove(label_toremove);
                }
            }

            comboBox_category.Items.Remove(label_toremove);

            BackGourndOutputFile();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}


