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
        private string savePath;
        private bool imageStatus;
        private bool videoStatus;
        private bool otherStatus;
        private bool musicStatus;
        private string jpgPath="/images/jpg/";
        private string pngPath = "/images/png/";
        private string gifPath = "/images/gif/";
        private string rawPath = "/images/raw/";
        private string mp4Path = "/video/mp4/";
        private string aviPath = "/video/avi/";
        private string mkvPath = "/video/mkv/";
        private string mp3Path = "/music/mp3/";
        private string apePath = "/music/ape/";
        private string flacPath = "/music/flac/";
        private string otherPath = "/other/";
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
                basePathText.Text = basePath;
            }
        }

        private async void eachPath(string path) {
            if (imageStatus)
            {
                //新建图片文件夹
                DirectoryInfo mypath = new DirectoryInfo(savePath+"/"+jpgPath);
                if (!mypath.Exists) {
                    mypath.Create();
                }
                 mypath = new DirectoryInfo(savePath + "/" + pngPath);
                if (!mypath.Exists)
                {
                    mypath.Create();
                }
                 mypath = new DirectoryInfo(savePath + "/" + gifPath);
                if (!mypath.Exists)
                {
                    mypath.Create();
                }
                 mypath = new DirectoryInfo(savePath + "/" + rawPath);
                if (!mypath.Exists)
                {
                    mypath.Create();
                }
            }
            if (videoStatus)
            {
                //新建视频文件夹
                DirectoryInfo mypath = new DirectoryInfo(savePath + "/" + mp4Path);
                if (!mypath.Exists)
                {
                    mypath.Create();
                }
                mypath = new DirectoryInfo(savePath + "/" + aviPath);
                if (!mypath.Exists)
                {
                    mypath.Create();
                }
                mypath = new DirectoryInfo(savePath + "/" + mkvPath);
                if (!mypath.Exists)
                {
                    mypath.Create();
                }
            }
            if (musicStatus)
            {
                //新建音乐文件夹
                DirectoryInfo mypath = new DirectoryInfo(savePath + "/" + mp3Path);
                if (!mypath.Exists)
                {
                    mypath.Create();
                }
                mypath = new DirectoryInfo(savePath + "/" + apePath);
                if (!mypath.Exists)
                {
                    mypath.Create();
                }
                mypath = new DirectoryInfo(savePath + "/" + flacPath);
                if (!mypath.Exists)
                {
                    mypath.Create();
                }
            }
            if (otherStatus) {
                //新建其他文件夹
                DirectoryInfo mypath = new DirectoryInfo(savePath + "/" + otherPath);
                if (!mypath.Exists)
                {
                    mypath.Create();
                }
            }
            //log.Text += "3333333333\r\n";
            await Task.Run(() => each(path));
        }

        private void each(string path) {
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            //遍历文件夹
            try
            {
                foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                {
                    each(path + '/' + NextFolder.Name);
                }
                foreach (FileInfo fileinfo in TheFolder.GetFiles())
                {
                    string saveFilePath;
                    switch (fileinfo.Extension)
                    {
                        case ".jpg":
                            if (imageStatus)
                            {
                                saveFilePath = savePath + "/" + jpgPath;
                            }
                            else
                            {
                                saveFilePath = null;
                            }

                            break;
                        case ".jpeg":
                            if (imageStatus)
                            {
                                saveFilePath = savePath + "/" + jpgPath;
                            }
                            else
                            {
                                saveFilePath = null;
                            }

                            break;
                        case ".png":
                            if (imageStatus)
                            {
                                saveFilePath = savePath + "/" + pngPath;
                            }
                            else
                            {
                                saveFilePath = null;
                            }

                            break;
                        case ".gif":
                            if (imageStatus)
                            {
                                saveFilePath = savePath + "/" + gifPath;
                            }
                            else
                            {
                                saveFilePath = null;
                            }

                            break;
                        case ".raw":
                            if (imageStatus)
                            {
                                saveFilePath = savePath + "/" + rawPath;
                            }
                            else
                            {
                                saveFilePath = null;
                            }

                            break;
                        case ".mp4":
                            if (videoStatus)
                            {
                                saveFilePath = savePath + "/" + mp4Path;
                            }
                            else
                            {
                                saveFilePath = null;
                            }

                            break;
                        case ".avi":
                            if (videoStatus)
                            {
                                saveFilePath = savePath + "/" + aviPath;
                            }
                            else
                            {
                                saveFilePath = null;
                            }

                            break;
                        case ".mkv":
                            if (videoStatus)
                            {
                                saveFilePath = savePath + "/" + mkvPath;
                            }
                            else
                            {
                                saveFilePath = null;
                            }

                            break;
                        case ".mp3":
                            if (musicStatus)
                            {
                                saveFilePath = savePath + "/" + mp3Path;
                            }
                            else
                            {
                                saveFilePath = null;
                            }

                            break;
                        case ".ape":
                            if (musicStatus)
                            {
                                saveFilePath = savePath + "/" + apePath;
                            }
                            else
                            {
                                saveFilePath = null;
                            }

                            break;
                        case ".flac":
                            if (musicStatus)
                            {
                                saveFilePath = savePath + "/" + flacPath;
                            }
                            else
                            {
                                saveFilePath = null;
                            }

                            break;
                        default:
                            if (otherStatus)
                            {
                                saveFilePath = savePath + "/" + otherPath + fileinfo.Extension;
                                //新建其他文件夹
                                DirectoryInfo mypath = new DirectoryInfo(saveFilePath);
                                if (!mypath.Exists)
                                {
                                    mypath.Create();
                                }
                            }
                            else
                            {
                                saveFilePath = null;
                            }
                            break;
                    }
                    //如果状态没打开，直接跳过。
                    if (saveFilePath == null)
                    {
                        continue;
                    }
                    if (fileinfo.Length < 1048576)
                    {
                        saveFilePath += "/小文件/";
                    }
                    else
                    {
                        saveFilePath += "/大文件/";
                    }
                    //新建其他文件夹
                    DirectoryInfo mypath1 = new DirectoryInfo(saveFilePath);
                    if (!mypath1.Exists)
                    {
                        mypath1.Create();
                    }
                    if (!File.Exists(saveFilePath + fileinfo.Name))
                    {

                        System.IO.File.Copy(fileinfo.FullName, saveFilePath + fileinfo.Name);
                        //this.Dispatcher.Invoke(new Action(delegate
                        //{
                        //    //这里写代码
                        //    log.Text += "移动了【" + fileinfo.Name + "】到新目录";
                        //    log.Text += "移动了【" + fileinfo.Length + "】到新目录";
                        //    log.ScrollToEnd();
                        //}));
                    }

                }
            }
            catch (Exception) {

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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            imageStatus = checkImage.IsChecked == true;
        }

        private void checkVideo_Checked(object sender, RoutedEventArgs e)
        {
            videoStatus = checkVideo.IsChecked == true;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            eachPath(basePath);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();  //选择文件夹

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                savePath = openFileDialog.SelectedPath;
                savePathText.Text = savePath;
            }
        }

        private void checkOther_Checked(object sender, RoutedEventArgs e)
        {
            otherStatus = checkOther.IsChecked == true;
        }

        private void checkMusic_Checked(object sender, RoutedEventArgs e)
        {
            musicStatus = checkMusic.IsChecked == true;
        }
    }
}
