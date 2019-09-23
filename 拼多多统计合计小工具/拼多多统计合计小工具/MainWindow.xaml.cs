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
        //private string apiUrl = "http://video.zxchobits.com:8282/";
        private string apiUrl = "http://center.com/";
        private string kdDir = "";//快递单号文件地址
        private string fhDir = "";//发货单号文件地址
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //选择文件
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "文本文件|*.csv;*.xlsx";
            if (dialog.ShowDialog() == true)
            {
                dir.Text = dialog.FileName;
                FileInfo fileInfo = new FileInfo(dialog.FileName);
                string filePath = copyFile(fileInfo);

                log.Text = "开始上传文件";
               Upload(filePath);

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

        private string copyFile(FileInfo file) {
            //获取文件扩展
            string filePath = "";
            if (file.Extension == ".csv")
            {
                //复制文件一份
                filePath = file.DirectoryName + "\\" + file.Name + ".xlsx";
                //判断文件是否存在
                if (!File.Exists(filePath)) {
                    File.Copy(file.FullName, filePath);
                }
            }
            else
            {
                filePath = file.FullName;
            }

            return filePath;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //选择文件
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "文本文件|*.csv";
            if (dialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(dialog.FileName);
                log.Text = "已加载文件【"+fileInfo.Name+"】【"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "】\r\n";
                dir.Text = dialog.FileName;
                DataTable List =  CsvHelper.OpenCSV(dir.Text);
                DataRow countRow = List.NewRow();
                log.Text += "开始处理数据【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "】\r\n";
                foreach (DataRow row in List.Rows)
                {
                    System.Type t = countRow[3].GetType();
                    if (t.Name == "DBNull") {
                        countRow[3] = 0.0;
                        countRow[4] = 0.0;
                        countRow[5] = 0.0;
                        countRow[6] = 0.0;
                        countRow[7] = 0.0;
                        countRow[8] = 0.0;
                        countRow[9] = 0.0;
                        countRow[10] = 0.0;
                        countRow[11] = 0.0;
                    }
                    countRow[3] = Convert.ToDouble(countRow[3]) + Convert.ToDouble(row[3].ToString().Replace("\t",""));
                    countRow[4] = Convert.ToDouble(countRow[4]) + Convert.ToDouble(row[4].ToString().Replace("\t",""));
                    countRow[5] = Convert.ToDouble(countRow[5]) + Convert.ToDouble(row[5].ToString().Replace("\t",""));
                    countRow[6] = Convert.ToDouble(countRow[6]) + Convert.ToDouble(row[6].ToString().Replace("\t",""));
                    countRow[7] = Convert.ToDouble(countRow[7]) + Convert.ToDouble(row[7].ToString().Replace("\t",""));
                    countRow[8] = Convert.ToDouble(countRow[8]) + Convert.ToDouble(row[8].ToString().Replace("\t",""));
                    countRow[9] = Convert.ToDouble(countRow[9]) + Convert.ToDouble(row[9].ToString().Replace("\t",""));
                    countRow[10] = Convert.ToDouble(countRow[10]) + Convert.ToDouble(row[10].ToString().Replace("\t",""));
                    countRow[11] = Convert.ToDouble(countRow[11]) + Convert.ToDouble(row[11].ToString().Replace("\t",""));
                }
                List.Rows.Add(countRow);
          
                string fileName = fileInfo.FullName.Replace(fileInfo.Extension, "");
                log.Text += "开始导出数据【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "】\r\n";
                CsvHelper.SaveCSV(List,fileName+"【统计】"+fileInfo.Extension);
                log.Text += "文件已保存成功【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "】\r\n";
                log.Text += "文件路径【" + fileName + "【统计】" + fileInfo.Extension + "】\r\n";
            }
        }


        //选择快递单号表格
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "文本文件|*.csv";
            if (dialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(dialog.FileName);
                kdDir = fileInfo.FullName;
                kdText.Text = "文件已加载";
            }
        }

        //选择发货单号表格
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "文本文件|*.csv";
            if (dialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(dialog.FileName);
                fhDir = fileInfo.FullName;
                fhText.Text = "文件已加载";
            }
        }


        //开始比对文件数据
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //快递列表
            DataTable kdList = CsvHelper.OpenCSV(kdDir);
            //发货列表
            DataTable fhList = CsvHelper.OpenCSV(fhDir);

            foreach (DataRow row in kdList.Rows)
            {

            }
        }
    }
}
