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
using System.Windows.Threading;

namespace 自动生成目录小工具
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string dirPath = "E:/媒体库/视频/新番/";
        private string savePath = "D:/www/center/public/static/video/";
        private bool kg = true;
        private DispatcherTimer dispatcherTimer;
        private int copyNum = 0;
        public MainWindow()
        {
            InitializeComponent();
            //while (true)
            //{

            //        //这里写代码
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(scanning);
            dispatcherTimer.Interval = new TimeSpan(1, 0, 0);
            dispatcherTimer.Start();
            

            //    //Thread.Sleep(200);
            //}
        }
        private void scanning(object sender, EventArgs e) {
            log.Text = "";
            dispatcherTimer.Stop();
            eachDir();
            dispatcherTimer.Start();
            log.Text += DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") +"刷新了一次\r\n";
            log.ScrollToEnd();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                dirName.Text = openFileDialog.SelectedPath;
                dirPath = dirName.Text;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            log.Text = "";
            try
            {
                DirectoryInfo mypath = new DirectoryInfo(dirPath);
                if (!mypath.Exists)
                {
                    MessageBox.Show("你还没有选择文件夹", "警告");
                }
                else
                {
                    if (kg)
                    {
                        kg = false;
                        //如果存在，就开始递归
                        Thread thread = new Thread(eachDir);
                        thread.Start();
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("你还没有选择文件夹", "警告");
            }


        }
        public void eachDir()
        {

            copyNum = 0;
            each(dirPath,"");
            kg = true;
            if (copyNum > 0) {
                //如果 有复制文件，请求网页刷新列表
                HttpRequestHelper.HttpGet("http://video.zxchobits.com:8282/nas/dir/index", "");
            }
            copyNum = 0;
        }

        private void each(string path,string p)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            //先遍历当前文件夹的文件
            foreach (FileInfo fileInfo in TheFolder.GetFiles())
            {
                //判断是不是mp4文件
                string ext = fileInfo.Extension.ToLower();
                string extension = ".mp4";
                if (extension.IndexOf(ext) >= 0) {

                    //判断该移动文件夹是否存在
                    string sPath = savePath + p;
                    if (Directory.Exists(sPath) == false)//如果不存在就创建file文件夹
                    {
                        Directory.CreateDirectory(sPath);
                    }
                    //判断文件是否存在
                    string fPath = sPath + "/" + fileInfo.Name;
                    if (File.Exists(fPath))
                    {
                        //如果文件存在，比较文件是否一致
                        FileInfo fileInfo2 = new FileInfo(fPath);
                        if (fileInfo.Length != fileInfo2.Length)
                        {
                            //删除目标目录文件，重新复制
                            fileInfo2.Delete();
                            File.Copy(fileInfo.FullName, fPath);
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                //这里写代码
                                log.Text += fileInfo.Name + "文件大小不对，重新复制\r\n";
                                log.ScrollToEnd();
                            }));
                        }
                        else
                        {
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                //这里写代码
                                log.Text += fileInfo.Name + "已存在\r\n";
                                log.ScrollToEnd();
                            }));
                        }
                    }
                    else
                    {
                        //如果文件不存在，直接复制
                        File.Copy(fileInfo.FullName, fPath);
                        copyNum++;
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            //这里写代码
                            log.Text += fileInfo.Name + "复制成功\r\n";
                            log.ScrollToEnd();
                        }));
                    }
                }
                  

                //strking ext = fileInfo.Extension.ToLower();
                //this.Dispatcher.Invoke(new Action(delegate
                //{
                //    //这里写代码
                //    log.Text += p+"\r\n";
                //    log.ScrollToEnd();
                //}));
                //string extension = ".mp4";
                //if (extension.IndexOf(ext) >= 0)
                //{
                    //string result = GetMD5HashFromFile(fileInfo.FullName);
                    //if (File.Exists(savePath+fileInfo.Name))
                    //{
                    //    this.Dispatcher.Invoke(new Action(delegate
                    //    {
                    //        //这里写代码
                    //        log.Text += fileInfo.Name + "文件已经存在\r\n";
                    //        log.ScrollToEnd();
                    //    }));
                    //}
                    //else
                    //{
                    //   // System.IO.File.Copy(fileInfo.FullName, savePath + fileInfo.Name);
                    //    this.Dispatcher.Invoke(new Action(delegate
                    //    {
                    //        //这里写代码
                    //        log.Text += fileInfo.Name + "复制成功\r\n";
                    //        log.ScrollToEnd();
                    //    }));
                    //}
                    ////是图片，开始上传信息
                    //try
                    //{

                    //    HttpRequestHelper.HttpGet("http://center.lovezhaoxin.com/zxchobits/szly_rand_q_q/wpfSetImage", "md5=" + result + "&content=" + fileInfo.Name.Replace(fileInfo.Extension, ""));
                    //    this.Dispatcher.Invoke(new Action(delegate {
                    //        //这里写代码
                    //        log.Text += fileInfo.Name + "设置成功\r\n";
                    //        log.ScrollToEnd();
                    //    }));
                    //}
                    //catch (Exception ex)
                    //{
                    //    this.Dispatcher.Invoke(new Action(delegate {
                    //        //这里写代码
                    //        log.Text += fileInfo.Name + "出错拉\r\n";
                    //        log.ScrollToEnd();
                    //    }));
                    //}

                //}
            }
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                each(path + '/' + NextFolder.Name, p+"/"+NextFolder.Name);
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
                return "";
            }
        }
    }
}
