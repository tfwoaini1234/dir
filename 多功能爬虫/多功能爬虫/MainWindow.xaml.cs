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
using mshtml;
namespace 多功能爬虫
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool btnStatus = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            btnStatus = !btnStatus;
            start.Visibility = btnStatus ? Visibility.Visible : Visibility.Collapsed;
            end.Visibility = !btnStatus ? Visibility.Visible : Visibility.Collapsed;
        }

        private void startF()
        {
            WebBrowser web = new WebBrowser();
            web.Navigate(new Uri("http://www.baidu.com/"));

            // ……等待网页加载……

            HTMLDocument doc = web.Document as HTMLDocument;
            //获取body内的html代码
            string html = doc.body.innerHTML;

        }

      

    }
}
