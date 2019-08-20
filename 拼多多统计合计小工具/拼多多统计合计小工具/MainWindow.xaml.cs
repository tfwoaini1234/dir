using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
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

namespace 拼多多统计合计小工具
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileDir = "";
        private string fileName = ""; 
        private string apiUrl = "http://video.zxchobits.com:8282/";
        //private string apiUrl = "http://center.com/";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //选择文件
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "文本文件|*.csv";
            if (dialog.ShowDialog() == true)
            {
                dir.Text = dialog.FileName;
                log.Text = "开始上传文件";
               Upload(dir.Text);

            }
        }

        public void Upload(string file)
        {
            FileInfo info = new FileInfo(file);
             fileDir = info.DirectoryName;
            fileName = info.Name;
            string url = string.Format(apiUrl+"pdd/Upload/index");
            WebClient client = new WebClient();
            client.Credentials = CredentialCache.DefaultCredentials;
            client.UploadFileAsync(new Uri(url), file);
            client.UploadFileCompleted += new UploadFileCompletedEventHandler(result_UploadFileCompleted);
        }
        private void result_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            try
            {
                string reply = System.Text.Encoding.UTF8.GetString(e.Result);
                if (HttpFileExist(apiUrl + reply))
                {
                    log.Text = "开始下载文件";
                    DownloadHttpFile(apiUrl + reply, @fileDir+"\\"+ fileName + "统计后.xlsx");
                    log.Text = "文件已放在" + fileDir + "\\" + fileName + "统计后.xlsx";
                }
                if (e.Error != null)
                {
                    MessageBox.Show("上传失败：" + e.Error.Message);
                }
                else
                {
                   // MessageBox.Show("上传成功！");
                }
            }
            catch (Exception e1) {
                MessageBox.Show("上传失败：" + e1.InnerException);
            }
            
        }


        public void DownloadHttpFile(String http_url, String save_url)
        {
            WebResponse response = null;
            //获取远程文件
            WebRequest request = WebRequest.Create(http_url);
            response = request.GetResponse();
            if (response == null) return;
            //读远程文件的大小
            //pbDown.Maximum = response.ContentLength;
            //下载远程文件
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                Stream netStream = response.GetResponseStream();
                Stream fileStream = new FileStream(save_url, FileMode.Create);
                byte[] read = new byte[1024];
                long progressBarValue = 0;
                int realReadLen = netStream.Read(read, 0, read.Length);
                while (realReadLen > 0)
                {
                    fileStream.Write(read, 0, realReadLen);
                    progressBarValue += realReadLen;
                   // pbDown.Dispatcher.BeginInvoke(new ProgressBarSetter(SetProgressBar), progressBarValue);
                    realReadLen = netStream.Read(read, 0, read.Length);
                }
                netStream.Close();
                fileStream.Close();

            }, null);
        }
        /// <summary>
        ///  判断远程文件是否存在
        /// </summary>
        /// <param name="fileUrl">文件URL</param>
        /// <returns>存在-true，不存在-false</returns>
        private bool HttpFileExist(string http_file_url)
        {
            WebResponse response = null;
            bool result = false;//下载结果
            try
            {
                response = WebRequest.Create(http_file_url).GetResponse();
                result = response == null ? false : true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }

    }
}
