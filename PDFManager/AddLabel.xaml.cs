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
using System.Windows.Shapes;

namespace PDFManager
{
    /// <summary>
    /// AddLabel.xaml 的交互逻辑
    /// </summary>
    public partial class AddLabel : Window
    {
        string _label;
        MainWindow _parent;

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        public AddLabel(MainWindow parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        private void button_ok_Click(object sender, RoutedEventArgs e)
        {
            OKCLICK();
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            _parent.IsEnabled = true;
            
        }

        private void textBox_label_KeyDown(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            if (key == Key.Enter)
            {
                OKCLICK();
            }
        }

        private void OKCLICK()
        {
            _label = textBox_label.Text;
            if (_parent.AddLabels())
            {
                this.Visibility = Visibility.Hidden;
                _parent.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("标签已经存在！");
            }
        }
    }
}
