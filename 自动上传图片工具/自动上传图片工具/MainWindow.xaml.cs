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

namespace 自动上传图片工具
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string dirPath="";
        private bool kg = true;
        public MainWindow()
        {
            InitializeComponent();
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
            catch(Exception ex)
            {
                MessageBox.Show("你还没有选择文件夹", "警告");
            }
           
            
        }
        public void eachDir()
        {
            each(dirPath);
            kg = true;
        }

        private void each(string path)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            //先遍历当前文件夹的文件
            foreach(FileInfo fileInfo in TheFolder.GetFiles())
            {
                string ext = fileInfo.Extension.ToLower();
                
                string extension = ".bmp,.jpg,.png,.tif,.gif,.pcx,.tga,.exif,.fpx,.svg,.psd,.cdr,.pcd,.dxf,.ufo,.eps,.ai,.raw,.wmf,.webp";
                if (extension.IndexOf(ext) >= 0)
                {
                    string result = GetMD5HashFromFile(fileInfo.FullName);
                    //是图片，开始上传信息
                    try
                    {
                        
                        HttpRequestHelper.HttpGet("http://center.lovezhaoxin.com/zxchobits/szly_rand_q_q/wpfSetImage", "md5=" + result + "&content=" + fileInfo.Name.Replace(fileInfo.Extension,""));
                         this.Dispatcher.Invoke(new Action(delegate {
                        //这里写代码
                       log.Text += fileInfo.Name + "设置成功\r\n";
                        log.ScrollToEnd();
                    }));
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.Invoke(new Action(delegate {
                            //这里写代码
                            log.Text += fileInfo.Name + "出错拉\r\n";
                            log.ScrollToEnd();
                        }));
                    }
                   
                }
            }
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                each(path + '/' + NextFolder.Name);
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
