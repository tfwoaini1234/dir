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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace 爬虫
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool btnStatus = true;
        public MainWindow()
        {
            InitializeComponent();
        }

        /**
         * 开始爬虫程序
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            startF();
            btnStatus = !btnStatus;
            start.Visibility = btnStatus ?Visibility.Visible:Visibility.Collapsed;
            end.Visibility = !btnStatus ?Visibility.Visible:Visibility.Collapsed;
            
        }

        private void startF() {
            webBrowser.Navigate("http://www.baidu.com/");

            // ……等待网页加载……

            HTMLDocument doc = this.wb3.Document as HTMLDocument;
            //获取body内的html代码
            string html = doc.body.innerHTML;

        }

    }
}
