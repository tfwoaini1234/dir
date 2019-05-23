using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace 自动整理小程序
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string basePath;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();  //选择文件夹


            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                basePath = openFileDialog.SelectedPath;
                path_text.Text = basePath;
                log.Text = "";
                //log.Text += "1111111111\r\n";
                eachPath(path_text.Text);
                //log.Text += "2222222222\r\n";

            }
        }

        private async void eachPath(string path) {
            //log.Text += "3333333333\r\n";
            await Task.Run(() => each(path));
        }

        private void each(string path) {
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories()) {
                Thread.Sleep(4);
                this.Dispatcher.Invoke(new Action(delegate {
                    //这里写代码
                   
                    //log.Text += path+'/'+NextFolder.Name + "\r\n";
                    log.ScrollToEnd();

                }));
                each(path + '/' + NextFolder.Name);
                foreach(FileInfo fileinfo in NextFolder.GetFiles())
                {
                    string result = GetMD5HashFromFile(fileinfo.FullName);
                    this.Dispatcher.Invoke(new Action(delegate {
                        //这里写代码
                        
                        log.Text += fileinfo.Name + "的创建时间" + fileinfo.CreationTime + "\r\n";
                        log.Text += fileinfo.Name+"的md5值"+result + "\r\n";
                        log.ScrollToEnd();

                    }));
                }

            }
            foreach (FileInfo fileinfo in TheFolder.GetFiles())
            {
                string result = GetMD5HashFromFile(fileinfo.FullName);
                this.Dispatcher.Invoke(new Action(delegate {
                    //这里写代码
                    log.Text += fileinfo.Name + "的创建时间" + fileinfo.CreationTime + "\r\n";
                    log.Text += fileinfo.Name + "的md5值" + result + "\r\n";
                    log.ScrollToEnd();

                }));
            }
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}
